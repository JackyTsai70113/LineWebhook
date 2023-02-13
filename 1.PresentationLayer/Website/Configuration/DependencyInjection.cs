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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Website.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config["RedisConnection"]));
        return services;
    }

    /// <summary>
    /// 將註冊服務獨立封裝出來
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
    }
}
