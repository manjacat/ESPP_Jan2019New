using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class BantuanJenazahController : Controller
    {
        // GET: BantuanJenazah
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PengesahanKematian()
        {
            return View();
        }

        public ActionResult WangPanjar()
        {
            return View();
        }
    }
}