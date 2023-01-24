using BL.Service;
using BL.Service.Cache;
using BL.Service.Cache.Redis;
using BL.Service.Interface;
using BL.Service.Line;
using BL.Service.Line.Interface;
using BL.Service.Map;
using BL.Service.Sinopac;
using BL.Service.TWSE_Stock;
using DA.Managers.CambridgeDictionary;
using DA.Managers.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Website.Data;

namespace Website;

public class Program {
    public static void Main(string[] args) {

        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(5000, configure => configure.UseHttps());
        });

        // register Mvc service
        builder.Services.AddMvc();
        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<LineWebhookContext>(options =>
            options.UseSqlServer("Server=localhost;Database=TestDB;User Id=sa;Password=Passw0rd1"));
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
        services.AddScoped<ILineBotService, LineBotService>();
        services.AddScoped<ILineWebhookService, LineWebhookService>();
        services.AddScoped<IMaskInstitutionService, MaskInstitutionService>();
        services.AddScoped<ITradingVolumeService, TradingVolumeService>();

        services.AddSingleton<IMapHereService, MapHereService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
    }
}