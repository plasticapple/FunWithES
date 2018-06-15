﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using HardwareService.command_data_access;
using HardwareService.Controllers;
using HardwareService.domain.consumers;
using HardwareService.domain.query_model;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;
using Module = Autofac.Module;


namespace HardwareService
{
    public class Startup
    {
        //private IContainer _container;

        public Startup(IHostingEnvironment env)
        {            
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddEnvironmentVariables();

           // Configuration = builder.Build();
        }

       public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var builder = new ContainerBuilder();           

            IEventStore eventstore = new KafkaEventStore();

            builder.RegisterInstance(eventstore).As<IEventStore>();

            builder.RegisterInstance(new SensorsRepository(eventstore));

            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();

            builder.RegisterInstance(new BusSettings{HostAddress = "localhost",Username = "guest", Password = "guest",QueueName = "SensorCommands"});

            builder.RegisterModule<BusModule>();
            builder.RegisterModule<ConsumerModule>();

            builder.RegisterModule<ConsumerModule>();

            builder.RegisterType<TempSensorsController>().PropertiesAutowired();                       

            services.AddMvc().AddControllersAsServices();
           
            builder.Populate(services);
            var container = builder.Build();

            var bc = container.Resolve<IBusControl>();           
            bc.Start();

            var bus = container.Resolve<IBus>();
          
            var eschannel = connection.CreateModel();

            Task.Factory.StartNew(() => eventstore.StartEventListener(bus));

            return new AutofacServiceProvider(container);

        }

        private class BusSettings
        {
            public string HostAddress { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }

            public string QueueName { get; set; }
        }

        class BusModule :Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.Register(context =>
                    {
                        var busSettings = context.Resolve<BusSettings>();

                        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var host = cfg.Host(busSettings.HostAddress, "/",  h =>
                            {
                                h.Username(busSettings.Username);
                                h.Password(busSettings.Password);
                            });

                            cfg.ReceiveEndpoint(busSettings.QueueName, ec => { ec.LoadFrom(context); });
                        });
                        return busControl;
                    })
                    .As<IBusControl>()
                    .As<IBus>()
                   .As<IPublishEndpoint>()
                    .SingleInstance();
            }
        }

        class ConsumerModule :Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<SensorCommandsConsumer>();
                builder.RegisterType<SensorModelConsumer>();
                //builder.RegisterType<SqlCustomerRegistry>()
                //    .As<ICustomerRegistry>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            try
            {
                loggerFactory.AddLog4Net();
               
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                }

                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}/{id?}"
                       );
                });
            }
            catch (Exception ex)
            {
                app.Run(async context =>
                {
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync(ex.Message);
                });
            }
        }    
    }
}
