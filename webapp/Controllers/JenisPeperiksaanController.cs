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
    public class JenisPeperiksaanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: JenisPeperiksaan
        public ActionResult SenaraiPelbagai()
        {
            return View(db.HR_JENIS_PEPERIKSAAN.ToList());
        }

        public ActionResult InfoJenisPeperiksaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_JENIS_PEPERIKSAAN Jenis = db.HR_JENIS_PEPERIKSAAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_JENIS_PEPERIKSAAN = db.HR_JENIS_PEPERIKSAAN.ToList();
            return PartialView("_InfoJenisPeperiksaan", Jenis);
        }

        public ActionResult TambahJenisPeperiksaan()
        {

            ViewBag.HR_JENIS_PEPERIKSAAN = db.HR_JENIS_PEPERIKSAAN.ToList();
            return PartialView("_TambahJenisPeperiksaan");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahJenisPeperiksaan([Bind(Include = "HR_KOD_PEPERIKSAAN,HR_KETERANGAN,HR_SINGKATAN")] HR_JENIS_PEPERIKSAAN Jenis)
        {
            if (ModelState.IsValid)
            {
            List<HR_JENIS_PEPERIKSAAN> selectJenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KOD_PEPERIKSAAN == Jenis.HR_KOD_PEPERIKSAAN).ToList();
            if (selectJenis.Count() <= 0)
            {
                db.HR_JENIS_PEPERIKSAAN.Add(Jenis);
                db.SaveChanges();
            }
            return RedirectToAction("SenaraiPelbagai");
        }
            return View();
         }


    public ActionResult EditJenisPeperiksaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JENIS_PEPERIKSAAN Jenis = db.HR_JENIS_PEPERIKSAAN.Find(id);
            if (Jenis == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditJenisPeperiksaan", Jenis);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJenisPeperiksaan([Bind(Include = "HR_KOD_PEPERIKSAAN,HR_KETERANGAN,HR_SINGKATAN")] HR_JENIS_PEPERIKSAAN Jenis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Jenis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPelbagai");
            }
            return View(Jenis);
        }

        public ActionResult PadamJenisPeperiksaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JENIS_PEPERIKSAAN Jenis = db.HR_JENIS_PEPERIKSAAN.Find(id);
            if (Jenis == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PadamJenisPeperiksaan", Jenis);
        }

        // POST: JenisPeperiksaan/Delete/5
        [HttpPost, ActionName("PadamJenisPeperiksaan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_JENIS_PEPERIKSAAN Jenis)
        {
            Jenis = db.HR_JENIS_PEPERIKSAAN.SingleOrDefault(s => s.HR_KOD_PEPERIKSAAN == Jenis.HR_KOD_PEPERIKSAAN);

            db.HR_JENIS_PEPERIKSAAN.Remove(Jenis);
            db.SaveChanges();
            return RedirectToAction("SenaraiPelbagai");
        }
        
        public ActionResult CariPeperiksaan ( string kod, string singkatan, string keterangan)
        {
            List<HR_JENIS_PEPERIKSAAN> Jenis = new List<HR_JENIS_PEPERIKSAAN>();
            if ( kod != null)
            {
                Jenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KOD_PEPERIKSAAN == kod).ToList();
            }
            if ( singkatan != null)
            {
                Jenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KOD_PEPERIKSAAN == kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if ( keterangan != null )
            {
                Jenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (Jenis.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditPeperiksaan ( string kod, string singkatan, string keterangan)
        {
            List<HR_JENIS_PEPERIKSAAN> Jenis = new List<HR_JENIS_PEPERIKSAAN>();
            if ( singkatan != null)
            {
                Jenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KOD_PEPERIKSAAN != kod &&  s.HR_SINGKATAN == singkatan).ToList();
            }
            if( keterangan != null)
            {
                Jenis = db.HR_JENIS_PEPERIKSAAN.Where(s => s.HR_KOD_PEPERIKSAAN != kod && s.HR_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (Jenis.Count() > 0)
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
