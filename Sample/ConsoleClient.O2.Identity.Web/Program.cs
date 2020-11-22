using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleClient.O2.Identity.Web
{
    class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }


            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("CertificateApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");


            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var customerInfo = new StringContent(JsonConvert.SerializeObject(
                    new
                    {
                        Id = 10,
                        FirstName = "",
                        LastName = ""
                    }),
                Encoding.UTF8,
                "application/json"
            );

            var createCustomerResponse = await client.PostAsync("http://localhost:5000/account/register", customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
            }

            var getCustomerResponse = await client.GetAsync("http://localhost:5000/account/register");
            if (!getCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(getCustomerResponse.StatusCode);
            }
            else
            {
                var content = await getCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.Read();
        }
    }
}
