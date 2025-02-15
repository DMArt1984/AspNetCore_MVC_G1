using Microsoft.AspNetCore.Identity;

namespace AspNetCore_MVC_Project.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
