using AutoMapper;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Domain.Entities.Products;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Profiles
{
    internal class PictureUrlResolver : IValueResolver<Product, ProductDto, string> // string type of member
    {
        private readonly UrlSettings _urlSettings;
        public PictureUrlResolver(IOptions<UrlSettings> options)
        {
            _urlSettings = options.Value;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            //source => "images/products/shoe-1.png"
            //Retrub => "https://localhost:7180/Files/images/products/shoe-1.png"
            //Set https://localhost:7180 => Insite appsettings.josn
            //Interface IOptions => Take Type and read configration(appsettings.josn) then map value In this type
            var baseUrl = _urlSettings.BaseUrl.TrimEnd('/');
            var path = source.PictureUrl.TrimStart('/');
            return $"{baseUrl}/Files/{path}";
        }
    }
    public class UrlSettings
    {
        public string BaseUrl { get; set; }
    }
}
