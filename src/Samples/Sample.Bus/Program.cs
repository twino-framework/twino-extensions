using System;
using System.Threading.Tasks;
using Sample.ConsumerFactory.Models;
using Sample.ConsumerFactory.Services;
using Twino.Client.TMQ.Bus;
using Twino.Extensions.Bus;
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


            ITwinoBus bus = services.Get<ITwinoBus>();

            //push to a queue
            await bus.Queue.PushJson(new ModelA());

            //publish to a router
            await bus.Route.PublishJson(new ModelA());

            //to a direct target, ModelA required DirectTarget attribute
            await bus.Direct.SendJson(new ModelA());

            Console.ReadLine();
        }
    }
}