using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Vacant.EventBusRabbitMQ;
using Vacant.EventBusServiceBus;

namespace Vacant.WebApi
{
    public static class ServiceProviderExtension
    {
        public static void AddEventBus(this IServiceCollection services, IConfiguration Configuration)
        {
            if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                //services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                //{
                //    var serviceBusConnectionString = Configuration["EventBusConnection"];
                //    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                //    var subscriptionClientName = Configuration["SubscriptionClientName"];
                //    return new DefaultServiceBusPersisterConnection(serviceBusConnection, subscriptionClientName);
                //});
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = Configuration["EventBusConnection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                    {
                        factory.UserName = Configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                    {
                        factory.Password = Configuration["EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            }




        }

    }
}
