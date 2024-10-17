namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：修改
/// </summary>
public interface IHasModifyOffset : IHasModifyTimeOffset, IHasModifyUser
{
}