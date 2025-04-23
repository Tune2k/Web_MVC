using PagedList;
using PhamTranXuanTan_2122110248.Context;
using PhamTranXuanTan_2122110248.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhamTranXuanTan_2122110248.Areas.Admin.Controllers
{
    public class orderController : Controller
    {
        // GET: Admin/order
        ECommerceDBEntities1 objECommerceDBEntities1 = new ECommerceDBEntities1();
        // GET: Admin/User

        private ECommerceDBEntities1 context;
        public orderController()
        {
            context = new ECommerceDBEntities1();
        }
        // GET: Admin/order
        public ActionResult Index(string Search, string currentFilter, int? page)
        {
            var listorder = new List<order>();
            if (Search != null)
            {
                page = 1;
            }
            else
            {
                Search = currentFilter;
            }
            if (!String.IsNullOrEmpty(Search))
            {
                listorder = objECommerceDBEntities1.orders.Where(n => n.name.Contains(Search)).ToList();
            }
            else
            {
                listorder = objECommerceDBEntities1.orders.ToList();
            }
            ViewBag.CurrentFilter = Search;
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            listorder = listorder.OrderByDescending(n => n.id).ToList();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_orderList", listorder.ToPagedList(pageNumber, pageSize));
            }

            return View(listorder.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public JsonResult UpdateStatus(int id, string status)
        {
            try
            {
                // Tìm đơn hàng theo Id
                var order = context.orders.FirstOrDefault(o => o.id == id);
                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại." });
                }

                // Cập nhật trạng thái
                order.status = status;
                order.created_at = DateTime.Now;

                // Lưu thay đổi
                context.SaveChanges();

                // Trả về phản hồi JSON kèm thông báo
                return Json(new { success = true, message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                // Trả về phản hồi lỗi kèm thông báo ngoại lệ
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            var order = objECommerceDBEntities1.orders.Where(n => n.id == id).FirstOrDefault();
            return View(order);
        }

        [HttpPost]
        public ActionResult Delete(order order)
        {
            try
            {
                var orderToDelete = objECommerceDBEntities1.orders.Where(n => n.id == order.id).FirstOrDefault();
                objECommerceDBEntities1.orders.Remove(orderToDelete);
                objECommerceDBEntities1.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error occurred while deleting the order.");
            }
            return View(order);
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Tìm đơn hàng theo ID
            var order = context.orders.FirstOrDefault(o => o.id == id);
            if (order == null)
            {
                return HttpNotFound("Đơn hàng không tồn tại.");
            }

            // Lấy danh sách chi tiết đơn hàng liên quan đến đơn hàng này
            var orderDetails = context.order_details
                .Where(od => od.order_id == id)
                .Select(od => new OrderDetailInfo
                {
                    ProductId = od.product_id,
                    ProductName = context.products
                        .Where(p => p.id == od.product_id)
                        .Select(p => p.name)
                        .FirstOrDefault(),
                    Quantity = od.quantity,
                    Price = context.products
                        .Where(p => p.id == od.product_id)
                        .Select(p => p.price) // Lấy giá từ bảng products
                        .FirstOrDefault(),
                    TotalPrice = od.quantity * (context.products
                        .Where(p => p.id == od.product_id)
                        .Select(p => p.price)
                        .FirstOrDefault()) // Tính toán tổng tiền = Số lượng * Giá
                })
                .ToList();

            // Tạo ViewModel để truyền dữ liệu sang View
            var model = new OrderDetailViewModel
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(model);
        }


    }

}
