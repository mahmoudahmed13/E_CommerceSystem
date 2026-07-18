namespace E_Commerce.Domain.Entities.Baskets
{
    public class CustomerBasket
    {
        public string Id { get; set; } // Created from Frontend by Guid
        public ICollection<BasketItem> Items { get; set; } = [];
    }
}