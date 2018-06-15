using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;


namespace HardwareService
{
    public class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            log.Info("Application - Main is invoked");

            var host = new WebHostBuilder()                
               .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
               .UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseIISIntegration()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                        logging.AddDebug();
                    })
               .UseStartup<Startup>()
               .Build();
                     
            host.Run();
        }
    }
}
