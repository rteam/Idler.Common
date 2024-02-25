using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Idler.Common.Core.Mail
{
    /// <summary>
    /// 默认电子邮件服务
    /// </summary>
    public class NullEmailSender : EmailSenderBase
    {
        public NullEmailSender()
            : base(null)
        {

        }

        public NullEmailSender(IOptions<EmailSetting> emailSettingOption)
            : base(emailSettingOption)
        {
        }

        public override APIReturnInfo<string> Send(string sender, string recipient, string title, string content, bool html = false)
        {
            return APIReturnInfo<string>.Error("未提供邮件发送服务");
        }

        public override APIReturnInfo<string> Send(string recipient, string title, string content, bool html = false)
        {
            return APIReturnInfo<string>.Error("未提供邮件发送服务");
        }

        public override APIReturnInfo<string> Send(string recipient, string title, string content, IList<KeyValuePair<string, string>> @params, bool html = false)
        {
            return APIReturnInfo<string>.Error("未提供邮件发送服务");
        }

        public override APIReturnInfo<string> Send(string sender, string recipient, string title, string content, IList<KeyValuePair<string, string>> @params, bool html = false)
        {
            return APIReturnInfo<string>.Error("未提供邮件发送服务");
        }
    }
}