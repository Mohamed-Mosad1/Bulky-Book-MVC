// Ignore Spelling: BLL env

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.ProductSpecs;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public ProductService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env
            )
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            var spec = new ProductWithCategoryAndImagesSpecification();

            return await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var spec = new ProductWithCategoryAndImagesSpecification(id);

            return await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
        }

        public async Task<ProductImage?> GetImageByIdAsync(int id)
        {
            return await _unitOfWork.Repository<ProductImage>().GetByIdAsync(id);
        }

        public async Task<bool> DeleteImageByIdAsync(int id, ProductImage productImage)
        {
            PhotoManager.DeleteFile(productImage.ImageUrl, _env);
            _unitOfWork.Repository<ProductImage>().Delete(productImage);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteProductByIdAsync(int id)
        {
            var spec = new ProductWithCategoryAndImagesSpecification(id);
            var productToBeDeleted = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
            if (productToBeDeleted is null)
                return false;

            var productPath = Path.Combine("images", "products", "product-" + productToBeDeleted.Id);
            string finalPath = Path.Combine(_env.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            _unitOfWork.Repository<Product>().Delete(productToBeDeleted);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> SaveProductAsync(Product product, List<IFormFile> files)
        {
            if (product.Id == 0)
                _unitOfWork.Repository<Product>().Add(product);
            else
                _unitOfWork.Repository<Product>().Update(product);

            var count = await _unitOfWork.CompleteAsync();

            if (count <= 0)
                return false;

            if (files != null && files.Any())
            {
                var uploadResult = await HandleFileUploadAsync(files, product.Id);
                if (!uploadResult)
                    return false;

                count = await _unitOfWork.CompleteAsync();
                if (count <= 0)
                    return false;
            }

            return true;
        }

        private async Task<bool> HandleFileUploadAsync(List<IFormFile> files, int productId)
        {
            foreach (var file in files)
            {
                if (!PhotoManager.IsValidFile(file))
                    return false;

                string productPath = Path.Combine("images", "products", "product-" + productId);
                string newFilePath = await PhotoManager.UploadFileAsync(file, productPath, _env);

                var productImage = new ProductImage
                {
                    ImageUrl = newFilePath,
                    ProductId = productId
                };

                _unitOfWork.Repository<ProductImage>().Add(productImage);
            }

            return true;
        }
    }
}
