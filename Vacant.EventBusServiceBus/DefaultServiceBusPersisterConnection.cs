using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        #region

        //    private readonly ServiceBusConnectionStringBuilder _serviceBusConnectionStringBuilder;
        //    private readonly string _subscriptionClientName;
        //    private SubscriptionClient _subscriptionClient;
        //    private ITopicClient _topicClient;

        //    bool _disposed;

        //    public DefaultServiceBusPersisterConnection(ServiceBusConnectionStringBuilder serviceBusConnectionStringBuilder,
        //        string subscriptionClientName)
        //    {
        //        _serviceBusConnectionStringBuilder = serviceBusConnectionStringBuilder ??
        //            throw new ArgumentNullException(nameof(serviceBusConnectionStringBuilder));
        //        _subscriptionClientName = subscriptionClientName;
        //        _subscriptionClient = new SubscriptionClient(_serviceBusConnectionStringBuilder, subscriptionClientName);
        //        _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        //    }

        //    public ITopicClient TopicClient
        //    {
        //        get
        //        {
        //            if (_topicClient.IsClosedOrClosing)
        //            {
        //                _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        //            }
        //            return _topicClient;
        //        }
        //    }

        //    public ISubscriptionClient SubscriptionClient
        //    {
        //        get
        //        {
        //            if (_subscriptionClient.IsClosedOrClosing)
        //            {
        //                _subscriptionClient = new SubscriptionClient(_serviceBusConnectionStringBuilder, _subscriptionClientName);
        //            }
        //            return _subscriptionClient;
        //        }
        //    }

        //    public ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder => _serviceBusConnectionStringBuilder;

        //    public ITopicClient CreateModel()
        //    {
        //        if (_topicClient.IsClosedOrClosing)
        //        {
        //            _topicClient = new TopicClient(_serviceBusConnectionStringBuilder, RetryPolicy.Default);
        //        }

        //        return _topicClient;
        //    }

        //    public void Dispose()
        //    {
        //        if (_disposed) return;

        //        _disposed = true;
        //    }

        #endregion

        private readonly string _serviceBusConnectionString;
        private ServiceBusClient _topicClient;
        private ServiceBusAdministrationClient _subscriptionClient;

        bool _disposed;

        public DefaultServiceBusPersisterConnection(string serviceBusConnectionString)
        {
            _serviceBusConnectionString = serviceBusConnectionString;
            _subscriptionClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);
            _topicClient = new ServiceBusClient(_serviceBusConnectionString);
        }

        public ServiceBusClient TopicClient
        {
            get
            {
                if (_topicClient.IsClosed)
                {
                    _topicClient = new ServiceBusClient(_serviceBusConnectionString);
                }
                return _topicClient;
            }
        }

        public ServiceBusAdministrationClient AdministrationClient
        {
            get
            {
                return _subscriptionClient;
            }
        }

        public ServiceBusClient CreateModel()
        {
            if (_topicClient.IsClosed)
            {
                _topicClient = new ServiceBusClient(_serviceBusConnectionString);
            }

            return _topicClient;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _topicClient.DisposeAsync().GetAwaiter().GetResult();
        }
    }

    



}
