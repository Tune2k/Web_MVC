using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TranNhatTu_2122110250.ViewModels;

namespace TranNhatTu_2122110250.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<ProductController> _logger;

        public ProductController(AppDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<ProductController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching products: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { errors });
            }

            string uniqueFileName = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
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
                    return StatusCode(500, new { message = "Error uploading image." });
                }
            }
            else
            {
                _logger.LogWarning("No image uploaded.");
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Image = uniqueFileName,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.Now,
                CreatedBy = "admin" // Consider using authenticated user
            };

            try
            {
                _context.Products.Add(product);
                _logger.LogInformation($"Entity state: {_context.Entry(product).State}");
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product created: {product.Name}");

                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error saving product: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi thêm sản phẩm." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // PUT: api/product/{id}
        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductEditViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning($"ID mismatch: URL ID {id} does not match model ID {model.Id}");
                return BadRequest(new { message = "ID không khớp." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid");
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { errors });
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning($"Product not found: {id}");
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            string uniqueFileName = null; // Initialize as null
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(existingProduct.Image))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, existingProduct.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                            _logger.LogInformation($"Deleted old image: {existingProduct.Image}");
                        }
                    }

                    _logger.LogInformation($"Image updated successfully: {uniqueFileName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error uploading file: {ex.Message}");
                    return StatusCode(500, new { message = "Error uploading image." });
                }
            }

            // Update product fields
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Price = model.Price;
            existingProduct.Stock = model.Stock;
            existingProduct.CategoryId = model.CategoryId;
            existingProduct.Image = uniqueFileName; // Will be null if no new image is uploaded

            try
            {
                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product updated: {existingProduct.Name}");
                return Ok(existingProduct);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error updating product: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi cập nhật sản phẩm." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Product not found: {id}");
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Product deleted: {product.Name}");

                // Delete associated image if it exists
                if (!string.IsNullOrEmpty(product.Image))
                {
                    string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images/products", product.Image);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"Deleted image: {product.Image}");
                    }
                }

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error deleting product: {ex.Message}");
                return Conflict(new { message = "Không thể xóa sản phẩm này vì nó đang được sử dụng ở bảng khác (ví dụ: đơn hàng)." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }
    }
}