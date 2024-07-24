using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Idler.Common.Core;

namespace Idler.Common.ExtendedAttribute
{
    /// <summary>
    /// 扩展属性扩展
    /// </summary>
    public static class ExtendedAttributeDataExtensions
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="setting">设置</param>
        /// <returns></returns>
        public static string Verify(this ExtendedAttributeDataItem data,
            ExtendedAttributeSettingItem setting)
        {
            if (data.Value.IsEmpty())
            {
                if (setting.Must)
                    return string.Concat(setting.Name, "为必填项");
            }
            else
            {
                if (setting.MaxLength > 0 && data.Value.Length > setting.MaxLength)
                    return string.Concat(setting.Name, "长度不能超过", setting.MaxLength, "个字");


                if (!setting.Extension.IsEmpty())
                {
                    try
                    {
                        Regex regex = new Regex(setting.Extension);
                        if (!regex.IsMatch(data.Value))
                            return setting.ExtensionErrorMessage;
                    }
                    catch (Exception e)
                    {
                        return setting.ExtensionErrorMessage;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 验证扩展表单
        /// </summary>
        /// <param name="extendedDatas">要验证的数据</param>
        /// <param name="extendedSetting">导出设置</param>
        /// <returns></returns>
        public static APIReturnInfo<string> Verify(this ExtendedAttributeData extendedDatas,
            ExtendedAttributeSetting extendedSetting)
        {
            foreach (var setting in extendedSetting.Items)
            {
                if (extendedDatas.Items.TryGetValue(setting.Key, out ExtendedAttributeDataItem item))
                {
                    string result = item.Verify(setting);
                    if (!result.IsEmpty())
                        return APIReturnInfo<string>.Error(result);
                }
                else if (setting.Must)
                {
                    return APIReturnInfo<string>.Error(setting.Name + "为必填项");
                }
            }

            return APIReturnInfo<string>.Success("ok");
        }

        /// <summary>
        /// 验证扩展表单
        /// </summary>
        /// <param name="extendedDatas">要验证的数据</param>
        /// <param name="extendedSetting">导出设置</param>
        /// <returns></returns>
        public static APIReturnInfo<string> Verify(this IEnumerable<ExtendedAttributeDataItem> extendedDatas,
            ExtendedAttributeSetting extendedSetting)
        {
            IDictionary<string, ExtendedAttributeDataItem> datas = extendedDatas.ToDictionary(t => t.Key, t => t);
            foreach (var setting in extendedSetting.Items)
            {
                if (datas.TryGetValue(setting.Key, out ExtendedAttributeDataItem item))
                {
                    string result = item.Verify(setting);
                    if (!result.IsEmpty())
                        return APIReturnInfo<string>.Error(result);
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