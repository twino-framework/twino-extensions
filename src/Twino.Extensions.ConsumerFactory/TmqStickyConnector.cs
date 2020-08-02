using System;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Used for using multiple twino bus in same provider.
    /// Template type is the identifier
    /// </summary>
    public class TmqStickyConnector<TIdentifier> : TmqStickyConnector, ITwinoBus<TIdentifier>
    {
        internal TmqStickyConnector(TimeSpan reconnectInterval, Func<TmqClient> createInstance = null) : base(reconnectInterval, createInstance)
        {
        }
    }
}