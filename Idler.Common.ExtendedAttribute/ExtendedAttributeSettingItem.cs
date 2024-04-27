using System;
using System.ComponentModel.DataAnnotations;

namespace Idler.Common.ExtendedAttribute
{
    public class ExtendedAttributeSettingItem
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = ExtendedAttributeTypes.SingleText.ToString();
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(50, ErrorMessage = "字段描述不能超过100个字")]
        [Display(Name = "字段描述")]
        public string Description { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        [Required(ErrorMessage = "必须设置Key")]
        [StringLength(50, ErrorMessage = "Key不能超过128个字")]
        [Display(Name = "Key")]
        public string Key { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "必须设置名称")]
        [StringLength(50, ErrorMessage = "名称不能超过50个字")]
        [Display(Name = "名称")]
        public string Name { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        [Display(Name = "默认值")]
        [StringLength(1000, ErrorMessage = "最长不能超过1000个字")]
        public string DefaultValue { get; set; }
        /// <summary>
        /// 数据字典
        /// </summary>
        [Display(Name = "数据字典")]
        [StringLength(100, ErrorMessage = "数据字典长度不能超过100个字")]
        public string DataDictionary { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序项")]
        [Required(ErrorMessage = "排序项必须存在")]
        public int OrderBy { get; set; }
        /// <summary>
        /// 最长长度
        /// </summary>
        [Required(ErrorMessage = "请设置最长长度")]
        [Range(0, 500, ErrorMessage = "值必须在0-500之间")]
        [Display(Name = "字段最长长度")]
        public int MaxLength { get; set; }
        /// <summary>
        /// 必填项
        /// </summary>
        [Display(Name = "是否为必填项")]
        public bool Must { get; set; }
        /// <summary>
        /// 格式验证
        /// </summary>
        [Display(Name = "扩展验证设置")]
        [StringLength(1000, ErrorMessage = "扩展验证设置不能超过1000个字节")]
        public string Extension { get; set; }
        /// <summary>
        /// 出错时显示的信息
        /// </summary>
        [Display(Name = "出错时显示的信息")]
        [StringLength(300, ErrorMessage = "出错信息不能超过300个字节")]
        public string ExtensionErrorMessage { get; set; }
    }
}