using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;

namespace E_Commerce.Application.Services
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketService(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdateAsync(BasketDto basket, TimeSpan? TLV, CancellationToken ct = default)
        {
            var customerBasket = _mapper.Map<CustomerBasket>(basket);
            var basketResult = await _basketRepository.CreateOrUpdateBasketAsync(customerBasket, TLV, ct);

            return basketResult == null ? Result<BasketDto>.Fail(Error.Failure("Basket.Failure", "Can Not Create Or Update Basket")) 
                : Result<BasketDto>.Ok(basket);
        }

        public async Task<Result<bool>> DeleteBasketAsync(string BasketId, CancellationToken ct = default)
        {
            var result = await _basketRepository.DeleteBasketAsync(BasketId, ct);
            return result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("BasketDelete.Failure", "Can Not Delete Basket"));
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string BasketId, CancellationToken ct = default)
        {
            var result = await _basketRepository.GetBasketAsync(BasketId, ct);
            return result == null ? Result<BasketDto>.Fail(Error.NotFound("Basket Not Found")) : _mapper.Map<BasketDto>(result);
        }
    }
}
