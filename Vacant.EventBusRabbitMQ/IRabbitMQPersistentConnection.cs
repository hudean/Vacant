using RabbitMQ.Client;
using System;

namespace Vacant.EventBusRabbitMQ
{
    /// <summary>
    /// RabbitMQ持续连接
    /// </summary>
    public interface IRabbitMQPersistentConnection
         : IDisposable
    {
        /// <summary>
        /// 已连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 尝试连接
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        IModel CreateModel();
    }
}
