using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Models.ViewModels;

namespace PROG6212POE.Controllers
{
    public class HrController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ClaimsDBContext _context;

        public HrController(ClaimsDBContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HrViewModel
            {
                Lecturers = await _context.Lecturers.Include(l => l.User).ToListAsync(),
                Admins = await _context.Admins.Include(a => a.User).ToListAsync()
            };

            return View(model);
        }


        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(Admin admin)
        {
            if (ModelState.IsValid)
            {
                // 1. Create Identity user
                var identityUser = new IdentityUser
                {
                    UserName = admin.Username,
                    Email = admin.Email,
                    PhoneNumber = admin.PhoneNumber
                };

                var result = await _userManager.CreateAsync(identityUser, "DefaultPassword123!");

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);
                    return View(admin);
                }

                // 2. Assign Role to Identity User
                if (!await _roleManager.RoleExistsAsync(admin.Role))
                    await _roleManager.CreateAsync(new IdentityRole(admin.Role));

                await _userManager.AddToRoleAsync(identityUser, admin.Role);

                // 3. Insert into Admin table
                admin.UserId = identityUser.Id;

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(admin);
        }

        public IActionResult CreateLecturer()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLecturer(Lecturers lecturer)
        {
            if (ModelState.IsValid)
            {
                // 1. Create Identity user
                var identityUser = new IdentityUser
                {
                    UserName = lecturer.Username,
                    Email = lecturer.Email,
                    PhoneNumber = lecturer.PhoneNumber
                };

                var result = await _userManager.CreateAsync(identityUser, "DefaultPassword123!");

                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);
                    return View(lecturer);
                }

                // 2. Assign Lecturer Role
                string role = "Lecturer";

                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(identityUser, role);

                // 3. Insert into Lecturer table
                lecturer.UserId = identityUser.Id;

                _context.Lecturers.Add(lecturer);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(lecturer);
        }

        public async Task<IActionResult> EditAdmin(int? id)
        {
            if (id == null) return NotFound();

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null) return NotFound();

            return View(admin);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, Admin admin)
        {
            if (id != admin.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // update identity user details too
                var identityUser = await _userManager.FindByIdAsync(admin.UserId);

                identityUser.UserName = admin.Username;
                identityUser.Email = admin.Email;
                identityUser.PhoneNumber = admin.PhoneNumber;

                await _userManager.UpdateAsync(identityUser);

                // update admin table
                _context.Update(admin);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(admin);
        }

        public async Task<IActionResult> EditLecturer(int? id)
        {
            if (id == null) return NotFound();

            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            return View(lecturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(int id, Lecturers lecturer)
        {
            if (id != lecturer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.FindByIdAsync(lecturer.UserId);

                identityUser.UserName = lecturer.Username;
                identityUser.Email = lecturer.Email;
                identityUser.PhoneNumber = lecturer.PhoneNumber;

                await _userManager.UpdateAsync(identityUser);

                _context.Update(lecturer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(lecturer);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string type)
        {
            if (type == "Admin")
            {
                var admin = await _context.Admins.FindAsync(id);
                if (admin == null) return NotFound();

                var identityUser = await _userManager.FindByIdAsync(admin.UserId);
                if (identityUser != null)
                    await _userManager.DeleteAsync(identityUser);

                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
            else if (type == "Lecturer")
            {
                var lecturer = await _context.Lecturers.FindAsync(id);
                if (lecturer == null) return NotFound();

                var identityUser = await _userManager.FindByIdAsync(lecturer.UserId);
                if (identityUser != null)
                    await _userManager.DeleteAsync(identityUser);

                _context.Lecturers.Remove(lecturer);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("Invalid delete type.");
            }

            return RedirectToAction(nameof(Index));
        }


    }
}

