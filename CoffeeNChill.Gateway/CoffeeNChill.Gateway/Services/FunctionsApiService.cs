
namespace CoffeeNChill.Gateway.Services
{
    public class FunctionsApiService
    {
        private readonly HttpClient _httpClient;

        public FunctionsApiService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            var baseUrl = configuration[
                "AzureFunctions:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "Azure Functions BaseUrl is missing.");

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(
                baseUrl.TrimEnd('/') + "/");
        }

        public async Task<HttpResponseMessage> GetAsync(
            string route)
        {
            return await _httpClient.GetAsync(route);
        }

        public async Task<HttpResponseMessage> PostAsync(
            string route,
            HttpContent content)
        {
            return await _httpClient.PostAsync(route, content);
        }

        public async Task<HttpResponseMessage> PutAsync(
            string route,
            HttpContent content)
        {
            return await _httpClient.PutAsync(route, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(
            string route)
        {
            return await _httpClient.DeleteAsync(route);
        }

    }
}
