using System;

namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：删除时间
/// </summary>
public interface IHasDeleteTimeOffset
{
    /// <summary>
    /// 删除时间
    /// </summary>
    DateTimeOffset? DeleteTime { get; set; }
}