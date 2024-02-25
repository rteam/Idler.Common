using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;

namespace Idler.Common.Core.Mail
{
    public class FluentEmailSender : EmailSenderBase
    {
        public FluentEmailSender(IOptions<EmailSetting> emailSetting) : base(emailSetting)
        {
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="html">正文是否为Html</param>
        /// <returns></returns>
        public override APIReturnInfo<string> Send(string sender, string recipient, string title, string content,
            bool html = false)
        {
            if (sender.IsEmpty())
                throw new ArgumentNullException(nameof(sender));

            if (recipient.IsEmpty())
                throw new ArgumentNullException(nameof(recipient));

            if (title.IsEmpty())
                throw new ArgumentNullException(nameof(title));

            if (content.IsEmpty())
                throw new ArgumentNullException(nameof(content));

            Email.DefaultSender = new SmtpSender(
                new SmtpClient(this.EmailSettingOption.Value.Server, this.EmailSettingOption.Value.Port)
                {
                    EnableSsl = false,
                    Credentials = new NetworkCredential(this.EmailSettingOption.Value.UserName,
                        this.EmailSettingOption.Value.Password)
                });

            var email = Email.From(sender).To(recipient).Subject(title).Body(content, html).Send();
            if (email.Successful)
                return APIReturnInfo<string>.Success();

            return APIReturnInfo<string>.Error("邮件发送失败：" +
                                               ((email.ErrorMessages == null || email.ErrorMessages.Count == 0)
                                                   ? ""
                                                   : string.Join(",", email.ErrorMessages.ToArray())));
        }
    }
}