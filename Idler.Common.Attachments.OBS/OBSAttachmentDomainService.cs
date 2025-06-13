using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Idler.Common.Attachments;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Logging;
using Idler.Common.Core.Upload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;

namespace Idler.Common.Attachments.OBS
{
    public class OBSAttachmentDomainService(
        IRepository<Attachment, Guid> attachmentRepository,
        ILogger<OBSAttachmentDomainService> logger,
        IOptions<UploadConfig> uploadConfigAccessHelper,
        IOptions<OBSSetting> obsConfigAccessHelper,
        IUnitOfWork unitOfWork)
        : BaseAttachmentDomainService(attachmentRepository, uploadConfigAccessHelper, unitOfWork)
    {
        /// <summary>
        /// 从服务器删除附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public override APIReturnInfo<string> RemoveFromServer(string uploadType, string dependentId)
        {
            if (uploadType.IsEmpty())
                throw new ArgumentNullException(nameof(uploadType));

            if (dependentId.IsEmpty())
                throw new ArgumentNullException(nameof(dependentId));

            IList<string> fullPaths = attachmentRepository
                .Find(t => t.UploadType == uploadType && t.DependentId == dependentId).Select(t => t.FullPath).ToList();
            if (fullPaths.Count == 0)
                return APIReturnInfo<string>.Success("ok");

            ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
            try
            {
                DeleteObjectsRequest request = new DeleteObjectsRequest();
                request.BucketName = obsConfigAccessHelper.Value.BucketName;
                request.Quiet = true;
                fullPaths.ForEach(t => request.AddKey(t));
                DeleteObjectsResponse response = client.DeleteObjects(request);
                if (response.DeleteErrors.Count == 0)
                    return APIReturnInfo<string>.Success("ok");

                return new APIReturnInfo<string>()
                {
                    State = false,
                    Message = "有一些文件删除失败了"
                };
            }
            catch (ObsException ex)
            {
                logger.HandleException<ObsException>(ex);
                return APIReturnInfo<string>.Error(ex.ErrorMessage);
            }
        }

        /// <summary>
        /// 从服务器删除指定附件
        /// </summary>
        /// <param name="removeId">要删除的附件Id</param>
        /// <returns></returns>
        public override APIReturnInfo<string> RemoveFromServer(Guid removeId)
        {
            if (removeId.IsEmpty())
                throw new ArgumentNullException(nameof(removeId));

            Attachment attachmentInfo = attachmentRepository.Single(removeId);
            if (attachmentInfo == null)
                return APIReturnInfo<string>.Error("附件不存在");

            ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = attachmentInfo.FullPath,
                };
                DeleteObjectResponse response = client.DeleteObject(request);

                if (response.StatusCode == HttpStatusCode.OK
                    || response.StatusCode == HttpStatusCode.NoContent)
                    return APIReturnInfo<string>.Success("ok");

                return APIReturnInfo<string>.Error("删除失败");
            }
            catch (ObsException ex)
            {
                logger.HandleException<ObsException>(ex);
                return APIReturnInfo<string>.Error(ex.ErrorMessage);
            }
        }

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="objectKey">ObjectKey</param>
        /// <returns></returns>
        public override APIReturnInfo<string> GenerateUploadAuthorizationUrl(string objectKey)
        {
            if (objectKey.IsEmpty())
                throw new ArgumentNullException(nameof(objectKey));

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
                CreateTemporarySignatureRequest request = new CreateTemporarySignatureRequest();
                request.BucketName = obsConfigAccessHelper.Value.BucketName;
                request.ObjectKey = objectKey;
                request.Method = HttpVerb.PUT;
                request.Expires = 60 * 60;

                CreateTemporarySignatureResponse response = client.CreateTemporarySignature(request);
                return new APIReturnInfo<string>() { State = true, Message = "ok", Data = response.SignUrl };
            }
            catch (ObsException e)
            {
                logger.HandleException<ObsException>(e);
                return APIReturnInfo<string>.Error(e.ErrorMessage);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="uploadType">上传类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public override APIReturnInfo<UploadFilInfo> Upload(string uploadType, string dependentId, string fileName,
            long fileSize, Stream fileStream)
        {
            UploadFilInfo fileInfo = new UploadFilInfo(string.Concat(uploadType, "/"), fileName, fileSize, uploadType);

            APIReturnInfo<UploadConfigItem> rinfo =
                this.Config().Verify(uploadType, fileInfo.FileExt, fileInfo.FileSize);
            if (!rinfo.State)
                return APIReturnInfo<UploadFilInfo>.Error(rinfo.Message);

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);

                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = $"{fileInfo.SavePath}{fileInfo.SaveFileName}",
                    InputStream = fileStream,
                };

                PutObjectResponse response = client.PutObject(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<UploadFilInfo>.Error("上传失败");

                Attachment attachmentInfo = attachmentRepository.Add(new Attachment()
                {
                    DependentId = dependentId,
                    FileExt = fileInfo.FileExt,
                    FileSize = fileInfo.FileSize,
                    UploadType = uploadType,
                    FilePath = fileInfo.SavePath,
                    Name = fileInfo.FileName,
                    FileName = fileInfo.SaveFileName,
                    FullPath = $"{fileInfo.SavePath}{fileInfo.SaveFileName}",
                    SyncState = true,
                    RelativelyUrl =
                        $"{obsConfigAccessHelper.Value.RootUrl}/{fileInfo.SavePath}{fileInfo.SaveFileName}"
                });

                this.SaveChange();

                return APIReturnInfo<UploadFilInfo>.Success(new UploadFilInfo(fileInfo.RootPath,
                        fileInfo.FileName, fileInfo.FileSize, uploadType, attachmentInfo.Id)
                {
                    RootUrl = obsConfigAccessHelper.Value.RootUrl,
                    SaveFileName = fileInfo.SaveFileName
                });
            }
            catch (ObsException e)
            {
                logger.HandleException<ObsException>(e);
                return APIReturnInfo<UploadFilInfo>.Error(e.ErrorMessage);
            }
        }

        /// <summary>
        /// 初始化大文件上传任务
        /// </summary>
        /// <param name="uploadType">上传类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <param name="fileName">文件名</param>
        /// <param name="partSize">分片大小</param>
        /// <param name="fileSize">文件大小</param>
        /// <returns></returns>
        public override APIReturnInfo<string> InitializeLargeFileUploadTask(string uploadType, string dependentId,
            string fileName, long partSize, long fileSize)
        {
            if (uploadType.IsEmpty())
                throw new ArgumentNullException(nameof(uploadType));

            if (dependentId.IsEmpty())
                throw new ArgumentNullException(nameof(dependentId));

            if (fileName.IsEmpty())
                throw new ArgumentNullException(nameof(fileName));

            UploadFilInfo fileInfo = new UploadFilInfo(string.Concat(uploadType, "/"), fileName, fileSize, uploadType);

            APIReturnInfo<UploadConfigItem> rinfo =
                this.Config().Verify(uploadType, fileInfo.FileExt, fileInfo.FileSize);
            if (!rinfo.State)
                return APIReturnInfo<string>.Error(rinfo.Message);

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);

                InitiateMultipartUploadRequest request = new InitiateMultipartUploadRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = $"{fileInfo.SavePath}{fileInfo.SaveFileName}"
                };

                InitiateMultipartUploadResponse response = client.InitiateMultipartUpload(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("上传失败");

                string taskKeyConfig =
                    new MultipartUploadTaskValue(response.UploadId, uploadType, dependentId, partSize, fileInfo)
                        .ToConfig();

                return new APIReturnInfo<string>()
                {
                    State = true,
                    Data = taskKeyConfig.AESEncrypt(obsConfigAccessHelper.Value.SecretKey)
                };
            }
            catch (ObsException e)
            {
                logger.HandleException<ObsException>(e);
                return APIReturnInfo<string>.Error(e.ErrorMessage);
            }
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="taskId">任务Id</param>
        /// <param name="PartNumber">分片编号</param>
        /// <param name="Total">总数</param>
        /// <param name="fileStream">待上传文件流</param>
        /// <returns></returns>
        public override APIReturnInfo<MultipartUploadResultValue> Upload(string taskId, Stream fileStream)
        {
            if (taskId.IsEmpty())
                throw new ArgumentNullException(nameof(taskId));

            try
            {
                string config = taskId.AESDecrypt(obsConfigAccessHelper.Value.SecretKey);
                if (config == "err")
                    return APIReturnInfo<MultipartUploadResultValue>.Error("任务不存在");

                MultipartUploadTaskValue taskInfo = new MultipartUploadTaskValue().FromConfig(config);
                if (taskInfo.CurrentPart > taskInfo.TotalPart)
                    return APIReturnInfo<MultipartUploadResultValue>.Error("已上传至文件末尾请不要重复上传分片");

                APIReturnInfo<UploadConfigItem> rinfo =
                    this.Config().Verify(taskInfo.UploadType, taskInfo.FileExt, taskInfo.FileSize);
                if (!rinfo.State)
                    return APIReturnInfo<MultipartUploadResultValue>.Error(rinfo.Message);

                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);

                UploadPartRequest uploadRequest = new UploadPartRequest
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = taskInfo.ObjectKey,
                    UploadId = taskInfo.TaskId,
                    PartNumber = taskInfo.CurrentPart,
                    InputStream = fileStream
                };

                UploadPartResponse response = client.UploadPart(uploadRequest);
                if (response.StatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<MultipartUploadResultValue>.Error("上传失败");

                if (taskInfo.CurrentPart < taskInfo.TotalPart)
                {
                    taskInfo.CurrentPart++;

                    return APIReturnInfo<MultipartUploadResultValue>.Success(new MultipartUploadResultValue()
                    {
                        TaskId = taskInfo.ToConfig().AESEncrypt(obsConfigAccessHelper.Value.SecretKey),
                        Total = taskInfo.TotalPart,
                        Current = taskInfo.CurrentPart
                    });
                }

                CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = taskInfo.ObjectKey,
                    UploadId = taskInfo.TaskId,
                };


                ListPartsRequest listrequest = new ListPartsRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = taskInfo.ObjectKey,
                    UploadId = taskInfo.TaskId
                };

                ListPartsResponse listresponse;
                do
                {
                    listresponse = client.ListParts(listrequest);
                    Console.WriteLine("List parts response: {0}", response.StatusCode);
                    foreach (PartDetail part in listresponse.Parts)
                    {
                        completeRequest.AddPartETags(new PartETag(part.PartNumber, part.ETag));
                    }

                    listrequest.PartNumberMarker = listresponse.NextPartNumberMarker;
                } while (listresponse.IsTruncated);

                CompleteMultipartUploadResponse completeUploadResponse =
                    client.CompleteMultipartUpload(completeRequest);
                if (completeUploadResponse.StatusCode != HttpStatusCode.OK)
                {
                    this.CancelMultipartUpload(client, obsConfigAccessHelper.Value.BucketName, taskInfo.ObjectKey,
                        taskInfo.TaskId);
                    return APIReturnInfo<MultipartUploadResultValue>.Error("上传失败");
                }

                Attachment attachmentInfo = attachmentRepository.Add(new Attachment()
                {
                    DependentId = taskInfo.DependentId,
                    FileExt = taskInfo.FileExt,
                    FileSize = taskInfo.FileSize,
                    UploadType = taskInfo.UploadType,
                    FilePath = taskInfo.SavePath,
                    Name = taskInfo.FileName,
                    FileName = taskInfo.SaveFileName,
                    FullPath = taskInfo.ObjectKey,
                    SyncState = true,
                    RelativelyUrl = $"{obsConfigAccessHelper.Value.RootUrl}/{taskInfo.ObjectKey}"
                });

                this.SaveChange();

                return APIReturnInfo<MultipartUploadResultValue>.Success(new MultipartUploadResultValue(taskInfo.TaskId,
                        taskInfo.TotalPart, taskInfo.CurrentPart, taskInfo.SavePath, taskInfo.FileName,
                        taskInfo.FileSize, taskInfo.UploadType, attachmentInfo.Id)
                {
                    RootUrl = obsConfigAccessHelper.Value.RootUrl
                });
            }
            catch (ObsException ex)
            {
                logger.HandleException<ObsException>(ex);
                return APIReturnInfo<MultipartUploadResultValue>.Error(ex.ErrorMessage);
            }
        }

        /// <summary>
        /// 取消分片上传
        /// </summary>
        private void CancelMultipartUpload(ObsClient client, string bucketName, string objectKey, string uploadId)
        {
            //取消分段上传任务
            try
            {
                AbortMultipartUploadRequest request = new AbortMultipartUploadRequest
                {
                    BucketName = bucketName,
                    ObjectKey = objectKey,
                    UploadId = uploadId
                };
                AbortMultipartUploadResponse response = client.AbortMultipartUpload(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    logger.Log(LogLevel.Information, $"取消分片上传失败，任务Id：{uploadId}，ObjectKey：{objectKey}");
            }
            catch (ObsException ex)
            {
                logger.HandleException<ObsException>(ex);
            }
        }
    }
}