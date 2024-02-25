namespace Idler.Common.Core.Domain
{
    /// <summary>
    /// 领域实体接口
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey> : IDomain
    {
        /// <summary>
        /// Id
        /// </summary>
        TKey Id { get; }
    }
}