using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {
        // Ef Core
        private Order() { }
        public Order(string buyerEmail, OrderAddress shipToAddress, ICollection<OrderItem> items, decimal subTotal, DeliveryMethod deliveryMethod)
        {
            BuyerEmail = buyerEmail;
            ShipToAddress = shipToAddress;
            Items = items;
            SubTotal = subTotal;
            DeliveryMethod = deliveryMethod;
        }

        public string BuyerEmail { get; set; } = default!;
        public OrderAddress ShipToAddress { get; set; } = default!;
        public ICollection<OrderItem> Items { get; set; } = [];
        public decimal SubTotal { get; set; } // Delivery Cost + Price Items
        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        public int DeliveryMethodId { get; set; } // FK
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public decimal GetTotal() => SubTotal + (DeliveryMethod?.Cost ?? 0);
    }
}
