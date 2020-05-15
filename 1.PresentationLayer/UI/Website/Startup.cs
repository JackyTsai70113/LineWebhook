using Core.Domain.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Website.Data;

namespace Website {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;

            // ³]©w Core.Domain.Utilities.ConfigurationUtility ªº configuration
            ConfigurationUtility.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();

            //using (SqlConnection connection = new SqlConnection(connectionString)) {
            //    var eventName = connection.QueryFirst<string>("SELECT TOP 1 Remark FROM Notes");
            //}

            services.AddDbContext<LineWebhookContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LineWebhookContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                //Åª¼glocalDB
            } else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Stock}/{action=Index}/{id?}");
            });
        }
    }
}