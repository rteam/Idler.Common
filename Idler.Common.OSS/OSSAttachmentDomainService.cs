using System.Collections;
using System.Net;
using System.Security.Cryptography;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using BCCore.DDD.Attachments;
using Idler.Common.Attachments;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Logging;
using Idler.Common.Core.Upload;
using Idler.Common.OBS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Idler.Common.OSS
{
    /// <summary>
    /// OSS上传服务
    /// </summary>
    /// <param name="AttachmentRepository"></param>
    /// <param name="logger"></param>
    /// <param name="uploadConfigAccessHelper"></param>
    /// <param name="ossSetting"></param>
    /// <param name="unitOfWork"></param>
    public class OSSAttachmentDomainService(
        IRepository<Attachment, Guid> AttachmentRepository,
        ILogger<OSSAttachmentDomainService> logger,
        IConfigAccessHelper<UploadConfig> uploadConfigAccessHelper,
        IOptions<OSSSetting> ossSetting,
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

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);

            try
            {
                var request = new DeleteObjectsRequest(ossSetting.Value.BucketName, FullPaths, false);

                var result = client.DeleteObjects(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("文件删除失败");

                if (result.Keys.Length == FullPaths.Count)
                    return APIReturnInfo<string>.Success("ok");

                return new APIReturnInfo<string>()
                    { State = false, Message = "有一些文件删除失败了" };
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
        /// <param name="RemoveId">要删除的附件Id</param>
        /// <returns></returns>
        public override APIReturnInfo<string> RemoveFromServer(Guid RemoveId)
        {
            if (RemoveId.IsEmpty())
                throw new ArgumentNullException(nameof(RemoveId));

            Attachment AttachmentInfo = this.AttachmentRepository.Single(RemoveId);
            if (AttachmentInfo == null)
                return APIReturnInfo<string>.Error("福建不存在");

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);
            try
            {
                DeleteObjectRequest request =
                    new DeleteObjectRequest(ossSetting.Value.BucketName, AttachmentInfo.FullPath);
                var result = client.DeleteObject(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
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
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        public override APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey)
        {
            if (ObjectKey.IsEmpty())
                throw new ArgumentNullException(nameof(ObjectKey));

            var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
            client.SetRegion(ossSetting.Value.Region);
            try
            {
                var generatePresignedUriRequest =
                    new GeneratePresignedUriRequest(ossSetting.Value.BucketName, ObjectKey, SignHttpMethod.Put)
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
                var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                    ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
                client.SetRegion(ossSetting.Value.Region);

                var result = client.PutObject(ossSetting.Value.BucketName,
                    $"{FileInfo.SavePath}{FileInfo.SaveFileName}", fileStream);

                if (result.HttpStatusCode != HttpStatusCode.OK)
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
                        $"{ossSetting.Value.RootUrl}/{FileInfo.SavePath}{FileInfo.SaveFileName}"
                });

                this.SaveChange();

                return APIReturnInfo<UploadFilInfo>.Success(new UploadFilInfo(FileInfo.RootPath,
                    FileInfo.FileName, FileInfo.FileSize, UploadType, AttachmentInfo.Id));
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
                var client = new OssClient(ossSetting.Value.EndPoint, ossSetting.Value.AccessKey,
                    ossSetting.Value.SecretKey, new ClientConfiguration() { SignatureVersion = SignatureVersion.V4 });
                client.SetRegion(ossSetting.Value.Region);

                InitiateMultipartUploadRequest request = new InitiateMultipartUploadRequest(ossSetting.Value.BucketName,
                    $"{FileInfo.SavePath}{FileInfo.SaveFileName}");
                var result = client.InitiateMultipartUpload(request);
                if (result.HttpStatusCode != HttpStatusCode.OK)
                    return APIReturnInfo<string>.Error("上传失败");

                string taskKeyConfig =
                    new MultipartUploadTaskValue(result.UploadId, UploadType, DependentId, PartSize, FileInfo)
                        .ToConfig();

                return new APIReturnInfo<string>()
                {
                    State = true, Data = taskKeyConfig.AESEncrypt(ossSetting.Value.SecretKey)
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
                string Config = TaskId.AESDecrypt(ossSetting.Value.SecretKey);
                if (Config == "err")
                    return APIReturnInfo<MultipartUploadResultValue>.Error("任务不存在");

                MultipartUploadTaskValue taskInfo = new MultipartUploadTaskValue().FromConfig(Config);
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
                    RelativelyUrl = $"{ossSetting.Value.RootUrl}/{taskInfo.ObjectKey}"
                });

                this.SaveChange();

                return APIReturnInfo<MultipartUploadResultValue>.Success(new MultipartUploadResultValue(taskInfo.TaskId,
                    taskInfo.TotalPart, taskInfo.CurrentPart, taskInfo.SavePath, taskInfo.FileName,
                    taskInfo.FileSize, taskInfo.UploadType, AttachmentInfo.Id));
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
        private void CancelMultipartUpload(OssClient client, string BucketName, string ObjectKey, string UploadId)
        {
            //取消分段上传任务
            try
            {
                AbortMultipartUploadRequest request =
                    new AbortMultipartUploadRequest(ossSetting.Value.BucketName, ObjectKey, UploadId);
                client.AbortMultipartUpload(request);
            }
            catch (OssException ex)
            {
                logger.HandleException<OssException>(ex);
            }
        }
    }
}