using PROG6212POE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PROG6212POE.Data  // <-- make sure this matches your project namespace
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)  // Fixed method name from SeedAsyc to SeedAsync
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = new[] { "AcademicManager", "ProgrammeCoordinator", "Lecturer" };

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        var msg = string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
                        throw new Exception($"Failed to create role '{role}': {msg}");
                    }
                }
            }

            // Seed admin user
            var adminEmail = "admin@library.local";
            var adminUserName = "admin";
            var adminPassword = "Admin#12345";

            var admin = await userManager.FindByEmailAsync(adminEmail) ?? await userManager.FindByNameAsync(adminUserName);
            if (admin == null)
            {
                admin = new User
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "John",
                    LastName = "Doe"
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (!createResult.Succeeded)
                {
                    var errorMsg = string.Join(", ", createResult.Errors.Select(x => $"{x.Code}: {x.Description}"));
                    throw new Exception($"Failed to create admin user '{adminEmail}': {errorMsg}");
                }
            }
            else if (!admin.EmailConfirmed)
            {
                admin.EmailConfirmed = true;
                await userManager.UpdateAsync(admin);
            }

            // Assign roles to admin user
            if (!await userManager.IsInRoleAsync(admin, "AcademicManager"))
            {
                await userManager.AddToRoleAsync(admin, "AcademicManager");
            }
            if (!await userManager.IsInRoleAsync(admin, "ProgrammeCoordinator"))
            {
                await userManager.AddToRoleAsync(admin, "ProgrammeCoordinator");
            }
        }
    }
}
