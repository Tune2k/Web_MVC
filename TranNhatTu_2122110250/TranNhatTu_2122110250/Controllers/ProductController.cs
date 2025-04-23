using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace PhamTranXuanTan_2122110248.Controllers
{
    public class ProductController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities = new ECommerceDBEntities1();

        public ActionResult AllProductList(int categoryId, int? page)
        {
            ViewBag.CategoryId = categoryId;
            var products = objECommerceDBEntities.products.Where(p => p.category_id == categoryId).ToList();
            // Lấy tổng số lượng sản phẩm trước khi phân trang
            int totalItems = products.Count();
            ViewBag.TotalItems = totalItems; // Truyền tổng số sản phẩm về View
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        // GET: Product
        public ActionResult Detail(int id)
        {
            var product = objECommerceDBEntities.products.FirstOrDefault(p => p.id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult AllProductGrid(string search = "", int? page = 1)
        {
            // Lấy danh sách sản phẩm từ cơ sở dữ liệu
            var products = objECommerceDBEntities.products.AsQueryable();

            // Áp dụng bộ lọc tìm kiếm nếu từ khóa không rỗng
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p => p.name.Contains(search) || p.description.Contains(search));
            }
            // Lấy tổng số lượng sản phẩm trước khi phân trang
            int totalItems = products.Count();
            ViewBag.TotalItems = totalItems; // Truyền tổng số sản phẩm về View
            // Số lượng sản phẩm hiển thị trên mỗi trang
            int pageSize = 4; // Bạn có thể thay đổi số lượng sản phẩm trên mỗi trang tại đây
            int pageNumber = page ?? 1; // Trang hiện tại (mặc định là 1)

            // Sắp xếp sản phẩm theo `id` giảm dần
            products = products.OrderByDescending(p => p.id);

            // Truyền từ khóa tìm kiếm và danh sách sản phẩm đã phân trang về View
            ViewBag.SearchTerm = search; // Truyền từ khóa tìm kiếm về View
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DealsAndOffers()
        {
            // Lấy sản phẩm được tạo sớm nhất
            var earliestProduct = objECommerceDBEntities.products
                                    .OrderByDescending(p => p.created_at).Take(5)
                                    .FirstOrDefault();

            // Kiểm tra nếu không có sản phẩm nào
            if (earliestProduct == null)
            {
                return HttpNotFound("Không tìm thấy sản phẩm nào.");
            }

            // Trả về View
            return View(earliestProduct);
        }
        //public ActionResult AllProductGrid(string search = "", int? page = 1)
        //{
        //    // Lấy danh sách sản phẩm từ cơ sở dữ liệu
        //    var products = objECommerceDBEntities.products.AsQueryable();

        //    // Áp dụng bộ lọc tìm kiếm nếu từ khóa không rỗng
        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        products = products.Where(p => p.name.Contains(search) || p.description.Contains(search));
        //    }

        //    // Phân trang
        //    int pageSize = 10; // Số sản phẩm trên mỗi trang
        //    int pageNumber = page ?? 1;

        //    // Truyền danh sách sản phẩm và trả về View
        //    return View(products.OrderBy(p => p.name).ToPagedList(pageNumber, pageSize));
        //}

    }
}