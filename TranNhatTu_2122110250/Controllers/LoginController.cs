using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Services;

namespace TranNhatTu_2122110250.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;




        public LoginController(AppDbContext context, IConfiguration config, IPasswordHasher<User> passwordHasher, IUserService userService, ITokenService tokenService)
        {
            _context = context;
            _config = config;
            _passwordHasher = passwordHasher;
            _userService = userService;
            _tokenService = tokenService;
        }

        // ---------- API Đăng ký ----------
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_context.User.Any(u => u.Email == model.Email))
            {
                return BadRequest("Email đã được sử dụng.");
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.User.Add(user);
            _context.SaveChanges();

            return Ok("Đăng ký thành công");
        }

        // ---------- API Đăng nhập ----------
        // Đảm bảo trả về JSON thay vì chuyển hướng
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Thông tin không hợp lệ.");

            var user = await _userService.Authenticate(model.Email, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu." });

            // Lưu userId vào session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.FirstName);  // Lưu tên người dùng vào session

            // Tạo danh sách claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.FirstName), // Lưu tên người dùng vào Claim
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Lưu userId vào Claim
    };

            // Tạo ClaimsIdentity và ClaimsPrincipal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Đăng nhập người dùng với cookie authentication
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Trả về thông tin đăng nhập thành công
            return Ok(new { message = "Đăng nhập thành công", token = "some-token-here" });
        }





        // ---------- API Đăng xuất ----------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa thông tin người dùng khỏi session
            HttpContext.Session.Remove("UserId");

            // Đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

    }

    // Model cho đăng ký người dùng
    public class RegisterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Model cho đăng nhập người dùng
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
