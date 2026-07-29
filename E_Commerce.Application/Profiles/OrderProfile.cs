using AutoMapper;
using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Domain.Entities.Orders;

namespace E_Commerce.Application.Profiles
{
    internal class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<AddressDto, OrderAddress>().ReverseMap();
            CreateMap<Order, OrderToRetrunDto>()
                .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.DeliveryMethodCost, opt => opt.MapFrom(src => src.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId , o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName , o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl , o => o.MapFrom<OrderItemPictureUrlResolver>());
            CreateMap<DeliveryMethod, DeliveryMethodDto>();
        }
    }
}
