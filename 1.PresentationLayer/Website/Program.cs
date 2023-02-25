using System;
using System.IO;
using System.Reflection;
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
        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(".env", LoadOptions.TraversePath())
            .Build();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHttpClient();
        builder.Services
            .AddCaching(builder.Configuration)
            .AddChatBot(builder.Configuration)
            .AddMyService();
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

        app.Run();
    }
}
