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
                // TODO: Create Manager dashboard
                return RedirectToAction("Index", "Admin");
            }
            else if (User.IsInRole("Employee"))
            {
                // TODO: Create Employee dashboard
                return RedirectToAction("Index", "Admin");
            }

            // Default fallback - redirect to login
            return RedirectToAction("Login", "Auth");
        }
    }
}
