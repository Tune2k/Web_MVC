using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Index()
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetail)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
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
