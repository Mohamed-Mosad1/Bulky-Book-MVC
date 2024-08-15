// Ignore Spelling: App

using BulkyBook.Model.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBook.Model.ViewModels
{
    public class ManageUserRoleVM
    {
        public AppUser AppUser { get; set; } = null!;
        public IEnumerable<SelectListItem> Roles { get; set; } = null!;
        public IEnumerable<SelectListItem> Companies { get; set; } = null!;

    }
}
