using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Controllers
{
    [Authorize] // 👈 Yêu cầu token để truy cập API
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/user/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                // Lấy thông tin từ claims của token JWT
                var usernameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(usernameClaim))
                {
                    return Unauthorized(new { error = "User not found" });
                }

                // Tìm người dùng trong cơ sở dữ liệu
                var user = await _context.User.FirstOrDefaultAsync(u => u.Username == usernameClaim);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                // Trả về thông tin người dùng (chỉ tên người dùng ở đây)
                return Ok(new { username = user.Username });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.User.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/user
        [HttpPost]
        public IActionResult CreateUser([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                _context.User.Add(user);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            return BadRequest(ModelState);
        }

        // PUT: api/user/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();

            _context.User.Update(user);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();

            _context.User.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
