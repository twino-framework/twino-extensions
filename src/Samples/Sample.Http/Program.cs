using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Twino.Extensions.Http;
using Twino.Ioc;

namespace Sample.Http
{
    class Program
    {
        static async Task Main(string[] args)
        {
        }

        static async Task BasicUsage()
        {
            IServiceContainer container = new ServiceContainer();

            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient(32, httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ey...");
            });

            //IHttpClientFactory is injectable
            IHttpClientFactory factory = await container.Get<IHttpClientFactory>();
            HttpClient client = factory.Create();
            //use client here
        }

        static async Task NamedUsage()
        {
            IServiceContainer container = new ServiceContainer();

            //for service a
            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient("service-a", 16, httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ey...");
            });
            
            //for service b
            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient("service-b", 8, httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ey...");
            });

            //IHttpClientFactory is injectable
            IHttpClientFactory factory = await container.Get<IHttpClientFactory>();
            
            //get for service-a
            HttpClient clientA = factory.Create("service-a");
            
            //get for service-b
            HttpClient clientB = factory.Create("service-b");
            //use client here
        }
    }
}