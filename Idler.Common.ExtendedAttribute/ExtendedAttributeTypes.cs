using System.ComponentModel;

namespace Idler.Common.ExtendedAttribute
{
    /// <summary>
    /// 扩展属性类型
    /// </summary>
    public enum ExtendedAttributeTypes
    {
        [Description("单行文本")] SingleText,
        [Description("数字")] Number,
        [Description("密文")] Password,
        [Description("多行文本")] MultiText,
        [Description("附件")] Attachment,
        [Description("下拉列表")] Select,
        [Description("日期")] Date,
        [Description("复选")] Check,
        [Description("单选")] Radio,
        [Description("开关")] Switch
    }
}