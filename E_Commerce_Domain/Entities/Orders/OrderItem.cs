using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Entities.Orders
{
    public class OrderItem : BaseEntity<Guid>
    {
        public ProductItemOrdered Product {  get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
