using System.ComponentModel.DataAnnotations;

namespace CoffeeNChill.Gateway.DTOs
{
    public class QueueOrderRequest
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public List<string> SelectedItemSKUs { get; set; } = new();

        [Range(0.01, 100000)]
        public decimal TotalPrice { get; set; }

        public DateTimeOffset OrderTimestamp { get; set; }
            = DateTimeOffset.UtcNow;
    }
}
 