using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Data;
using Microsoft.EntityFrameworkCore;

namespace PROG6212POE.Controllers
{
    [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
    public class AdminController : Controller
    {
        private readonly ClaimsDBContext _context;

        public AdminController(ClaimsDBContext context)
        {
            _context = context;
        }

        // View for Programme Coordinator
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult CoordinatorView()
        {
            var pendingClaims = _context.Claims
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.Created)
                .ToList();

            return View("CoordinatorView", pendingClaims);
        }

        // View for Academic Manager
        [Authorize(Roles = "AcademicManager")]
        public IActionResult ManagerView()
        {
            var verifiedClaims = _context.Claims
                .Where(c => c.Status == "Verified")
                .OrderByDescending(c => c.Created)
                .ToList();

            return View("ManagerView", verifiedClaims);
        }

        // Programme Coordinator verifies a claim
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Verify(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Verified";
            _context.SaveChanges();
            return RedirectToAction("CoordinatorView");
        }

        // Programme Coordinator rejects a claim
        [Authorize(Roles = "ProgrammeCoordinator")]
        public IActionResult Reject(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("CoordinatorView");
        }

        // Academic Manager approves a claim
        [Authorize(Roles = "AcademicManager")]
        public IActionResult Approve(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("ManagerView");
        }

        // Academic Manager rejects a claim
        [Authorize(Roles = "AcademicManager")]
        public IActionResult RejectFromManager(int id)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            claim.Status = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("ManagerView");
        }
    }
}
