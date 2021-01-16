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
            try
            {
                var host = CreateHostBuilder(args);

                Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .CreateLogger();
                using (var scope = host.Services.CreateScope())
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
                host.Run();
            
            }
            catch (Exception ex)
            {
                
                Log.Error(ex, "An error occurred while seeding the AuthorizationServer database.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    //http://www.binaryintellect.net/articles/612cf2d1-5b3d-40eb-a5ff-924005955a62.aspx
                    options.Limits.MaxRequestBodySize = 209715200;
                })
                .UseSerilog() // <- Add this line
                .Build();
    }
}
