using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class LaporanController : Controller
    {
        // GET: Laporan
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuratPengesahanHospital()
        {
            return View();
        }
        public ActionResult TawaranLantikan()
        {
            return View();
        }
        public ActionResult PengesahanJawatan()
        {
            return View();
        }
        public ActionResult LaporanTuntutanPerjalanan()
        {
            return View();
        }
        public ActionResult PenyedianLaporan()
        {
            return View();
        }
        public ActionResult LaporanAhliMajlis()
        {
            return View();
        }
    }
}