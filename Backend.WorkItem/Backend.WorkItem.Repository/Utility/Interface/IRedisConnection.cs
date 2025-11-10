using StackExchange.Redis;

namespace Backend.WorkItem.Repository.Utility.Interface
{
    public interface IRedisConnection
    {
        IDatabase Database { get; }
    }
}
