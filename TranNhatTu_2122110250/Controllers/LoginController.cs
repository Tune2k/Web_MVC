using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Services;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TranNhatTu_2122110250.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public LoginController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // ---------- API Đăng ký ----------
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_context.User.Any(u => u.Email == model.Email))
                return Ok(new { Message = "Email đã được sử dụng." });

            if (_context.User.Any(u => u.Username == model.Username))
                return Ok(new { Message = "Tên đăng nhập đã tồn tại." });

            // Hash password manually
            var hashedPassword = HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Role = model.Role ?? "User", // Default role
                Password = hashedPassword
            };

            _context.User.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "Đăng kí thành công" });
        }

        // ---------- API Đăng nhập ----------
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return Ok(new { Message = "Vui lòng nhập email và mật khẩu." });

            var user = _context.User.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
                return Ok(new { Message = "Email không tồn tại." });

            if (!VerifyPassword(model.Password, user.Password))
                return Ok(new { Message = "Mật khẩu không đúng." });

            var token = _tokenService.GenerateToken(user);

            // 👉 Ghi username vào session
            HttpContext.Session.SetString("Username", user.Username);

            return Ok(new
            {
                Message = "Đăng nhập thành công",
                Token = token,
                Username = user.Username
            });
        }

        [HttpPost("set-session")]
        public IActionResult SetSession([FromBody] JObject data)
        {
            var username = data["username"]?.ToString();
            if (!string.IsNullOrEmpty(username))
            {
                HttpContext.Session.SetString("Username", username);
                return Ok(new { Message = "Session đã được thiết lập." });
            }
            return BadRequest(new { Message = "Thiếu username." });
        }

        // ---------- Helper methods for password hashing ----------
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }
    }

    // ✅ Model đăng ký mới (theo Username & Role)
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; } // Tùy chọn
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
