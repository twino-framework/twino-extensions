using System;
using Twino.Client.TMQ;
using Twino.Client.TMQ.Connectors;
using Twino.Extensions.ConsumerFactory.Internal;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Used for using multiple twino bus in same provider.
    /// Template type is the identifier
    /// </summary>
    public class TmqStickyConnector<TIdentifier> : TmqStickyConnector
    {
        internal TmqStickyConnector(TimeSpan reconnectInterval, Func<TmqClient> createInstance = null) : base(reconnectInterval, createInstance)
        {
            Bus = new TwinoBus<TIdentifier>(this);
        }
    }
}