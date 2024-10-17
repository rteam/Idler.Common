using System;

namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：创建时间
/// </summary>
public interface IHasCreateTimeOffset
{
    /// <summary>
    /// 创建时间
    /// </summary>
    DateTimeOffset? CreateTime { get; set; }
}