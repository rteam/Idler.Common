using System;
using System.ComponentModel.DataAnnotations;

namespace Idler.Common.Core
{
    public class Dependent
    {
        public Dependent(Guid dependentId, string dependentType, string dependentName)
            : this(dependentId.ToString(), dependentType, dependentName)
        {

        }

        public Dependent(string dependentId, string dependentType)
            : this(dependentId, dependentType, "无")
        {

        }

        public Dependent(string dependentId, string dependentType, string dependentName)
        {
            this.DependentId = dependentId;
            this.DependentType = dependentType;
            this.DependentName = dependentName;
        }

        public Dependent(string dependent)
        {
            if (dependent.IsEmpty())
                return;

            string[] arr = dependent.Split('%');
            if (arr.Length != 2)
                return;

            this.DependentId = arr[0];
            this.DependentType = arr[1];
        }

        public Dependent()
        {
        }

        /// <summary>
        /// 依赖Id
        /// </summary>
        [StringLength(255)]
        [Required(ErrorMessage = "一般系统保护性错误")]
        public string DependentId { get; set; }
        /// <summary>
        /// 依赖类型
        /// </summary>
        [StringLength(255)]
        [Required(ErrorMessage = "一般系统保护性错误")]
        public string DependentType { get; set; }
        /// <summary>
        /// 依赖名称
        /// </summary>
        [StringLength(150)]
        public string DependentName { get; set; }
        /// <summary>
        /// 拼接字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(this.DependentId, "%", this.DependentType);
        }
    }
}