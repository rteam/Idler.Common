using System;
using System.Text;
using Microsoft.Extensions.Logging;
namespace Idler.Common.Core.Logging
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool HandleException(this ILogger logger, Exception ex)
        {
            logger.LogError(GetMessage(ex));
            return true;
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool HandleException<TException>(this ILogger logger, TException ex)
            where TException : Exception
        {
            logger.LogError(GetMessage(ex));
            return true;
        }

        /// <summary>
        /// 取得异常中的所有错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        private static string GetMessage(Exception ex)
        {
            if (ex.InnerException != null)
                return $"{ex.Message}\r\n{ex.StackTrace}\r\n{GetMessage(ex.InnerException)}";

            return $"{ex.Message}\r\n{ex.StackTrace}";
        }


        /// <summary>
        /// 取得扩展数据
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns></returns>
        private static string GetExpandData(Exception ex)
        {
            if (ex.Data == null || ex.Data.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(500);
            sb.Append("<ExpandData>");
            sb.Append("<Items>");
            foreach (System.Collections.DictionaryEntry item in ex.Data)
            {
                sb.Append("<Item>");
                sb.Append("<Key>");
                sb.Append(item.Key);
                sb.Append("</Key>");
                sb.Append("<Value>");
                sb.Append(item.Value);
                sb.Append("</Value>");
                sb.Append("</Item>");
            }
            sb.Append("</Items>");
            sb.Append("</ExpandData>");
            return sb.ToString();
        }
    }
}
