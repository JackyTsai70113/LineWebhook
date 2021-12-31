using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Website {

    public class Program {

        public static void Main(string[] args) {
            //Serilog
            //不要印出Method Name -> 吃資源
            //string template = "{Timestamp:HH:mm:ss fff} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}";
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Log.Logger = new LoggerConfiguration()
            //         .MinimumLevel.Verbose()
            //         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //         .Enrich.FromLogContext()
            //         .WriteTo.Console()
            //         //.ReadFrom.AppSettings()
            //         //.WriteTo.File("Logs/.txt", rollingInterval: RollingInterval.Day, outputTemplate: template)
            //         .WriteTo.File("Logs/123.txt")
            //         //.WriteTo.Seq("http://localhost:5341")
            //         .CreateLogger();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                // .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}