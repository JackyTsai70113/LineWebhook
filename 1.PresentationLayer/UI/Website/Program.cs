using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

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
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            // string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            // string url = string.Concat("https://0.0.0.0:", port);
            // return Host.CreateDefaultBuilder(args)
            //     .UseSerilog()
            //     .ConfigureWebHostDefaults(webBuilder => {
            //         webBuilder.UseStartup<Startup>().UseUrls(url);
            //     });
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}