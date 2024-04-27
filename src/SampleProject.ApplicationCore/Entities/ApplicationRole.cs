using Microsoft.AspNetCore.Identity;

namespace SampleProject.ApplicationCore.Entities
{
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
