using BL.Services.Line;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Website {

    public class Program {

        public static void Main(string[] args) {
            //Serilog
            //不要印出Method Name -> 吃資源
            string template = "{Timestamp:HH:mm:ss fff} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("Log/.txt", rollingInterval: RollingInterval.Day, outputTemplate: template)
                    //.WriteTo.Seq("http://localhost:5341")
                    .CreateLogger();
            CreateHostBuilder(args).Build().Run();

            Task.Run(() => {
                for (int i = 0; i < 5; i++) {
                    new LineNotifyBotService().PushMessage_Jacky(DateTime.Now.ToString());
                    Thread.Sleep(3000);
                }
            });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}