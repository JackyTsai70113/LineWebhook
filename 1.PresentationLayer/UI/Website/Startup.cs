using System.Text;
using BL.Services;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Website.Data;
using Website.Services;

namespace Website {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            ConfigurationUtility.Configuration = configuration;
            ConfigService.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddControllersWithViews();

            // register Mvc service
            services.AddMvc();
            services.AddDbContext<LineWebhookContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LineWebhookContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                //Ū�glocalDB
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            // 強迫將 HTTP 全部轉向 HTTPS
            app.UseHttpsRedirection();
            // 服務靜態檔案傳輸
            app.UseStaticFiles();

            app.UseRouting();
            // 設定授權檢查, 權限不夠直接403
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Test}/{id?}");
            });
        }
    }
}