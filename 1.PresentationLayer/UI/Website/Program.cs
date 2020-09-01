using System;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;

namespace Website {

    public class Program {

        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                // `LogEventLevel` requires `using Serilog.Events;`
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                //.Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    new RenderedCompactJsonFormatter(),
                    string.Format("Log/{0}.txt", DateTime.Now.ToString("yyyyMMdd")))
                //.WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
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