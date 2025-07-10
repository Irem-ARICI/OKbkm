//using Confluent.Kafka;
//using OKbkm.Models;
//using System.Text.Json;

//namespace OKbkm.Services
//{
//    public class KafkaProducerService
//    {
//        private readonly string _bootstrapServers = "3.121.208.190:9092";

//        public async Task SendMessageAsync(string topic, TransactionEvent message)
//        {
//            var config = new ProducerConfig
//            {
//                BootstrapServers = _bootstrapServers,
//                Acks = Acks.All
//            };

//            using var producer = new ProducerBuilder<Null, string>(config).Build();
//            string jsonMessage = JsonSerializer.Serialize(message);

//            await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
//        }
//    }
//}
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using OKbkm.Models;
using System.Text.Json;

namespace OKbkm.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers = "3.121.208.190:9092";

        // Kafka topic varsa geç, yoksa oluştur
        public async Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 1, short replicationFactor = 1)
        {
            var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                bool exists = metadata.Topics.Exists(t => t.Topic == topicName);

                if (!exists)
                {
                    await adminClient.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = numPartitions,
                            ReplicationFactor = replicationFactor
                        }
                    });
                }
            }
            catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                // Zaten varsa bir şey yapma
            }
        }

        // Kafka'ya mesaj gönder
        public async Task SendMessageAsync(string topic, TransactionEvent message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
                Acks = Acks.All
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            string jsonMessage = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
        }
    }
}