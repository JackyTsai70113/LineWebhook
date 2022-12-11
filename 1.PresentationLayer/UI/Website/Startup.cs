using System.Text;
using BL.Services;
using BL.Services.Cache;
using BL.Services.Cache.Redis;
using BL.Services.Interfaces;
using BL.Services.Line;
using BL.Services.Line.Interfaces;
using BL.Services.Map;
using BL.Services.Sinopac;
using BL.Services.TWSE_Stock;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Website.Data;

namespace Website {

    //     public class Startup {
    //         private IConfiguration Configuration { get; }
    // 
    //         public Startup() {
    //             var builder = new ConfigurationBuilder()
    //                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    //             Configuration = builder.Build();
    //             // ConfigService.Configuration = Configuration;
    //         }
    // 
    //         // This method gets called by the runtime. Use this method to add services to the container.
    //         public void ConfigureServices(IServiceCollection services) {
    //             Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    //             services.AddControllersWithViews();
    // 
    //             // register Mvc service
    //             services.AddMvc();
    // 
    //             services.AddDbContext<LineWebhookContext>(options =>
    //                 options.UseSqlServer(Configuration.GetConnectionString("LineWebhookContext")));
    // 
    //             services.AddMyService();
    // 
    //             // Register the Swagger services
    //             services.AddSwaggerDocument(config => {
    //                 config.PostProcess = document => {
    //                     document.Info.Version = "v1";
    //                     document.Info.Title = "LineWebhook API";
    //                     document.Info.Description = "A simple ASP.NET Core web API";
    //                     document.Info.TermsOfService = "None";
    //                     document.Info.Contact = new NSwag.OpenApiContact {
    //                         Name = "Jacky Tsai",
    //                         Email = string.Empty,
    //                         Url = string.Empty
    //                     };
    //                     document.Info.License = new NSwag.OpenApiLicense {
    //                         Name = string.Empty,
    //                         Url = string.Empty
    //                     };
    //                 };
    //             });
    //         }
    // 
    //         // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //         public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    //             if (env.IsDevelopment()) {
    //                 app.UseDeveloperExceptionPage();
    //             } else {
    //                 app.UseExceptionHandler("/Home/Error");
    //                 // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //                 app.UseHsts();
    //             }
    // 
    //             // Register the Swagger generator and the Swagger UI middlewares
    //             app.UseOpenApi();
    //             app.UseSwaggerUi3();
    // 
    // 
    //             // 強迫將 HTTP 全部轉向 HTTPS
    //             app.UseHttpsRedirection();
    //             // 服務靜態檔案傳輸
    //             app.UseStaticFiles();
    // 
    //             app.UseRouting();
    //             // 設定授權檢查, 權限不夠直接403
    //             app.UseAuthorization();
    // 
    //             app.UseEndpoints(endpoints => {
    //                 endpoints.MapControllerRoute(
    //                     name: "default",
    //                     pattern: "{controller=Home}/{action=Test}/{id?}");
    //             });
    //         }
    //     }
}