using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Moshrefy.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to appropriate dashboard based on role
            if (User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "SuperAdmin");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Manager");
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("Employee");
            }

            // Default fallback - redirect to login
            return RedirectToAction("Login", "Auth");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Manager()
        {
            return View();
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Employee()
        {
            return View();
        }
    }
}

