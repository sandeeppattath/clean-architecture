using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleProject.ApplicationCore.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public ApplicationUser() : base()
        {

        }
        public DateTime CreatedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
