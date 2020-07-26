using System;
using System.ComponentModel;
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
            BasicUsage();
        }

        static void BasicUsage()
        {
            IServiceContainer container = new ServiceContainer();

            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient(32, httpClient => { httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token..."); });

            //IHttpClientFactory is injectable
            IHttpClientFactory factory = container.Get<IHttpClientFactory>();
            HttpClient client = factory.Create();
            //use client here
        }

        static void NamedUsage()
        {
            IServiceContainer container = new ServiceContainer();

            //for service a
            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient("service-a", 16, httpClient => { httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token-A..."); });

            //for service b
            //pool size 32
            //configuration action is executed before each factory.Create() usage
            container.AddHttpClient("service-b", 8, httpClient => { httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token-B..."); });

            //IHttpClientFactory is injectable
            IHttpClientFactory factory = container.Get<IHttpClientFactory>();

            //get for service-a
            HttpClient clientA = factory.Create("service-a");

            //get for service-b
            HttpClient clientB = factory.Create("service-b");
            //use client here
        }
    }
}