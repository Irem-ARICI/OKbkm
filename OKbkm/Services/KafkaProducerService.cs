using Confluent.Kafka;
using System.Text.Json;

namespace OKbkm.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers = "localhost:9092"; // docker-compose içi host adı

        public async Task SendMessageAsync<T>(string topic, T message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            string json = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(topic, new Message<Null, string> { Value = json });
        }
    }
}
