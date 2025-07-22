using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OKbkm.Models;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace OKbkm.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _bootstrapServers;
        private readonly List<string> _topics;

        public KafkaConsumerService(
            IConfiguration configuration,
            ILogger<KafkaConsumerService> logger,
            IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _bootstrapServers = _configuration["Kafka:BootstrapServers"];
            _topics = new List<string>
            {
                _configuration["Kafka:DepositTopic"],
                _configuration["Kafka:WithdrawTopic"],
                _configuration["Kafka:TransferTopic"]
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topics);

            _logger.LogInformation("Kafka consumer başlatıldı ve topic'lere abone olundu.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    var rawMessage = result.Message.Value;

                    _logger.LogInformation($"[✔] Kafka'dan mesaj alındı | Topic: {result.Topic} | İçerik: {rawMessage}");

                    var transaction = JsonSerializer.Deserialize<TransactionEvent>(rawMessage);

                    if (transaction != null)
                    {
                        _logger.LogInformation($"[✔] Mesaj deserialize edildi | Hesap: {transaction.AccountNo} | Tür: {transaction.Type} | Tutar: {transaction.Amount}");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<Context>();

                            await dbContext.TransactionEvents.AddAsync(transaction);
                            await dbContext.SaveChangesAsync();

                            _logger.LogInformation("✅ TransactionEvent veritabanına kaydedildi.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Mesaj deserialize edilemedi, null döndü.");
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError($"❌ JSON çözümleme hatası: {jsonEx.Message}");
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"❌ Kafka tüketim hatası: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Genel hata: {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }

            consumer.Close();
            _logger.LogInformation("Kafka consumer durduruldu.");
        }
    }
}
