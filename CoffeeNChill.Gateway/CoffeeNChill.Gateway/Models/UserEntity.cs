
using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Gateway.Models
{
    public class UserEntity : ITableEntity
    {
        // Azure Table Storage keys
        public string PartitionKey { get; set; } = string.Empty;
        public string RowKey { get; set; } = string.Empty;

        // User information
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Required by Azure Table Storage
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
