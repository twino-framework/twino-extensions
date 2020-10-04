using System;
using System.Threading.Tasks;
using Twino.Client.TMQ;
using Twino.Ioc;

namespace Twino.Extensions.Bus
{
    internal class TwinoIocConsumerFactory : IConsumerFactory
    {
        private readonly IServiceContainer _container;
        private IContainerScope _scope;
        private readonly ImplementationType _implementationType;

        public TwinoIocConsumerFactory(IServiceContainer container, ImplementationType implementationType)
        {
            _container = container;
            _implementationType = implementationType;
        }

        public Task<object> CreateConsumer(Type consumerType)
        {
            _scope = _implementationType == ImplementationType.Scoped ? _container.CreateScope() : null;
            object consumer = _container.Get(consumerType, _scope);
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