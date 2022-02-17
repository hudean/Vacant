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

            //ע�ὡ��������
            services.AddCustomHealthCheck(Configuration);
            //ע����Ϣ�������ӷ���
            services.AddIntegrationServices(Configuration);
            //ע���¼����߷���
            services.AddEventBus(Configuration);

            #region ����ע��redis����

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


            #region ����reids����

            //��һ�� 
            services.AddSingleton<IRedisCacheClient, RedisCacheClient>();
            //�ڶ���
            //services.AddTransient<IRedisCacheClient, RedisCacheClient>();


            #endregion

            //��ģʽע��
            var serviceLifetime= ServiceLifetime.Scoped;
            //��ע�뷺�Ͳִ�
            services.AddInjectionRepositorys(serviceLifetime);
            // ����Vacant.Applications��������ע��Services
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
            //�����¼����߷������ڶ���
            ConfigureEventBus(app);
        }



        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            //���ķ���
            //eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
            //eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        }
    }
}
