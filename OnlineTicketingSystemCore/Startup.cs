using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnlineTicketingSystemCore.Data;
using Microsoft.EntityFrameworkCore;

namespace OnlineTicketingSystemCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OnlineTicketingSystemCoreContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("OnlineTicketingSystemCore")));

            //services.AddControllersWithViews();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc().AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(Language));
            });         

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr")
                };

                options.DefaultRequestCulture = new RequestCulture("en"); 
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                var provider = new RouteDataRequestCultureProvider();
                provider.RouteDataStringKey = "lang";
                provider.UIRouteDataStringKey = "lang";
                provider.Options = options;

                options.RequestCultureProviders = new[] { provider };
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("lang", typeof(LanguageRouteConstraint));
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "LocalizedDefault",
                    pattern: "{lang:lang}/{controller=Tickets}/{action=Index}/{id?}");
                    //pattern: "{controller=Home}/{action=Index}/{id?}/{culture?}");
            endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Tickets}/{action=Index}/{id?}");
                
            });
        }
    }
}
