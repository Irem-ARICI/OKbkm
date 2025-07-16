using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OKbkm.Services
{
    public static class KafkaTopicInitializer
    {
        public static async Task EnsureTopicsExistAsync(string bootstrapServers, string[] topics)
        {
            var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

            var topicsToCreate = topics
                .Where(topic => !existingTopics.Contains(topic))
                .Select(topic => new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = 3,
                    ReplicationFactor = 3
                })
                .ToList();

            if (topicsToCreate.Any())
            {
                await adminClient.CreateTopicsAsync(topicsToCreate);
            }
        }
    }
}
