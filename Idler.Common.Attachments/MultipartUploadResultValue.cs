using System;
using Idler.Common.Core.Upload;

namespace Idler.Common.Attachments
{
    /// <summary>
    /// 分片上传返回对象
    /// </summary>
    public class MultipartUploadResultValue : UploadFilInfo
    {
        public MultipartUploadResultValue()
        {
        }

        public MultipartUploadResultValue(string TaskId, int Total, int Current, string rootPath, string fileName,
            long fileSize, string uploadType, Guid id)
            : base(rootPath, fileName, fileSize, uploadType, id)
        {
            this.TaskId = TaskId;
            this.Total = Total;
            this.Current = Current;
        }

        /// <summary>
        /// 分片任务上传Id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前批次
        /// </summary>
        public int Current { get; set; }
    }
}