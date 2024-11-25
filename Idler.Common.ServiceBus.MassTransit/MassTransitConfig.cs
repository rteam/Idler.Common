using System;
using System.Collections.Generic;
using System.Text;
using BCCore;
namespace BCCore.DDD.ServiceBus
{
    public class MassTransitOption
    {
        /// <summary>
        /// RabbitMQ服务器地址
        /// </summary>
        public string RabbitMQServerUrl { get; set; }
        /// <summary>
        /// VHost
        /// </summary>
        public string VirtualHosts { get; set; }
        /// <summary>
        /// 队列名称
        /// </summary>
        public string RabbitMQQueueName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string RabbitMQUserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string RabbitMQPassword { get; set; }
    }
}
