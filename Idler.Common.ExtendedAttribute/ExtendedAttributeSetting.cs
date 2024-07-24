using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Idler.Common.ExtendedAttribute
{
    /// <summary>
    /// 扩展属性设置
    /// </summary>
    public class ExtendedAttributeSetting : IValidatableObject
    {
        #region 属性
        /// <summary>
        /// 所属表单项列表
        /// </summary>
        public List<ExtendedAttributeSettingItem> Items { get; set; } = new List<ExtendedAttributeSettingItem>();
        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            this.Items = this.Items.Where(t => !StringValidation.IsEmpty((string)t.Key) && !StringValidation.IsEmpty((string)t.Name)).ToList();
            int index = 0;
            this.Items.ForEach(t =>
            {
                t.OrderBy = index;
                index = index + 1;
            });

            if (this.Items == null)
                yield return new ValidationResult("一般系统保护性错误");
        }
    }
}