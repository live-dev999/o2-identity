using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using O2.Identity.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using O2.Identity.Web.Models;
using Serilog;
using Serilog.Sinks.Elasticsearch;


namespace O2.Identity.Web
{
    public class Program
    {

        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);


        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
#if DEBUG
                .WriteTo.File("Logs/system_logs.txt")
#endif
                .WriteTo.Console()
                // .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://localhost:8080" : logstashUrl)
                // .WriteTo.Elasticsearch(
                //     new ElasticsearchSinkOptions(new Uri("http://elk.o2bus.com:9200"))
                //     {
                //         IndexFormat = $"o2-{AppName}-logs-{0:yyyy.MM}",
                //         AutoRegisterTemplate = true,
                //         AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                //     }
                // )
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
        
        public static int Main(string[] args)
        {
            try
            {
                var configuration = GetConfiguration();

                Log.Logger = CreateSerilogLogger(configuration);
                
                var host = CreateHostBuilder(args);

               
                            

                
                using (var scope = host.Services.CreateScope())
                {

                        Log.Information("Configuring web host ({ApplicationContext})...", AppName);                    
                        Log.Information("O2.Identity.Web - Starting up");

                        var services = scope.ServiceProvider;
                        //var logger = services.GetRequiredService<ILogger<Program>>();
                
                        Log.Information("========== Starting Application ==========");
                        var context = services.GetRequiredService<ApplicationDbContext>();

                        var userManager = services.GetRequiredService<UserManager<O2User>>();
                        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                        Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                        IdentityDbInit.Initialize(context, userManager);
                        // Log.Fatal("SUPER ERROR");
                }
                
                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();
            
               return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost CreateHostBuilder(string[] args) =>
            
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                // .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSerilog()
                .UseKestrel(options =>
                {
                    //http://www.binaryintellect.net/articles/612cf2d1-5b3d-40eb-a5ff-924005955a62.aspx
                    options.Limits.MaxRequestBodySize = 209715200;
                })
                .Build();
    }
}
