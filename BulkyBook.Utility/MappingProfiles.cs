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
            CreateMap<ProductVM, Product>().ReverseMap();
            CreateMap<ProductImageVM, ProductImage>().ReverseMap();
            CreateMap<ShoppingCart, ShoppingCartVM>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemVM>().ReverseMap();
        }
    }
}
