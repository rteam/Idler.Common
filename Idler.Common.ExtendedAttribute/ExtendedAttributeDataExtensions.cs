using System.Collections.Generic;
using System.Linq;
using Idler.Common.Core;

namespace Idler.Common.ExtendedAttribute
{
    /// <summary>
    /// 扩展属性扩展
    /// </summary>
    public static class ExtendedAttributeDataExtensions
    {
        public static APIReturnInfo<string> VerifyExtendedData(this IList<ExtendedAttributeDataItem> ExtendedDatas, ExtendedAttributeSetting ExtendedSetting)
        {
            IDictionary<string, ExtendedAttributeDataItem> datas = ExtendedDatas.ToDictionary(t => t.Key, t => t);
            foreach (var setting in ExtendedSetting.Items)
            {
                if (datas.TryGetValue(setting.Key, out ExtendedAttributeDataItem item))
                {
                    if (setting.MaxLength > 0 && item.Value.Length > setting.MaxLength)
                        return APIReturnInfo<string>.Error(setting.Name + "长度不能超过" + setting.MaxLength + "个字");
                }
                else if (setting.Must)
                {
                    return APIReturnInfo<string>.Error(setting.Name + "为必填项");
                }
            }

            return APIReturnInfo<string>.Success("ok");
        }
    }
}