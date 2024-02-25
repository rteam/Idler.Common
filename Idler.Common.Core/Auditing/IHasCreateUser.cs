namespace Idler.Common.Core.Auditing
{
    /// <summary>
    /// 审计接口：创建执行人信息
    /// </summary>
    public interface IHasCreateUser
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        string CreateUser { get; set; }
    }
}