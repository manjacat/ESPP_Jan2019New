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
    public class KategoriElaunController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KategoriElaun
        public ActionResult SenaraiKategori()
        {
            return View(db.HR_KATEGORI_ELAUN.ToList());
        }

        public ActionResult InfoKategori(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KATEGORI_ELAUN kategori = db.HR_KATEGORI_ELAUN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KATEGORI_ELAUN = db.HR_KATEGORI_ELAUN.ToList();
            return PartialView("_InfoKategori", kategori);
        }

        public ActionResult TambahKategori()
        {

            ViewBag.HR_KATEGORI_ELAUN = db.HR_KATEGORI_ELAUN.ToList();
            return PartialView("_TambahKategori");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKategori([Bind(Include = "HR_KOD_KATEGORI,HR_PENERANGAN,HR_SINGKATAN")] HR_KATEGORI_ELAUN kategori)
        {   
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_KATEGORI_ELAUN.OrderByDescending(s => s.HR_KOD_KATEGORI).FirstOrDefault().HR_KOD_KATEGORI;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'K' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodKategoriElaun = Convert.ToString(Increment).PadLeft(4, '0');
                kategori.HR_KOD_KATEGORI = "K" + KodKategoriElaun;

                db.HR_KATEGORI_ELAUN.Add(kategori);
                db.SaveChanges();
                return RedirectToAction("SenaraiKategori");
            }
           
            return View(kategori);
        }

        public ActionResult EditKategori(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KATEGORI_ELAUN kategori = db.HR_KATEGORI_ELAUN.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditKategori", kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKategori([Bind(Include = "HR_KOD_KATEGORI, HR_PENERANGAN, HR_SINGKATAN")] HR_KATEGORI_ELAUN kategori)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kategori).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiKategori");
            }
            return View(kategori);
        }

        public ActionResult PadamKategori(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KATEGORI_ELAUN kategori = db.HR_KATEGORI_ELAUN.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PadamKategori", kategori);
        }

        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamKategori")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KATEGORI_ELAUN kategori)
        {
            kategori = db.HR_KATEGORI_ELAUN.SingleOrDefault(s => s.HR_KOD_KATEGORI == kategori.HR_KOD_KATEGORI);

            db.HR_KATEGORI_ELAUN.Remove(kategori);
            db.SaveChanges();
            return RedirectToAction("SenaraiKategori");
        }

        
        public ActionResult CariKategori (string penerangan, string singkatan, string kod)
        {
            List<HR_KATEGORI_ELAUN> kategoriElaun = new List<HR_KATEGORI_ELAUN>();
            if ( penerangan !=null)
            {
                kategoriElaun = db.HR_KATEGORI_ELAUN.Where(s => s.HR_PENERANGAN == penerangan).ToList();
            }
            if ( singkatan != null)
            {
                kategoriElaun = db.HR_KATEGORI_ELAUN.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (kategoriElaun.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditKategori (string penerangan, string kod, string singkatan)
        {
            List<HR_KATEGORI_ELAUN> kategoriElaun = new List<HR_KATEGORI_ELAUN>();
            if ( penerangan != null)
            {
                kategoriElaun = db.HR_KATEGORI_ELAUN.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
                if (singkatan != null )
            {
                kategoriElaun = db.HR_KATEGORI_ELAUN.Where(s => s.HR_KOD_KATEGORI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (kategoriElaun.Count()>0)
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
