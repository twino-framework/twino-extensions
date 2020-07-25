using Twino.Client.TMQ.Annotations;

namespace Sample.ConsumerFactory.Models
{
    [QueueId(100)]
    [ChannelName("model-a")]
    public class ModelA
    {
    }
}