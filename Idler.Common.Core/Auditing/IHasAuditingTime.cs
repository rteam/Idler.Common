namespace Idler.Common.Core.Auditing
{
    /// <summary>
    /// 时间审计
    /// </summary>
    public interface IHasAuditingTime : IHasCreateTime, IHasModifyTime, IHasDeleteTime
    {

    }
}