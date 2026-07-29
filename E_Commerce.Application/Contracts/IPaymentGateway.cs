using E_Commerce.Application.Common;

namespace E_Commerce.Application.Contracts
{
    public interface IPaymentGateway
    {
        // amount + Currency => PaymentIntentId + ClientSecret
        Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, CancellationToken ct = default);
        Task<PaymentIntentResult> UpdatePaymentIntentAsync(decimal amount, string paymentIntentId, CancellationToken ct = default);
    }
}
