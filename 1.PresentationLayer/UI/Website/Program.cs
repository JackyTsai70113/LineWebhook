using System;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Website {

    public class Program {

        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(string.Format("Log2/{0}.txt", DateTime.Now.ToString("yyyyMMdd")))
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
            Log.Information("Main");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
    }
}