// Ignore Spelling: App

using BulkyBook.Model.Identity;

namespace BulkyBook.DAL.Specifications.UserSpecs
{
    public class AppUserWithCompanySpec : BaseSpecifications<AppUser>
    {
        public AppUserWithCompanySpec()
        {
            AddInclude(x => x.Company);
        }

        public AppUserWithCompanySpec(string userId) : base(x => x.Id == userId)
        {
            AddInclude(x => x.Company);
        }
    }
}
