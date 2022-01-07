using Microsoft.AspNetCore.Mvc;

namespace CoreMvcIdentity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profil", "Member");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
