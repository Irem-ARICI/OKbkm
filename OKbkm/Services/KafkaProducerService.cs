using Confluent.Kafka;
using Confluent.Kafka.Admin;
using OKbkm.Models;
using System.Text.Json;

namespace OKbkm.Services
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;

        public KafkaProducerService(IConfiguration configuration)
        {
            // Docker ortamı için environment variable veya appsettings'ten alınır
            _bootstrapServers = configuration["KAFKA__BOOTSTRAP__SERVERS"] ?? "kafka1:9092,kafka2:9092,kafka3:9092";
        }

        public async Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 1, short replicationFactor = 1)
        {
            var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                if (metadata.Topics.Any(t => t.Topic == topicName))
                {
                    Console.WriteLine($"[Kafka] '{topicName}' topic'i zaten mevcut.");
                    return;
                }

                Console.WriteLine($"[Kafka] '{topicName}' topic'i oluşturuluyor...");
                await adminClient.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = numPartitions,
                        ReplicationFactor = replicationFactor
                    }
                });
                Console.WriteLine($"[Kafka] '{topicName}' topic'i oluşturuldu.");
            }
            catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                Console.WriteLine($"[Kafka] '{topicName}' zaten mevcut (Exception ile yakalandı).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kafka] Topic oluşturulurken hata: {ex.Message}");
            }
        }

        public async Task SendMessageAsync(string topic, TransactionEvent message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _bootstrapServers,
                Acks = Acks.All
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            string jsonMessage = JsonSerializer.Serialize(message);

            try
            {
                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
                Console.WriteLine($"[Kafka] Mesaj gönderildi: {result.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"[Kafka] Mesaj gönderme hatası: {ex.Error.Reason}");
            }
        }
    }
}





//using Confluent.Kafka;
//using Confluent.Kafka.Admin;
//using OKbkm.Models;
//using System.Text.Json;

//namespace OKbkm.Services
//{
//    public class KafkaProducerService
//    {
//        private readonly string _bootstrapServers = "kafka:9092"; // Docker ortamında host ismi

//        // Topic varsa geç, yoksa oluştur
//        public async Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 1, short replicationFactor = 1)
//        {
//            var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };

//            using var adminClient = new AdminClientBuilder(config).Build();

//            try
//            {
//                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
//                bool exists = metadata.Topics.Any(t => t.Topic == topicName);

//                if (!exists)
//                {
//                    Console.WriteLine($"[Kafka] '{topicName}' topic'i oluşturuluyor...");
//                    await adminClient.CreateTopicsAsync(new[]
//                    {
//                        new TopicSpecification
//                        {
//                            Name = topicName,
//                            NumPartitions = numPartitions,
//                            ReplicationFactor = replicationFactor
//                        }
//                    });
//                    Console.WriteLine($"[Kafka] '{topicName}' topic'i oluşturuldu.");
//                }
//                else
//                {
//                    Console.WriteLine($"[Kafka] '{topicName}' topic'i zaten mevcut.");
//                }
//            }
//            catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
//            {
//                Console.WriteLine($"[Kafka] '{topicName}' zaten mevcut (Exception ile yakalandı).");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[Kafka] Topic oluşturulurken hata: {ex.Message}");
//            }
//        }

//        // Kafka'ya mesaj gönder
//        public async Task SendMessageAsync(string topic, TransactionEvent message)
//        {
//            var config = new ProducerConfig
//            {
//                BootstrapServers = _bootstrapServers,
//                Acks = Acks.All
//            };

//            using var producer = new ProducerBuilder<Null, string>(config).Build();
//            string jsonMessage = JsonSerializer.Serialize(message);

//            try
//            {
//                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = jsonMessage });
//                Console.WriteLine($"[Kafka] Mesaj gönderildi: {result.TopicPartitionOffset}");
//            }
//            catch (ProduceException<Null, string> ex)
//            {
//                Console.WriteLine($"[Kafka] Mesaj gönderme hatası: {ex.Error.Reason}");
//            }
//        }
//    }
//}


////using Npgsql.EntityFrameworkCore.PostgreSQL;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Extensions.DependencyInjection;
////using Microsoft.Extensions.Hosting;
////using Microsoft.Extensions.Configuration;
////using OKbkm;
////using OKbkm.Models;
////using OKbkm.Services;

////var builder = WebApplication.CreateBuilder(new WebApplicationOptions
////{
////    ContentRootPath = AppContext.BaseDirectory,
////    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
////});

////// Docker için dış bağlantılara açık hale getiriyoruz (0.0.0.0:8081)
////builder.WebHost.UseUrls("http://0.0.0.0:8080", "http://0.0.0.0:8081");

////// appsettings.json’dan ConnectionString’i al
////var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
////builder.Services.AddDbContext<Context>(options =>
////    options.UseNpgsql(connectionString));

////builder.Services.AddControllers();
////builder.Services.AddEndpointsApiExplorer();
////builder.Services.AddSwaggerGen();
////builder.Services.AddControllersWithViews();
////builder.Services.AddSession(); // Session'ı ekliyoruz
////builder.Services.AddSingleton<KafkaProducerService>();

////var app = builder.Build();

////// ❗ Kafka Topic'lerini Uygulama Başlarken Oluştur
////using (var scope = app.Services.CreateScope())
////{
////    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
////    dbContext.Database.Migrate();

////    // Kafka topic'lerini otomatik oluştur
////    var kafka = scope.ServiceProvider.GetRequiredService<KafkaProducerService>();
////    await kafka.CreateTopicIfNotExistsAsync("deposit-topic");
////    await kafka.CreateTopicIfNotExistsAsync("withdraw-topic");
////    await kafka.CreateTopicIfNotExistsAsync("transfer-topic");
////}

////app.UseStaticFiles();
////app.UseRouting();

////app.MapControllerRoute(
////    name: "default",
////    pattern: "{controller=Home}/{action=Index}/{id?}");

////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}

////app.UseSession();
////// app.UseHttpsRedirection(); // İstersen aktif edebilirsin
////app.UseAuthorization();
////app.MapControllers();

////app.Run();
///
