using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhamTranXuanTan_2122110248.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities1 = new ECommerceDBEntities1();
        // GET: Admin/User
        public ActionResult Index()
        {
            var users = objECommerceDBEntities1.users.ToList();
            return View(users);
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            var details = objECommerceDBEntities1.users.Where(x => x.id == id).FirstOrDefault();
            return View(details);

        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var del = objECommerceDBEntities1.users.Where(x => x.id == id).FirstOrDefault();
            return View(del);

        }
        [HttpPost]
        public ActionResult Delete(user objUser)
        {
            try
            {
                var del = objECommerceDBEntities1.users.Where(x => x.id == objUser.id).FirstOrDefault();
                if (del != null)
                {
                    

                    // Xóa sản phẩm khỏi database
                    objECommerceDBEntities1.users.Remove(del);
                    objECommerceDBEntities1.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["LogMessage"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        

    }
}