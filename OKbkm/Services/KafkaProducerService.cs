using Confluent.Kafka;
using System.Text.Json;

namespace OKbkm.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;
        private readonly string _topic;

        public KafkaProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"];
            _topic = configuration["Kafka:Topic"];
        }

        public async Task SendMessageAsync<T>(T message, string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var jsonMessage = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = jsonMessage
            });
        }
    }
}








//using Confluent.Kafka;
//using System.Text.Json;

//namespace OKbkm.Services
//{
//    public class KafkaProducerService
//    {
//        private readonly IProducer<string, string> _producer;

//        public KafkaProducerService(string bootstrapServers)
//        {
//            var config = new ProducerConfig
//            {
//                BootstrapServers = bootstrapServers,
//                Acks = Acks.All,
//                MessageTimeoutMs = 10000,
//                SocketTimeoutMs = 10000,
//                EnableIdempotence = true,
//                //DeliveryTimeoutMs = 15000,
//                RetryBackoffMs = 500
//            };

//            _producer = new ProducerBuilder<string, string>(config).Build();
//        }

//        public async Task ProduceAsync(string topic, object message)
//        {
//            var jsonMessage = JsonSerializer.Serialize(message);

//            try
//            {
//                var result = await _producer.ProduceAsync(topic, new Message<string, string>
//                {
//                    Key = Guid.NewGuid().ToString(),
//                    Value = jsonMessage
//                });

//                Console.WriteLine($"✅ Kafka'ya mesaj gönderildi: {result.TopicPartitionOffset}");
//            }
//            catch (ProduceException<string, string> ex)
//            {
//                Console.WriteLine("🚨 Kafka ProduceException:");
//                Console.WriteLine($"• Hata: {ex.Error.Reason}");
//                Console.WriteLine($"• Hata Kodu: {ex.Error.Code}");
//                Console.WriteLine($"• Fatal mı? {ex.Error.IsFatal}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"🔥 Genel hata oluştu: {ex}");
//            }
//        }
//    }
//}





////using Confluent.Kafka;
////using System.Text.Json;

////namespace OKbkm.Services
////{
////    public class KafkaProducerService
////    {
////        private readonly IProducer<string, string> _producer;

////        public KafkaProducerService(string bootstrapServers)
////        {
////            var config = new ProducerConfig
////            {
////                BootstrapServers = bootstrapServers
////            };
////            _producer = new ProducerBuilder<string, string>(config).Build();
////        }

////        public async Task ProduceAsync(string topic, object message)
////        {
////            var jsonMessage = JsonSerializer.Serialize(message);
////            await _producer.ProduceAsync(topic, new Message<string, string>
////            {
////                Key = Guid.NewGuid().ToString(),
////                Value = jsonMessage
////            });
////        }
////    }
////}
