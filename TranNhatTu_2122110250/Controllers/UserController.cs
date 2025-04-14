using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
