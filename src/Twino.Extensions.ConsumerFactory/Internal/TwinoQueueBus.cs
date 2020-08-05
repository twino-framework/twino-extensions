using Twino.Client.TMQ.Bus;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.ConsumerFactory.Internal
{
    internal class TwinoQueueBus<TIdentifier> : TwinoQueueBus, ITwinoQueueBus<TIdentifier>
    {
        internal TwinoQueueBus(TmqStickyConnector connector) : base(connector)
        {
        }
    }
}