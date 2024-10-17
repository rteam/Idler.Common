using System;
using System.ComponentModel.DataAnnotations;
using Idler.Common.Core.Domain;

namespace Idler.Common.Attachments
{
    public abstract class BaseAttachment : EntityWithGuidNoTrace, ISoftDelete
    {

        /// <summary>
        /// 完整文件路径
        /// </summary>
        [StringLength(255)]
        public virtual string FullPath { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 文件名（不含扩展名）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        [Required]
        [StringLength(300)]
        public string FilePath { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        [StringLength(100)]
        public string FileType { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FileExt { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        [Required]
        public long FileSize { get; set; }
        /// <summary>
        /// 同步状态
        /// </summary>
        public bool SyncState { get; set; } = false;
        /// <summary>
        /// 相对访问地址
        /// </summary>
        [Required]
        [StringLength(500)]
        public string RelativelyUrl { get; set; }
        /// <summary>
        /// 是否标记为删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 文件哈希码
        /// </summary>
        [StringLength(255)]
        public string HashCode { get; set; }
        /// <summary>
        /// 上传类型
        /// </summary>
        public string UploadType { get; set; }
        /// <summary>
        /// 附件依赖Id
        /// </summary>
        public string DependentId { get; set; }
    }
}
