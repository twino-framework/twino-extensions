using System;
using System.Threading.Tasks;
using Sample.ConsumerFactory.Models;
using Sample.ConsumerFactory.Services;
using Twino.Client.TMQ;
using Twino.Protocols.TMQ;

namespace Sample.ConsumerFactory.Consumers
{
    public class QueueConsumerA : IQueueConsumer<ModelA>
    {
        private readonly ISampleService _sampleService;

        public QueueConsumerA(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        public Task Consume(TmqMessage message, ModelA model, TmqClient client)
        {
            Console.WriteLine("Model A Consumed");
            return Task.CompletedTask;
        }
    }
}