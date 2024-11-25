using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Idler.Common.AutoMapper;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Specification;
using Idler.Common.Core.Upload;
using Microsoft.Extensions.Options;

namespace Idler.Common.Attachments
{
    /// <summary>
    /// 基础附件服务实现
    /// </summary>
    public abstract class BaseAttachmentDomainService(
        IRepository<Attachment, Guid> attachmentRepository,
        IOptions<UploadConfig> uploadConfigAccessHelper,
        IUnitOfWork unitOfWork)
        : BaseDomainService(unitOfWork), IAttachmentDomainService
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="addInfo"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Create(AttachmentValue addInfo)
        {
            if (addInfo == null)
                throw new ArgumentNullException(nameof(addInfo));

            Attachment attachment = addInfo.Map<AttachmentValue, Attachment>();
            attachment = attachmentRepository.Add(attachment);
            this.SaveChange();
            return APIReturnInfo<AttachmentValue>.Success(attachment.Map<Attachment, AttachmentValue>());
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="editInfo">要编辑的信息</param>
        /// <param name="id">要编辑信息的Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Edit(AttachmentValue editInfo, Guid id)
        {
            if (editInfo == null)
                throw new ArgumentNullException("editInfo");

            Attachment attachment = attachmentRepository.Single(id);
            if (attachment == null)
                return APIReturnInfo<AttachmentValue>.Error("要编辑的信息不存在");

            editInfo.Map(attachment);

            attachmentRepository.Update(attachment);
            this.SaveChange();

            return APIReturnInfo<AttachmentValue>.Success(attachment.Map<Attachment, AttachmentValue>());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Remove(Guid id)
        {
            if (id.IsEmpty())
                throw new ArgumentNullException("id");

            Attachment attachmentInfo = attachmentRepository.Remove(id);
            this.SaveChange();

            return APIReturnInfo<AttachmentValue>.Success(attachmentInfo.Map<Attachment, AttachmentValue>());
        }

        /// <summary>
        /// 取得特定信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Attachment Single(Guid id)
        {
            if (id.IsEmpty())
                throw new ArgumentNullException("id");

            return attachmentRepository.Single(id);
        }

        /// <summary>
        /// 搜索附件
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="uploadType"></param>
        /// <param name="dependentId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<ReturnPaging<AttachmentValue>> Search(string keyword, string uploadType,
            string dependentId, int pageNum = 1, int pageSize = 20)
        {
            if (uploadType.IsEmpty())
                throw new ArgumentNullException("uploadType");

            if (dependentId.IsEmpty())
                throw new ArgumentNullException("dependentId");


            ISpecification<Attachment> spec =
                new Specification<Attachment>(t => t.DependentId == dependentId && t.UploadType == uploadType);
            if (!keyword.IsEmpty())
                spec = spec.And(t => t.Name.Contains(keyword));

            ReturnPaging<AttachmentValue> paging = new ReturnPaging<AttachmentValue>()
            {
                PageSize = pageSize,
                PageNum = pageNum,
                Total = attachmentRepository.Find(spec.Expressions).Count()
            };

            paging.Compute();
            paging.PageListInfos = attachmentRepository.Find(spec.Expressions).OrderBy(t => t.CreateDate)
                .Skip(paging.Skip).Take(paging.Take).ProjectTo<AttachmentValue>()
                .ToList();

            return APIReturnInfo<ReturnPaging<AttachmentValue>>.Success(paging);
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, string dependentId)
        {
            if (uploadType.IsEmpty())
                throw new ArgumentNullException("uploadType");

            if (dependentId.IsEmpty())
                throw new ArgumentNullException("dependentId");

            return APIReturnInfo<IList<AttachmentValue>>.Success(attachmentRepository
                .Find(t => t.DependentId == dependentId && t.UploadType == uploadType)
                .ProjectTo<AttachmentValue>().ToList());
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">附件Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, Guid dependentId)
        {
            return this.Find(uploadType, dependentId.ToString());
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">附件Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string uploadType, int dependentId)
        {
            return this.Find(uploadType, dependentId.ToString());
        }

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="objectKey">ObjectKey</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> GenerateUploadAuthorizationUrl(string objectKey);

        /// <summary>
        /// 从服务器删除附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> RemoveFromServer(string uploadType, string dependentId);

        /// <summary>
        /// 从服务器删除指定附件
        /// </summary>
        /// <param name="removeId">要删除的附件Id</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> RemoveFromServer(Guid removeId);

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, string dependentId)
        {
            if (uploadType.IsEmpty())
                throw new ArgumentNullException(nameof(uploadType));

            if (tempDependentId.IsEmpty())
                throw new ArgumentNullException(nameof(tempDependentId));

            if (dependentId.IsEmpty())
                throw new ArgumentNullException(nameof(dependentId));

            IList<Attachment> attachments = attachmentRepository
                .Find(t => t.UploadType == uploadType && t.DependentId == tempDependentId).ToList();
            if (attachments == null || attachments.Count == 0)
                return APIReturnInfo<string>.Success("ok");

            attachments.ForEach(t => { t.DependentId = dependentId; });

            this.SaveChange();

            return APIReturnInfo<string>.Success("ok");
        }

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, Guid dependentId)
        {
            return this.HandleTemp(uploadType, tempDependentId, dependentId.ToString());
        }

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="uploadType">附件类型</param>
        /// <param name="tempDependentId">临时依赖Id</param>
        /// <param name="dependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string uploadType, string tempDependentId, int dependentId)
        {
            return this.HandleTemp(uploadType, tempDependentId, dependentId.ToString());
        }

        /// <summary>
        /// 上传配置文件
        /// </summary>
        /// <returns></returns>
        public virtual UploadConfig Config()
        {
            return uploadConfigAccessHelper.Value;
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
        public abstract APIReturnInfo<UploadFilInfo> Upload(string uploadType, string dependentId, string fileName,
            long fileSize, Stream fileStream);

        /// <summary>
        /// 初始化大文件上传任务
        /// </summary>
        /// <returns></returns>
        public abstract APIReturnInfo<string> InitializeLargeFileUploadTask(string uploadType, string dependentId,
            string fileName, long partSize, long fileSize);

        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="taskId">任务Id</param>
        /// <param name="fileStream">待上传文件流</param>
        /// <returns></returns>
        public abstract APIReturnInfo<MultipartUploadResultValue> Upload(string taskId, Stream fileStream);
    }
}