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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.WindowsAzure.Storage;

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
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // _tokenProvider = new AzureServiceTokenProvider();
        }

        public IConfiguration Configuration { get; }

        
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];
            var settings = Configuration.GetSection("DataProtection").Get<DataProtectionSettings>();
            Console.WriteLine(" ========================= SETTINGS ========================== ");
            Console.WriteLine($"ConnectionString={connectionString}");
            Console.WriteLine($"DataProtection AadTenantId={settings.AadTenantId} keyId={settings.KeyVaultKeyId} account={settings.StorageAccountName} blob={settings.StorageKeyBlobName}");
            Console.WriteLine(" ================= END SETTINGS ====================\r\n");
            
            // Custom ProfileService
            services.AddScoped<IProfileService, ProfileService>();
            //services.AddTransient
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<O2User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=o2platform;AccountKey=EYEQMcWR9T82+fdqO4JyawF3Mc1HIEY5ML7476tCFw/mFh9SnyatcnJ5cwlZ9o2vD19BEr1/8WyedkEdcF/rCg==;EndpointSuffix=core.windows.net"); 
                //CloudStorageAccount.DevelopmentStorageAccount;
                Console.WriteLine($"storageAccount={storageAccount}");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(settings.StorageKeyContainerName);
            
            // The container must exist before calling the DataProtection APIs.
            // The specific file within the container does not have to exist,
            // as it will be created on-demand.

            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            services.AddDataProtection().PersistKeysToAzureBlobStorage(container, "keys.xml");
            // // Connect
            // var redis = ConnectionMultiplexer.Connect(Configuration["DPConnectionString"]);
            // services.AddDataProtection(opts =>
            //     {
            //         opts.ApplicationDiscriminator = "eshop.identity";
            //     }).PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                
                // .PersistKeysToRedis(ConnectionMultiplexer.Connect(Configuration["DPConnectionString"]), "DataProtection-Keys"));
            
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});


           



    

            //
            //
            // var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=o2platform;AccountKey=EYEQMcWR9T82+fdqO4JyawF3Mc1HIEY5ML7476tCFw/mFh9SnyatcnJ5cwlZ9o2vD19BEr1/8WyedkEdcF/rCg==;EndpointSuffix=core.windows.net");
            // var client = storageAccount.CreateCloudBlobClient();
            // var container = client.GetContainerReference(settings.StorageKeyContainerName);
            //
            // var azureServiceTokenProvider = new AzureServiceTokenProvider();
            // var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
            //     azureServiceTokenProvider.KeyVaultTokenCallback));
            //
            
             
            //     //This blob must already exist before the application is run
            //     .PersistKeysToAzureBlobStorage(container, settings.StorageKeyBlobName)
            //     // //Removing this line below for an initial run will ensure the file is created correctly
            //     //Todo: I don't understand this code, I will read a description later
            //     .ProtectKeysWithAzureKeyVault(keyVaultClient, settings.KeyVaultKeyId);
            //
            //var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_tokenProvider.KeyVaultTokenCallback));
            // services.AddDataProtection()
            //     .ProtectKeysWithAzureKeyVault(kvClient, settings.KeyVaultKeyId);


            // // Replicates PersistKeysToAzureBlobStorage
            // // There is no overload to give it the func it ultimately uses
            // // We need to do that so that we can get refreshed tokens when needed
            // services.Configure<KeyManagementOptions>(options =>
            // {
            //     options.XmlRepository = new AzureBlobXmlRepository(() =>
            //     {
            //         // This func is called every time before getting the blob and before modifying the blob
            //         // Get access token for Storage
            //         // User / managed identity needs Blob Data Contributor on the Storage Account (container was not enough)
            //         string accessToken = _tokenProvider.GetAccessTokenAsync("https://storage.azure.com/", tenantId: settings.AadTenantId)
            //      .GetAwaiter()
            //      .GetResult();
            //         // Create blob reference with token
            //         var tokenCredential = new TokenCredential(accessToken);
            //         var storageCredentials = new StorageCredentials(tokenCredential.Token);
            //         var uri = new Uri($"https://{settings.StorageAccountName}.blob.core.windows.net/{settings.StorageKeyContainerName}/{settings.StorageKeyBlobName}");
            //         // Note this func is expected to return a new instance on each call
            //         var blob = new CloudBlockBlob(uri, storageCredentials);
            //         return blob;
            //     });
            // });

            // services.AddDataProtection().SetApplicationName("O2 Platform for Business");

            //     
            // var storageAccount = CloudStorageAccount.Parse("<storage account connection string">);
            // var client = storageAccount.CreateCloudBlobClient();
            // var container = client.GetContainerReference("<key store container name>");
            //
            // var azureServiceTokenProvider = new AzureServiceTokenProvider();
            // var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
            //     azureServiceTokenProvider.KeyVaultTokenCallback));
            //
            // services.AddDataProtection()
            //     //This blob must already exist before the application is run
            //     .PersistKeysToAzureBlobStorage(container, "<key store blob name>")
            //     //Removing this line below for an initial run will ensure the file is created correctly
            //     .ProtectKeysWithAzureKeyVault(keyVaultClient, "<keyIdentifier>");
            //
            // var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=o2platform;AccountKey=EYEQMcWR9T82+fdqO4JyawF3Mc1HIEY5ML7476tCFw/mFh9SnyatcnJ5cwlZ9o2vD19BEr1/8WyedkEdcF/rCg==;EndpointSuffix=core.windows.net");
            // var client = storageAccount.CreateCloudBlobClient();
            // var container = client.GetContainerReference("o2-temp-keys");
            //
            // var azureServiceTokenProvider = new AzureServiceTokenProvider();
            // var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
            //         azureServiceTokenProvider.KeyVaultTokenCallback));
            //
            // services.AddDataProtection()
            //     //This blob must already exist before the application is run
            //     .PersistKeysToAzureBlobStorage(container, "o2-protection-key")
            //     //Removing this line below for an initial run will ensure the file is created correctly
            //     .ProtectKeysWithAzureKeyVault(keyVaultClient, "actualkey")
            //     .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            // ----- finally Add this DataProtection -----
            // var keysFolder = Path.Combine(WebHostEnvironment.ContentRootPath, "temp-keys");
            //     services.AddDataProtection()
            // //    .SetApplicationName("O2 Platform for Business")
            //    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
            //    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

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
                .AddProfileService<ProfileService>()
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(Config.GetUrls(Configuration)))
                .AddAspNetIdentity<O2User>();

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
                                    "http://localhost:4200")
                                .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                        });
                });

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

            // app.UseIdentity(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();

            // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure", 
            // the coockies shold be expided from https, but in eShop, the internal comunicacion in aks and docker compose is http.
            // To avoid this problem, the policy of cookies shold be in Lax mode.
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                //MinimumSameSitePolicy = AspNetCore.Http.SameSiteMode.Lax
            });

            app.UseCors(x => x.AllowAnyOrigin()
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
                    "http://localhost:4200")
                .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}
