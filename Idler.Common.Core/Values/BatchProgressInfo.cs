using System;
using System.Collections.Generic;
using System.Linq;

namespace Idler.Common.Core
{
    public class BatchProgressInfo
    {
        /// <summary>
        /// 成功信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static BatchProgressInfo Success(string message)
        {
            return new BatchProgressInfo()
            {
                State = true,
                Message = message
            };
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static BatchProgressInfo Error(string message)
        {
            return new BatchProgressInfo()
            {
                State = false,
                Message = message
            };
        }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 验证返回的信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 批次名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所有批次内信息数量的累加
        /// </summary>
        public int Total { get; set; } = 0;

        /// <summary>
        /// 批次数量
        /// </summary>
        public int BatchCount { get; set; } = 1;

        /// <summary>
        /// 当前批次
        /// </summary>
        public int CurrentBatch { get; set; } = 1;

        /// <summary>
        /// 是否为最后一批
        /// </summary>
        public bool LastBatch => this.BatchCount == this.CurrentBatch;
    }
}