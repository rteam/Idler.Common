using System;
using System.Threading;
using System.Threading.Tasks;

namespace Idler.Common.Core.ServiceBus
{
    /// <summary>
    /// IServiceBus
    /// </summary>
    public interface IServiceBus : IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <typeparam name="TMessage">信息类型</typeparam>
        /// <param name="message">信息体</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Publish<TMessage>(TMessage message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class;

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息体</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task Publish(object message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken));
    }
}