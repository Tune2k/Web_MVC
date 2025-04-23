using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhamTranXuanTan_2122110248.Models;
namespace PhamTranXuanTan_2122110248.Controllers
{
    public class CartController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities = new ECommerceDBEntities1();
        // GET: Cart
        public ActionResult Cart()
        {
            return View((List<CartModel>)Session["cart"]);
        }
        public ActionResult AddToCart(int Id, int quantity)
        {
            if (Session["cart"] == null)
            {
                List<CartModel> cart = new List<CartModel>();
                cart.Add(new CartModel { product = objECommerceDBEntities.products.Find(Id), Quantity = quantity });
                Session["cart"] = cart;
                Session["count"] = 1;
            }
            else
            {
                List<CartModel> cart = (List<CartModel>)Session["cart"];
                //kiểm tra sản phẩm có tồn tại trong giỏ hàng chưa???
                int index = isExist(Id);
                if (index != -1)
                {
                    //nếu sp tồn tại trong giỏ hàng thì cộng thêm số lượng
                    cart[index].Quantity += quantity;
                }
                else
                {
                    // nếu không tồn tại thì thêm sản phẩm vào giỏ hàng
                    cart.Add(new CartModel { product = objECommerceDBEntities.products.Find(Id), Quantity = quantity });
                    //Tính lại số sản phẩm trong giỏ hàng
                    Session["count"] = Convert.ToInt32(Session["count"]) + 1;
                }
                Session["cart"] = cart;
            }
            return Json(new { message = "Đã thêm sản phẩm vào giỏ hàng" }, JsonRequestBehavior.AllowGet);
        }

        private int isExist(int id)
        {
            List<CartModel> cart = (List<CartModel>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].product.id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        public ActionResult Remove(int Id)
        {
            List<CartModel> li = (List<CartModel>)Session["cart"];
            li.RemoveAll(x => x.product.id == Id);
            Session["cart"] = li;
            Session["count"] = Convert.ToInt32(Session["count"]) - 1;
            return Json(new { message = "Đã xóa sản phẩm khỏi giỏ hàng" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckOut()
        {
            return View();
        }
    }
}