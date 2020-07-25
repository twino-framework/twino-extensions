using Twino.Client.TMQ.Connectors;

namespace Twino.Extensions.ConsumerFactory
{
    /// <summary>
    /// Used for using multiple twino bus in same provider.
    /// Template type is the identifier
    /// </summary>
    public interface ITwinoBus<TIdentifier> : ITwinoBus
    {
    }
}