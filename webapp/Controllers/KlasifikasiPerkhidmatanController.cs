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
    public class KlasifikasiPerkhidmatanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KlasifikasiPerkhidamatan
        public ActionResult SenaraiKlasifikasi()
        {
            return View(db.HR_KLAS_PERKHIDMATAN.ToList());
        }

        public ActionResult InfoKlasifikasi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KLAS_PERKHIDMATAN klasifikasi = db.HR_KLAS_PERKHIDMATAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KLAS_PERKHIDMATAN = db.HR_KLAS_PERKHIDMATAN.ToList();
            return PartialView("_InfoKlasifikasi", klasifikasi);
        }

        public ActionResult TambahKlasifikasi()
        {

            ViewBag.HR_KLAS_PERKHIDMATAN = db.HR_KLAS_PERKHIDMATAN.ToList();
            return PartialView("_TambahKlasifikasi");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKlasifikasi([Bind(Include = "HR_GRED,HR_PENERANGAN")] HR_KLAS_PERKHIDMATAN klasifikasi)
        {
            if (ModelState.IsValid)
            {
                db.HR_KLAS_PERKHIDMATAN.Add(klasifikasi);
                db.SaveChanges();
                return RedirectToAction("SenaraiKlasifikasi");
            }

            return View(klasifikasi);

        }

        public ActionResult EditKlasifikasi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KLAS_PERKHIDMATAN klasifikasi = db.HR_KLAS_PERKHIDMATAN.Find(id);
            if (klasifikasi == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditKlasifikasi", klasifikasi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKlasifikasi([Bind(Include = "HR_GRED,HR_PENERANGAN")] HR_KLAS_PERKHIDMATAN klasifikasi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(klasifikasi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiKlasifikasi");
            }
            return View(klasifikasi);
        }

        public ActionResult PadamKlasifikasi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KLAS_PERKHIDMATAN klasifikasi = db.HR_KLAS_PERKHIDMATAN.Find(id);
            if (klasifikasi == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PadamKlasifikasi", klasifikasi);
        }

      
        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamKlasifikasi")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KLAS_PERKHIDMATAN klasifikasi)
        {
            klasifikasi = db.HR_KLAS_PERKHIDMATAN.SingleOrDefault(s => s.HR_GRED == klasifikasi.HR_GRED);

            db.HR_KLAS_PERKHIDMATAN.Remove(klasifikasi);
            db.SaveChanges();
            return RedirectToAction("SenaraiKlasifikasi");
        }

        public ActionResult CariKlasifikasi(string gred, string penerangan)
        {
            List<HR_KLAS_PERKHIDMATAN> klasifikasi = new List<HR_KLAS_PERKHIDMATAN>();
            if (gred != null)
            {
                klasifikasi = db.HR_KLAS_PERKHIDMATAN.Where(s => s.HR_GRED == gred ).ToList();
            }
            if (penerangan != null)
            {
                klasifikasi = db.HR_KLAS_PERKHIDMATAN.Where(s => s.HR_GRED != gred && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (klasifikasi.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditKlasifikasi(string gred, string penerangan)
        {
            List<HR_KLAS_PERKHIDMATAN> klasifikasi = new List<HR_KLAS_PERKHIDMATAN>();
            if (gred != null)
            {
                klasifikasi = db.HR_KLAS_PERKHIDMATAN.Where(s => s.HR_GRED == gred).ToList();
            }
            if (penerangan != null)
            {
                klasifikasi = db.HR_KLAS_PERKHIDMATAN.Where(s => s.HR_GRED != gred && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (klasifikasi.Count() > 0)
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
