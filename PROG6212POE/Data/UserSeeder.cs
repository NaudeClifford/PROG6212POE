using Microsoft.AspNetCore.Identity;
using PROG6212POE.Models;

namespace PROG6212POE.Data
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ClaimsDBContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = {"HR", "AcademicManager", "ProgrammeCoordinator", "Lecturer" };

            //Check if roles exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var error = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                        throw new Exception($"Failed to create role '{role}': {error}");
                    }
                }
            }

            // Seed Lecturer
            var lecturerEmail = "lecturer@system.local";
            var lecturerUser = await userManager.FindByEmailAsync(lecturerEmail);

            if (lecturerUser == null)
            {
                lecturerUser = new User
                {
                    UserName = "lecturer",
                    Email = lecturerEmail,
                    EmailConfirmed = true,
                    FirstName = "Lecturer",
                    LastName = "One"
                };
                await userManager.CreateAsync(lecturerUser, "Lecturer#12345");
                await userManager.AddToRoleAsync(lecturerUser, "Lecturer");

                context.Lecturers.Add(new Lecturers
                {
                    UserId = lecturerUser.Id,
                    User = lecturerUser,
                    Name = lecturerUser.FirstName,
                    Surname = lecturerUser.LastName,
                    Username = lecturerUser.UserName,
                    Email = lecturerUser.Email,
                    PhoneNumber = "333-333-3333",
                    HoursWorked = 0,
                    HourRate = 0
                });
            }
            //Seed HR Manager
            var hrEmail = "hr@system.local";
            var hrUser = await userManager.FindByEmailAsync(hrEmail);

            if (hrUser == null)
            {
                hrUser = new User
                {
                    UserName = "hr",
                    Email = hrEmail,
                    EmailConfirmed = true,
                    FirstName = "hr",
                    LastName = "One"
                };
                await userManager.CreateAsync(hrUser, "Hr#12345");
                await userManager.AddToRoleAsync(hrUser, "HR");

                context.Admins.Add(new Admin
                {
                    UserId = hrUser.Id,
                    User = hrUser,
                    Role = "HR",
                    PhoneNumber = "111-111-1111",
                    Username = hrUser.UserName,
                    Email = hrUser.Email,
                    Name = hrUser.FirstName,
                    Surname = hrUser.LastName
                });
            }

            //Seed Academic Manager
            var managerEmail = "manager@system.local";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);
            
            if (managerUser == null)
            {
                managerUser = new User
                {
                    UserName = "manager", 
                    Email = managerEmail,
                    EmailConfirmed = true,
                    FirstName = "Manager",
                    LastName = "One"
                };
                await userManager.CreateAsync(managerUser, "Manager#12345");
                await userManager.AddToRoleAsync(managerUser, "AcademicManager");

                context.Admins.Add(new Admin
                {
                    UserId = managerUser.Id,
                    User = managerUser,
                    Role = "AcademicManager",
                    PhoneNumber = "111-111-1111",
                    Username = managerUser.UserName,
                    Email = managerUser.Email,
                    Name = managerUser.FirstName,
                    Surname = managerUser.LastName
                });
            }

            //Seed Programme Coordinator
            var coordinatorEmail = "coordinator@system.local";
            var coordinatorUser = await userManager.FindByEmailAsync(coordinatorEmail);
            
            if (coordinatorUser == null)
            {
                coordinatorUser = new User
                {
                    UserName = "coordinator",
                    Email = coordinatorEmail,
                    EmailConfirmed = true,
                    FirstName = "Coordinator",
                    LastName = "One"
                };
                await userManager.CreateAsync(coordinatorUser, "Coordinator#12345");
                await userManager.AddToRoleAsync(coordinatorUser, "ProgrammeCoordinator");

                context.Admins.Add(new Admin
                {
                    UserId = coordinatorUser.Id,
                    User = coordinatorUser,
                    Role = "ProgrammeCoordinator",
                    PhoneNumber = "222-222-2222",
                    Username = coordinatorUser.UserName,
                    Email = coordinatorUser.Email,
                    Name = coordinatorUser.FirstName,
                    Surname = coordinatorUser.LastName
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
