using System.Collections.Generic;

namespace Idler.Common.Core.SMS
{
    public interface ISMSSender
    {
        /// <summary>
        /// 发信息
        /// </summary>
        /// <param name="recipient">接收号码</param>
        /// <param name="template">内容模板</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string recipient, string template, IList<KeyValuePair<string, string>> @params);
        /// <summary>
        /// 发信息
        /// </summary>
        /// <param name="recipient">接收号码</param>
        /// <param name="template">内容模板</param>
        /// <param name="params">参数</param>
        /// <param name="smsType">信息类型</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string recipient, string template, IList<KeyValuePair<string, string>> @params, string smsType);

        ///// <summary>
        ///// 发送验证码
        ///// </summary>
        ///// <param name="Phone">手机号</param>
        ///// <param name="Code">验证码</param>
        ///// <param name="CodeType">验证码类型</param>
        ///// <returns></returns>
        //BaseReturnInfo SendVerificationCode(string Phone, string Code, string CodeType);
    }
}