using Backend.WorkItem.Model;
using Backend.WorkItem.Repository.Utility.Interface;
using Backend.WorkItem.Repository.WorkItem.Interface;
using Backend.WorkItem.Service.WorkItem.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Text.Json;

namespace Backend.WorkItem.Service.WorkItem
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _repo;
        private readonly IDatabase _redis;
        private readonly IKafkaConnection _kafka;
        private readonly IMemoryCache _cache;

        private const string Topic = "workitem-events";
        private const string WorkItemCacheKeys = "WorkItem_Cache_Keys";

        public WorkItemService(
            IWorkItemRepository repo,
            IRedisConnection redis,
            IKafkaConnection kafka,
            IMemoryCache cache)
        {
            _repo = repo;
            _redis = redis.Database;
            _kafka = kafka;
            _cache = cache;
        }

        public async Task<WorkItemList> GetAllAsync(int page)
        {
            string cacheKey = $"WorkItem_Page_{page}";

            if (!_cache.TryGetValue(cacheKey, out WorkItemList result))
            {
                result = await _repo.GetAllAsync(page);
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
                AddWorkItemCacheKey(cacheKey);
            }

            return result;
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

            await PublishAsync(msg);

            //await _redis.KeyDeleteAsync(CacheListKey);
        }

        public async Task UpdateAsync(Model.WorkItem item)
        {
            item.Title = item.Title!.Trim();

            var msg = new Model.WorkItemKafkaMessage
            {
                Operation = "update",
                Id = item.Id,
                Title = item.Title,
                Description = item.Description
            };

            await PublishAsync(msg);

            //await _redis.KeyDeleteAsync(CacheListKey);
            await _redis.KeyDeleteAsync($"WorkItems:{item.Id}");
        }

        public async Task DeleteAsync(int id)
        {
            var msg = new Model.WorkItemKafkaMessage
            {
                Operation = "delete",
                Id = id
            };
            await PublishAsync(msg);

            //await _redis.KeyDeleteAsync(CacheListKey);
            await _redis.KeyDeleteAsync($"WorkItems:{id}");
        }

        private async Task PublishAsync(Model.WorkItemKafkaMessage msg)
        {
            var json = JsonSerializer.Serialize(msg);
            await _kafka.Producer.ProduceAsync(Topic, new Message<Null, string> { Value = json });
        }

        private void AddWorkItemCacheKey(string key)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };

            if (!_cache.TryGetValue(WorkItemCacheKeys, out HashSet<string> keys))
            {
                keys = new HashSet<string>();
            }

            keys.Add(key);

            _cache.Set(WorkItemCacheKeys, keys, options);
        }
    }
}
