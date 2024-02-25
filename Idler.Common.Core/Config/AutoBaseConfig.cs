using System;
using System.Text.Json;

namespace Idler.Common.Core.Config
{
    [Serializable]
    public abstract class AutoBaseConfig<TVal> : IConfigBase<TVal>
        where TVal : class, new()
    {
        /// <summary>
        /// 将配置文件转换为对象
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <returns></returns>
        public virtual TVal FromConfig(string config)
        {
            if (config.IsEmpty())
                return new TVal();

            return JsonSerializer.Deserialize<TVal>(config, JsonDefaultOptions.Instance) ?? new TVal();
        }

        /// <summary>
        /// 将对象转换为配置文件
        /// </summary>
        /// <returns></returns>
        public virtual string ToConfig()
        {
            return this.ToJSON();
        }
    }
}