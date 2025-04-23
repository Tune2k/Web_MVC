using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhamTranXuanTan_2122110248.Context;
using PhamTranXuanTan_2122110248.Models;
namespace PhamTranXuanTan_2122110248.Controllers
{
    public class OrderController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities = new ECommerceDBEntities1();
        // GET: Order
        public ActionResult Order()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var lstCart = (List<CartModel>)Session["cart"];
                order objOrder = new order();
                objOrder.name = "DonHang-" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                objOrder.user_id = Convert.ToInt32(Session["UserID"].ToString());
                objOrder.created_at = DateTime.Now;
                objOrder.status = "1";
                objECommerceDBEntities.orders.Add(objOrder);
                objECommerceDBEntities.SaveChanges();
                int intOrderID = objOrder.id;
                List<order_details> lstOrderDetails = new List<order_details>();
                foreach (var item in lstCart)
                {
                    order_details objOrderDetails = new order_details();
                    objOrderDetails.quantity = item.Quantity;
                    objOrderDetails.order_id = intOrderID;
                    objOrderDetails.product_id = item.product.id;
                    lstOrderDetails.Add(objOrderDetails);
                }
                objECommerceDBEntities.order_details.AddRange(lstOrderDetails);
                objECommerceDBEntities.SaveChanges();
                Session["cart"] = null;
            }
            return View();
        }
    }
}