using Backend.WorkItem.Repository.WorkItem.Interface;
using Confluent.Kafka;
using StackExchange.Redis;
using System.Text.Json;

namespace Backend.WorkItem.BackgroundServices
{
    public class WorkItemKafkaConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private readonly ConsumerConfig _config;

        private const string Topic = "workitem-events";
        private const string CacheListKey = "WorkItems:List";

        public WorkItemKafkaConsumer(
            IServiceProvider serviceProvider,
            IConnectionMultiplexer redis,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _redis = redis;
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

                var db = _redis.GetDatabase();

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

                                await db.KeyDeleteAsync(CacheListKey);
                                break;
                            }
                    }

                    consumer.Commit(consumerResult);
                }

                consumer.Close();
            }, stoppingToken);
        }
    }
}
