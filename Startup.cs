using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
//using Microsoft.Extensions.Hosting;
using BackgroundTasksSample.Services;

namespace netbu {
    public class Startup {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices (IServiceCollection services) {
            services.AddCors ();

            // установка конфигурации подключения, Аутентификация
            services.AddAuthentication (CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie (options => //CookieAuthenticationOptions
                    {
                        options.LoginPath = new Microsoft.AspNetCore.Http.PathString ("/Access.html");
                        //options.SlidingExpiration = true;
                        options.ExpireTimeSpan = TimeSpan.FromDays(30);

                        
                    });

            services.AddMvc ().AddJsonOptions (options => {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver ();
            });
            //Задача по таймеру    
            //services.AddHostedService<TimedHostedService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }
            app.UseCors (builder => builder.AllowAnyOrigin ().AllowAnyHeader ().AllowAnyMethod ());
            app.UseDefaultFiles ();
            app.UseStaticFiles ();
            /* 
            string docfiles = Program.AppConfig["docfiles"];
            app.UseStaticFiles (new StaticFileOptions {
                FileProvider = new PhysicalFileProvider (docfiles),
                    RequestPath = "/docfiles"
            });

            app.UseDirectoryBrowser (new DirectoryBrowserOptions {
                FileProvider = new PhysicalFileProvider (docfiles),
                    RequestPath = "/docfiles"
            });
            */
            //Аутентификация
            app.UseAuthentication();

            
            app.UseMvc (routes => {
                routes.MapRoute (
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}