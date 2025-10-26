using Backend.WorkItem.Repository.Utility.Interface;
using Microsoft.Extensions.Configuration;

namespace Backend.WorkItem.Repository.Utility
{
    public class ConnectionString : IConnectionString
    {
        public string DBConnString { get; set; }

        public ConnectionString(IConfiguration config)
        {
            DBConnString= config.GetConnectionString("DefaultConnection");
        }
    }
}
