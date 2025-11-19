using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212POE.Data;

namespace PROG6212POE.Controllers
{
    [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
    public class AdminController : Controller
    {
        private readonly ClaimsDBContext _context;
        private readonly IWebHostEnvironment _env;
        public AdminController(ClaimsDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //View for Programme Coordinator
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult CoordinatorView()
        {
            var pendingClaims = _context.Claims
                .Include(c => c.User)  //Fixed this error by including this for commit 10
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.Created)
                .ToList();

            return View("CoordinatorView", pendingClaims);
        }

        //View for Academic Manager
        [Authorize(Roles = "AcademicManager")]
        public IActionResult ManagerView()
        {
            var verifiedClaims = _context.Claims
                .Include(c => c.User)  //Fixed this error by including this for commit 10
                .Where(c => c.Status == "Verified")
                .OrderByDescending(c => c.Created)
                .ToList();


            return View("ManagerView", verifiedClaims);
        }

        //Programme Coordinator verifies a claim
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Verify(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Verified";
            _context.SaveChanges();
            return RedirectToAction("CoordinatorView");
        }

        //Programme Coordinator rejects a claim
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Reject(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("CoordinatorView");
        }

        //Academic Manager approves a claim
        [Authorize(Roles = "AcademicManager")]
        public IActionResult Approve(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("ManagerView");
        }

        //Academic Manager rejects a claim
        [Authorize(Roles = "AcademicManager")]
        public IActionResult RejectFromManager(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("ManagerView");
        }

        [Authorize(Roles = "Lecturer, ProgrammeCoordinator, AcademicManager, Hr")]
        public async Task<IActionResult> DownloadFile(string fileName, bool download = false)
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

            var disposition = download ? "attachment" : "inline";

            Response.Headers.Add("Content-Disposition", $"{disposition}; filename=\"{fileName}\"");

            return File(memory, contentType);
        }

    }
}
