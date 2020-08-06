using Twino.Client.TMQ.Bus;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.ConsumerFactory.Internal
{
    internal class TwinoBus<TIdentifier> : TwinoBus, ITwinoBus<TIdentifier>
    {
        internal TwinoBus(TmqStickyConnector connector) : base(connector)
        {
            Direct = new TwinoDirectBus<TIdentifier>(connector);
            Queue = new TwinoQueueBus<TIdentifier>(connector);
            Route = new TwinoRouteBus<TIdentifier>(connector);
        }
    }
}