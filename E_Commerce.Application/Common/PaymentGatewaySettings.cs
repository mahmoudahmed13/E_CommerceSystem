namespace E_Commerce.Application.Common
{
    public class PaymentGatewaySettings
    {
        public string SecretKey { get; set; } = default!;
        public string DefaultCuurency { get; set; } = default!;
    }
}
