using System;
using System.IO;
using System.Reflection;
using BL.Service.Telegram;
using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Website.Configuration;

namespace Website;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 設定優先順序（後者覆蓋前者）：
        // 1. appsettings.json（非敏感預設值）
        // 2. .env 檔（本機開發用）
        // 3. 環境變數（Render.com / Docker 部署用）
        builder.Configuration
            .AddDotNetEnv(".env", LoadOptions.TraversePath())
            .AddEnvironmentVariables();
        builder.Services.AddHttpClient();
        builder.Services.AddCaching(builder.Configuration)
                        .AddMyService();
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            options.CustomSchemaIds(type => type.FullName); // Use the full namespace as the schema ID
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/healthz");

        app.Run();
    }
}
