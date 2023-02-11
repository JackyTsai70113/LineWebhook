using System;
using System.IO;
using System.Reflection;
using BL.Service;
using BL.Service.Cache;
using BL.Service.Cache.Redis;
using BL.Service.Interface;
using BL.Service.Interface.TWSE_Stock;
using BL.Service.Line;
using BL.Service.Line.Interface;
using BL.Service.Map;
using BL.Service.Sinopac;
using BL.Service.TWSE_Stock;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using DA.Managers.Interfaces.TWSE_Stock;
using DA.Managers.TWSE_Stock;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Website;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMyService();
        builder.Services.AddControllers();
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API - V1",
                Version = "v1"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        // builder.Services.AddDbContext<LineWebhookContext>(options =>
        //     options.UseSqlServer("Server=localhost;Database=TestDB;User Id=sa;Password=Passw0rd1"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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

static class ServiceExtensions
{
    /// <summary>
    /// 根據微軟建議，透過延伸方法將註冊服務獨立封裝出來
    /// </summary>
    /// <param name="services">collection of service</param>
    /// <remarks>https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0</remarks>
    public static void AddMyService(this IServiceCollection services)
    {
        services.AddScoped<ICambridgeDictionaryManager, CambridgeDictionaryManager>();
        services.AddScoped<IMaskInstitutionService, MaskInstitutionService>();
        services.AddScoped<LineMessageService, LineMessageService>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        services.AddScoped<ILineBotService, LineBotService>();
        services.AddScoped<ILineWebhookService, LineWebhookService>();
        services.AddScoped<IMaskInstitutionService, MaskInstitutionService>();
        services.AddScoped<ITradingVolumeService, TradingVolumeService>();
        services.AddScoped<IDailyQuoteService, DailyQuoteService>();
        services.AddScoped<IDailyQuoteManager, DailyQuoteManager>();

        services.AddSingleton<IMapHereService, MapHereService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
    }
}