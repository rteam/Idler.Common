using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Idler.Common.Core.ServiceBus;
using Microsoft.Extensions.Options;

namespace Idler.Common.ServiceBus
{
    public class ServiceBus(DaprClient client, IOptions<DaprServiceBusSetting> setting) : IServiceBus
    {
        /// <summary>
        /// 发布信息
        /// </summary>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task Publish(object message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await client.PublishEventAsync(setting.Value.PubSubName, topicName, message, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Initialize()
        {
            return;
        }

        /// <summary>
        /// 发布信息
        /// </summary>
        /// <typeparam name="TMessage">信息类型</typeparam>
        /// <param name="message">信息</param>
        /// <param name="topicName">消息主题</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async Task Publish<TMessage>(TMessage message, string topicName = "",
            CancellationToken cancellationToken = default(CancellationToken))
            where TMessage : class
        {
            await client
                .PublishEventAsync<TMessage>(setting.Value.PubSubName, topicName, message, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Dispose()
        {
            return;
        }
    }
    
}