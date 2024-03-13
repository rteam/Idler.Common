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
        /// <param name="configName"></param>
        public LocalConfigurationAccessHelper(string configName)
        {
            this.ConfigName = configName.IsEmpty() ? typeof(TValue).Name : configName;
        }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string ConfigPath { get; set; }

        private string _configName;

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string ConfigName
        {
            get => _configName;
            private set
            {
                _configName = value;
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
        public void Replace(string config)
        {
            if (config.IsEmpty())
                return;

            File.WriteAllText(this.ConfigPath, config);
            this.Load(config);
        }

        private TValue _configEntity;

        /// <summary>
        /// 配置对象实体
        /// </summary>
        public TValue ConfigEntity
        {
            get
            {
                if (this._configEntity == null)
                    this.Load();

                return this._configEntity;
            }
            private set => this._configEntity = value;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            string config = File.ReadAllText(this.ConfigPath);
            this.Load(config);
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="config"></param>
        private void Load(string config)
        {
            this.ConfigEntity = new TValue().FromConfig(config);
        }

        #endregion
    }
}