using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Baskets;

namespace E_Commerce.Application.Contracts
{
    public interface IPaymentService
    {
        Task<Result<BasketDto>> CreateAndUpdataPaymentIntentAsync(string basketId, CancellationToken ct = default);
    }
}
