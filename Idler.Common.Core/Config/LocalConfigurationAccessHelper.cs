using System;
using System.IO;

namespace Idler.Common.Core.Config
{
    /// <summary>
    /// 本地帮助文件访问助手
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class LocalConfigurationAccessHelper<TValue> : IConfigAccessHelper<TValue>
        where TValue : class, IConfigBase<TValue>, new()
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LocalConfigurationAccessHelper()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 加载制定配置
        /// </summary>
        /// <param name="ConfigName"></param>
        public LocalConfigurationAccessHelper(string ConfigName)
        {
            this.ConfigName = ConfigName.IsEmpty() ? typeof(TValue).Name : ConfigName;
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string ConfigPath { get; set; }

        private string configName;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string ConfigName
        {
            get { return configName; }
            private set
            {
                configName = value;
                ConfigPath = Path.GetFullPath(Path.Combine(".", "Config", $"{value}.config"));
            }
        }

        #region IConfigAccessHelper

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public void Save()
        {
            File.WriteAllText(this.ConfigPath, this.ConfigEntity.ToConfig());
        }

        /// <summary>
        /// 替换配置文件
        /// </summary>
        public void Replace(string Config)
        {
            if (Config.IsEmpty())
                return;

            File.WriteAllText(this.ConfigPath, Config);
            this.Load(Config);
        }

        private TValue _ConfigEntity;

        /// <summary>
        /// 配置对象实体
        /// </summary>
        public TValue ConfigEntity
        {
            get
            {
                if (this._ConfigEntity == null)
                    this.Load();

                return this._ConfigEntity;
            }
            protected set { this._ConfigEntity = value; }
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            string ConfigStr = File.ReadAllText(this.ConfigPath);
            this.Load(ConfigStr);
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="Config"></param>
        private void Load(string Config)
        {
            this.ConfigEntity = new TValue().FromConfig(Config);
        }

        #endregion
    }
}