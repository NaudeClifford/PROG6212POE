using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Models.ViewModels;

namespace PROG6212POE.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class ClaimsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ClaimsDBContext _context;

        public ClaimsController(UserManager<User> userManager, IWebHostEnvironment env, ClaimsDBContext context)
        {
            _userManager = userManager;
            _env = env;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var claims = _context.Claims
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Created)
                .ToList();

            return View(claims);
        }

        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId);

            if (lecturer == null)
                return Unauthorized("Lecturer profile not found.");

            var model = new ClaimViewModel
            {
                HourlyRate = (decimal)lecturer.HourRate
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId);

            if (lecturer == null)
            {
                ModelState.AddModelError("", "Lecturer information not found.");
                return View(model);
            }

            // Ensure hourly rate cannot be changed by the user
            model.HourlyRate = (decimal)lecturer.HourRate;

            if (!ModelState.IsValid)
                return View(model);

            // Validate file
            if (model.Document == null || model.Document.Length == 0)
            {
                ModelState.AddModelError("Document", "Please upload a document.");
                return View(model);
            }

            long maxSize = 5 * 1024 * 1024;

            if (model.Document.Length > maxSize)
            {
                ModelState.AddModelError("Document", "File must be under 5MB.");
                return View(model);
            }

            var extension = Path.GetExtension(model.Document.FileName).ToLower();
            var allowedExt = new[] { ".pdf", ".docx", ".xlsx" };

            if (!allowedExt.Contains(extension))
            {
                ModelState.AddModelError("Document", "Only PDF, DOCX, XLSX allowed.");
                return View(model);
            }

            try
            {
                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadDir = Path.Combine(_env.ContentRootPath, "EncryptedUploads");
                Directory.CreateDirectory(uploadDir);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = model.Document.OpenReadStream())
                {
                    await FileEncryptionHelper.EncryptFileAsync(stream, filePath);
                }

                // Save claim
                var claim = new Claim
                {
                    UserId = userId,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = (decimal)lecturer.HourRate,
                    Notes = model.Notes,
                    FilePath = fileName,
                    Created = DateTime.Now,
                    Status = "Pending"
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred during upload.");
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var claim = _context.Claims.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (claim == null)
                return NotFound();

            if (claim.Status != "Pending")
                return BadRequest("Only pending claims can be cancelled.");

            claim.Status = "Cancelled";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Lecturer, ProgrammeCoordinator, AcademicManager, Hr")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            var folder = Path.Combine(_env.ContentRootPath, "EncryptedUploads");
            var filePath = Path.Combine(folder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await FileEncryptionHelper.DecryptFileAsync(filePath, memory);
            memory.Position = 0;

            var contentType = fileName.EndsWith(".pdf") ? "application/pdf" :
                              fileName.EndsWith(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                              fileName.EndsWith(".xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                              "application/octet-stream";

            return File(memory, contentType, fileName);
        }
    }
}
