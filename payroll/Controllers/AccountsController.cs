using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace payroll.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _signInUser;

        public AccountsController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> signInUser)
        {
            _signInManager = signInManager;
            _signInUser = signInUser;
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string email, string password, bool rememberme)
        {
            if (ModelState.IsValid) 
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberme, lockoutOnFailure: false);

                if (result.Succeeded) 
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = email, Email = email };
                var result = await _signInUser.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "Accounts");
        }
    }
}
