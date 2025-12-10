using Microsoft.AspNetCore.Mvc;
using Moshrefy.Web.Models;

namespace Moshrefy.Web.Controllers
{
    public class AuthConroller : Controller
    {

        public IActionResult Login(LoginVM loginVM)
        {
            return View( );
        }
    }
}
