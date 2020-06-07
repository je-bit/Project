using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Project
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

            services.AddControllersWithViews();


            services.AddScoped<StoreDbContext>();
            services.AddScoped<IStoreContextData, StoreContextData>();
            services.AddDefaultIdentity<User>()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<StoreDbContext>().AddDefaultUI();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            //add this package Microsoft.AspNetCore.Authentication.Facebook to add external login. Note: when get facebook apiId and apiSecret, create json file in main dir of web app.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });
            //.AddFacebook(facebookOptions =>
            //{
            //  facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //  facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            // });
            services.AddAuthorization();
            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseStatusCodePagesWithRedirects("/Home/Error?code={0}");
                //app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseFileServer();

            //Three types midlewares Map, Use, Run
            //app.Map("/Test",TestMap);

            //You can see result in Response headers in your browser
            //app.Run(async x =>
            //{
            //    x.Response.StatusCode = 201;
            //    x.Items.Add("Hello", "World");
            //    x.Response.Headers.Add("HeaderTest", new Microsoft.Extensions.Primitives.StringValues("Header"));
            //    await x.Response.WriteAsync("Run middleware");
            //});



            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "custom",
                //    pattern: "Products/all/{action=index}/",
                //    defaults: new { controller = "Products" }
                //    );



                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
