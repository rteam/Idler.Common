namespace Idler.Common.OSS
{
    public class OSSSetting
    {
        /// <summary>
        /// 桶名
        /// </summary>
        public string BucketName { get; set; }

        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// OBS根路径
        /// </summary>
        public string RootUrl { get; set; }

        /// <summary>
        /// 所在区域
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// AK
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// SK
        /// </summary>
        public string SecretKey { get; set; }
    }
}