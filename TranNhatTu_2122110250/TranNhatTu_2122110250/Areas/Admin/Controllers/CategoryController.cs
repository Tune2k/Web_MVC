using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using System.Linq;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // Action hiển thị danh sách các danh mục
        public IActionResult Index(string searchTerm, int page = 1)
        {
            int pageSize = 5;

            var categoriesQuery = _context.Category.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                categoriesQuery = categoriesQuery.Where(c => c.Name.Contains(searchTerm));
            }

            int totalCategories = categoriesQuery.Count();
            int totalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);

            var categories = categoriesQuery
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(new CategoryIndexViewModel
            {
                Categories = categories,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages
            });
        }


        // GET: Action để hiển thị form tạo danh mục mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Action để xử lý form tạo danh mục mới
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Category.Add(category);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Action để hiển thị form chỉnh sửa danh mục
        public IActionResult Edit(int id)
        {
            var category = _context.Category.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Action để xử lý form chỉnh sửa danh mục
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Category.Find(category.Id);
                if (existing == null)
                    return NotFound();

                // Cập nhật tên danh mục
                existing.Name = category.Name;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Action để hiển thị form xác nhận xóa danh mục
        public IActionResult Delete(int id)
        {
            var category = _context.Category.Find(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Action để xử lý xóa danh mục
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _context.Category.Find(id);
            if (category != null)
            {
                _context.Category.Remove(category);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
