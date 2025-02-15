using System.Collections.Generic;

namespace AspNetCore_MVC_Project.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
