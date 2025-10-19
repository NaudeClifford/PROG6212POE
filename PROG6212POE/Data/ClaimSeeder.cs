using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;

namespace PROG6212POE.Data
{
    public class ClaimSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ClaimsDBContext>();
            await db.Database.EnsureCreatedAsync();

            if (!await db.ClaimDB.AnyAsync())
            {

                await db.ClaimDB.AddRangeAsync(
                    new Claims
                    {
                        Name = "",
                        Description = "",
                        Created = DateTime.Now,
                        Approval = false,
                    }
                );


                await db.SaveChangesAsync();
            }
        }
    }
}
