using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using X.PagedList;
using System.Linq;
using X.PagedList.Extensions;

namespace TranNhatTu_2122110250.Controllers
{
    public class ProductViewController : Controller
    {
        private readonly AppDbContext _context;

        public ProductViewController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm dạng danh sách (list view)
        //public IActionResult AllProductList(int? page, int? categoryId)
        //{
        //    int pageSize = 6; // Số sản phẩm trên mỗi trang
        //    int pageNumber = page ?? 1; // Trang mặc định nếu không có tham số 'page'

        //    // Truy vấn sản phẩm, có thể lọc theo categoryId nếu có
        //    var productsQuery = _context.Products.AsQueryable();

        //    if (categoryId.HasValue)
        //    {
        //        productsQuery = productsQuery.Where(p => p.Category_id == categoryId);
        //    }

        //    // Sắp xếp sản phẩm theo Id (hoặc theo tên sản phẩm)
        //    var products = productsQuery
        //        .OrderByDescending(p => p.Id)  // Sắp xếp sản phẩm theo Id
        //        .ToPagedList(pageNumber, pageSize); // Phân trang

        //    // Truyền dữ liệu vào ViewBag
        //    ViewBag.TotalItems = productsQuery.Count();  // Tổng số sản phẩm không phân trang
        //    ViewBag.CategoryId = categoryId; // Lưu categoryId vào ViewBag để truyền vào View
        //    ViewBag.Categories = _context.Category.ToList(); // Lấy danh sách các danh mục

        //    return View(products); // Trả về view với danh sách sản phẩm đã phân trang
        //}
        public IActionResult AllProductList(int? categoryId)
        {
            var productsQuery = _context.Products.AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                productsQuery = productsQuery.Where(p => p.Category_id == categoryId);
            }

            var products = productsQuery.OrderByDescending(p => p.Id).ToList();

            return PartialView("_ProductListPartial", products); // Trả về Partial View
        }

        public IActionResult AllCategory()
        {
            var allCategories = _context.Category.ToList(); // Lấy tất cả các danh mục
            return View(allCategories); // Trả về view với danh sách danh mục
        }

        // Hiển thị sản phẩm dạng lưới (grid view)
        public IActionResult AllProductGrid(int? page, int? categoryId)
        {
            int pageSize = 6; // Số sản phẩm trên mỗi trang
            int pageNumber = page ?? 1; // Trang mặc định nếu không có tham số 'page'

            // Truy vấn sản phẩm, có thể lọc theo categoryId nếu có
            var productsQuery = _context.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Category_id == categoryId);
            }

            // Sắp xếp và phân trang
            var products = productsQuery
                .OrderByDescending(p => p.Id)  // Sắp xếp sản phẩm theo Id
                .ToPagedList(pageNumber, pageSize); // Phân trang

            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalItems = productsQuery.Count();  // Tổng số sản phẩm không phân trang
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = _context.Category.ToList(); // Lấy danh sách các danh mục

            return View(products); // Trả về view với sản phẩm dạng lưới đã phân trang
        }

        [HttpGet]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            List<Product> products;

            if (categoryId == 0)
            {
                products = _context.Products.ToList(); // tất cả sản phẩm
            }
            else
            {
                products = _context.Products
                                   .Where(p => p.Category_id == categoryId)
                                   .ToList();
            }

            return PartialView("_ProductListPartial", products);
        }
        // GET: ProductView/Details/5
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Trả về view "productdetail" thay vì "GetProductById"
            return View("ProductDetail", product);
        }
    }
}
