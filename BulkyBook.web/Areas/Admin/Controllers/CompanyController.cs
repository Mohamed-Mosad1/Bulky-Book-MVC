// Ignore Spelling: Admin env Upsert

using AutoMapper;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CompanyController(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IMapper mapper
            )
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var companies = _unitOfWork.Repository<Company>().GetAllAsync().Result;

            return View(companies);
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
                var company = await _unitOfWork.Repository<Company>().GetByIdAsync(id.Value);

                if (company is null)
                    return NotFound();

                ViewData["ActionName"] = "Update";

                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert([FromRoute] int id, Company company)
        {
            if (id != company.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(company);

            try
            {
                if (id == 0)
                {
                    _unitOfWork.Repository<Company>().Add(company);
                    TempData["success"] = "Product created successfully";
                    await _unitOfWork.CompleteAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _unitOfWork.Repository<Company>().Update(company);
                    TempData["success"] = "Product updated successfully";
                    await _unitOfWork.CompleteAsync();
                    
                    return RedirectToAction(nameof(Upsert), new { id = company.Id });
                }

            }
            catch (Exception ex)
            {
                HandleException(ex);

                return View(company);
            }
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
        public async Task<ActionResult<IReadOnlyList<Company>>> GetAll()
        {
            var companies = await _unitOfWork.Repository<Company>().GetAllAsync();

            return Json(new { data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var companyToDelete = await _unitOfWork.Repository<Company>().GetByIdAsync(id);

            if (companyToDelete is null)
                return Json(new { success = false, message = "Error while deleting Company" });

            _unitOfWork.Repository<Company>().Delete(companyToDelete);
            await _unitOfWork.CompleteAsync();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
