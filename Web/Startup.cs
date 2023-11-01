using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Core.Services;
using Core.Services.Interfaces;
using DataLayer.Context;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebMarkupMin.AspNetCore3;

namespace Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; set; }
        public IWebHostEnvironment WebHostEnvironment { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebMarkupMin(options =>
            {
                options.AllowMinificationInDevelopmentEnvironment = true;
                options.AllowCompressionInDevelopmentEnvironment = true;
            })
                    .AddHtmlMinification()
                    .AddHttpCompression();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<CookiePolicyOptions>(options =>
            {

                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue; // <-- ! long.MaxValue
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddElmah(options =>
            {
                options.Path = @"orgElmah";
                options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated;
                // به گونه ای که ما آن را پیاده سازی می کنیم elmah محدود کردن دسترسی به 
                options.CheckPermissionAction = CheckPermissionAction;
            });


            #region Database Context
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("PSConnection"));
            }
            );
            #region Encoder

            services.AddSingleton<HtmlEncoder>(
                HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin,
                    UnicodeRanges.Arabic }));

            #endregion
            #endregion Database Context
            //#region IoC
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGalleryService, GalleryService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IBordroService, BordroService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IComplementaryService, ComplementaryService>();


            //#endregion
            #region Authentication

            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = "retUrl";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);//Minute 
                
            });
            var keysFolder = Path.Combine(WebHostEnvironment.ContentRootPath, "temp-keys");
            services.AddDataProtection()
           .SetApplicationName("Web")
           .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
           .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

            #endregion

            services
             .AddRazorPages()
             .AddRazorPagesOptions(options =>
             {
                 options.Conventions.AddPageRoute("/Index", "{*url}");
             });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseElmah();

            app.UseWebMarkupMin();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "Admin",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });


            // app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!").ConfigureAwait(true);
                });

            });
        }
        private bool CheckPermissionAction(HttpContext httpContext)
        {
            // می باشد؟ elamh کاربری جاری سیستم دارای نقش ادمین برای دسترسی به 
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return (httpContext.User.Identity.IsAuthenticated && httpContext.User.Identity.Name == "290070");
            }
            return false;

            // در این قسمت ما تنها برای نمایش آزمایشی میگوییم که دسترسی دارند
            //return true;
        }
    }
}
