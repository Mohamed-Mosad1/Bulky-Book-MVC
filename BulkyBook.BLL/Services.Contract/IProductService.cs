// Ignore Spelling: BLL

using BulkyBook.Model;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<bool> SaveProductAsync(Product productVM, List<IFormFile> files);
        Task<ProductImage?> GetImageByIdAsync(int id);
        Task<bool> DeleteImageByIdAsync(int id, ProductImage productImage);
        Task<bool> DeleteProductByIdAsync(int id);
    }
}
