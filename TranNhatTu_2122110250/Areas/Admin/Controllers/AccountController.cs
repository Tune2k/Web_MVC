using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(AppDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Admin/Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã có session Role=Admin rồi, chuyển thẳng vào Admin Home
            var currentRole = HttpContext.Session.GetString("Role");
            if (currentRole == "Admin")
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            // Hiển thị lỗi nếu có (nếu bị đá ra từ Home do thiếu quyền)
            if (TempData.ContainsKey("LoginError"))
                ViewBag.Error = TempData["LoginError"];

            return View();
        }

        // POST: /Admin/Account/Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            //_logger.LogInformation(">>> ĐÃ VÀO POST /Admin/Account/Login, ModelState.IsValid={Valid}", ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                //_logger.LogInformation(">>> ModelState lỗi: {Errors}",
                //    string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
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

            if (result == PasswordVerificationResult.Success)
            {
                //_logger.LogInformation(">>> Trước khi set session, user.Role = {Role}", user.Role);
                HttpContext.Session.SetString("Role", user.Role);
                //_logger.LogInformation(">>> Sau khi set session, Role trong Session = {Role}", HttpContext.Session.GetString("Role"));
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // 1. Lưu session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            //_logger.LogInformation(">>> ĐÃ VÀO POST /Admin/Account/Login");
            // 2. Log chính xác giá trị trong session
            //_logger.LogInformation("Đã set session: Role = {Role}", HttpContext.Session.GetString("Role"));

            // 3. Redirect theo role
            if (user.Role == "Admin"){
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            // Nếu không phải Admin, chuyển về home phía user
            return RedirectToAction("Index", "Home");
        }

        // POST: /Admin/Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
