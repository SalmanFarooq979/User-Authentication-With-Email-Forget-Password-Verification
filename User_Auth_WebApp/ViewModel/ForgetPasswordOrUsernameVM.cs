using System.ComponentModel.DataAnnotations;

namespace User_Auth_WebApp.ViewModel
{
    public class ForgetPasswordOrUsernameVM
    {
        [Required]
        public string Email { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public string Token { get; set; } = default!;
        [Required(ErrorMessage ="Please Enter New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
        [Required(ErrorMessage = "Please Enter Confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Confirm Password is not matched with entered Password")]
        public string ConfirmPassword { get; set; } = default!;

    }
}
