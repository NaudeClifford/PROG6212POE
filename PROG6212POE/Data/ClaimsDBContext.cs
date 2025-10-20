using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Models;

namespace PROG6212POE.Data
{
    public class ClaimsDBContext : IdentityDbContext<User>
    {
        public ClaimsDBContext(DbContextOptions<ClaimsDBContext> options) : base(options) { }

        // Use pluralized DbSet names for clarity and EF convention
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Lecturers> Lecturers { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithMany()       // or .WithOne(...) depending on your model
                .HasForeignKey(a => a.UserId)
                .IsRequired();
        }

    }
}
