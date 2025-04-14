using Microsoft.AspNetCore.Authorization; // 👈 Thêm namespace này
using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.EntityFrameworkCore;

namespace TranNhatTu_2122110250.Controllers
{
    [Authorize] // 👈 BẮT BUỘC phải có token mới truy cập được
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            return BadRequest();
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Update(product);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
