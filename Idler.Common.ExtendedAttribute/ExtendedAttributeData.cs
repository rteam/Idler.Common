using System.Collections.Generic;

namespace Idler.Common.ExtendedAttribute
{
    /// <summary>
    /// 扩展属性数据
    /// </summary>
    public class ExtendedAttributeData 
    {
        /// <summary>
        /// 扩展属性数据列表
        /// </summary>
        public IDictionary<string, ExtendedAttributeDataItem> Items { get; set; } = new Dictionary<string, ExtendedAttributeDataItem>();
    }
}