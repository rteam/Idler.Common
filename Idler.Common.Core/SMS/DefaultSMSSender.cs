using System.Collections.Generic;

namespace Idler.Common.Core.SMS
{
    public class DefaultSMSSender : ISMSSender
    {
        /// <summary>
        /// 发信息
        /// </summary>
        /// <param name="recipient">接收号码</param>
        /// <param name="template">内容模板</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        public APIReturnInfo<string> Send(string recipient, string template, IList<KeyValuePair<string, string>> @params)
        {
            return APIReturnInfo<string>.Error("短信服务不可用");
        }

        /// <summary>
        /// 发信息
        /// </summary>
        /// <param name="recipient">接收号码</param>
        /// <param name="template">内容模板</param>
        /// <param name="params">参数</param>
        /// <param name="smsType">信息类型</param>
        /// <returns></returns>
        public APIReturnInfo<string> Send(string recipient, string template, IList<KeyValuePair<string, string>> @params, string smsType)
        {
            return APIReturnInfo<string>.Error("短信服务不可用");
        }
    }
}