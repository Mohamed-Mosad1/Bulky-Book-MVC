// Ignore Spelling: Admin env Upsert

using AutoMapper;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.UserSpecs;
using BulkyBook.Model;
using BulkyBook.Model.Identity;
using BulkyBook.Model.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> RoleManagement(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required.");

            var spec = new AppUserWithCompanySpec(userId);
            var user = await _unitOfWork.Repository<AppUser>().GetWithSpecAsync(spec);

            if (user is null)
                return NotFound("User not found.");

            var companies = await _unitOfWork.Repository<Company>().GetAllAsync();

            ManageUserRoleVM roleVM = new ManageUserRoleVM()
            {
                AppUser = user,
                Roles = _roleManager.Roles.Select(r => new SelectListItem()
                {
                    Text = r.Name,
                    Value = r.Name
                }),
                Companies = companies.Select(c => new SelectListItem()
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            roleVM.AppUser.RoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return View(roleVM);
        }


        [HttpPost]
        public async Task<IActionResult> RoleManagement(ManageUserRoleVM manageUserRoleVM)
        {
            var user = await _userManager.FindByIdAsync(manageUserRoleVM.AppUser.Id);

            if (user is null)
                return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            string newRole = manageUserRoleVM.AppUser.RoleName!;

            if (!(currentRoles.FirstOrDefault() != newRole))
            {
                if (newRole == SD.Role_Company)
                {
                    user.CompanyId = manageUserRoleVM.AppUser.CompanyId;
                }

                if (currentRoles.FirstOrDefault() == SD.Role_Company)
                {
                    user.CompanyId = null;
                }

                await _userManager.UpdateAsync(user);

                var rolesToRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                var result = await _userManager.AddToRoleAsync(user, newRole);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to add user to role.");
                    TempData["error"] = "Failed to update user role.";
                    return View(manageUserRoleVM);
                }
            }
            else if (currentRoles.FirstOrDefault() == SD.Role_Company && user.CompanyId != manageUserRoleVM.AppUser.CompanyId)
            {
                user.CompanyId = manageUserRoleVM.AppUser.CompanyId;
                await _userManager.UpdateAsync(user);
            }

            TempData["success"] = "Role updated successfully.";

            return RedirectToAction(nameof(Index));
        }



        #region API CALLS

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetAll()
        {
            var spec = new AppUserWithCompanySpec();
            var users = await _unitOfWork.Repository<AppUser>().GetAllWithSpecAsync(spec);

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleName = roles.FirstOrDefault();

                if (user.Company is null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string userId)
        {
            var UserToDelete = await _unitOfWork.Repository<AppUser>().GetByIdAsync(userId);

            if (UserToDelete is null)
                return Json(new { success = false, message = "Error while Updating the User" });

            if (UserToDelete.LockoutEnd is not null && UserToDelete.LockoutEnd > DateTime.Now)
            {
                UserToDelete.LockoutEnd = DateTimeOffset.UtcNow;
            }
            else
            {
                UserToDelete.LockoutEnd = DateTimeOffset.UtcNow.AddYears(1000);
            }

            _unitOfWork.Repository<AppUser>().Update(UserToDelete);
            await _unitOfWork.CompleteAsync();

            return Json(new { success = true, message = "User Updated Successfully" });
        }

        #endregion

    }
}
