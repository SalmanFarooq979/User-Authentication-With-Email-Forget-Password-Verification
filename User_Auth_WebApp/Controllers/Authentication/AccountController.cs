using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User_Auth_WebApp.Repository.Interface;
using User_Auth_WebApp.ViewModel;

namespace User_Auth_WebApp.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkEmail = await userManager.FindByEmailAsync(registerVM.Email);
                    if (checkEmail != null) 
                    {
                        ModelState.AddModelError(string.Empty, "Email is already Exists");
                        return View(registerVM);
                    }
                    IdentityUser user = new IdentityUser
                    {
                        
                        UserName = registerVM.Email,
                        Email = registerVM.Email,
                    };
                    var result = await userManager.CreateAsync(user,registerVM.Password);
                    if (result.Succeeded)
                    {
                        bool status = await emailSender.EmailSendAsync(registerVM.Email, "Account Created", "Congratulation your account has been creared successfully!");
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
            return View(registerVM);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkEmail = await userManager.FindByEmailAsync(loginVM.Email);
                    if (checkEmail == null)
                    {
                        ModelState.AddModelError("", "Email is not Exists");
                        return View(loginVM);
                    }
                    if (await userManager.CheckPasswordAsync(checkEmail, loginVM.Password)==false) 
                    {
                        ModelState.AddModelError("", "Invalid Cridentials");
                        return View(loginVM);
                    }
                    var result = await signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password,loginVM.RememberMe,lockoutOnFailure:false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Invalid Login Attempt!");
                }

            }
            catch (Exception ex) 
            { 
                throw;
            }
            return View(loginVM);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordOrUsernameVM forget)
        {
            ModelState.Remove("UserID");
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Token");
            if (!ModelState.IsValid)
            {
                return View(forget);
            }
            var user = await userManager.FindByEmailAsync(forget.Email);
            if(user != null)
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackURL = Url.Action("ResetPassword", "Account", new {userId  = user.Id,Token = code},protocol:Request.Scheme);
                bool isSendEmail = await emailSender.EmailSendAsync(forget.Email, "Reset Password", "Please reset your password by clicking here <a href =\"" + callbackURL +"\">Click Here</a>");
                if (isSendEmail)
                {
                    Response response = new Response();
                    response.Message = "Reset Password Request";
                    return RedirectToAction("ForgetPasswordConfirmation","Account",response);
                }

            }
            return View();
        }
        public IActionResult ForgetPasswordConfirmation(Response response)
        {
            return View(response);
        }
        public IActionResult ResetPassword(string userId,string token)
        {
            var model = new ForgetPasswordOrUsernameVM
            {
                UserID = userId,
                Token = token
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ForgetPasswordOrUsernameVM forget)
        {
            Response response = new Response();
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
            {
                return View(forget);
            }
            var user = await userManager.FindByIdAsync(forget.UserID);
            if (user == null) 
            {
               return View(forget);
            }
            var result = await userManager.ResetPasswordAsync(user,forget.Token,forget.Password);
            if (result.Succeeded) 
            {
                response.Message = "Your Password has benn Successfully Reset";
                return RedirectToAction("ForgetPasswordConfirmation", response);
            }
            return View(forget);
        }
    }
}
