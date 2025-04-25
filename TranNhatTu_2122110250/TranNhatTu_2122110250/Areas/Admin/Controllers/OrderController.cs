using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model; // namespace chứa AppDbContext, Order, OrderDetail, Product, ...

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public IActionResult Index(int page = 1, string searchTerm = null)
        {
            // Số lượng đơn hàng mỗi trang
            int pageSize = 5;

            var ordersQuery = _context.Orders
                .Include(o => o.OrderDetail)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            // Tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                ordersQuery = ordersQuery.Where(o => o.CustomerName.Contains(searchTerm));
            }

            // Lấy tổng số đơn hàng để tính số trang
            int totalOrders = ordersQuery.Count();

            // Lấy danh sách đơn hàng cho trang hiện tại
            var orders = ordersQuery
                .Skip((page - 1) * pageSize)   // Bỏ qua các đơn hàng ở các trang trước
                .Take(pageSize)                // Lấy 10 đơn hàng trên mỗi trang
                .ToList();

            // Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            // Trả về View với ViewModel chứa danh sách đơn hàng và thông tin phân trang
            return View(new OrderIndexViewModel
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            });
        }


        // GET: Admin/Order/Details/5
        public IActionResult Details(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetail)
                    .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET: Admin/Order/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetail)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine("ID nhận được là: " + id); // => Xem có phải 0 không?
            var order = _context.Orders
                .Include(o => o.OrderDetail)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Xoá các OrderDetail trước
            _context.OrderDetails.RemoveRange(order.OrderDetail);

            // Xoá Order
            _context.Orders.Remove(order);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }




    }
}
