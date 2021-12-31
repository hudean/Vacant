using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.ServiceBus;
using System;

namespace Vacant.EventBusServiceBus
{
    public interface IServiceBusPersisterConnection : IDisposable
    {
        //ITopicClient TopicClient { get; }
        //ISubscriptionClient SubscriptionClient { get; }


        ServiceBusClient TopicClient { get; }
        ServiceBusAdministrationClient AdministrationClient { get; }
    }
}
