using Microsoft.AspNetCore.Mvc;

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            // 👇 Debug thử ra console
            Console.WriteLine("ROLE trong session là: " + role);

            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                TempData["LoginError"] = "Bạn không có quyền truy cập trang Admin.";
                //return RedirectToAction("Login", "Account");
            }

            return View();
        }

    }
}
