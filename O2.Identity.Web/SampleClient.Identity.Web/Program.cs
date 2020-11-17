using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace SampleClient.Identity.Web
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            //discover all the points using matadate of identity server

            // var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var client = new HttpClient();
            var discoResponse = await client.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = "http://localhost:5000",
                    Policy =
                    {
                        ValidateIssuerName = false,
                        ValidateEndpoints = false,
                    },
                }
            );

            if (discoResponse.IsError)
            {
                Console.WriteLine(discoResponse.Error);
                return;
            }
            
            
            //grab a bearer token
            var tokenClient = new TokenClient(d);
        }
    }
}