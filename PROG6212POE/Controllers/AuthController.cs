using PROG6212POE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PROG6212POE.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // CLEAR SESSION
            HttpContext.Session.Clear();

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // FIND USER BY USERNAME OR EMAIL
            var user = await _userManager.FindByNameAsync(model.UserNameOrEmail);

            if (user == null && model.UserNameOrEmail.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(model.UserNameOrEmail);
            }

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    // GET ROLES
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "Lecturer";

                    // 🚀 SAVE SESSION VALUES
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.UserName ?? "");
                    HttpContext.Session.SetString("UserRole", role);

                    // REDIRECT BASED ON ROLE
                    if (role == "AcademicManager")
                        return RedirectToAction("ManagerView", "Admin");

                    if (role == "ProgrammeCoordinator")
                        return RedirectToAction("CoordinatorView", "Admin");

                    if (role == "Hr")
                        return RedirectToAction("Index", "Hr");

                    if (role == "Lecturer")
                        return RedirectToAction("Index", "Claims");

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }
    }
}
