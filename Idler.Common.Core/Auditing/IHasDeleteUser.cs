namespace Idler.Common.Core.Auditing
{
    /// <summary>
    /// 审计接口：删除执行人信息
    /// </summary>
    public interface IHasDeleteUser
    {
        /// <summary>
        /// 删除时间
        /// </summary>
        string DeleteUser { get; set; }
    }
}