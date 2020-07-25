using System;
using System.Threading.Tasks;
using Sample.ConsumerFactory.Consumers;
using Sample.ConsumerFactory.Services;
using Twino.Extensions.ConsumerFactory;
using Twino.Ioc;

namespace Sample.ConsumerFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //can can test this sample with Sample.Server and Sample.Producer projects in twino-mq project
            
            IServiceContainer container = new ServiceContainer();
            container.AddTransient<ISampleService, SampleService>();

            container.UseTwinoBus(cfg => cfg.AddHost("tmq://127.0.0.1:22200")
                                            .AddTransientConsumer<QueueConsumerA>());
                                            //.AddSingletonConsumers(typeof(Program)));


            Console.ReadLine();
        }
    }
}