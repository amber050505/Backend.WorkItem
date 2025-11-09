using Backend.WorkItem.Repository.WorkItem.Interface;
using Backend.WorkItem.Service.WorkItem.Interface;
using Confluent.Kafka;
using StackExchange.Redis;
using System.Text.Json;

namespace Backend.WorkItem.Service.WorkItem
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _repo;
        private readonly IDatabase _redis;
        private readonly IProducer<Null, string> _producer;

        private const string CacheListKey = "WorkItems:List";

        public WorkItemService(
            IWorkItemRepository repo, 
            IConnectionMultiplexer redis,
            IProducer<Null,string> producer)
        {
            _repo = repo;
            _redis = redis.GetDatabase();
            _producer = producer;
        }

        public async Task<IEnumerable<Model.WorkItem>> GetAllAsync()
        {
            var cached = await _redis.StringGetAsync(CacheListKey);
            if (!cached.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<IEnumerable<Model.WorkItem>>(cached!)!;
            }

            var list = await _repo.GetAllAsync();
            if (list.Any())
            {
                var json = JsonSerializer.Serialize(list);
                await _redis.StringSetAsync(CacheListKey, json, TimeSpan.FromMinutes(1));
            }
            return list;
        }

        public async Task<Model.WorkItem> GetByIdAsync(int id)
        {
            var cacheKey = $"WorkItems:{id}";
            var cached = await _redis.StringGetAsync(cacheKey);
            if (!cached.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Model.WorkItem>(cached!)!;
            }

            var item = await _repo.GetByIdAsync(id);
            if (item != null)
            {
                await _redis.StringSetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(item),
                    TimeSpan.FromMinutes(1));
            }

            return item;
        }

        public async Task CreateAsync(Model.WorkItem item)
        {
            item.Title = item.Title!.Trim();

            var msg = new Model.WorkItemKafkaMessage
            {
                Operation = "create",
                Title = item.Title,
                Description = item.Description
            };

            await PublishAsync("workitem-events", msg);

            await _redis.KeyDeleteAsync(CacheListKey);
        }

        public async Task<bool> UpdateAsync(Model.WorkItem item)
        {
            item.Title = item.Title!.Trim();

            var isUpdate = await _repo.UpdateAsync(item);

            await _redis.KeyDeleteAsync(CacheListKey);
            await _redis.KeyDeleteAsync($"WorkItems:{item.Id}");

            return isUpdate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isDelete = await _repo.DeleteAsync(id);

            await _redis.KeyDeleteAsync(CacheListKey);
            await _redis.KeyDeleteAsync($"WorkItems:{id}");

            return isDelete;
        }

        private async Task PublishAsync(string topic, Model.WorkItemKafkaMessage msg)
        {
            var json= JsonSerializer.Serialize(msg);
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json }); 
        }
    }
}
