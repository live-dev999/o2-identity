using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace O2.Identity.Web.Extensions
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddConfiguredLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services
                .Configure<RequestLocalizationOptions>(options =>
                {
                    var cultures = new[]
                    {
                        new CultureInfo("en"),
                        new CultureInfo("ru"),
                        new CultureInfo("de"),
                        new CultureInfo("tr")
                    };
                    options.DefaultRequestCulture = new RequestCulture("en");
                    options.SupportedCultures = cultures;
                    options.SupportedUICultures = cultures;
                });

            return services;
        }
    }
}