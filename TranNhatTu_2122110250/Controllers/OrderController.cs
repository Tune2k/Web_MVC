using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _context.Order
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _context.Order
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                _context.Order.Add(order);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.Id)
                return BadRequest();

            _context.Order.Update(order);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound();

            _context.Order.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
