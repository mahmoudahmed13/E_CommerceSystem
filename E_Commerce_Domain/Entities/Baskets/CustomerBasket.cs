namespace E_Commerce.Domain.Entities.Baskets
{
    public class CustomerBasket
    {
        public string Id { get; set; } // Created from Frontend by Guid
        public ICollection<BasketItem> Items { get; set; } = [];

        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }

        public int? DeliveryMethodId { get; set; }
        public decimal? ShippingPrice { get; set; }
    }
}