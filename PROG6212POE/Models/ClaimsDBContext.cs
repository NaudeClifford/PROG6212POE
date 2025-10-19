using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Models
{
    public class ClaimsDBContext : IdentityDbContext<User>
    {
        public ClaimsDBContext(DbContextOptions<ClaimsDBContext> options) : base(options) { }

        public DbSet<Claims> ClaimDB { get; set; }
        public DbSet<Lecturers> LecturerDB { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claims>().HasKey(x => x.Id);
            modelBuilder.Entity<Lecturers>().HasKey(x => x.Id);

        }





    }
}
