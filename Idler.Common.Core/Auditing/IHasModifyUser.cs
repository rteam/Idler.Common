namespace Idler.Common.Core.Auditing
{
    /// <summary>
    /// 审计接口：修改执行人信息
    /// </summary>
    public interface IHasModifyUser
    {
        /// <summary>
        /// 修改时间
        /// </summary>
        string ModifyUser { get; set; }
    }
}