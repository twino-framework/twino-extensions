using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twino.Client.TMQ;

namespace Twino.Extensions.ConsumerFactory
{
    internal class MicrosoftDependencyConsumerFactory : IConsumerFactory
    {
        private readonly ServiceLifetime _lifetime;
        private readonly IServiceProvider _provider;
        private IServiceScope _scope;

        public MicrosoftDependencyConsumerFactory(IServiceProvider provider, ServiceLifetime lifetime)
        {
            _lifetime = lifetime;
            _provider = provider;
        }

        public Task<object> CreateConsumer(Type consumerType)
        {
            if (_lifetime == ServiceLifetime.Scoped)
            {
                _scope = _provider.CreateScope();
                return Task.FromResult(_scope.ServiceProvider.GetService(consumerType));
            }

            object consumer = _provider.GetService(consumerType);
            return Task.FromResult(consumer);
        }

        public void Consumed(Exception error)
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }
    }
}