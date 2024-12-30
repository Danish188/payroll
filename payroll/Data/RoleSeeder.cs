using Microsoft.AspNetCore.Identity;
using payroll.Models.Enums;
using System.Security.Claims;

namespace payroll.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new List<string> { "Admin", "User" };

            foreach (var role in roles) 
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var adminEmail = "admin@gmail.com";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public static async Task SeedPermissionsForRoles(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                var claims = new[] { Permissions.ManageUsers, Permissions.ViewReports, Permissions.EditContent };

                foreach (var claim in claims)
                {
                    if (!await roleManager.RoleClaimsExists(adminRole, claim))
                    {
                        await roleManager.AddClaimAsync(adminRole, new Claim("Permission", claim));
                    }
                }
            }
        }

        public static async Task<bool> RoleClaimsExists(this RoleManager<IdentityRole> roleManager, IdentityRole role, string claimValue)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            return claims.Any(c => c.Type == "Permission" && c.Value == claimValue);
        }
    }
}
