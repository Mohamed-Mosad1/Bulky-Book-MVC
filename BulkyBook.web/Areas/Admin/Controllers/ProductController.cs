// Ignore Spelling: Admin env Upsert

using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.Model;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductController(
            IProductService productService,
            IWebHostEnvironment env,
            IMapper mapper
            )
        {
            _productService = productService;
            _env = env;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
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
                var productWithCategoryAndImages = await _productService.GetProductByIdAsync(id.Value);

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
            if (id != productVM.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(productVM);

            try
            {
                var mappedProduct = _mapper.Map<Product>(productVM);

                var success = await _productService.SaveProductAsync(mappedProduct, files);
                if (!success)
                {
                    TempData["error"] = "Something went wrong while creating or updating the product";
                    return View(productVM);
                }

                if (id == 0)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                    TempData["success"] = "Product updated successfully";

                return RedirectToAction(nameof(Upsert), new { id = mappedProduct.Id });
            }
            catch (Exception ex)
            {
                HandleException(ex);

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if ( !id.HasValue || id == 0)
                return NotFound();

            var image = await _productService.GetImageByIdAsync(id.Value);

            if (image is null)
                return NotFound();

            var success = await _productService.DeleteImageByIdAsync(id.Value, image);
            if (!success)
                TempData["error"] = "Something went wrong while deleting the image";

            TempData["success"] = "Image deleted successfully";

            return RedirectToAction(nameof(Upsert), new { id = image?.ProductId });
        }

        private void HandleException(Exception ex)
        {
            if (_env.IsDevelopment())
                ModelState.AddModelError("", ex.Message);
            else
                ModelState.AddModelError("", "Something went wrong while processing your request");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            var mappedProducts = _mapper.Map<IEnumerable<ProductVM>>(products);

            return Json(new { data = mappedProducts });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductByIdAsync(id);

            if (!success)
                return Json(new { success = false, message = "Error while deleting" });

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
