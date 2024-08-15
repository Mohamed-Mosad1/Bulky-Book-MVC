// Ignore Spelling: env Admin

using BulkyBook.DAL.InterFaces;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

            return View(categories);
        }

        public IActionResult Create()
        {
            ViewData["ActionName"] = "Create";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");

            if (ModelState.IsValid)
            {
                _unitOfWork.Repository<Category>().Add(category);
                await _unitOfWork.CompleteAsync();

                TempData["success"] = "Category created successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id.Value);

            if (category is null)
                return NotFound();

            ViewData["ActionName"] = "Update";

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();

            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");

            if (!ModelState.IsValid)
                return View(category);

            try
            {
                _unitOfWork.Repository<Category>().Update(category);
                await _unitOfWork.CompleteAsync();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (_env.IsDevelopment())
                    ModelState.AddModelError("", ex.Message);
                else
                    ModelState.AddModelError("", "Something went wrong while updating category");

                return View(category);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id.Value);

            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            if (!id.HasValue || id == 0)
                return NotFound();

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id.Value);

            if (category is null)
                return NotFound();

            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.CompleteAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
