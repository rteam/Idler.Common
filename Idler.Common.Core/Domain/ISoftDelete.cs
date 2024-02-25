namespace Idler.Common.Core.Domain
{
    /// <summary>
    /// 实现此接口代表此对象支持软删除
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 此对象已被标记为删除
        /// </summary>
        bool IsDelete { get; set; }
    }
}