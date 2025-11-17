using Backend.WorkItem.Repository.Utility.Interface;
using Backend.WorkItem.Repository.WorkItem.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Backend.WorkItem.BackgroundServices
{
    public class WorkItemKafkaConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRedisConnection _redis;
        private readonly IMemoryCache _cache;
        private readonly ConsumerConfig _config;

        private const string Topic = "workitem-events";
        private const string WorkItemCacheKeys = "WorkItem_Cache_Keys";

        public WorkItemKafkaConsumer(
            IServiceProvider serviceProvider,
            IRedisConnection redis,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _redis = redis;
            _cache = cache;
            _config = new ConsumerConfig()
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "workitem-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
                consumer.Subscribe(Topic);

                var db = _redis.Database;

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumerResult = consumer.Consume(stoppingToken);
                    if (consumerResult == null || string.IsNullOrWhiteSpace(consumerResult.Message.Value))
                    {
                        continue;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IWorkItemRepository>();

                    var msg = JsonSerializer.Deserialize<Model.WorkItemKafkaMessage>(consumerResult.Message.Value);
                    if (msg == null || string.IsNullOrEmpty(msg.Operation))
                    {
                        continue;
                    }

                    switch (msg.Operation)
                    {
                        case "create":
                            {
                                var item = new Model.WorkItem
                                {
                                    Title = msg.Title,
                                    Description = msg.Description
                                };
                                var newId = await repo.CreateAsync(item);

                                ClearWorkItemCache();
                                break;
                            }
                        case "update":
                            {
                                var item = new Model.WorkItem
                                {
                                    Id = (int)msg.Id!,
                                    Title = msg.Title,
                                    Description = msg.Description
                                };
                                await repo.UpdateAsync(item);

                                ClearWorkItemCache();
                                await db.KeyDeleteAsync($"WorkItems:{item.Id}");
                                break;
                            }
                        case "delete":
                            {
                                await repo.DeleteAsync((int)msg.Id!);

                                ClearWorkItemCache();
                                await db.KeyDeleteAsync($"WorkItems:{msg.Id}");
                                break;
                            }
                    }

                    consumer.Commit(consumerResult);
                }

                consumer.Close();
            }, stoppingToken);
        }

        private void ClearWorkItemCache()
        {
            if (_cache.TryGetValue(WorkItemCacheKeys, out HashSet<string> keys))
            {
                foreach (var key in keys)
                {
                    _cache.Remove(key);
                }
                _cache.Remove(WorkItemCacheKeys);
            }
        }
    }
}
