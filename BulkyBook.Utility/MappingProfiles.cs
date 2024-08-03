using AutoMapper;
using BulkyBook.Model;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels;

namespace BulkyBook.Utility
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductVM>().ReverseMap();
            CreateMap<ProductImage, ProductImageVM>().ReverseMap();

            CreateMap<ShoppingCart, ShoppingCartVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.AppUser.UserName))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.AppUser.Street))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.AppUser.State))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.AppUser.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.AppUser.Country))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AppUser.PhoneNumber))
                .ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemVM>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault().ImageUrl))
                .ReverseMap();

            CreateMap<OrderAddress, OrderAddressVM>().ReverseMap();

        }
    }
}
