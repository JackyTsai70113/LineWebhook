using BL.Services;
using BL.Services.Cache;
using BL.Services.Cache.Redis;
using BL.Services.Interfaces;
using BL.Services.Line;
using BL.Services.Line.Interfaces;
using BL.Services.Map;
using BL.Services.Sinopac;
using BL.Services.TWSE_Stock;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Website.Controllers;
using Website.Data;

namespace Website;

public class Program {
    public static void Main(string[] args) {

        var builder = WebApplication.CreateBuilder(args);

        // register Mvc service
        builder.Services.AddMvc();
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        // builder.Services.AddDbContext<LineWebhookContext>(options =>
        //                 options.UseSqlServer(Configuration.GetConnectionString("LineWebhookContext")));
        builder.Services.AddMyService();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Register the Swagger generator and the Swagger UI middlewares
        app.UseOpenApi();
        app.UseSwaggerUi3();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Test}/{id?}");

        app.Run();
    }
}

static class ServiceExtensions {

    /// <summary>
    /// 根據微軟建議，透過延伸方法將註冊服務獨立封裝出來
    /// </summary>
    /// <param name="services">collection of service</param>
    /// <remarks>https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0</remarks>
    public static void AddMyService(this IServiceCollection services) {
        services.AddScoped<ICambridgeDictionaryManager, CambridgeDictionaryManager>();
        services.AddScoped<IMaskInstitutionService, MaskInstitutionService>();
        services.AddScoped<LineMessageService, LineMessageService>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        services.AddScoped<ILineNotifyBotService, LineNotifyBotService>();
        services.AddScoped<ILineWebhookService, LineWebhookService>();
        services.AddScoped<IMaskInstitutionService, MaskInstitutionService>();
        services.AddScoped<ITradingVolumeService, TradingVolumeService>();

        services.AddSingleton<Config, Config>();
        services.AddSingleton<IMapHereService, MapHereService>();
        services.AddSingleton<ICacheService, RedisCacheService>(
            p => new RedisCacheService(Config.Redis_Endpoint, Config.Redis_Password));
    }
}