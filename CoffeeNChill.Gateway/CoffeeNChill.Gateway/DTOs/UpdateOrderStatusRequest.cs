
using System.ComponentModel.DataAnnotations;

namespace CoffeeNChill.Gateway.DTOs
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public string Date { get; set; } = string.Empty;

        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            "^(Received|Preparing|Ready|Collected)$",
            ErrorMessage = "Invalid order status.")]
        public string Status { get; set; } = string.Empty;
    }
}
