using System.Net;
using System.Security.Cryptography;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Logging;
using Idler.Common.Core.Upload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Idler.Common.Attachments.OSS
{
    /// <summary>
    /// OSS上传服务
    /// </summary>
    /// <param name="attachmentRepository"></param>
    /// <param name="logger"></param>
    /// <param name="uploadConfigAccessHelper"></param>
    /// <param name="ossSetting"></param>
    /// <param name="unitOfWork"></param>
    public class OSSAttachmentDomainService(
        IRepository<Attachment, Guid> attachmentRepository,
        ILogger<OSSAttachmentDomainService> logger,
        IOptions<UploadConfig> uploadConfigAccessHelper,
        IOptions<OSSSetting> ossSetting,
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

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);

            try
            {
                var request = new DeleteObjectsRequest(ossSetting.Value.BucketName, fullPaths, false);

                var result = client.DeleteObjects(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("文件删除失败");

                if (result.Keys.Length == fullPaths.Count)
                    return APIReturnInfo<string>.Success("ok");

                return new APIReturnInfo<string>()
                {
                    State = false,
                    Message = "有一些文件删除失败了"
                };
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
                return APIReturnInfo<string>.Error(ex.Message);
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

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);
            try
            {
                DeleteObjectRequest request =
                    new DeleteObjectRequest(ossSetting.Value.BucketName, attachmentInfo.FullPath);
                var result = client.DeleteObject(request);
                if (result.HttpStatusCode != HttpStatusCode.OK
                    && result.HttpStatusCode != HttpStatusCode.NoContent)
                    return APIReturnInfo<string>.Error("删除失败");

                return APIReturnInfo<string>.Success("ok");
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
                return APIReturnInfo<string>.Error(ex.Message);
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

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);
            try
            {
                var generatePresignedUriRequest =
                    new GeneratePresignedUriRequest(ossSetting.Value.BucketName, objectKey, SignHttpMethod.Put)
                    {
                        Expiration = DateTime.Now.AddHours(1),
                    };
                var signedUrl = client.GeneratePresignedUri(generatePresignedUriRequest);

                return new APIReturnInfo<string>() { State = true, Message = "ok", Data = signedUrl.ToString() };
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
                return APIReturnInfo<string>.Error(ex.Message);
            }
            catch (Exception ex)
            {
                logger.HandleException(ex);
                return APIReturnInfo<string>.Error(ex.Message);
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
                var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                    ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
                client.SetRegion(ossSetting.Value.Region);

                var result = client.PutObject(ossSetting.Value.BucketName,
                    $"{fileInfo.SavePath}{fileInfo.SaveFileName}", fileStream);

                if (result.HttpStatusCode != HttpStatusCode.OK)
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
                        $"{ossSetting.Value.RootUrl}/{fileInfo.SavePath}{fileInfo.SaveFileName}"
                });

                this.SaveChange();

                return APIReturnInfo<UploadFilInfo>.Success(new UploadFilInfo(fileInfo.RootPath,
                    fileInfo.FileName, fileInfo.FileSize, uploadType, attachmentInfo.Id)
                {
                    RootUrl = ossSetting.Value.RootUrl,
                    SaveFileName = fileInfo.SaveFileName
                });
            }
            catch (OssException e)
            {
                logger.HandleException<OssException>(e);
                return APIReturnInfo<UploadFilInfo>.Error(e.Message);
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
                var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                    ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
                client.SetRegion(ossSetting.Value.Region);

                InitiateMultipartUploadRequest request = new InitiateMultipartUploadRequest(ossSetting.Value.BucketName,
                    $"{fileInfo.SavePath}{fileInfo.SaveFileName}");
                var result = client.InitiateMultipartUpload(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("上传失败");

                string taskKeyConfig =
                    new MultipartUploadTaskValue(result.UploadId, uploadType, dependentId, partSize, fileInfo)
                        .ToConfig();

                return new APIReturnInfo<string>()
                {
                    State = true,
                    Data = taskKeyConfig.AESEncrypt(ossSetting.Value.SecretKey)
                };
            }
            catch (OssException e)
            {
                logger.HandleException<OssException>(e);
                return APIReturnInfo<string>.Error(e.Message);
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
                string config = taskId.AESDecrypt(ossSetting.Value.SecretKey);
                if (config == "err")
                    return APIReturnInfo<MultipartUploadResultValue>.Error("任务不存在");

                MultipartUploadTaskValue taskInfo = new MultipartUploadTaskValue().FromConfig(config);
                if (taskInfo.CurrentPart > taskInfo.TotalPart)
                    return APIReturnInfo<MultipartUploadResultValue>.Error("已上传至文件末尾请不要重复上传分片");

                APIReturnInfo<UploadConfigItem> rinfo =
                    this.Config().Verify(taskInfo.UploadType, taskInfo.FileExt, taskInfo.FileSize);
                if (!rinfo.State)
                    return APIReturnInfo<MultipartUploadResultValue>.Error(rinfo.Message);

                var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                    ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
                client.SetRegion(ossSetting.Value.Region);

                var request = new UploadPartRequest(ossSetting.Value.BucketName, taskInfo.ObjectKey, taskInfo.TaskId)
                {
                    InputStream = fileStream,
                    PartSize = taskInfo.PartSize,
                    PartNumber = taskInfo.CurrentPart
                };
                var result = client.UploadPart(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<MultipartUploadResultValue>.Error("上传失败");

                if (taskInfo.CurrentPart < taskInfo.TotalPart)
                {
                    taskInfo.CurrentPart++;

                    return APIReturnInfo<MultipartUploadResultValue>.Success(new MultipartUploadResultValue()
                    {
                        TaskId = taskInfo.ToConfig().AESEncrypt(ossSetting.Value.SecretKey),
                        Total = taskInfo.TotalPart,
                        Current = taskInfo.CurrentPart
                    });
                }

                CompleteMultipartUploadRequest completeRequest =
                    new CompleteMultipartUploadRequest(ossSetting.Value.BucketName, taskInfo.ObjectKey,
                        taskInfo.TaskId);

                ListPartsRequest listrequest =
                    new ListPartsRequest(ossSetting.Value.BucketName, taskInfo.ObjectKey, taskInfo.TaskId);

                PartListing listresponse;
                do
                {
                    listresponse = client.ListParts(listrequest);
                    foreach (var part in listresponse.Parts)
                    {
                        completeRequest.PartETags.Add(new PartETag(part.PartNumber, part.ETag));
                    }

                    listrequest.PartNumberMarker = listresponse.NextPartNumberMarker;
                } while (listresponse.IsTruncated);

                CompleteMultipartUploadResult? completeUploadResponseResult =
                    client.CompleteMultipartUpload(completeRequest);
                if (completeUploadResponseResult.HttpStatusCode != HttpStatusCode.OK)
                {
                    this.CancelMultipartUpload(client, ossSetting.Value.BucketName, taskInfo.ObjectKey,
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
                    RelativelyUrl = $"{ossSetting.Value.RootUrl}/{taskInfo.ObjectKey}"
                });

                this.SaveChange();

                return APIReturnInfo<MultipartUploadResultValue>.Success(new MultipartUploadResultValue(taskInfo.TaskId,
                    taskInfo.TotalPart, taskInfo.CurrentPart, taskInfo.SavePath, taskInfo.FileName,
                    taskInfo.FileSize, taskInfo.UploadType, attachmentInfo.Id)
                {
                    RootUrl = ossSetting.Value.RootUrl,
                    SaveFileName = taskInfo.SaveFileName
                });
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
                return APIReturnInfo<MultipartUploadResultValue>.Error(ex.Message);
            }
        }

        /// <summary>
        /// 取消分片上传
        /// </summary>
        private void CancelMultipartUpload(OssClient client, string bucketName, string objectKey, string uploadId)
        {
            //取消分段上传任务
            try
            {
                AbortMultipartUploadRequest request =
                    new AbortMultipartUploadRequest(ossSetting.Value.BucketName, objectKey, uploadId);
                client.AbortMultipartUpload(request);
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
            }
        }
    }
}