using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vacant.Applications.IService;
using Vacant.EventBus.Abstractions;
using Vacant.Redis;
using Vacant.WebApi;

namespace Vacant.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vacant.WebApi", Version = "v1" });
            });

            //注册健康检查服务
            services.AddCustomHealthCheck(Configuration);
            //注入消息队列连接服务
            services.AddIntegrationServices(Configuration);
            //注入事件总线服务
            services.AddEventBus(Configuration);

            #region 单例注入redis连接

            services.Configure<Settings>(Configuration);

            //by connecting here we are making sure that our service
            //cannot start until redis is ready. this might slow down startup,
            //but given that there is a delay on resolving the ip address
            //and then creating the connection it seems reasonable to move
            //that cost to startup instead of having the first request pay the
            //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<Settings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.RedisConnectionString, true);

                configuration.ResolveDns = true;

                return ConnectionMultiplexer.Connect(configuration);
            });
            #endregion


            #region 启用reids服务

            //第一种 
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            //第二种
            //services.AddTransient<IRedisCacheClient, RedisCacheClient>();


            #endregion

            //域模式注入
            var serviceLifetime= ServiceLifetime.Scoped;
            //简单注入泛型仓储
            services.AddInjectionRepositorys(serviceLifetime);
            // 加载Vacant.Applications程序集批量注入Services
            services.AddApplicationService(serviceLifetime);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vacant.WebApi v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //配置事件总线服务用于订阅
            ConfigureEventBus(app);
        }



        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //订阅服务
            //eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            //eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}
