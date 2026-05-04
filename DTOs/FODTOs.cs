using System.ComponentModel.DataAnnotations;

namespace SalesApi.DTOs
{
    public class CheckoutRequest
    {
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "Giỏ hàng phải có ít nhất 1 món")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        [Required]
        public int FoodId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }
    }
}
