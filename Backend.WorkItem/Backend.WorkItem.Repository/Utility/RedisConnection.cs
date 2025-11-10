using Backend.WorkItem.Repository.Utility.Interface;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Backend.WorkItem.Repository.Utility
{
    public class RedisConnection : IRedisConnection
    {
        public IDatabase Database { get; }

        public RedisConnection(IConfiguration configuration)
        {
            var connectionStrings = configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(connectionStrings))
            {
                throw new InvalidOperationException("Redis connection string not found in configuration.");
            }

            Database = ConnectionMultiplexer.Connect(connectionStrings!).GetDatabase();
        }
    }
}
