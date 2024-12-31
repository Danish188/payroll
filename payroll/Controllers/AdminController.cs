using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

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

        [HttpPost]
        public async Task<IActionResult> DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is not null)
            {
                await _roleManager.DeleteAsync(role);
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

        [HttpGet]
        public async Task<IActionResult> ManagePermission(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

            return View(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> ManagePermission(string permissionName, string roleName)
        {
            if (string.IsNullOrEmpty(permissionName) || string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError("", "Permission name and role are required.");
                return View();
            }

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                ModelState.AddModelError("", "Role not found.");
                return View();
            }

            var claims = await _roleManager.GetClaimsAsync(role);
            var existingPermission = claims.Any(c => c.Type == "Permission" && c.Value == permissionName);

            if (existingPermission)
            {
                ModelState.AddModelError("", "Permission already exists for this role.");
                return View();
            }

            await _roleManager.AddClaimAsync(role, new Claim("Permission", permissionName));

            return RedirectToAction("ListPermissions");
        }
    }
}
