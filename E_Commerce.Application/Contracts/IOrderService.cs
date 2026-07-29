using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Orders;

namespace E_Commerce.Application.Contracts
{
    public interface IOrderService
    {
        Task<Result<OrderToRetrunDto>> CreateOrderAsync(OrderDto orderDto , string email, CancellationToken ct = default);
        Task<Result<IReadOnlyList<OrderToRetrunDto>>> GetAllOrdersForUserAsyn(string email, CancellationToken ct = default);
        Task<Result<OrderToRetrunDto>> GetOrderByIdAndEmailForUser(Guid id, string email, CancellationToken ct = default);
        Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethodAsync(CancellationToken ct = default);
    
    }
}
