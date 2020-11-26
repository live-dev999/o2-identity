using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using Microsoft.AspNetCore.Identity;
using O2.Identity.Web.Models;

namespace O2.Identity.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    logger.LogInformation("========== Starting Application ==========");
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    var userManager = services.GetRequiredService<UserManager<O2User>>();
                    // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    IdentityDbInit.Initialize(context, userManager);
                }
                catch (Exception ex)
                {
                    
                    logger.LogError(ex, "An error occurred while seeding the AuthorizationServer database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
