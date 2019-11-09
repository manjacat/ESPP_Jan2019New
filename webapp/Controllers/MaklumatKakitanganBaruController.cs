using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class MaklumatKakitanganBaruController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: MaklumatKakitanganBaru
        public ActionResult Index(string key, string value, string status, int? i)
        {
            var username = User.Identity.Name;
            string pekerjaLogin = null;
            HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == username);
            if (pekerja != null)
            {
                pekerjaLogin = pekerja.HR_NO_PEKERJA;
            }
            ViewBag.pekerjaLogin = pekerjaLogin;

            HR_MAKLUMAT_PERIBADI Peribadi = new HR_MAKLUMAT_PERIBADI();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = Peribadi.CariPekerja(key, value, status);

            return View(mPeribadi);
        }
    }
}