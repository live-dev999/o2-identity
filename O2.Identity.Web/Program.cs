using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using Microsoft.AspNetCore.Identity;
using O2.Identity.Web.Models;
using Serilog;


namespace O2.Identity.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    Log.Information("O2.Identity.Web - Starting up");

                var services = scope.ServiceProvider;
                    //var logger = services.GetRequiredService<ILogger<Program>>();
              
                    Log.Information("========== Starting Application ==========");
                    var context = services.GetRequiredService<ApplicationDbContext>();

                    var userManager = services.GetRequiredService<UserManager<O2User>>();
                    // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    IdentityDbInit.Initialize(context, userManager);
                }
                catch (Exception ex)
                {
                    
                    Log.Error(ex, "An error occurred while seeding the AuthorizationServer database.");
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog() // <- Add this line
                .Build();
    }
}
