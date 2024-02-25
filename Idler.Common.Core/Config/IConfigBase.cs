namespace Idler.Common.Core.Config
{
    /// <summary>
    /// 配置文件基础接口
    /// 用来序列化和反序列化配置对象
    /// </summary>
    /// <typeparam name="TVal">配置实体</typeparam>
    public interface IConfigBase<TVal>
    {
        /// <summary>
        /// 将配置实体保存为配置文件
        /// </summary>
        /// <returns>配置文件</returns>
        string ToConfig();
        /// <summary>
        /// 从配置文件构建配置实体
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <returns>配置实体</returns>
        TVal FromConfig(string config);
    }
}