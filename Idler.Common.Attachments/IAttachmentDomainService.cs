using System;
using System.Collections.Generic;
using System.IO;
using Idler.Common.Core;
using Idler.Common.Core.Upload;

namespace Idler.Common.Attachments
{
    /// <summary>
    /// 附件服务
    /// </summary>
    public interface IAttachmentDomainService : IDomainService
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="addInfo"></param>
        /// <returns></returns>
        APIReturnInfo<AttachmentValue> Create(Attachment addInfo);

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="editInfo"></param>
        /// <returns></returns>
        APIReturnInfo<AttachmentValue> Edit(Attachment editInfo);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="removeId"></param>
        /// <returns></returns>
        APIReturnInfo<AttachmentValue> Remove(Guid removeId);

        /// <summary>
        /// 取得特定信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Attachment Single(Guid id);

        /// <summary>
        /// 搜索附件
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="uploadType"></param>
        /// <param name="dependentId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        APIReturnInfo<ReturnPaging<AttachmentValue>> Search(string keyword, string uploadType, string dependentId,
            int pageNum = 1, int pageSize = 20);

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, string dependentId);

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">附件Id</param>
        /// <returns></returns>
        APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, Guid dependentId);

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">附件Id</param>
        /// <returns></returns>
        APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, int dependentId);

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey);

        /// <summary>
        /// 从服务器删除附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        APIReturnInfo<string> RemoveFromServer(string uploadType, string dependentId);

        /// <summary>
        /// 从服务器删除指定附件
        /// </summary>
        /// <param name="removeId"></param>
        /// <returns></returns>
        APIReturnInfo<string> RemoveFromServer(Guid removeId);

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, string dependentId);

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, Guid dependentId);

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, int dependentId);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="uploadType">上传类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileSize">文件大小</param>
        /// <param name="fileStream">文件流</param>
        /// <returns></returns>
        APIReturnInfo<UploadFilInfo> Upload(string uploadType, string dependentId, string fileName, long fileSize,
            Stream fileStream);

        /// <summary>
        /// 初始化大文件上传任务
        /// </summary>
        /// <returns></returns>
        APIReturnInfo<string> InitializeLargeFileUploadTask(string uploadType, string dependentId, string fileName,
            long partSize, long fileSize);

        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="taskId">任务Id</param>
        /// <param name="fileStream">待上传文件流</param>
        /// <returns></returns>
        APIReturnInfo<MultipartUploadResultValue> Upload(string taskId, Stream fileStream);

        /// <summary>
        /// 上传配置文件
        /// </summary>
        /// <returns></returns>
        UploadConfig Config();
    }
}