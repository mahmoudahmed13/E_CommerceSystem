using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IMapper _mapper;
        private readonly PaymentGatewaySettings _paymentGatewaySettings;

        public PaymentService(IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentGateway paymentGateway,
            IOptions<PaymentGatewaySettings> options,
            IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
            _paymentGatewaySettings = options.Value;
        }
        public async Task<Result<BasketDto>> CreateAndUpdataPaymentIntentAsync(string basketId, CancellationToken ct = default)
        {

            #region 1. Retrieves the basket by its ID and validates its existence.

            var basket = await _basketRepository.GetBasketAsync(basketId, ct);
            if (basket == null)
                return Error.NotFound("Basket Is Not Found", $"Basket With Id {basketId} is Not Found");
            if (basket.Items.Count == 0)
                return Error.Validation("Basket Is Empty");
            #endregion

            #region 2. Recalculates the delivery cost using the basket's DeliveryMethodId.
            if (!basket.DeliveryMethodId.HasValue)
                Error.Validation("DeliveryMethod Id Is Required");

            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value, ct);
            if (deliveryMethod == null) return Error.NotFound("Delivery Method Is Not Found");

            basket.ShippingPrice = deliveryMethod.Cost;
            #endregion

            #region 3. Recalculates each item's price from the product catalog, to avoid trusting client-side prices.

            var productsIds = basket.Items.Select(p => p.Id).ToHashSet();
            var products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecifications(productsIds), ct)).ToDictionary(x => x.Id);
            foreach (var item in basket.Items)
            {
                if (!products.TryGetValue(item.Id, out var product))
                    return Error.NotFound("Product Not Found");

                item.Price = product.Price;
            } 
            #endregion
            
            //4. Computes the total amount in the smallest currency unit(e.g.cents).
            
            var subTotal = basket.Items.Sum(i=> i.Price * i.Quantity);
            var amount =(long) ((subTotal + deliveryMethod.Cost) * 100m);
            
            //5. If PaymentIntentId is empty, creates a new PaymentIntent on Stripe; otherwise updates the existing one with the new amount.
            
            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                //Create
                var result = await _paymentGateway.CreatePaymentIntentAsync(amount, _paymentGatewaySettings.DefaultCuurency, ct);
                basket.PaymentIntentId = result.PaymentIntentId;
                basket.ClientSecret = result.ClientSecret;
            }
            else
            {
                await _paymentGateway.UpdatePaymentIntentAsync(amount, basket.PaymentIntentId, ct);
            }

            //6. Saves the PaymentIntentId and ClientSecret back onto the basket in Redis.
            await _basketRepository.CreateOrUpdateBasketAsync(basket,ct:ct);

            //7. Returns the updated basket to the client.
            return _mapper.Map<BasketDto>(basket);
        }
    }
}
