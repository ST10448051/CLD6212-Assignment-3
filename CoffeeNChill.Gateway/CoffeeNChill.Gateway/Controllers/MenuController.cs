using System.Text;
using System.Text.Json;
using CoffeeNChill.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace CoffeeNChill.Gateway.Controllers
{
    [ApiController]
    [Route("api/menu")]
    public class MenuController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public MenuController(FunctionsApiService functionsApi)
        {
            // We'll use the configured Functions service
            _functionsApi = functionsApi;
        }

        private readonly FunctionsApiService _functionsApi;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetMenu()
        {
            using var response =
                await _functionsApi.GetAsync("api/menu");

            var content =
                await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(
            string category)
        {
            using var response =
                await _functionsApi.GetAsync(
                    $"api/menu/category/{Uri.EscapeDataString(category)}");

            var content =
                await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = content,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateMenuItem(
            [FromBody] JsonElement menuItem)
        {
            using var content = new StringContent(
                menuItem.GetRawText(),
                Encoding.UTF8,
                "application/json");

            using var response =
                await _functionsApi.PostAsync("api/menu", content);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPut("{category}/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateMenuItem(
            string category,
            string id,
            [FromBody] JsonElement menuItem)
        {
            var route =
                $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}";

            using var content = new StringContent(
                menuItem.GetRawText(),
                Encoding.UTF8,
                "application/json");

            using var response =
                await _functionsApi.PutAsync(route, content);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpDelete("{category}/{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteMenuItem(
            string category,
            string id)
        {
            var route =
                $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}";

            using var response =
                await _functionsApi.DeleteAsync(route);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

    }
}
