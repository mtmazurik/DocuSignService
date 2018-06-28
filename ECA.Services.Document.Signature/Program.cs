using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace ECA.Services.Document.Signature
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                BuildWebHost(args).Run();       // .Net Core lowest lvl event handler, thread.   Allows controller to listen, for WebApi REST MVC events
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

       public static IWebHost BuildWebHost(string[] args) =>        // NLog suggested rework of BuildWebHost
            WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .ConfigureLogging(logging =>
               {
                   logging.ClearProviders();
                   logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
               })
               .UseNLog()                                           // NLog: setup NLog for Dependency injection
               .Build();
    }
}
