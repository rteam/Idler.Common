namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：删除
/// </summary>
public interface IHasDeleteOffset : IHasDeleteTimeOffset, IHasDeleteUser
{
}