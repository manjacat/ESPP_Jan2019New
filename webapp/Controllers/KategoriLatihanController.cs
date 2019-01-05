using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class KategoriLatihanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KategoriLatihan
        public ActionResult SenaraiKategori()
        {
            return View(db.HR_KATEGORI_KURSUS.ToList());
        }

        public ActionResult InfoLatihan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KATEGORI_KURSUS latihan = db.HR_KATEGORI_KURSUS.Find(id);

            if (latihan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KATEGORI_KURSUS = db.HR_KATEGORI_KURSUS.ToList();
            return PartialView("_InfoLatihan", latihan);
        }

        public ActionResult TambahLatihan()
        {

            ViewBag.HR_KATEGORI_KURSUS = db.HR_KATEGORI_KURSUS.ToList();
            return PartialView("_TambahLatihan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahLatihan([Bind(Include = "HR_KOD_KATEGORI,HR_SINGKATAN,HR_PENERANGAN")] HR_KATEGORI_KURSUS latihan)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_KATEGORI_KURSUS.OrderByDescending(s => s.HR_KOD_KATEGORI).FirstOrDefault().HR_KOD_KATEGORI;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'K' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodLatihan = Convert.ToString(Increment).PadLeft(4, '0');
                latihan.HR_KOD_KATEGORI = "K" + KodLatihan;

                db.HR_KATEGORI_KURSUS.Add(latihan);
                db.SaveChanges();
                return RedirectToAction("SenaraiKategori");
            }

            return View(latihan);
        }

        public ActionResult EditLatihan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KATEGORI_KURSUS latihan = db.HR_KATEGORI_KURSUS.Find(id);
            if (latihan == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditLatihan", latihan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLatihan([Bind(Include = "HR_KOD_KATEGORI,HR_SINGKATAN,HR_PENERANGAN")] HR_KATEGORI_KURSUS latihan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(latihan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiKategori");
            }
            return View(latihan);
        }

        public ActionResult PadamLatihan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KATEGORI_KURSUS latihan = db.HR_KATEGORI_KURSUS.Find(id);
            if (latihan == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PadamLatihan", latihan);
        }

        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamLatihan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KATEGORI_KURSUS latihan)
        {
            latihan = db.HR_KATEGORI_KURSUS.SingleOrDefault(s => s.HR_KOD_KATEGORI == latihan.HR_KOD_KATEGORI);

            db.HR_KATEGORI_KURSUS.Remove(latihan);
            db.SaveChanges();
            return RedirectToAction("SenaraiKategori");
        }


        public ActionResult CariLatihan (string singkatan, string kod, string penerangan )
        {
            List<HR_KATEGORI_KURSUS> latihan = new List<HR_KATEGORI_KURSUS>();
            if (singkatan != null)
            {
                latihan = db.HR_KATEGORI_KURSUS.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if (penerangan !=null)
            {
                latihan = db.HR_KATEGORI_KURSUS.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (latihan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditLatihan(string singkatan, string kod, string penerangan)
        {
            List<HR_KATEGORI_KURSUS> latihan = new List<HR_KATEGORI_KURSUS>();
            if (singkatan != null)
            {
                latihan = db.HR_KATEGORI_KURSUS.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if (penerangan != null)
            {
                latihan = db.HR_KATEGORI_KURSUS.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (latihan.Count() > 0)
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