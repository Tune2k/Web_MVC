using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;
using TranNhatTu_2122110250.Helpers;

namespace TranNhatTu_2122110250.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string tab)
        {
            ViewBag.ActivePanel = tab == "register" ? "register" : "";
            ViewBag.RegisterError = TempData["RegisterError"];
            ViewBag.RegisterSuccess = TempData["RegisterSuccess"];
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = _context.User.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");

            if (HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") == null)
            {
                HttpContext.Session.SetObjectAsJson("Cart", new List<CartItem>());
            }

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            if (_context.User.Any(u => u.Email == email))
            {
                TempData["RegisterError"] = "Email đã được sử dụng.";
                return RedirectToAction("Login", new { tab = "register" });
            }

            if (_context.User.Any(u => u.Username == username))
            {
                TempData["RegisterError"] = "Tên đăng nhập đã tồn tại.";
                return RedirectToAction("Login", new { tab = "register" });
            }

            var user = new User
            {
                Username = username,
                Email = email,
                Role = "User"
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, password);

            try
            {
                _context.User.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["RegisterError"] = "Lỗi khi lưu: " + ex.Message;
                return RedirectToAction("Login", new { tab = "register" });
            }

            TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View(); // Trả về Logout.cshtml
        }
    }
}
