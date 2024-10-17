using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using BCCore.DDD.Attachments;
using Idler.Common.Attachments;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Logging;
using Idler.Common.Core.Upload;
using log4net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OBS;
using OBS.Model;

namespace Idler.Common.OBS
{
    public class OBSAttachmentDomainService(
        IRepository<Attachment, Guid> AttachmentRepository,
        ILogger<OBSAttachmentDomainService> logger,
        IConfigAccessHelper<UploadConfig> uploadConfigAccessHelper,
        IOptions<OBSSetting> obsConfigAccessHelper,
        IUnitOfWork unitOfWork)
        : BaseAttachmentDomainService(AttachmentRepository, uploadConfigAccessHelper, unitOfWork)
    {


        /// <summary>
        /// 从服务器删除附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public override APIReturnInfo<string> RemoveFromServer(string UploadType, string DependentId)
        {
            if (UploadType.IsEmpty())
                throw new ArgumentNullException(nameof(UploadType));

            if (DependentId.IsEmpty())
                throw new ArgumentNullException(nameof(DependentId));

            IList<string> FullPaths = this.AttachmentRepository
                .Find(t => t.UploadType == UploadType && t.DependentId == DependentId).Select(t => t.FullPath).ToList();
            if (FullPaths.Count == 0)
                return APIReturnInfo<string>.Success("ok");

            ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
            try
            {
                DeleteObjectsRequest request = new DeleteObjectsRequest();
                request.BucketName = obsConfigAccessHelper.Value.BucketName;
                request.Quiet = true;
                FullPaths.ForEach(t => request.AddKey(t));
                DeleteObjectsResponse response = client.DeleteObjects(request);
                if (response.DeleteErrors.Count == 0)
                    return APIReturnInfo<string>.Success("ok");

                return new APIReturnInfo<string>()
                    { State = false, Message = "有一些文件删除失败了" };
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
        /// <param name="RemoveId">要删除的附件Id</param>
        /// <returns></returns>
        public override APIReturnInfo<string> RemoveFromServer(Guid RemoveId)
        {
            if (RemoveId.IsEmpty())
                throw new ArgumentNullException(nameof(RemoveId));

            Attachment AttachmentInfo = this.AttachmentRepository.Single(RemoveId);
            if (AttachmentInfo == null)
                return APIReturnInfo<string>.Error("福建不存在");

            ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = AttachmentInfo.FullPath,
                };
                DeleteObjectResponse response = client.DeleteObject(request);

                if (response.StatusCode == HttpStatusCode.OK)
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
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        public override APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey)
        {
            if (ObjectKey.IsEmpty())
                throw new ArgumentNullException(nameof(ObjectKey));

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);
                CreateTemporarySignatureRequest request = new CreateTemporarySignatureRequest();
                request.BucketName = obsConfigAccessHelper.Value.BucketName;
                request.ObjectKey = ObjectKey;
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
        /// <param name="UploadType">上传类型</param>
        /// <param name="DependentId">依赖Id</param>
        /// <param name="FileName">文件名</param>
        /// <param name="FileSize">文件大小</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        public override APIReturnInfo<UploadFilInfo> Upload(string UploadType, string DependentId, string FileName,
            long FileSize, Stream fileStream)
        {
            UploadFilInfo FileInfo = new UploadFilInfo(string.Concat(UploadType, "/"), FileName, FileSize, UploadType);

            APIReturnInfo<UploadConfigItem> rinfo =
                this.Config().Verify(UploadType, FileInfo.FileExt, FileInfo.FileSize);
            if (!rinfo.State)
                return APIReturnInfo<UploadFilInfo>.Error(rinfo.Message);

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);

                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = $"{FileInfo.SavePath}{FileInfo.SaveFileName}",
                    InputStream = fileStream,
                };

                PutObjectResponse response = client.PutObject(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<UploadFilInfo>.Error("上传失败");

                Attachment AttachmentInfo = this.AttachmentRepository.Add(new Attachment()
                {
                    DependentId = DependentId,
                    FileExt = FileInfo.FileExt,
                    FileSize = FileInfo.FileSize,
                    UploadType = UploadType,
                    FilePath = FileInfo.SavePath,
                    Name = FileInfo.FileName,
                    FileName = FileInfo.SaveFileName,
                    FullPath = $"{FileInfo.SavePath}{FileInfo.SaveFileName}",
                    SyncState = true,
                    RelativelyUrl =
                        $"{obsConfigAccessHelper.Value.RootUrl}/{FileInfo.SavePath}{FileInfo.SaveFileName}"
                });

                this.SaveChange();

                //   public UploadFilInfo(string rootPath, string fileName, long fileSize, string uploadType, Guid id)

                return APIReturnInfo<UploadFilInfo>.Success(new UploadFilInfo(FileInfo.RootPath,
                    FileInfo.FileName, FileInfo.FileSize, UploadType, AttachmentInfo.Id));
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
        /// <param name="UploadType">上传类型</param>
        /// <param name="DependentId">依赖Id</param>
        /// <param name="FileName">文件名</param>
        /// <param name="PartSize">分片大小</param>
        /// <param name="FileSize">文件大小</param>
        /// <returns></returns>
        public override APIReturnInfo<string> InitializeLargeFileUploadTask(string UploadType, string DependentId,
            string FileName, long PartSize, long FileSize)
        {
            if (UploadType.IsEmpty())
                throw new ArgumentNullException(nameof(UploadType));

            if (DependentId.IsEmpty())
                throw new ArgumentNullException(nameof(DependentId));

            if (FileName.IsEmpty())
                throw new ArgumentNullException(nameof(FileName));

            UploadFilInfo FileInfo = new UploadFilInfo(string.Concat(UploadType, "/"), FileName, FileSize, UploadType);

            APIReturnInfo<UploadConfigItem> rinfo =
                this.Config().Verify(UploadType, FileInfo.FileExt, FileInfo.FileSize);
            if (!rinfo.State)
                return APIReturnInfo<string>.Error(rinfo.Message);

            try
            {
                ObsClient client = new ObsClient(obsConfigAccessHelper.Value.AccessKey,
                    obsConfigAccessHelper.Value.SecretKey, obsConfigAccessHelper.Value.EndPoint);

                InitiateMultipartUploadRequest request = new InitiateMultipartUploadRequest()
                {
                    BucketName = obsConfigAccessHelper.Value.BucketName,
                    ObjectKey = $"{FileInfo.SavePath}{FileInfo.SaveFileName}"
                };

                InitiateMultipartUploadResponse response = client.InitiateMultipartUpload(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("上传失败");

                string taskKeyConfig =
                    new MultipartUploadTaskValue(response.UploadId, UploadType, DependentId, PartSize, FileInfo)
                        .ToConfig();

                return new APIReturnInfo<string>()
                {
                    State = true, Data = taskKeyConfig.AESEncrypt(obsConfigAccessHelper.Value.SecretKey)
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
        /// <param name="TaskId">任务Id</param>
        /// <param name="PartNumber">分片编号</param>
        /// <param name="Total">总数</param>
        /// <param name="fileStream">待上传文件流</param>
        /// <returns></returns>
        public override APIReturnInfo<MultipartUploadResultValue> Upload(string TaskId, Stream fileStream)
        {
            if (TaskId.IsEmpty())
                throw new ArgumentNullException(nameof(TaskId));

            try
            {
                string Config = TaskId.AESDecrypt(obsConfigAccessHelper.Value.SecretKey);
                if (Config == "err")
                    return APIReturnInfo<MultipartUploadResultValue>.Error("任务不存在");

                MultipartUploadTaskValue taskInfo = new MultipartUploadTaskValue().FromConfig(Config);
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

                Attachment AttachmentInfo = this.AttachmentRepository.Add(new Attachment()
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
                    taskInfo.FileSize, taskInfo.UploadType, AttachmentInfo.Id));
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
        private void CancelMultipartUpload(ObsClient client, string BucketName, string ObjectKey, string UploadId)
        {
            //取消分段上传任务
            try
            {
                AbortMultipartUploadRequest request = new AbortMultipartUploadRequest
                {
                    BucketName = BucketName,
                    ObjectKey = ObjectKey,
                    UploadId = UploadId
                };
                AbortMultipartUploadResponse response = client.AbortMultipartUpload(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    logger.Log(LogLevel.Information, $"取消分片上传失败，任务Id：{UploadId}，ObjectKey：{ObjectKey}");
            }
            catch (ObsException ex)
            {
                logger.HandleException<ObsException>(ex);
            }
        }
    }
}