namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：创建
/// </summary>
public interface IHasCreateOffset : IHasCreateTimeOffset, IHasCreateUser
{
}