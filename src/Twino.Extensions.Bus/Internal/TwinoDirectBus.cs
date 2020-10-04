using Twino.Client.TMQ.Bus;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.Bus.Internal
{
    internal class TwinoDirectBus<TIdentifier> : TwinoDirectBus, ITwinoDirectBus<TIdentifier>
    {
        internal TwinoDirectBus(TmqStickyConnector connector) : base(connector)
        {
        }
    }
}