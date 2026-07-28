using E_Commerce.Application.DTOs.Authentications;

namespace E_Commerce.Application.DTOs.Orders
{
    public class OrderToRetrunDto
    {
        public Guid Id { get; set; }
        public string BuyerEmail { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public AddressDto ShipToAddress { get; set; } = default!;
        public ICollection<OrderItemDto> Items { get; set; } = [];
        public string DeliveryMethod { get; set; } = default!;
        public string Status { get; set; } = default!;
        public decimal SubTotal { get; set; }
        public decimal DeliveryMethodCost { get; set; }
        public decimal Total { get; set; }
    }
}
