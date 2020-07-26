using System;
using System.Threading.Tasks;
using Sample.ConsumerFactory.Models;
using Sample.ConsumerFactory.Services;
using Twino.Client.TMQ.Connectors;
using Twino.Extensions.ConsumerFactory;
using Twino.Ioc;

namespace Sample.ConsumerFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //can can test this sample with Sample.Server and Sample.Producer projects in twino-mq project

            IServiceContainer services = new ServiceContainer();
            services.AddTransient<ISampleService, SampleService>();

            services.UseTwinoBus(cfg => cfg.AddHost("tmq://127.0.0.1:22200")
                                           .AddTransientConsumers(typeof(Program)));

            /*
            ITwinoBus bus = await services.Get<ITwinoBus>();

            //push to a queue
            await bus.PushJson(new ModelA(), false);

            //publish to a router
            await bus.PublishJson(new ModelA());

            //to a direct target, ModelA required DirectTarget attribute
            await bus.SendDirectJsonAsync(new ModelA());

*/
            Console.ReadLine();
        }
    }
}