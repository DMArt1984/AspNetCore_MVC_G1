using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore_MVC_Project.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public List<string> SelectedModules { get; set; } = new List<string>();
    }
}
