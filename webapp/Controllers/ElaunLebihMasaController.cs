using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;

namespace eSPP.Controllers
{
    public class ElaunLebihMasaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: ElaunLebihMasa
        public ActionResult SenaraiLebihMasa()
        {

            ViewBag.HR_JENIS_WAKTU = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120).ToList();
            ViewBag.HR_JENIS_HARI = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119).ToList();
            return View(db.HR_KADAR_ELAUN_LEBIHMASA.ToList());
        }

        public ActionResult InfoElaunLebihMasa(string hari, string waktu)
        {
            if (hari == null || waktu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.SingleOrDefault(s => s.HR_JENIS_HARI == hari && s.HR_JENIS_WAKTU == waktu);


            if (elaunLebihMasa == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_HARI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JENIS_WAKTU = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_InfoElaunLebihMasa", elaunLebihMasa);
        }

        public ActionResult TambahElaunLebihMasa()
        {

            ViewBag.HR_JENIS_HARI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JENIS_WAKTU = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_TambahElaunLebihMasa");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahElaunLebihMasa([Bind(Include = "HR_JENIS_HARI,HR_JENIS_WAKTU,HR_KADAR_SEJAM,HR_AKTIF_IND")] HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa)
        {

            if (ModelState.IsValid)
            {
                List<HR_KADAR_ELAUN_LEBIHMASA> selectElaun = db.HR_KADAR_ELAUN_LEBIHMASA.Where(s => s.HR_JENIS_HARI == elaunLebihMasa.HR_JENIS_HARI && s.HR_JENIS_WAKTU == elaunLebihMasa.HR_JENIS_WAKTU).ToList();
                if (selectElaun.Count() <= 0)
                {
                    db.HR_KADAR_ELAUN_LEBIHMASA.Add(elaunLebihMasa);
                    db.SaveChanges();
                }
                return RedirectToAction("SenaraiLebihMasa");
            }

            return View(elaunLebihMasa);
        }

        public ActionResult EditElaunLebihMasa(string hari, string waktu)
        {
            if (hari == null || waktu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.SingleOrDefault(s => s.HR_JENIS_HARI == hari && s.HR_JENIS_WAKTU == waktu);


            if (elaunLebihMasa == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_HARI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JENIS_WAKTU = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_EditElaunLebihMasa", elaunLebihMasa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditElaunLebihMasa([Bind(Include = "HR_JENIS_HARI,HR_JENIS_WAKTU,HR_KADAR_SEJAM,HR_AKTIF_IND")] HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(elaunLebihMasa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiLebihMasa");
            }
            ViewBag.HR_JENIS_HARI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JENIS_WAKTU = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(elaunLebihMasa);
        }

        
        public ActionResult PadamElaunLebihMasa(string hari, string waktu)
        {
            if (hari == null || waktu == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.SingleOrDefault(s => s.HR_JENIS_HARI == hari && s.HR_JENIS_WAKTU == waktu);


            if (elaunLebihMasa == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_HARI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 119), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JENIS_WAKTU = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 120), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_PadamElaunLebihMasa", elaunLebihMasa);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamElaunLebihMasa")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KADAR_ELAUN_LEBIHMASA elaunLebihMasa)
        {

            elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.SingleOrDefault(s => s.HR_JENIS_HARI == elaunLebihMasa.HR_JENIS_HARI && s.HR_JENIS_WAKTU == elaunLebihMasa.HR_JENIS_WAKTU);

            db.HR_KADAR_ELAUN_LEBIHMASA.Remove(elaunLebihMasa);
            db.SaveChanges();
            return RedirectToAction("SenaraiLebihMasa");
        }

        public ActionResult CariElaun(string hari, string waktu)
        {
            List<HR_KADAR_ELAUN_LEBIHMASA> elaunLebihMasa = new List<HR_KADAR_ELAUN_LEBIHMASA>();
            if (hari != null)
            {
                elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.Where(s => s.HR_JENIS_HARI == hari && s.HR_JENIS_WAKTU == waktu).ToList();
            }
            if (waktu != null)
            {
                elaunLebihMasa = db.HR_KADAR_ELAUN_LEBIHMASA.Where(s => s.HR_JENIS_WAKTU == waktu && s.HR_JENIS_HARI == hari).ToList();
            }
            string msg = null;
            if (elaunLebihMasa.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}

