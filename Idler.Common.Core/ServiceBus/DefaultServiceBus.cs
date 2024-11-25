using System;
using System.Threading;
using System.Threading.Tasks;

namespace Idler.Common.Core.ServiceBus
{
    /// <summary>
    /// 空实例
    /// </summary>
    public class NullServiceBus : IServiceBus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            return;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            return;
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public Task Publish(object message, string topicName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException("要想使用ServiceBus功能请先注册");
        }


        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public Task Publish<TMessage>(TMessage message, string topicName,
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class
        {
            throw new NotImplementedException("要想使用ServiceBus功能请先注册");
        }
    }
}