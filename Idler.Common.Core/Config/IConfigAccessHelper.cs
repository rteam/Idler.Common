namespace Idler.Common.Core.Config
{
    /// <summary>
    /// 配置信息（聚合根）仓储接口
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IConfigAccessHelper<TValue>
        where TValue : class, IConfigBase<TValue>, new()
    {
        /// <summary>
        /// 保存配置文件
        /// </summary>
        void Save();
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        void Load();
        /// <summary>
        /// 替换配置文件
        /// </summary>
        void Replace(string config);
        /// <summary>
        /// 配置文件实体
        /// </summary>
        TValue ConfigEntity { get; }
        /// <summary>
        /// 配置文件名称
        /// </summary>
        string ConfigName { get; }
    }
}