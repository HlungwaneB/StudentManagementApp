using ASPNETCore_DB.Data;
using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPNETCore_DB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SQLiteDBContext _context;
        private readonly IDBInitializer _seedDatabase;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SQLiteDBContext context, IDBInitializer seedDatabase, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _seedDatabase = seedDatabase;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewData["UserID"] = _userManager.GetUserId(this.User);
            ViewData["UserName"] = _userManager.GetUserName(this.User);

            if (this.User.IsInRole("Admin"))
            {
                ViewData["UserRole"] = "Admin";
            }

            if (this.User.IsInRole("User"))
            {
                ViewData["UserRole"] = "User";
            }

            return View();
        }

        public IActionResult SeedDatabase()
        {
            _seedDatabase.Initialize(_context); 
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}