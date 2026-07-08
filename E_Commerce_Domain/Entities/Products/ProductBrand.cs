using E_Commerce_Domain.Common;

namespace E_Commerce_Domain.Entities.Products
{
    public class ProductBrand : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
    }
}
