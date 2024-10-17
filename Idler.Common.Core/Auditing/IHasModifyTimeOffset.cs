using System;

namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：修改时间
/// </summary>
public interface IHasModifyTimeOffset
{
    /// <summary>
    /// 修改时间
    /// </summary>
    DateTimeOffset? ModifyTime { get; set; }
}