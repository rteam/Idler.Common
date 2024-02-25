using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System;
using System.Collections;

namespace Idler.Common.Core.Mail
{
    public abstract class EmailSenderBase : IEmailSender
    {
        protected readonly IOptions<EmailSetting> EmailSettingOption;

        public EmailSenderBase(IOptions<EmailSetting> emailSettingOption)
        {
            this.EmailSettingOption = emailSettingOption;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="html">是否为Html格式</param>
        /// <returns></returns>
        public abstract APIReturnInfo<string> Send(string sender, string recipient, string title, string content,
            bool html = false);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="html">是否为Html格式</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> Send(string recipient, string title, string content, bool html = false)
        {
            return this.Send(this.EmailSettingOption.Value.Sender, recipient, title, content);
        }

        /// <summary>
        /// 发送模板邮件
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="params">参数</param>
        /// <param name="html">是否为Html格式</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> Send(string sender, string recipient, string title, string content,
            IList<KeyValuePair<string, string>> @params, bool html = false)
        {
            if (@params != null && @params.Count > 0)
                @params.ForEach(t => content = content.Replace(string.Concat("{", t.Key, "}"), t.Value));

            return this.Send(sender, recipient, title, content);
        }

        /// <summary>
        /// 发送模板邮件
        /// </summary>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="params">参数</param>
        /// <param name="html">是否为Html格式</param>
        /// <returns></returns>
        public virtual APIReturnInfo<string> Send(string recipient, string title, string content,
            IList<KeyValuePair<string, string>> @params, bool html = false)
        {
            return this.Send(this.EmailSettingOption.Value.Sender, recipient, title, content, @params);
        }
    }
}