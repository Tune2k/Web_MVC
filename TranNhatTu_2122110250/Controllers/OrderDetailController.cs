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
    public class OrderDetailController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orderdetail
        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            try
            {
                var orderDetails = await _context.OrderDetails
                    .Include(od => od.Product)
                    .Include(od => od.Order)
                    .ToListAsync();

                return Ok(orderDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/orderdetail/{id}
        [HttpGet("{id}")]
        public IActionResult GetOrderDetailById(int id)
        {
            var detail = _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .FirstOrDefault(od => od.Id == id);

            if (detail == null)
                return NotFound();

            return Ok(detail);
        }

        // POST: api/orderdetail
        [HttpPost]
        public IActionResult CreateOrderDetail([FromBody] OrderDetail detail)
        {
            if (ModelState.IsValid)
            {
                _context.OrderDetails.Add(detail);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetOrderDetailById), new { id = detail.Id }, detail);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/orderdetail/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderDetail(int id, [FromBody] OrderDetail detail)
        {
            if (id != detail.Id)
                return BadRequest();

            _context.OrderDetails.Update(detail);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE: api/orderdetail/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            var detail = _context.OrderDetails.FirstOrDefault(od => od.Id == id);
            if (detail == null)
                return NotFound();

            _context.OrderDetails.Remove(detail);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
