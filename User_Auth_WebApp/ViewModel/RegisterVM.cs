using System.ComponentModel.DataAnnotations;

namespace User_Auth_WebApp.ViewModel
{
    public class RegisterVM
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage ="Please Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="Confrim Passoword is not matched with Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
