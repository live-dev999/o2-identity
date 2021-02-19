using System;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using O2.Identity.Web.Models;
using O2.Identity.Web.Services;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using O2.Identity.Web.Controllers;
using O2.Identity.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Razor;
using Serilog;
using Serilog.Events;
using O2.Identity.Web.Filters;

namespace O2.Identity.Web
{
    public class Startup
    {
        private readonly AzureServiceTokenProvider _tokenProvider;
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public class DataProtectionSettings
        {
            public string KeyVaultKeyId { get; set; }
            public string AadTenantId { get; set; }
            public string StorageAccountName { get; set; }
            public string StorageKeyContainerName { get; set; }
            public string StorageKeyBlobName { get; set; }
            public string StorageDevKeyBlobName { get; set; }
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // _tokenProvider = new AzureServiceTokenProvider();
        }

        public IConfiguration Configuration { get; }

        public bool IsProduction { get; set; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 209715200;
                //options.MultipartBodyLengthLimit = 80000000;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            var connectionString = Configuration["ConnectionString"];
            var settings = Configuration.GetSection("DataProtection").Get<DataProtectionSettings>();
            Console.WriteLine(" ========================= SETTINGS ========================== ");
            Console.WriteLine($"ConnectionString={connectionString}");
            Console.WriteLine($"DataProtection AadTenantId={settings.AadTenantId} keyId={settings.KeyVaultKeyId} account={settings.StorageAccountName} blob={settings.StorageKeyBlobName}  blob-dev={settings.StorageDevKeyBlobName}");
            Console.WriteLine(" ================= END SETTINGS ====================\r\n");
            
            // Custom ProfileService
            services.AddTransient<IProfileService, ProfileService>();
            //services.AddTransient
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


            
            
            services.AddIdentity<O2User, IdentityRole>(options =>
                {
                    // Basic built in validations
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                } )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            

            services.Configure<ManageController.CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=o2platform;AccountKey=DqudZCNaYAcVTMYRP87Tk4+za8+wuKXTKIkY/E22cI6sx8hWkgoyRu32TnuPBc/EavyilImSOMtMwZvUnj3lQA==;EndpointSuffix=core.windows.net"); 
                //CloudStorageAccount.DevelopmentStorageAccount;
                Console.WriteLine($"storageAccount={storageAccount}");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(settings.StorageKeyContainerName);
            
            // The container must exist before calling the DataProtection APIs.
            // The specific file within the container does not have to exist,
            // as it will be created on-demand.

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var blobName = IsProduction ? settings.StorageKeyBlobName : settings.StorageDevKeyBlobName;
            services.AddDataProtection().PersistKeysToAzureBlobStorage(container, blobName);
            
            
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddScoped<VerifyFilter>();
            
            services.AddScoped<IVerification, Verification>();
            services.AddConfiguredLocalization();
            services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
                // .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat)
                // .AddDataAnnotationsLocalization();
       
            // Adds IdentityServer
            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(
                    
                    options =>
                     {
                        options.IssuerUri = "null";
                        options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                    //     options.Events.RaiseErrorEvents = true;
                    //     options.Events.RaiseInformationEvents = true;
                    //     options.Events.RaiseFailureEvents = true;
                    //     options.Events.RaiseSuccessEvents = true;
                     }
                )
                // // this adds the operational data from DB (codes, tokens, consents)
                // .AddOperationalStore(options =>
                // {
                //     options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                //     // this enables automatic token cleanup. this is optional.
                //     options.EnableTokenCleanup = true;
                //     options.TokenCleanupInterval = 30; // interval in seconds
                // })
                // .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(Config.GetUrls(Configuration)))
                .AddAspNetIdentity<O2User>()
                .Services.AddTransient<IProfileService, ProfileService>();
                
            // .SetApplicationName("fow-customer-portal")
            // .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/dpkeys/"));
                // ----- finally Add this DataProtection -----
                // var keysFolder = Path.Combine(WebHostEnvironment.ContentRootPath, "temp-keys");
               
                // .SetApplicationName("Your_Project_Name")
                // .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
                // .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

                services.AddCors(options =>
                {
                    options.AddPolicy(name: MyAllowSpecificOrigins,
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                                .WithOrigins(
                                    "https://pfr-centr.com",
                                    "http://pfr-centr.com",

                                    "https://beta.pfr-centr.com",
                                    "http://beta.pfr-centr.com",

                                    "http://pfr-community.o2bus.com",
                                    "https://pfr-community.o2bus.com",

                                    "http://chat-api.o2bus.com",
                                    "https://chat-api.o2bus.com",

                                    "http://localhost:5010",
                                    "http://localhost:4200",
                                    
                                    "http://localhost:5988",
                                    
                                    "https://client-history-api.staging.o2bus.com",
                                    "http://client-media-api.staging.o2bus.com",
                                    
                                    "http://client-history-api.o2bus.com",
                                    "https://client-media-api.o2bus.com"
                                    
                                    
                                    
                                    )
                                .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                        });
                });
                services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
                
                services.AddSingleton<IVerification>(new Verification(
                    Configuration.GetSection("Twilio").Get<Configuration.Twilio>()));

                

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            
            app.UseSerilogRequestLogging(
                options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath}";
    
                // Emit debug-level events instead of the defaults
                // options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
    
                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            }
                ); // <-- Add this line
            
            IsProduction = env.IsProduction();
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();

            // ref: https://github.com/aspnet/Docs/issues/2384
            app.UseForwardedHeaders(forwardOptions);
            // // Make work identity server redirections in Edge and lastest versions of browers. WARN: Not valid in a production environment.
            // app.Use(async (context, next) =>
            // {
            //     context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
            //     await next();
            // });
            app.UseForwardedHeaders();
            app.UseStaticFiles();
            IsProduction = env.IsProduction();
            // app.UseIdentity(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();
            
            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
            // // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // // the coockies shold be expided from https, but in eShop, the internal comunicacion in aks and docker compose is http.
            // // To avoid this problem, the policy of cookies shold be in Lax mode.
            // app.UseCookiePolicy(new CookiePolicyOptions
            // {
            //     //MinimumSameSitePolicy = AspNetCore.Http.SameSiteMode.Lax
            // });

            app.UseCors(MyAllowSpecificOrigins);
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

 
    }

}
