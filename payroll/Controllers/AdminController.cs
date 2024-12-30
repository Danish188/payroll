using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace payroll.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult ManageRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            return RedirectToAction("ManageRoles");
        }

        [HttpGet]
        public async Task<IActionResult> AssignRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.UserId = userId;
            ViewBag.UserName = user.UserName;
            ViewBag.Roles = roles;
            ViewBag.UserRoles = userRoles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return RedirectToAction("AssignRole", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
            return RedirectToAction("AssignRole", new { userId });
        }
    }
}
