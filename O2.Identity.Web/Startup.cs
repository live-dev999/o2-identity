using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using O2.Identity.Web.Models;
using O2.Identity.Web.Services;

namespace O2.Identity.Web
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
            var connectionString = Configuration["ConnectionString"];
            Console.WriteLine(" ========================= SETTINGS ========================== ");
            Console.WriteLine($"ConnectionString={connectionString}");
            Console.WriteLine(" ================= END SETTINGS ====================\r\n");
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<O2User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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
            services.AddDataProtection()
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30));
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
