using Twino.Client.TMQ.Annotations;

namespace Sample.ConsumerFactory.Models
{
    [QueueName("model-a")]
    [RouterName("model-a")]
    [DirectTarget(FindTargetBy.Name, "receiver-name")]
    public class ModelA
    {
    }
}