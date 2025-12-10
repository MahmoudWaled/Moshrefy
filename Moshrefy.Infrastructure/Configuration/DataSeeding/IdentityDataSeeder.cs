using Microsoft.AspNetCore.Identity;
using Moshrefy.Domain.Exceptions;
using Moshrefy.Domain.Identity;

namespace Moshrefy.infrastructure.Configuration.DataSeeding
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "SuperAdmin", "Admin", "Manager", "Employee" };

            try
            {
                // Seed Roles
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }


                // Seed SuperAdmin User

                string superAdminEmail = "fordevelopment00@gmail.com";
                var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);
                if (superAdmin == null)
                {
                    superAdmin = new ApplicationUser
                    {
                        Name = "Mahmoud Waled",
                        UserName = "MahmoudWaled",
                        Email = superAdminEmail,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = "System",
                        CreatedByName = "System",
                        IsActive = true,
                        CenterId = null
                    };

                    var result = await userManager.CreateAsync(superAdmin, "AdminP@ssw0rd");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRolesAsync(superAdmin, roles);
                    }
                    else
                    {
                        throw new FailedException("Failed to create SuperAdmin: " +
                                string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                }

            }
            catch (Exception ex)
            {
                throw new FailedException($"Failed to seed identity data: {ex.Message}");
            }
        }
    }
}
