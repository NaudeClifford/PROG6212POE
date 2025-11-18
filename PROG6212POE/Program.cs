using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibrarysPractice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //  ENABLE SESSION SUPPORT
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //Add MVC + Global Authorization
            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // APPLY AUTH TO ALL CONTROLLERS,except Auth/Login
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));

            });

            //Dependency Injection
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<ILecturerService, LecturerService>();
            builder.Services.AddScoped<IClaimService, ClaimService>();

            builder.Services.AddDbContext<ClaimsDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            //Identity Configuration
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ClaimsDBContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/Login";
            });

            var app = builder.Build();

            var env = app.Environment;
            Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

            //DATABASE MIGRATION + SEED
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ClaimsDBContext>();
                db.Database.Migrate();

                await UserSeeder.SeedAsync(app.Services);
            }

            //Middlewares
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //ENABLE SESSION MIDDLEWARE
            app.UseSession();

            //ROUTES
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
