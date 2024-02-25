using Idler.Common.Core.Config;

namespace Idler.Common.Core.Mail
{
    public class EmailSetting : AutoBaseConfig<EmailSetting>
    {
        /// <summary>
        /// 服务器
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender { get; set; }
    }
}