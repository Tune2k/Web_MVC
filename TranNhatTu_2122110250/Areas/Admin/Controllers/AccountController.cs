using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity; // 👈 cần thêm dòng này

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>(); // 👈 khởi tạo để dùng kiểm tra mật khẩu
        }



        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            ViewBag.Role = role ?? "null";

            if (role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                // Trả lỗi validate cụ thể từ annotation
                return View(model);
            }

            var user = _context.User.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản không tồn tại.";
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ViewBag.Error = "Mật khẩu không đúng.";
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            if (user.Role == "Admin")
            {
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Điều hướng
            //if (user.Role == "Admin")
            //{
            //    HttpContext.Session.SetString("IsAdmin", "true");
            //    return RedirectToAction("Index", "Home", new { area = "Admin" });
            //}

            return RedirectToAction("Index", "Home");
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
