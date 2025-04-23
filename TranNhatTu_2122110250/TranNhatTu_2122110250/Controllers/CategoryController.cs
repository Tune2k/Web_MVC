using PhamTranXuanTan_2122110248.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhamTranXuanTan_2122110248.Controllers
{
    public class CategoryController : Controller
    {
        ECommerceDBEntities1 objECommerceDBEntities = new ECommerceDBEntities1();
        // GET: Category
        public ActionResult AllCategory()
        {
            var categories = objECommerceDBEntities.categories.ToList();
            int totalItems = categories.Count();
            ViewBag.TotalItems = totalItems; // Truyền tổng số sản phẩm về View
            return View(categories);
        }
    }
}