using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Models.ViewModels;
using Rotativa.AspNetCore;

namespace PROG6212POE.Controllers
{
    public class HrController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ClaimsDBContext _context;

        public HrController(ClaimsDBContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: HR Index
        public async Task<IActionResult> Index()
        {
            var model = new HrViewModel
            {
                Lecturers = await _context.Lecturers.Include(l => l.User).ToListAsync(),
                Admins = await _context.Admins.Include(a => a.User).ToListAsync()
            };
            return View(model);
        }

        // GET: Create Admin
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // POST: Create Admin
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            if (!ModelState.IsValid) return View(admin);

            var identityUser = new User
            {
                UserName = admin.Username,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                FirstName = admin.Name,
                LastName = admin.Surname
            };

            var result = await _userManager.CreateAsync(identityUser, "DefaultPassword123!");
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(admin);
            }

            if (!await _roleManager.RoleExistsAsync(admin.Role))
                await _roleManager.CreateAsync(new IdentityRole(admin.Role));

            await _userManager.AddToRoleAsync(identityUser, admin.Role);

            admin.UserId = identityUser.Id;
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Create Lecturer
        public IActionResult CreateLecturer()
        {
            return View();
        }

        // POST: Create Lecturer
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLecturer(Lecturers lecturer)
        {
            if (!ModelState.IsValid) return View(lecturer);

            var identityUser = new User
            {
                UserName = lecturer.Username,
                Email = lecturer.Email,
                PhoneNumber = lecturer.PhoneNumber,
                FirstName = lecturer.Name,
                LastName = lecturer.Surname
            };

            var result = await _userManager.CreateAsync(identityUser, "DefaultPassword123!");
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(lecturer);
            }

            const string role = "Lecturer";
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(identityUser, role);

            lecturer.UserId = identityUser.Id;
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Admin
        public async Task<IActionResult> EditAdmin(int? id)
        {
            if (id == null) return NotFound();

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        // POST: Edit Admin
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, Admin admin)
        {
            if (id != admin.Id) return NotFound();
            if (!ModelState.IsValid) return View(admin);

            var identityUser = await _userManager.FindByIdAsync(admin.UserId);
            if (identityUser != null)
            {
                identityUser.UserName = admin.Username;
                identityUser.Email = admin.Email;
                identityUser.PhoneNumber = admin.PhoneNumber;
                identityUser.FirstName = admin.Name;
                identityUser.LastName = admin.Surname;
                await _userManager.UpdateAsync(identityUser);
            }

            _context.Update(admin);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit Lecturer
        public async Task<IActionResult> EditLecturer(int? id)
        {
            if (id == null) return NotFound();

            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            return View(lecturer);
        }

        // POST: Edit Lecturer
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(int id, Lecturers lecturer)
        {
            if (id != lecturer.Id) return NotFound();
            if (!ModelState.IsValid) return View(lecturer);

            var identityUser = await _userManager.FindByIdAsync(lecturer.UserId);
            if (identityUser != null)
            {
                identityUser.UserName = lecturer.Username;
                identityUser.Email = lecturer.Email;
                identityUser.PhoneNumber = lecturer.PhoneNumber;
                identityUser.FirstName = lecturer.Name;
                identityUser.LastName = lecturer.Surname;
                await _userManager.UpdateAsync(identityUser);
            }

            _context.Update(lecturer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Admin or Lecturer
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string type)
        {
            if (type == "Admin")
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin != null)
                {
                    var identityUser = await _userManager.FindByIdAsync(admin.UserId);
                    if (identityUser != null)
                        await _userManager.DeleteAsync(identityUser);

                    _context.Admins.Remove(admin);
                    await _context.SaveChangesAsync();
                }
            }
            else if (type == "Lecturer")
            {
                var lecturer = await _context.Lecturers.FindAsync(id);
                if (lecturer != null)
                {
                    var identityUser = await _userManager.FindByIdAsync(lecturer.UserId);
                    if (identityUser != null)
                        await _userManager.DeleteAsync(identityUser);

                    _context.Lecturers.Remove(lecturer);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return BadRequest("Invalid delete type.");
            }

            return RedirectToAction(nameof(Index));
        }

        // View Claims Report in browser
        public async Task<IActionResult> ClaimsReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == "Approved")
                .Include(c => c.User)
                .ToListAsync();

            var claimReports = new List<ClaimInvoiceViewModel>();

            foreach (var group in approvedClaims.GroupBy(c => c.UserId))
            {
                var user = group.First().User;
                var identityUser = await _userManager.FindByIdAsync(group.Key);
                var roles = await _userManager.GetRolesAsync(identityUser);

                claimReports.Add(new ClaimInvoiceViewModel
                {
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                    Role = roles.FirstOrDefault() ?? "Unknown",
                    ApprovedClaims = group.ToList()
                });
            }

            return View(claimReports);
        }

        // Download Claims Report as PDF
        public async Task<IActionResult> DownloadClaimsReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == "Approved")
                .Include(c => c.User)
                .ToListAsync();

            var claimReports = new List<ClaimInvoiceViewModel>();

            foreach (var group in approvedClaims.GroupBy(c => c.UserId))
            {
                var user = group.First().User;
                var identityUser = await _userManager.FindByIdAsync(group.Key);
                var roles = await _userManager.GetRolesAsync(identityUser);

                claimReports.Add(new ClaimInvoiceViewModel
                {
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                    Role = roles.FirstOrDefault() ?? "Unknown",
                    ApprovedClaims = group.ToList()
                });
            }

            return new ViewAsPdf("ClaimsReport", claimReports)
            {
                FileName = $"Claims_Report_{DateTime.Now:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
    }
}
