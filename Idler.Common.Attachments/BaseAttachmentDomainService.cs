using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Idler.Common.Attachments;
using Idler.Common.AutoMapper;
using Idler.Common.Core;
using Idler.Common.Core.Config;
using Idler.Common.Core.Specification;
using Idler.Common.Core.Upload;

namespace BCCore.DDD.Attachments
{
    /// <summary>
    /// 基础附件服务实现
    /// </summary>
    public abstract class BaseAttachmentDomainService : BaseDomainService, IAttachmentDomainService
    {
        public BaseAttachmentDomainService(
            IRepository<Attachment, Guid> AttachmentRepository,
            IConfigAccessHelper<UploadConfig> UploadConfigAccessHelper,
            IUnitOfWork UnitOfWork)
            : base(UnitOfWork)
        {
            this.AttachmentRepository = AttachmentRepository;
            this.UploadConfigAccessHelper = UploadConfigAccessHelper;
        }

        protected readonly IConfigAccessHelper<UploadConfig> UploadConfigAccessHelper;
        protected readonly IRepository<Attachment, Guid> AttachmentRepository;

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="AddInfo"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Create(Attachment AddInfo)
        {
            if (AddInfo == null)
                throw new ArgumentNullException(nameof(AddInfo));

            AddInfo = this.AttachmentRepository.Add(AddInfo);
            this.SaveChange();
            return APIReturnInfo<AttachmentValue>.Success(AddInfo);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="EditInfo"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Edit(Attachment EditInfo)
        {
            if (EditInfo == null)
                throw new ArgumentNullException("EditInfo");

            this.AttachmentRepository.Update(EditInfo);
            this.SaveChange();

            return APIReturnInfo<AttachmentValue>.Success(EditInfo);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="RemoveId"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<AttachmentValue> Remove(Guid RemoveId)
        {
            if (RemoveId.IsEmpty())
                throw new ArgumentNullException("RemoveId");

            Attachment AttachmentInfo = this.AttachmentRepository.Remove(RemoveId);
            this.SaveChange();

            return APIReturnInfo<AttachmentValue>.Success(AttachmentInfo);
        }

        /// <summary>
        /// 取得特定信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual Attachment Single(Guid Id)
        {
            if (Id.IsEmpty())
                throw new ArgumentNullException("Id");

            return this.AttachmentRepository.Single(Id);
        }

        /// <summary>
        /// 搜索附件
        /// </summary>
        /// <param name="Keyword"></param>
        /// <param name="UploadType"></param>
        /// <param name="DependentId"></param>
        /// <param name="PageNum"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public virtual APIReturnInfo<ReturnPaging<AttachmentValue>> Search(string Keyword, string UploadType,
            string DependentId, int PageNum = 1, int PageSize = 20)
        {
            if (UploadType.IsEmpty())
                throw new ArgumentNullException("UploadType");

            if (DependentId.IsEmpty())
                throw new ArgumentNullException("DependentId");


            ISpecification<Attachment> Spec =
                new Specification<Attachment>(t => t.DependentId == DependentId && t.UploadType == UploadType);
            if (!Keyword.IsEmpty())
                Spec = Spec.And(t => t.Name.Contains(Keyword));

            ReturnPaging<AttachmentValue> Paging = new ReturnPaging<AttachmentValue>()
            {
                PageSize = PageSize,
                PageNum = PageNum,
                Total = this.AttachmentRepository.Find(Spec.Expressions).Count()
            };

            Paging.Compute();
            Paging.PageListInfos = this.AttachmentRepository.Find(Spec.Expressions).OrderBy(t => t.CreateDate)
                .Skip(Paging.Skip).Take(Paging.Take).ProjectTo<AttachmentValue>()
                .ToList();

            return APIReturnInfo<ReturnPaging<AttachmentValue>>.Success(Paging);
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string UploadType, string DependentId)
        {
            if (UploadType.IsEmpty())
                throw new ArgumentNullException("UploadType");

            if (DependentId.IsEmpty())
                throw new ArgumentNullException("DependentId");

            return APIReturnInfo<IList<AttachmentValue>>.Success(this.AttachmentRepository
                .Find(t => t.DependentId == DependentId && t.UploadType == UploadType)
                .ProjectTo<AttachmentValue>().ToList());
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="DependentId">附件Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string UploadType, Guid DependentId)
        {
            return this.Find(UploadType, DependentId.ToString());
        }

        /// <summary>
        /// 查找附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="DependentId">附件Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<IList<AttachmentValue>> Find(string UploadType, int DependentId)
        {
            return this.Find(UploadType, DependentId.ToString());
        }

        /// <summary>
        /// 生成上传指定文件的授权地址
        /// </summary>
        /// <param name="ObjectKey">ObjectKey</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> GenerateUploadAuthorizationUrl(string ObjectKey);
        
        /// <summary>
        /// 从服务器删除附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> RemoveFromServer(string UploadType, string DependentId);

        /// <summary>
        /// 从服务器删除指定附件
        /// </summary>
        /// <param name="RemoveId">要删除的附件Id</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> RemoveFromServer(Guid RemoveId);

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="TempDependentId">临时依赖Id</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string UploadType, string TempDependentId, string DependentId)
        {
            if (UploadType.IsEmpty())
                throw new ArgumentNullException(nameof(UploadType));

            if (TempDependentId.IsEmpty())
                throw new ArgumentNullException(nameof(TempDependentId));

            if (DependentId.IsEmpty())
                throw new ArgumentNullException(nameof(DependentId));

            IList<Attachment> Attachments = this.AttachmentRepository
                .Find(t => t.UploadType == UploadType && t.DependentId == TempDependentId).ToList();
            if (Attachments == null || Attachments.Count == 0)
                return APIReturnInfo<string>.Success("ok");

            Attachments.ForEach(t => { t.DependentId = DependentId; });

            this.SaveChange();

            return APIReturnInfo<string>.Success("ok");
        }

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="TempDependentId">临时依赖Id</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string UploadType, string TempDependentId, Guid DependentId)
        {
            return this.HandleTemp(UploadType, TempDependentId, DependentId.ToString());
        }

        /// <summary>
        /// 处理临时附件
        /// </summary>
        /// <param name="UploadType">附件类型</param>
        /// <param name="TempDependentId">临时依赖Id</param>
        /// <param name="DependentId">依赖Id</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> HandleTemp(string UploadType, string TempDependentId, int DependentId)
        {
            return this.HandleTemp(UploadType, TempDependentId, DependentId.ToString());
        }

        /// <summary>
        /// 上传配置文件
        /// </summary>
        /// <returns></returns>
        public virtual UploadConfig Config()
        {
            return this.UploadConfigAccessHelper.ConfigEntity;
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
        public abstract APIReturnInfo<UploadFilInfo> Upload(string UploadType, string DependentId, string FileName,
            long FileSize, Stream fileStream);

        /// <summary>
        /// 初始化大文件上传任务
        /// </summary>
        /// <returns></returns>
        public abstract APIReturnInfo<string> InitializeLargeFileUploadTask(string UploadType, string DependentId,
            string FileName, long PartSize, long FileSize);

        /// <summary>
        /// 分块上传文件
        /// </summary>
        /// <param name="TaskId">任务Id</param>
        /// <param name="fileStream">待上传文件流</param>
        /// <returns></returns>
        public abstract APIReturnInfo<MultipartUploadResultValue> Upload(string TaskId, Stream fileStream);
    }
}