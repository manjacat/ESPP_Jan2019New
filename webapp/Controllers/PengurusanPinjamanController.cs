using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class PengurusanPinjamanController : Controller
    {
        // GET: PengurusanPinjaman
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Kenderaan()
        {
            return View();
        }
        public ActionResult Komputer()
        {
            return View();
        }
        public ActionResult Perumahan()
        {
            return View();
        }
    }
}