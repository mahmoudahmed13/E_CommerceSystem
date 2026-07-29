using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{

    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpPost("{basketId}")]

        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken ct)
            => ToActionResult(await _paymentService.CreateAndUpdataPaymentIntentAsync(basketId, ct));
    }
}
