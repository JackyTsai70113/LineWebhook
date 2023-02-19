using BL.Service;
using BL.Service.Interface;
using BL.Service.Interface.TWSE_Stock;
using BL.Service.Line;
using BL.Service.MapQuest;
using BL.Service.Redis;
using BL.Service.Sinopac;
using BL.Service.Telegram;
using BL.Service.TWSE_Stock;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using DA.Managers.Interfaces.TWSE_Stock;
using DA.Managers.TWSE_Stock;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Telegram.Bot;

namespace Website.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config["RedisConnection"]));
        return services;
    }

    public static IServiceCollection AddChatBot(
        this IServiceCollection services,
        IConfiguration config)
    {
        return services
            .AddSingleton<ITelegramService, TelegramService>()
            .AddSingleton<ITelegramBotClient, TelegramBotClient>(x => new TelegramBotClient(config["Telegram:Token"]));
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

        services.AddSingleton<IMapQuestService, MapQuestService>();
    }
}
