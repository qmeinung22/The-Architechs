using System.ComponentModel.DataAnnotations;
namespace project_wildfire_web
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email {get; set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe {get; set; }
    }
}