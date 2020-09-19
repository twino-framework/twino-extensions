using Twino.Client.TMQ.Bus;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.Bus.Internal
{
    internal class TwinoRouteBus<TIdentifier> : TwinoRouteBus, ITwinoRouteBus<TIdentifier>
    {
        internal TwinoRouteBus(TmqStickyConnector connector) : base(connector)
        {
        }
    }
}