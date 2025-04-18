using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Model;
using System.Linq;
using TranNhatTu_2122110250.Data;
using Microsoft.EntityFrameworkCore;

namespace TranNhatTu_2122110250.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .OrderByDescending(p => p.CreatedDate)  // Sắp xếp theo ngày tạo (giả sử bạn có trường CreatedDate kiểu DateTime)
                .Take(3)
                .ToList();


            var categories = _context.Category.ToList();
            var productRecommended = _context.Products
                .OrderByDescending(p => p.CreatedBy)
                .Take(10)
                .ToList();

            foreach (var product in productRecommended)
            {
                var category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                if (category != null)
                {
                    product.Category_name = category.Name;
                }
            }


            var model = new HomeViewModel
            {
                Categories = _context.Category.ToList(),
                Product = products,
                ProductRecommended = productRecommended
            };

            return View(model);
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DealsAndOffers()
        {
            return View();
        }
    }
}
