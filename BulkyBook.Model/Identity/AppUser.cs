// Ignore Spelling: App

using Microsoft.AspNetCore.Identity;

namespace BulkyBook.Model.Identity
{
    public class AppUser : IdentityUser
    {
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string Country { get; set; } = null!;

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}
