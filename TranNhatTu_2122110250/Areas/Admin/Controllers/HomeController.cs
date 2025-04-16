using Microsoft.AspNetCore.Mvc;

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //var role = HttpContext.Session.GetString("Role");
            //if (role != "Admin")
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            return View();
        }
    }
}
