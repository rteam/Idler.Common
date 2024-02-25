using System.Collections.Generic;

namespace Idler.Common.Core.Mail
{
    public interface IEmailSender
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="html">正文是否为Html</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string sender, string recipient, string title, string content, bool html = false);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="html">正文是否为Html</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string recipient, string title, string content, bool html = false);
        /// <summary>
        /// 发送模板邮件
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="params">参数</param>
        /// <param name="html">正文是否为Html</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string sender, string recipient, string title, string content, IList<KeyValuePair<string, string>> @params, bool html = false);
        /// <summary>
        /// 发送模板邮件
        /// </summary>
        /// <param name="recipient">接收人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="params">参数</param>
        /// <param name="html">正文是否为Html</param>
        /// <returns></returns>
        APIReturnInfo<string> Send(string recipient, string title, string content, IList<KeyValuePair<string, string>> @params, bool html = false);
    }
}