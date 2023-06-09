﻿using System;
using System.Collections.Generic;
using System.Linq;
using FunctionsCore;
using FunctionsCore.Contexts;
using Automata.Controllers.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Serialization;
using Automata.Functions;
using FunctionsCore.Services;
using FunctionsCore.Commons.Functions;

namespace Automata
{
    public class Startup
    {
        public static bool Inited = false;
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            BaseAppContext.Instance.HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IPrinterFunctions, PrinterFunctions>();
            services.AddTransient<IBookingFunctions, BookingFunctions>();
            services.AddTransient<IHttpRequestService, HttpRequestService>();
            services.AddTransient<ICrmFunctions, CrmFunctions>();
            services.AddTransient<IIdScannerFunctions, IdScannerFunctions>();

            services.AddSingleton<IKerongLockFunctions, KerongLockFunctions>();
            services.AddSingleton<IIdScannerFunctions, IdScannerFunctions>();

            services.AddControllersWithViews(options =>
            {
                options.ModelBinderProviders.Insert(0, new DoubleBinderProvider());
                options.MaxModelBindingCollectionSize = int.MaxValue;

            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                            {
                               "image/svg+xml",
                               "application/atom+xml"
                            });
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(1000);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = "AutomataAppSession";
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
            //services.AddAuthentication(IISDefaults.AuthenticationScheme);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = TokenValidationParametersSetting.Parameters;
            //});
            services.AddHttpContextAccessor();

            services.AddHttpClient<IHttpRequestService, HttpRequestService>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

           
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 1073741824; // 1GB
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 1073741824;
                options.ValueLengthLimit = 1073741824;
                options.MultipartBodyLengthLimit = 1073741824;
                options.MultipartBoundaryLengthLimit = 1073741824;
                options.MultipartHeadersCountLimit = 1073741824;
                options.MultipartHeadersLengthLimit = 1073741824;
            });
            services.AddControllers().AddNewtonsoftJson(opts =>
                    opts.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //services.AddScoped<TokenAuthenticationFunctions>();

            AddTimedHostedServices(services);

            KerongLockFunctions.Init();
        }


        public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            //Assembly.LoadFrom("Driver/EcrWrapperDotNetMlib.dll");
            Fonix3HttpContextAccessor.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseSession();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<EndRequestMiddleware>();


            app.UseDefaultFiles(new DefaultFilesOptions()
            {
                DefaultFileNames = new List<string>() { "index.html" }
            });

            app.UseStaticFiles();
            app.UseCors(options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Home}/{action=Index}/{id?}");
            });

            lifetime.ApplicationStarted.Register(() => ApplicationStart());
            lifetime.ApplicationStopping.Register(ApplicationEnd);
        }

        private static void AddTimedHostedServices(IServiceCollection services)
        {
            //services.AddHostedService<QueuedHostedService>();
            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueueTimedHostedService>();
            services.AddHostedService<ComHostedService>();
            MoneraTerminalFunctions.InitDailyTask();
        }

        public static DateTime? StartTime;

        void ApplicationStart()
        {
          
            Log.Info("App startup is complete");
        }

        void ApplicationEnd()
        {

        }
    }
}
