using System;
using System.Linq;
using System.Web.Mvc;
using PhamTranXuanTan_2122110248.Context;
using PhamTranXuanTan_2122110248.Models;
using PhamTranXuanTan_2122110248.Utils;
using System.Data.Entity;
namespace PhamTranXuanTan_2122110248.Controllers
{
    public class HomeController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities = new ECommerceDBEntities1();

        public ActionResult Index()
        {
            // Lấy danh sách sản phẩm mới, sắp xếp theo ngày tạo, chỉ lấy 3 sản phẩm mới nhất
            var products = objECommerceDBEntities.products
                            .OrderByDescending(p => p.created_at)
                            .Take(3)
                            .ToList();

            // Lấy danh sách sản phẩm đề xuất, sắp xếp theo ngày tạo, chỉ lấy 10 sản phẩm mới nhất
            var productRecommended = objECommerceDBEntities.products
                            .OrderByDescending(p => p.created_at)
                            .Take(10)
                            .ToList();

            // Lấy danh mục cho từng sản phẩm đề xuất
            foreach (var product in productRecommended)
            {
                var category = objECommerceDBEntities.categories
                                 .FirstOrDefault(c => c.id == product.category_id);

                if (category != null)
                {
                    product.category_name = category.name;  // Gán tên danh mục cho sản phẩm
                }
            }

            var viewModel = new HomeViewModel
            {
                Categories = objECommerceDBEntities.categories.ToList(),
                Brands = objECommerceDBEntities.brands.ToList(),
                Product = products,
                ProductRecommended = productRecommended,
            };

            return View(viewModel);
        }


        public ActionResult Login(user user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = objECommerceDBEntities.users
                    .FirstOrDefault(u => u.username == user.username || u.email == user.email);

                if (userInDb != null)
                {
                    if (PasswordHelper.VerifyPassword(user.password, userInDb.password))
                    {
                        Session["UserID"] = userInDb.id;
                        Session["Username"] = userInDb.username;
                        TempData["SuccessMessage"] = "Đăng nhập thành công!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Mật khẩu không đúng!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Tài khoản không tồn tại!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu nhập không hợp lệ!";
            }

            return View(user);
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(user user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem email hoặc username đã tồn tại chưa
                    var existingUser = objECommerceDBEntities.users
                        .FirstOrDefault(u => u.email == user.email || u.username == user.username);

                    if (existingUser == null)
                    {
                        // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                        user.password = PasswordHelper.HashPassword(user.password);
                        user.created_at = DateTime.Now;

                        objECommerceDBEntities.users.Add(user);
                        objECommerceDBEntities.SaveChanges();

                        // Lưu thông báo vào TempData
                        TempData["SuccessMessage"] = "Đăng ký thành công!";

                        // Chuyển hướng người dùng đến trang Login sau khi đăng ký
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Email hoặc Tên người dùng đã tồn tại.";
                    }
                }
                catch (Exception)
                {
                    // Ghi log lỗi (nếu cần thiết)
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.";
            }

            return View(user);
        }
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa tất cả các session
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DealsAndOffers()
        {
            return View();
        }
    }
}
