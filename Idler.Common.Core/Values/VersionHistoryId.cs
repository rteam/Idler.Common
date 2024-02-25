namespace Idler.Common.Core
{
    public class VersionHistoryId
    {
        public VersionHistoryId(string version, string dependentId, string dependentType)
        {
            this.Version = version;
            this.Dependent = new Dependent(dependentId, dependentType);
        }

        public VersionHistoryId(string version, string dependent)
        {
            this.Version = version;
            this.Dependent = new Dependent(dependent);
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 依赖
        /// </summary>
        public Dependent Dependent { get; set; }
        public override string ToString()
        {
            return string.Concat(this.Version, "%", this.Dependent.DependentId, "%", this.Dependent.DependentType);
        }
    }
}