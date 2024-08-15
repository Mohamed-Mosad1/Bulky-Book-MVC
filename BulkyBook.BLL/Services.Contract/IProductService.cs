// Ignore Spelling: BLL

using BulkyBook.Model;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetAllProductsAsync();
        Task<bool> SaveProductAsync(Product productVM, List<IFormFile> files);
        Task<ProductImage?> GetImageByIdAsync(int id);
        Task<bool> DeleteImageByIdAsync(int id, ProductImage productImage);
        Task<bool> DeleteProductByIdAsync(int id);
    }
}
