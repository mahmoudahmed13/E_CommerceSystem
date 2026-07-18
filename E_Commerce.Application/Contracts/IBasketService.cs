using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Baskets;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        //Get Basket => Take BasketId, Return Baket Dto

        Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken ct = default);

        //Create Or Update Basket => Take Basket , Retrun Basket After Creation Or Update
        Task<Result<BasketDto>> CreateOrUpdateAsync(BasketDto basket, TimeSpan? TLV = default, CancellationToken ct = default);
        
        //Delete Basket => Take BaketId , Retrun Bool
        Task<Result<bool>> DeleteBasketAsync(string BasketId, CancellationToken ct = default);
    }
}
