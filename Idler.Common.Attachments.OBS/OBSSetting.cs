using System;
using System.Security.Cryptography;
using System.Text;
using Idler.Common.Core.Config;

namespace Idler.Common.Attachments.OBS
{
    public class OBSSetting
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
        /// AK
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// SK
        /// </summary>
        public string SecretKey { get; set; }

    }
}