using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.messagebus;
using GreenPipes;
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
using Microsoft.Extensions.Logging.AzureAppServices.Internal;
using RabbitMQ.Client;
using Module = Autofac.Module;


namespace HardwareService
{
    public class BusControlEvents {}
    public class BusControlCommand { }



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




            builder.Register(a =>
            {
                var logger = a.Resolve<ILoggerFactory>();
                return new KafkaEventStore(logger.CreateLogger("LoggerForSensorRepo"));

            }).As<IEventStore>().SingleInstance();

            builder.Register(a =>
            {
                var logger = a.Resolve<ILoggerFactory>();
                var eventStore = a.Resolve<IEventStore>();
                return new SensorsRepository(logger.CreateLogger("LoggerForSensorRepo"),eventStore);
            }).SingleInstance();

            
       
           

           

           // builder.RegisterInstance(new BusSettings{HostAddress = "localhost",Username = "guest", Password = "guest",QueueName = "SensorEvents"}).Named<BusSettings>("Events");
            builder.RegisterInstance(new BusSettings { HostAddress = "localhost", Username = "guest", Password = "guest", QueueName = "SensorCommands" }).Named<BusSettings>("Commands");

            builder.RegisterModule<BusModule>();
            builder.RegisterModule<EventProcessorsModule>();

            builder.RegisterType<TempSensorsController>().PropertiesAutowired();
            builder.RegisterType<MockController>().PropertiesAutowired();

            services.AddMvc().AddControllersAsServices();
           
            builder.Populate(services);
            var container = builder.Build();

            //var bc = container.Resolve<IBusControl>();           
            //bc.Start();

            Task.Factory.StartNew(() => container.Resolve<IEventStore>().StartEventListener(container));

            return new AutofacServiceProvider(container);

        }
      

        class BusModule :Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<IBusControl>()
                    .Named<IBusControl>("handler");
                builder.RegisterType<IBusControl>()
                    .Named<IBusControl>("handler");



                //builder.Register(context =>
                //    {
                //        var busSettings = context.ResolveNamed<BusSettings>("Events");
                //        var sensorRepo = context.Resolve<SensorsRepository>();
                //        var loggerFactory = context.Resolve<ILoggerFactory>();

                //        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                //        {
                //            var host = cfg.Host(busSettings.HostAddress, "/", h =>
                //            {
                //                h.Username(busSettings.Username);
                //                h.Password(busSettings.Password);
                //            });

                //            //cfg.ReceiveEndpoint(busSettings.QueueName, ec => { ec.LoadFrom(context); });
                //            cfg.ReceiveEndpoint(busSettings.QueueName, e =>
                //            {
                //                e.Consumer(() => new SensorEventConsumers(sensorRepo, loggerFactory.CreateLogger("SensorEventConsumerLogger")));
                //            });
                //        });
                //        return busControl;
                //    })
                //    .As<IBusControl>()
                //    .As<IBus>()
                //   .As<IPublishEndpoint>().SingleInstance().Named<IBusControl>("Events");

                builder.Register(context =>
                    {
                        var busSettings = context.ResolveNamed<BusSettings>("Commands");
                        var sensorRepo = context.Resolve<SensorsRepository>();
                        var loggerFactory = context.Resolve<ILoggerFactory>();

                        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var host = cfg.Host(busSettings.HostAddress, "/", h =>
                            {
                                h.Username(busSettings.Username);
                                h.Password(busSettings.Password);
                            });
                          
                            cfg.ReceiveEndpoint(busSettings.QueueName, e =>
                             {
                                 e.Consumer(() => new SensorCommandsConsumer(sensorRepo, loggerFactory.CreateLogger("CommandConsumerLogger")));
                             });

                            cfg.UseConcurrencyLimit(1);
                        });
                       

                        return busControl;
                    })
                    .As<IBusControl>()
                    .As<IBus>()
                    .As<IPublishEndpoint>().SingleInstance().Named<IBusControl>("Commands");
            }
        }

        class EventProcessorsModule :Module
        {
            protected override void Load(ContainerBuilder builder)
            {               
                builder.RegisterType<SensorEventProcessor>().SingleInstance();                
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
