using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToRetrunDto>> CreateOrder(OrderDto orderDto, CancellationToken ct)
        {
            return ToActionResult( await _orderService.CreateOrderAsync(orderDto, GetEmailFromToken(), ct));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToRetrunDto>>> GetAllOrders(CancellationToken ct)
        {
            return ToActionResult(await _orderService.GetAllOrdersForUserAsyn(GetEmailFromToken(), ct));
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderToRetrunDto>> GetOrderById(Guid id, CancellationToken ct)
        {
            return ToActionResult(await _orderService.GetOrderByIdAndEmailForUser(id, GetEmailFromToken(), ct));
        }

        [AllowAnonymous]
        [HttpGet("DeliveryMethod")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethods(CancellationToken ct)
        {
            return ToActionResult(await _orderService.GetDeliveryMethodAsync(ct));
        }
    }
}
