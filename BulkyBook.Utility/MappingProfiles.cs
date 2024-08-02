using AutoMapper;
using BulkyBook.Model;
using BulkyBook.Model.Cart;
using BulkyBook.Model.ViewModels;

namespace BulkyBook.Utility
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductVM>().ReverseMap();
            CreateMap<ProductImage, ProductImageVM>().ReverseMap();

            CreateMap<ShoppingCart, ShoppingCartVM>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemVM>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ProductImages.FirstOrDefault().ImageUrl))
                .ReverseMap();
        }
    }
}
