using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using TranNhatTu_2122110250.ViewModels;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;
using TranNhatTu_2122110250.Helpers;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
                                   .Include(p => p.Category) // Load danh mục liên quan
                                   .ToList();

            return View(new ProductIndexViewModel
            {
                Products = products
            });
        }





        private void LoadData()
        {
            // Lấy danh sách Category từ cơ sở dữ liệu
            var categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            // Thêm tùy chọn "Chọn danh mục" vào đầu danh sách
            categories.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Chọn danh mục --",
                Selected = true
            });

            // Cập nhật danh sách danh mục vào ViewBag
            ViewBag.ListCategory = categories;

            // Tương tự nếu bạn có danh sách thương hiệu
            var brands = _context.Brands
                .Select(b => new SelectListItem
                {
                    Value = b.id.ToString(),
                    Text = b.name
                }).ToList();

            // Cập nhật danh sách thương hiệu vào ViewBag (nếu có)
            ViewBag.ListBrand = brands;
        }




        [HttpGet]
        public IActionResult Create()
        {
            LoadData();  // Gọi LoadData để lấy danh sách danh mục và thương hiệu

            var vm = new ProductCreateViewModel
            {
                Product = new Product()  // Khởi tạo đối tượng Product mới
            };

            return View(vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductCreateViewModel viewModel, IFormFile ImageUpload)
        {
            LoadData();
            var product = viewModel.Product;

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý ảnh nếu có
                    if (ImageUpload != null && ImageUpload.Length > 0)
                    {
                        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

                        // Kiểm tra xem thư mục có tồn tại không, nếu không thì tạo
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        // Kiểm tra quyền ghi thư mục
                        if (!FilePermissionHelper.HasWritePermission(folderPath))
                        {
                            ModelState.AddModelError("", "Không có quyền ghi vào thư mục lưu ảnh: " + folderPath);
                            return View(viewModel);
                        }

                        // Tạo tên file duy nhất cho ảnh
                        string uniqueFileName = Guid.NewGuid() + "_" + Path.GetFileName(ImageUpload.FileName);
                        string fullPath = Path.Combine(folderPath, uniqueFileName);

                        // Lưu ảnh vào thư mục
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            ImageUpload.CopyTo(stream);
                        }

                        // ✅ Chỉ lưu tên file ảnh vào DB (không lưu đường dẫn)
                        product.Image = uniqueFileName;
                    }

                    // Thiết lập thông tin khác cho sản phẩm
                    product.CreatedDate = DateTime.Now;

                    // Lấy thông tin danh mục từ DB
                    var category = _context.Category.FirstOrDefault(c => c.Id == product.CategoryId);
                    if (category != null)
                    {
                        product.Category_name = category.Name;
                    }
                    else
                    {
                        viewModel.CategoryLoadError = "Danh mục không tồn tại.";
                        return View(viewModel);
                    }

                    // Thêm sản phẩm vào cơ sở dữ liệu
                    _context.Products.Add(product);
                    _context.SaveChanges();

                    // Chuyển hướng về trang danh sách sản phẩm
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu sản phẩm: " + ex.Message);
                }
            }

            return View(viewModel);
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
                existing.Price = product.Price;
                existing.Description = product.Description;
                existing.Stock = product.Stock;
                existing.CategoryId = product.CategoryId;
                existing.Category_name = product.Category_name;

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
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }



    }

}
