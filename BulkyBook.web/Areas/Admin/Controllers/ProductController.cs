// Ignore Spelling: Admin env

using AutoMapper;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.ProductSpecs;
using BulkyBook.Model;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

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
            var spec = new ProductsWithCategorySpecification();
            var productsWithCategory = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var mappedProducts = _mapper.Map<IEnumerable<ProductVM>>(productsWithCategory);

            return View(mappedProducts);
        }

        public IActionResult Create()
        {
            ViewData["ActionName"] = "Create";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var mappedProduct = _mapper.Map<Product>(productVM);

                _unitOfWork.Repository<Product>().Add(mappedProduct);
                var count = await _unitOfWork.CompleteAsync();

                TempData["success"] = count > 0 ? "Product Created Successfully" : "Something went wrong while creating the product";

            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var spec = new ProductsWithCategorySpecification(id.Value);
            var productWithCategory = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (productWithCategory is null)
                return NotFound();

            var mappedProduct = _mapper.Map<ProductVM>(productWithCategory);

            ViewData["ActionName"] = "Update";

            return View(mappedProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, ProductVM productVM)
        {
            if (id != productVM.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(productVM);

            try
            {
                var mappedProduct = _mapper.Map<Product>(productVM);

                _unitOfWork.Repository<Product>().Update(mappedProduct);
                var count = await _unitOfWork.CompleteAsync();

                TempData["success"] = count > 0 ? "Product Updated Successfully" : "Something went wrong while updating the product";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                HandleException(ex);

                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var spec = new ProductsWithCategorySpecification(id.Value);
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

            var spec = new ProductsWithCategorySpecification(id.Value);
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
