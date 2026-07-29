using E_Commerce.Domain.Entities.Products;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithIdSpecifications : BaseSpecification<Product, int>
    {
        public ProductWithIdSpecifications(HashSet<int> productIds) : base(p => productIds.Contains(p.Id))
        {
            
        }
    }
}
