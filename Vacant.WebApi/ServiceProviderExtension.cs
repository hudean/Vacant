using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Vacant.Applications.IService;
using Vacant.EntityFrameworkCore.Repositories;
using Vacant.EventBus;
using Vacant.EventBus.Abstractions;
using Vacant.EventBusRabbitMQ;
using Vacant.EventBusServiceBus;
using Vacant.IntegrationEventLogEF.Services;

namespace Vacant.WebApi
{
    /// <summary>
    /// ServiceProvider的扩展方法
    /// </summary>
    public static class ServiceProviderExtension
    {

        /// <summary>
        /// 注册集成服务（AzureServiceBus或者RabbitMQ集成服务）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static IServiceCollection AddIntegrationServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region 注册事件总线日志

            //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            //    sp => (DbConnection c) => new IntegrationEventLogService(c));

            //services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();

            #endregion

            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {

                #region .NET 5.0版本

                //services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                //{
                //    var serviceBusConnectionString = Configuration["EventBusConnection"];
                //    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                //    var subscriptionClientName = Configuration["SubscriptionClientName"];
                //    return new DefaultServiceBusPersisterConnection(serviceBusConnection, subscriptionClientName);
                //});

                #endregion

                #region .NET 6.0版本
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var serviceBusConnectionString = configuration["EventBusConnection"];

                    return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
                });

                #endregion
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBusConnection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                    {
                        factory.UserName = configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                    {
                        factory.Password = configuration["EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });
            }

            return services;
        }

        /// <summary>
        /// 注册事件总线服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="Configuration"></param>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            {
                #region .NET 5.0版本

                //services.AddSingleton<IEventBus, Vacant.EventBusServiceBus.EventBusServiceBus>(sp =>
                //{
                //    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                //    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                //    var logger = sp.GetRequiredService<ILogger<Vacant.EventBusServiceBus.EventBusServiceBus>>();
                //    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                //    return new Vacant.EventBusServiceBus.EventBusServiceBus(serviceBusPersisterConnection, logger,
                //        eventBusSubcriptionsManager, iLifetimeScope);
                //});

                #endregion

                #region .NET 6.0版本
                services.AddSingleton<IEventBus, Vacant.EventBusServiceBus.EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<Vacant.EventBusServiceBus.EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    string subscriptionName = configuration["SubscriptionClientName"];

                    return new Vacant.EventBusServiceBus.EventBusServiceBus(serviceBusPersisterConnection, logger,
                        eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
                });

                #endregion

            }
            else
            {
                services.AddSingleton<IEventBus, Vacant.EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
                {
                    var subscriptionClientName = configuration["SubscriptionClientName"];
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<Vacant.EventBusRabbitMQ.EventBusRabbitMQ>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new Vacant.EventBusRabbitMQ.EventBusRabbitMQ(sp,rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
                });
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //注册要订阅事件的处理器
            //services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
            //services.AddTransient<OrderStartedIntegrationEventHandler>();

            return services;
        }


        /// <summary>
        /// 注册健康检测服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            //hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            //hcBuilder
            //    .AddRedis(
            //        configuration["ConnectionString"],
            //        name: "redis-check",
            //        tags: new string[] { "redis" });

            //if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            //{
            //    hcBuilder
            //        .AddAzureServiceBusTopic(
            //            configuration["EventBusConnection"],
            //            topicName: "eshop_event_bus",
            //            name: "basket-servicebus-check",
            //            tags: new string[] { "servicebus" });
            //}
            //else
            //{
            //    hcBuilder
            //        .AddRabbitMQ(
            //            $"amqp://{configuration["EventBusConnection"]}",
            //            name: "basket-rabbitmqbus-check",
            //            tags: new string[] { "rabbitmqbus" });
            //}

            return services;
        }

        /// <summary>
        /// 注入Redis服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {


            return services;
        }

        /// <summary>
        /// 依赖注入仓储
        /// 参考 文档https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1
        /// </summary>
        /// <param name="services"></param>
        /// <param name="DIMethod"></param>
        public static IServiceCollection AddInjectionRepositorys(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    // AddScoped 作用域 方法使用范围内生存期（单个请求的生存期）注册服务。作用域生存期服务 (AddScoped) 以每个客户端请求（连接）一次的方式创建。
                    services.AddScoped(typeof(IEfCoreRepository<,>), typeof(SimpleEfCoreRepository<,>));
                    services.AddScoped(typeof(IEfCoreRepository<>), typeof(SimpleEfCoreRepository<>));
                    break;

                case ServiceLifetime.Transient:
                    //暂时/瞬态 暂时生存期服务 (AddTransient) 是每次从服务容器进行请求时创建的。 这种生存期适合轻量级、 无状态的服务。
                    services.AddTransient(typeof(IEfCoreRepository<,>), typeof(SimpleEfCoreRepository<,>));
                    services.AddTransient(typeof(IEfCoreRepository<>), typeof(SimpleEfCoreRepository<>));
                    break;

                case ServiceLifetime.Singleton:
                    //单例 在首次请求它们时进行创建；或者在向容器直接提供实现实例时由开发人员进行创建。 很少用到此方法
                    services.AddSingleton(typeof(IEfCoreRepository<,>), typeof(SimpleEfCoreRepository<,>));
                    services.AddSingleton(typeof(IEfCoreRepository<>), typeof(SimpleEfCoreRepository<>));
                    break;

                default:
                    services.AddScoped(typeof(IEfCoreRepository<,>), typeof(SimpleEfCoreRepository<,>));
                    services.AddScoped(typeof(IEfCoreRepository<>), typeof(SimpleEfCoreRepository<>));
                    break;
            }

            return services;
        }

        /// <summary>
        /// 注入应用层
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationService(this IServiceCollection services, ServiceLifetime serviceLifetime)
        {
            #region Vacant.Applications中的批量注入Services

            string assemblyName = "Vacant.Applications";
            //加载程序集Vacant.Applications
            var serviceAsm = Assembly.Load(new AssemblyName(assemblyName));
            //排除程序程序集中的接口、私有类、抽象类、通用类型

            var typeList = serviceAsm.GetTypes().Where(t => typeof(IBaseService).IsAssignableFrom(t) && t.IsClass && !t.GetTypeInfo().IsAbstract && !t.IsGenericType && !t.IsSealed && !t.IsInterface).ToList();
            foreach (Type serviceType in typeList)
            {
                #region 循环注入该累所有继承的接口（不推荐）
                //var interfaceTypes = serviceType.GetInterfaces();
                //foreach (var interfaceType in interfaceTypes)
                //{
                //    //services.AddScoped(interfaceType, serviceType);
                //    switch (serviceLifetime)
                //    {
                //        case ServiceLifetime.Transient:
                //            services.AddTransient(interfaceType, serviceType);
                //            break;

                //        case ServiceLifetime.Scoped:
                //            services.AddScoped(interfaceType, serviceType);
                //            break;

                //        case ServiceLifetime.Singleton:
                //            services.AddSingleton(interfaceType, serviceType);
                //            break;

                //        default:
                //            services.AddScoped(interfaceType, serviceType);
                //            break;
                //    }
                //}

                #endregion

                #region 推荐注入

                //查找当前类继承且包含当前类名的接口
                var interfaceType = serviceType.GetInterfaces().Where(o => o.Name.Contains(serviceType.Name)).FirstOrDefault();
                if (interfaceType != null)
                {
                    //把当前类和继承接口加入Dictionary
                    switch (serviceLifetime)
                    {
                        case ServiceLifetime.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;

                        case ServiceLifetime.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;

                        default:
                            services.AddScoped(interfaceType, serviceType);
                            break;
                    }
                }
                #endregion
            }

            #endregion

            return services;
        }

    }
}
