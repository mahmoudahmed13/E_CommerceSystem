using E_Commerce.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(oi => oi.Price).HasColumnType("decimal(8, 2)");

            builder.OwnsOne(oi => oi.Product, product =>
            {
                product.Property(x => x.ProductName).HasMaxLength(100);
                product.Property(x => x.PictureUrl).HasMaxLength(200);
            });
        }
    }
}
