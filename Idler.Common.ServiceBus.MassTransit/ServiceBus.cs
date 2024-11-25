using System.Threading;
using System.Threading.Tasks;
using Idler.Common.Core.ServiceBus;
using MassTransit;

namespace Idler.Common.ServiceBus
{
    public class ServiceBus : IServiceBus
    {
        private readonly IBusControl mBus;

        public ServiceBus(IBusControl BusControl)
        {
            this.mBus = BusControl;
        }

        public void Initialize()
        {
            mBus.Start();
        }

        public void Dispose()
        {
            mBus.Stop();
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public Task Publish(object message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.mBus.Publish(message, cancellationToken);
        }


        /// <summary>
        /// 发布信息
        /// </summary>
        /// <typeparam name="TMessage">信息类型</typeparam>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public Task Publish<TMessage>(TMessage message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class
        {
            return this.mBus.Publish<TMessage>(message, cancellationToken);
        }
    }
}