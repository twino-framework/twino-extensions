using System;
using System.Threading.Tasks;
using Sample.ConsumerFactory.Consumers;
using Sample.ConsumerFactory.Services;
using Twino.Client.TMQ;
using Twino.Extensions.ConsumerFactory;
using Twino.Ioc;

namespace Sample.ConsumerFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceContainer container = new ServiceContainer();
            container.AddScoped<ISampleService, SampleService>();
            
            MessageConsumer consumer = MessageConsumer.JsonConsumer();
            container.AddScopedConsumers(consumer, typeof(Program));
            
            IContainerScope scope = container.CreateScope();
            QueueConsumerA consumerA = await container.Get<QueueConsumerA>(scope);
            Console.WriteLine(consumerA);
        }
    }
}