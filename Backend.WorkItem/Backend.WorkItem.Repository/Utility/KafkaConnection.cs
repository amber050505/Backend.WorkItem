using Backend.WorkItem.Repository.Utility.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Backend.WorkItem.Repository.Utility
{
    public class KafkaConnection : IKafkaConnection
    {
        public IProducer<Null, string> Producer { get; }

        public KafkaConnection(IConfiguration configuration)
        {
            var kafkaBootstrap = configuration["Kafka:BootstrapServers"];
            if (string.IsNullOrWhiteSpace(kafkaBootstrap))
            {
                throw new InvalidOperationException("Kafka:BootstrapServers not found in configuration.");
            }

            var config = new ProducerConfig
            {
                BootstrapServers = kafkaBootstrap
            };
            
            Producer = new ProducerBuilder<Null, string>(config).Build();
        }
    }
}
