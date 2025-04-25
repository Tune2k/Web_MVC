using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;

namespace TranNhatTu_2122110250.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly AppDbContext _context;

        public UserOrderController(AppDbContext context)
        {
            _context = context;
        }
        // Action để hiển thị danh sách đơn hàng của người dùng
        public IActionResult Index()
        {
            var userId = GetUserId(); // Lấy userId từ session
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách đơn hàng của người dùng
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders); // Trả về view với danh sách đơn hàng
        }

        // Hàm lấy userId từ session hoặc cookie
        private int? GetUserId()
        {
            // Lấy userId từ session hoặc từ phương thức khác
            return HttpContext.Session.GetInt32("UserId");
        }
        // Action để hiển thị chi tiết đơn hàng

        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetail)      // Bao gồm chi tiết đơn hàng
                .ThenInclude(od => od.Product)     // Bao gồm thông tin sản phẩm trong OrderDetail
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound(); // Nếu không tìm thấy đơn hàng
            }

            return View(order); // Trả về view hiển thị chi tiết đơn hàng
        }

    }
}
