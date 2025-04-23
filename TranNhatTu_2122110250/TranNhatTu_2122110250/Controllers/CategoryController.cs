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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Category
                    .Include(c => c.Products) // nếu muốn load danh sách sản phẩm luôn
                    .ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var category = _context.Category
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public IActionResult CreateCategory([FromBody] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Add(category);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            return BadRequest();
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
                return BadRequest();

            _context.Update(category);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Category.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            _context.Category.Remove(category);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
