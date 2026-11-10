using CoffeeNChill.Gateway.DTOs;
using CoffeeNChill.Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CoffeeNChill.Gateway.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly FunctionsApiService _functionsApi;

        public OrdersController(FunctionsApiService functionsApi)
        {
            _functionsApi = functionsApi;
        }

        [HttpPost("queue")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> QueueOrder(
            [FromBody] QueueOrderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderId) ||
                string.IsNullOrWhiteSpace(request.CustomerName) ||
                request.SelectedItemSKUs == null ||
                request.SelectedItemSKUs.Count == 0 ||
                request.SelectedItemSKUs.Any(string.IsNullOrWhiteSpace) ||
                request.TotalPrice <= 0 ||
                request.OrderTimestamp == default ||
                request.OrderId.Contains('/') ||
                request.OrderId.Contains('\\'))
            {
                return BadRequest(new
                {
                    message = "Invalid order details."
                });
            }

            using var content = JsonContent.Create(request);

            using var response = await _functionsApi.PostAsync(
                "api/orders/queue", content);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpGet("{date}/{orderId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(
            string date,
            string orderId)
        {
            var route =
                $"api/orders/{Uri.EscapeDataString(date)}/" +
                $"{Uri.EscapeDataString(orderId)}";

            using var response = await _functionsApi.GetAsync(route);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)response.StatusCode
            };
        }

        [HttpPut("status")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromBody] UpdateOrderStatusRequest request)
        {
            using var content = JsonContent.Create(request);

            using var response = await _functionsApi.PutAsync(
                "api/orders/status", content);

            var result = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                Content = result,
                ContentType = response.Content.Headers.ContentType?
                    .ToString() ?? "application/json",
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
