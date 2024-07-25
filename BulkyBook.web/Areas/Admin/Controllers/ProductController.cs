// Ignore Spelling: Admin env Upsert

using AutoMapper;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.ProductSpecs;
using BulkyBook.Model;
using BulkyBook.Model.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };


        public ProductController(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var spec = new ProductWithCategoryAndImagesSpecification();
            var productsWithCategory = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var mappedProducts = _mapper.Map<IEnumerable<ProductVM>>(productsWithCategory);

            return View(mappedProducts);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                ViewData["ActionName"] = "Create";
                return View();
            }
            else
            {
                var spec = new ProductWithCategoryAndImagesSpecification(id.Value);
                var productWithCategoryAndImages = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

                if (productWithCategoryAndImages is null)
                    return NotFound();

                var mappedProduct = _mapper.Map<ProductVM>(productWithCategoryAndImages);
                ViewData["ActionName"] = "Update";

                return View(mappedProduct);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromRoute] int id, ProductVM productVM, List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return View(productVM);

            var mappedProduct = _mapper.Map<Product>(productVM);

            if (productVM.Id == 0)
               _unitOfWork.Repository<Product>().Add(mappedProduct);
            else 
                _unitOfWork.Repository<Product>().Update(mappedProduct);
               
            var count = await _unitOfWork.CompleteAsync();

            if (count <= 0)
            {
                TempData["error"] = "Something went wrong while creating the product";
                return View(productVM);
            }

            try
            {
                if (files is not null && files.Any())
                {
                    foreach (IFormFile file in files)
                    {
                        if (!PhotoManager.IsValidFile(file))
                        {
                            ModelState.AddModelError("", "Invalid file type or size.");
                            return View(productVM);
                        }
                        string productPath = Path.Combine("images", "products", "product-" + mappedProduct.Id);
                     
                        string newFilePath = await PhotoManager.UploadFileAsync(file, productPath, _env);

                        var productImage = new ProductImage()
                        {
                            ImageUrl = newFilePath,
                            ProductId = mappedProduct.Id,
                        };

                        _unitOfWork.Repository<ProductImage>().Update(productImage);
                    }

                    count = await _unitOfWork.CompleteAsync();
                }

                TempData["success"] = count > 0 ? "Product Created or Updated Successfully" : "Something went wrong while creating the product";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                HandleException(ex);

                return View(productVM);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var spec = new ProductWithCategoryAndImagesSpecification(id.Value);
            var productWithCategory = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (productWithCategory is null)
                return NotFound();

            var mappedProduct = _mapper.Map<ProductVM>(productWithCategory);


            return View(mappedProduct);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (!id.HasValue || id == 0)
                return BadRequest();

            var spec = new ProductWithCategoryAndImagesSpecification(id.Value);
            var productWithCategory = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (productWithCategory is null)
                return NotFound();

            try
            {
                _unitOfWork.Repository<Product>().Delete(productWithCategory);
                var count = await _unitOfWork.CompleteAsync();

                TempData["success"] = count > 0 ? "Product Deleted Successfully" : "Something went wrong while deleting the product";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                HandleException(ex);

                return RedirectToAction(nameof(Index));
            }
        }

        private void HandleException(Exception ex)
        {
            if (_env.IsDevelopment())
                ModelState.AddModelError("", ex.Message);
            else
                ModelState.AddModelError("", "Something went wrong while processing your request");
        }
    }
}
