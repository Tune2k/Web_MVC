using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhamTranXuanTan_2122110248.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Main()
        {
            return View();
        }
        public ActionResult Address()
        {
            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }
    }
}