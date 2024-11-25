using System;
using Idler.Common.Core.Config;
using Idler.Common.Core.Upload;

namespace Idler.Common.Attachments
{
    public class MultipartUploadTaskValue : AutoBaseConfig<MultipartUploadTaskValue>
    {
        public MultipartUploadTaskValue() { }

        public MultipartUploadTaskValue(string taskId, string uploadType, string DependentId, long partSize, UploadFilInfo FileInfo)
        {
            this.TaskId = taskId;
            this.DependentId = DependentId;
            this.UploadType = uploadType;
            this.PartSize = partSize;
            this.FileName = FileInfo.FileName;
            this.FileSize = FileInfo.FileSize;
            this.FileExt = FileInfo.FileExt;
            this.SaveFileName = FileInfo.SaveFileName;
            this.SavePath = FileInfo.SavePath;
            this.ObjectKey = $"{this.SavePath}{this.SaveFileName}";


            this.TotalPart = (int)Math.Floor((decimal)this.FileSize / (decimal)partSize);
            if (this.TotalPart * partSize < this.FileSize)
                this.TotalPart++;
        }
        /// <summary>
        /// 依赖Id
        /// </summary>
        public string DependentId { get; set; }

        /// <summary>
        /// 分片任务上传Id
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// ObjectKey
        /// </summary>
        public string ObjectKey { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; set; }
        /// <summary>
        /// 保存文件名
        /// </summary>
        public string SaveFileName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 分片大小
        /// </summary>
        public long PartSize { get; set; }
        /// <summary>
        /// 当前传输分片
        /// </summary>
        public int CurrentPart { get; set; } = 1;
        /// <summary>
        /// 最大分片数
        /// </summary>
        public int TotalPart { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string FileExt { get; set; }
        /// <summary>
        /// 上传对象类型
        /// </summary>
        public string UploadType { get; set; }

    }
}
