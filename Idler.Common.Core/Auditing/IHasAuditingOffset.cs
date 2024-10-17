namespace Idler.Common.Core.Auditing;

/// <summary>
/// 审计接口：创建、编辑、删除
/// </summary>
public interface IHasAuditingOffset: IHasCreateTimeOffset, IHasCreateUser, IHasModifyTimeOffset, IHasModifyUser, IHasDeleteTimeOffset,
    IHasDeleteUser
{
}