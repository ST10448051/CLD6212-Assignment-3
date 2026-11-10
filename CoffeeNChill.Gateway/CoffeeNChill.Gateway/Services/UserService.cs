
using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Gateway.Models;

namespace CoffeeNChill.Gateway.Services
{
    public class UserService
    {
        private readonly TableClient _tableClient;

        public UserService(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "AzureStorage connection string is missing.");

            _tableClient = new TableClient(
                connectionString,
                "Users");

            _tableClient.CreateIfNotExists();
        }

        public async Task<UserEntity?> GetUserByEmailAsync(
            string email)
        {
            email = email.Trim().ToLowerInvariant();

            await foreach (var user in
                _tableClient.QueryAsync<UserEntity>(
                    u => u.RowKey == email))
            {
                return user;
            }

            return null;
        }

        public async Task CreateUserAsync(UserEntity user)
        {
            await _tableClient.AddEntityAsync(user);
        }
    }
}
