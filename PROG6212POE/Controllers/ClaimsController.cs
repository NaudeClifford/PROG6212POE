using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using PROG6212POE.Models;
using PROG6212POE.Models.ViewModels;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: /Claims/Index
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            // Get claims for the logged-in lecturer
            var claims = _context.Claims
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Created)
                .ToList();

            return View(claims);
        }

        // GET: /Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Claims/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check if file is attached
            if (model.Document == null || model.Document.Length == 0)
            {
                ModelState.AddModelError("Document", "Please upload a document.");
                return View(model);
            }

            // Check file size (limit: 5MB)
            long maxSize = 5 * 1024 * 1024;
            if (model.Document.Length > maxSize)
            {
                ModelState.AddModelError("Document", "File size must be under 5MB.");
                return View(model);
            }

            // Check allowed types
            var extension = Path.GetExtension(model.Document.FileName).ToLower();
            var allowed = new[] { ".pdf", ".docx", ".xlsx" };
            if (!allowed.Contains(extension))
            {
                ModelState.AddModelError("Document", "Only PDF, DOCX, and XLSX files are allowed.");
                return View(model);
            }

            try
            {
                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "EncryptedUploads");
                Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = model.Document.OpenReadStream())
                {
                    await FileEncryptionHelper.EncryptFileAsync(stream, filePath);
                }

                // Save claim
                var claim = new Claim
                {
                    UserId = _userManager.GetUserId(User),
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    Notes = model.Notes,
                    FilePath = fileName,
                    Created = DateTime.Now,
                    Status = "Pending"
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // General fallback error
                ModelState.AddModelError(string.Empty, "An error occurred while uploading the file. Please try again.");
                // Optionally log: _logger.LogError(ex, "File upload failed");
                return View(model);
            }
        }


        // GET: /Claims/DownloadFile?fileName=xxx
        [Authorize(Roles = "Lecturer,ProgrammeCoordinator,AdcademicManager")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return BadRequest();

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "EncryptedUploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memoryStream = new MemoryStream();
            await FileEncryptionHelper.DecryptFileAsync(filePath, memoryStream);
            memoryStream.Position = 0;

            var contentType = fileName.EndsWith(".pdf") ? "application/pdf" :
                              fileName.EndsWith(".docx") ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" :
                              fileName.EndsWith(".xlsx") ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                              "application/octet-stream";

            return File(memoryStream, contentType, Path.GetFileName(fileName));
        }
    }
}
