using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using PROG6212POE.Services;
namespace LibrarysPractice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILecturerService _lecturerService;
        private readonly IClaimService _claimService;

        public HomeController(ILogger<HomeController> logger, ILecturerService lecturerService, IClaimService claimService)
        {
            _logger = logger;
            _lecturerService = lecturerService;
            _claimService = claimService;

        }

        public IActionResult Index(int isLogged = 0)
        {

            var claims = _claimService.GetClaims();
            
            return View(claims);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
