using Confluent.Kafka;
using OKbkm.Models;
using System.Text.Json;

namespace OKbkm.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers = "3.79.102.137:9092";

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
