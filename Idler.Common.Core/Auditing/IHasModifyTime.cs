using System;

namespace Idler.Common.Core.Auditing
{
    /// <summary>
    /// 审计接口：修改时间
    /// </summary>
    public interface IHasModifyTime
    {
        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime? ModifyTime { get; set; }
    }
}