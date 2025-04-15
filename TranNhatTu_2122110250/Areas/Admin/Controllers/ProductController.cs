using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using TranNhatTu_2122110250.ViewModels;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;

//using YourProject.Models;

namespace YourProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                                   .Include(p => p.Category)
                                   .ToList();

            var viewModel = new ProductIndexViewModel
            {
                Products = products
            };

            return View(viewModel);
        }




        private List<SelectListItem> GetCategorySelectList()
        {
            var categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            Console.WriteLine("== Danh sách danh mục ==");
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.Value}, Name: {category.Text}");
            }
            if (!categories.Any())
                throw new Exception("Không có danh mục nào trong database!");

            return categories;
        }

        //public IActionResult Create() => View();

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ProductCreateViewModel
            {
                Product = new Product(),
                Categories = GetCategorySelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ProductCreateViewModel viewModel, IFormFile imageFile)
        {
            // Luôn nạp lại danh mục cho dropdown (nếu có lỗi)
            viewModel.Categories = GetCategorySelectList();

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(viewModel);
            }

            var product = viewModel.Product;

            // Xử lý ảnh nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                product.Image = "/images/" + uniqueFileName;
            }

            // Gán tên danh mục
            var selectedCategory = _context.Category.FirstOrDefault(c => c.Id == product.CategoryId);
            if (selectedCategory != null)
            {
                product.Category_name = selectedCategory.Name; // chỉ để hiển thị nếu cần
            }


            product.Category_name = selectedCategory.Name;
            product.CreatedDate = DateTime.Now;
            product.CreatedBy = "admin";

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Products.Find(product.Id);
                if (existing == null)
                    return NotFound();

                // Cập nhật ảnh nếu có file mới
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    existing.Image = "/images/" + uniqueFileName;
                }

                // Cập nhật các trường
                existing.Name = product.Name;
                existing.Model3D = product.Model3D;
                existing.Price = product.Price;
                existing.Description = product.Description;
                existing.Stock = product.Stock;
                existing.CategoryId = product.CategoryId;
                existing.Category_name = product.Category_name;
                existing.UpdatedDate = DateTime.Now;
                existing.UpdatedBy = "admin";

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }


        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.DeletedBy = "admin";
                product.DeletedDate = DateTime.Now;
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



    }
}
