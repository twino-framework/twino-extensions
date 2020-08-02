using Twino.Client.TMQ.Annotations;

namespace Sample.ConsumerFactory.Models
{
    [QueueId(100)]
    [ChannelName("model-a")]
    [DirectTarget(FindTargetBy.Name, "receiver-name")]
    public class ModelA
    {
    }
}