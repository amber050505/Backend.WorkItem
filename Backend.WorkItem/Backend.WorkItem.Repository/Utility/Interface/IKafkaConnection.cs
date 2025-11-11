using Confluent.Kafka;

namespace Backend.WorkItem.Repository.Utility.Interface
{
    public interface IKafkaConnection
    {
        IProducer<Null, string> Producer { get; }
    }
}
