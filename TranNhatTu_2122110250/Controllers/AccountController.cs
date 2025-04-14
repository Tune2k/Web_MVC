using Microsoft.AspNetCore.Mvc;

namespace TranNhatTu_2122110250.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
