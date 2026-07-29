using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;

namespace E_Commerce.Application.Services
{
    internal class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<OrderToRetrunDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId, ct);
            if (basket == null)
                return Error.NotFound("Basket Is Not Found", $"Basket With ID {orderDto.BasketId} Is Not Found");
            if(basket.Items.Count == 0)
                return Error.Validation("Basket Is Empty", $"Can Not Create Order With Basket id {orderDto.BasketId}");

            // Add Items To Order Item List
            var orderItems = new List<OrderItem>(basket.Items.Count);
            // 1. Get All Product that Id Contain(=) Basket Item Id
            var productIds = basket.Items.Select(x => x.Id).ToHashSet();
            var products = (await _unitOfWork.GetRepository<Product, int>()
                .GetAllAsync(new ProductWithIdSpecifications(productIds), ct)).ToDictionary(x => x.Id);

            // 2. Loop to Get product item id that in products 
            foreach (var item in basket.Items)
            {
                // Get product item id that in products (If Exists store it in product)
                if (!products.TryGetValue(item.Id, out var product))
                    return Error.NotFound("Product Not Found", $"Product With Id {item.Id} Is Not Found");

                orderItems.Add(new OrderItem()
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrdered()
                    {
                        PictureUrl = item.PictureUrl,
                        ProductId = item.Id,
                        ProductName = item.ProductName,
                    }
                });
            }

            //ShipToAddress
            var orderAddress = _mapper.Map<OrderAddress>(orderDto.ShipToAddress);

            //Delivery Method
            var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>()
                .GetByIdAsync(orderDto.DeliveryMethodId);
            if(deliveryMethod == null)
                return Error.NotFound("Delivery Method Not Found",$"Delivery Method With ID {orderDto.DeliveryMethodId} Is Not Found");

            //SubTotal
            var subTotal = orderItems.Sum(x => x.Quantity *  x.Price);

            //End Create Order
            var order = new Order(email, orderAddress, orderItems, subTotal, deliveryMethod);

            _unitOfWork.GetRepository<Order, Guid>().Add(order); //Local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result == 0)
                return Error.Failure("Order Save Failed", "Can Not Create Order");
            else
            {
                await _basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
                return _mapper.Map<OrderToRetrunDto>(order);
            }
        }

        public async Task<Result<IReadOnlyList<OrderToRetrunDto>>> GetAllOrdersForUserAsyn(string email, CancellationToken ct = default)
        {
            var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications(email), ct);

            if (orders.Any())
                return Result<IReadOnlyList<OrderToRetrunDto>>.Ok(_mapper.Map<IReadOnlyList<OrderToRetrunDto>>(orders));
            else
                return Error.NotFound("Orders Not Found", $"No Orders Found For User With Email {email}");
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethodAsync(CancellationToken ct = default)
        {
            var deliveryMethods = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            if (deliveryMethods.Any())
                return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(_mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
            else
                return Error.NotFound("No Delivery Methods Found");

        }

        public async Task<Result<OrderToRetrunDto>> GetOrderByIdAndEmailForUser(Guid id, string email, CancellationToken ct = default)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(id, email));
            if (order == null)
                return Error.NotFound("Order Not Found", $"No Order Found For User With Email {email} and id {id}");
            else
                return Result<OrderToRetrunDto>.Ok(_mapper.Map<OrderToRetrunDto>(order));
        }
    }
}
