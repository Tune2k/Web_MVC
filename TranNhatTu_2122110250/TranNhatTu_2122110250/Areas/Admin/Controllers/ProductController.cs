using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using TranNhatTu_2122110250.ViewModels;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;
using Microsoft.Extensions.Logging; // Đảm bảo import namespace này

using TranNhatTu_2122110250.Helpers;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Hosting.Internal;

//using YourProject.Models;

namespace YourProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductController> _logger; // Thêm biến _logger

        public ProductController(AppDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<ProductController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        public IActionResult Index(string searchTerm, int page = 1)
        {
            int pageSize = 5;

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(searchTerm));
            }

            int totalProducts = productsQuery.Count();
            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var products = productsQuery
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(new ProductIndexViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchTerm = searchTerm
            });
        }






        private void LoadData()
        {
            // Get all categories from the database
            var categories = _context.Category.ToList();

            // Convert to SelectList to use in the dropdown
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
        }











        [HttpGet]
        public IActionResult Create()
        {
            // Fetch categories from the database
            var categories = _context.Category.ToList();

            // Create a SelectList for the dropdown
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            return View();
        }






        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid");

                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogError($"ModelState error for {key}: {error.ErrorMessage}");
                    }
                }

                // Gọi lại hàm LoadData để nạp lại SelectList vào ViewData
                LoadData();

                return View(model);
            }

            _logger.LogInformation("ModelState is valid");

            // Xử lý upload file hình ảnh
            string uniqueFileName = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/products");
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    _logger.LogInformation($"Image uploaded successfully: {uniqueFileName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error uploading file: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Error uploading image.");
                    return View(model);
                }
            }
            else
            {
                _logger.LogWarning("No image uploaded.");
            }

            // Tạo sản phẩm
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Image = uniqueFileName, // Null nếu không có hình ảnh
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.Now,
                CreatedBy = "admin" // Tạm thời sử dụng "admin"
            };

            try
            {
                _context.Products.Add(product);
                _logger.LogInformation($"Entity state: {_context.Entry(product).State}"); // Phải là 'Added'

                var affected = await _context.SaveChangesAsync();
                _logger.LogInformation($"Rows affected: {affected}");

                // Redirect đến trang Index nếu lưu thành công
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving product: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error saving product.");
                LoadData(); // 🔥 Đừng quên đây nữa
                return View(model);
            }
        }
















        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Stock = product.Stock,
                Image = product.Image,
                CategoryId = product.CategoryId
            };

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.Products.Find(model.Id);
                if (existing == null)
                    return NotFound();

                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    existing.Image = uniqueFileName;

                }

                existing.Name = model.Name;
                existing.Price = model.Price;
                existing.Description = model.Description;
                existing.Stock = model.Stock;
                existing.CategoryId = model.CategoryId;

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", model.CategoryId);
            return View(model);
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
                try
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Ghi log nếu cần: _logger.LogError(ex, "Error deleting product");

                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm này vì nó đang được sử dụng trong các đơn hàng hoặc bảng liên quan.";
                    return RedirectToAction("Index");
                }
            }

            TempData["SuccessMessage"] = "Xóa sản phẩm thành công.";
            return RedirectToAction("Index");
        }




    }

}
