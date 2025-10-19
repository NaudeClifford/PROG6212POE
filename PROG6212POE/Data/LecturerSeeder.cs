using PROG6212POE.Models;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Data
{
    public static class LecturerSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ClaimsDBContext>();
            await db.Database.EnsureCreatedAsync();

            if (!await db.LecturerDB.AnyAsync())
            {

                await db.LecturerDB.AddRangeAsync(
                    new Lecturers
                    {
                        Name = "",
                        Surname = "",
                        Username = "",
                        Email = "",
                        PhoneNumber = "",
                        HoursWorked = 0,
                        HourRate = 0,
                    }
                );


                await db.SaveChangesAsync();
            }
        }
    }
}
