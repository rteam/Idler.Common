using System.Threading;
using System.Threading.Tasks;

namespace Idler.Common.Core.ServiceBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public static class EventBus
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static IServiceBus Instance { get; set; }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static Task Publish(object message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.Publish(message, topicName, cancellationToken);
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static Task Publish<TMessage>(TMessage message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class
        {
            return Instance.Publish<TMessage>(message, topicName, cancellationToken);
        }
    }
}