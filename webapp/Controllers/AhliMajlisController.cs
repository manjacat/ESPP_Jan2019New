using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class AhliMajlisController : Controller
    {
        // GET: AhliMajlis
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MaklumatPeribadi()
        {
            return View();
        }
        public ActionResult NamaMensyuarat()
        {
            return View();
        }
        public ActionResult Jawatankuasa()
        {
            return View();
        }

        public ActionResult MaklumatJawatankuasa()
        {
            return View();
        }
        public ActionResult MaklumatPegawai()
        {
            return View();
        }
        public ActionResult KehadiranMensyuarat()
        {
            return View();
        }
        public ActionResult ProsesKelayakan()
        {
            return View();
        }
    }
}