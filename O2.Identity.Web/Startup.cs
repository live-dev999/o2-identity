﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using O2.Identity.Web.Models;
using O2.Identity.Web.Services;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StorageCredentials = Microsoft.WindowsAzure.Storage.Auth.StorageCredentials;

namespace O2.Identity.Web
{
    public class Startup
    {
        private readonly AzureServiceTokenProvider _tokenProvider;
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
            _tokenProvider = new AzureServiceTokenProvider();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionString"];
            Console.WriteLine(" ========================= SETTINGS ========================== ");
            Console.WriteLine($"ConnectionString={connectionString}");
            Console.WriteLine(" ================= END SETTINGS ====================\r\n");
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<O2User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});


            var settings = Configuration.GetSection("DataProtection").Get<DataProtectionSettings>();
            
            var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(_tokenProvider.KeyVaultTokenCallback));

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=o2platform;AccountKey=EYEQMcWR9T82+fdqO4JyawF3Mc1HIEY5ML7476tCFw/mFh9SnyatcnJ5cwlZ9o2vD19BEr1/8WyedkEdcF/rCg==;EndpointSuffix=core.windows.net");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(settings.StorageKeyContainerName);

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                azureServiceTokenProvider.KeyVaultTokenCallback));

            services.AddDataProtection()
                //This blob must already exist before the application is run
                .PersistKeysToAzureBlobStorage(container, settings.StorageKeyBlobName)
                // //Removing this line below for an initial run will ensure the file is created correctly
                //Todo: I don't understand this code, I will read a description later
                .ProtectKeysWithAzureKeyVault(keyVaultClient, settings.KeyVaultKeyId);
            
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

            // configure identity server with in-memory stores, keys, clients and scopes
            services.AddIdentityServer(
                    
                    options =>
                     {
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

            app.UseStaticFiles();

            // app.UseIdentity(); // not needed, since UseIdentityServer adds the authentication middleware
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

}
