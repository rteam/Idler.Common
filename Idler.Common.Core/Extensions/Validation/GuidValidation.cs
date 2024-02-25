using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class GuidValidation
    {
        /// <summary>
        /// 判断Guid是否为空
        /// </summary>
        /// <param name="inputGuid">待判断的Guid</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static Guid ThrowIfNull(this Guid inputGuid, string parameterName)
        {
            if (inputGuid == Guid.Empty)
                throw new ArgumentNullException(parameterName);

            return inputGuid;
        }

        /// <summary>
        /// 判断一组Guid中是否存在空值
        /// </summary>
        /// <param name="inputGuids">待验证的Guid</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns></returns>
        public static IEnumerable<Guid> ThrowIfNull(this IEnumerable<Guid> inputGuids, string parameterName)
        {
            if (inputGuids == null || inputGuids.Count() == 0)
                throw new ArgumentNullException(parameterName);

            foreach (Guid item in inputGuids)
            {
                if (item.IsEmpty())
                    throw new ArgumentNullException(parameterName);
            }

            return inputGuids;
        }

        /// <summary>
        /// 判断Guid是否为空
        /// </summary>
        /// <param name="inputGuid">待判断的Guid</param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid inputGuid)
        {
            return inputGuid == Guid.Empty;
        }

        /// <summary>
        /// 判断一组Guid中是否存在空值
        /// </summary>
        /// <param name="inputGuids">待验证的Guid</param>
        /// <returns></returns>
        public static bool IsEmpty(this IEnumerable<Guid> inputGuids)
        {
            if (inputGuids == null || inputGuids.Count() == 0)
                return true;

            foreach (Guid item in inputGuids)
            {
                if (item.IsEmpty())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 从字符串转换Guid
        /// </summary>
        /// <param name="inputString">待转换的字符串</param>
        /// <returns>如果转换失败则返回Guid.Empty</returns>
        public static Guid GuidByString(this string inputString)
        {
            Guid t = Guid.Empty;
            return Guid.TryParse(inputString, out t) ? t : Guid.Empty;
        }

        /// <summary>
        /// 将一组字符串转换为Guid
        /// </summary>
        /// <param name="inputStrings">待转换为Guid的字符串列表</param>
        /// <returns>转换失败的直接抛弃</returns>
        public static IList<Guid> GuidByString(this IList<string> inputStrings)
        {
            IList<Guid> tList = new List<Guid>();

            foreach (string item in inputStrings)
                if (Guid.TryParse(item, out var guid))
                    tList.Add(guid);

            return tList;
        }

        /// <summary>
        /// 将字符串数组转换为Guid数组
        /// </summary>
        /// <param name="inputStrings"></param>
        /// <returns></returns>
        public static Guid[] GuidByString(this string[] inputStrings)
        {
            if (inputStrings == null || inputStrings.Length == 0)
                return new Guid[] { };

            IList<Guid> tList = new List<Guid>();

            foreach (string item in inputStrings)
                if (Guid.TryParse(item, out var guid))
                    tList.Add(guid);

            return tList.ToArray<Guid>();
        }

        /// <summary>
        /// 字符串是否能转换为Guid
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <returns></returns>
        public static bool IsGuid(this string inputString)
        {
            return Guid.TryParse(inputString, out _);
        }

    }
}