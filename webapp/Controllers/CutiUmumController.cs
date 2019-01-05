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
    public class CutiUmumController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CutiUmum
        public ActionResult SenaraiCutiUmum()
        {
       
            ViewBag.HR_KOD_CUTI_UMUM = db.HR_CUTI.ToList();
            return View(db.HR_CUTI_UMUM.ToList());
        }


        public ActionResult InfoCuti(string id, string kod)
        {
            var date = Convert.ToDateTime(kod);
            if (id == null || kod == null )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_CUTI_UMUM cutiumum = db.HR_CUTI_UMUM.SingleOrDefault(s => s.HR_KOD_CUTI_UMUM == id && s.HR_TARIKH == date );

            if (cutiumum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_CUTI_UMUM = new SelectList(db.HR_CUTI, "HR_KOD_CUTI", "HR_KETERANGAN");
            return PartialView("_InfoCuti", cutiumum);
        }

        public ActionResult TambahCuti()
        {
            
            ViewBag.HR_KOD_CUTI_UMUM = new SelectList(db.HR_CUTI.Where(s  => s.HR_CUTI_IND == "C"), "HR_KOD_CUTI", "HR_KETERANGAN");
         
            return PartialView("_TambahCuti");
        }

   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahCuti(HR_CUTI_UMUM cutiumum)
        {
            if (ModelState.IsValid)
            {
                List<HR_CUTI_UMUM> selectCuti= db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == cutiumum.HR_KOD_CUTI_UMUM && s.HR_TARIKH == cutiumum.HR_TARIKH).ToList();
                if (selectCuti.Count() <= 0)
                {
                   
               
                    db.HR_CUTI_UMUM.Add(cutiumum);
                    db.SaveChanges();
                }
                return RedirectToAction("SenaraiCutiUmum");
            }
            return View(cutiumum);
        }

        public ActionResult EditCuti(string id, string kod)
        {
            var date = Convert.ToDateTime(kod);
            if (id == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_CUTI_UMUM cutiumum = db.HR_CUTI_UMUM.SingleOrDefault(s => s.HR_KOD_CUTI_UMUM == id && s.HR_TARIKH == date);

            if (cutiumum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_CUTI_UMUM = new SelectList(db.HR_CUTI, "HR_KOD_CUTI", "HR_KETERANGAN");
            return PartialView("_EditCuti", cutiumum);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCuti( HR_CUTI_UMUM cutiumum, string kod)
        {
            var date = Convert.ToDateTime(kod);
            if (ModelState.IsValid)
            {
                db.HR_CUTI_UMUM.RemoveRange(db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == cutiumum.HR_KOD_CUTI_UMUM && s.HR_TARIKH == date));
                db.HR_CUTI_UMUM.Add(cutiumum);
                //db.Entry(matapelajaran).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiCutiUmum");
            }
            ViewBag.HR_KOD_CUTI_UMUM = new SelectList(db.HR_CUTI, "HR_KOD_CUTI", "HR_KETERANGAN");
            return View(cutiumum);
        }


        public ActionResult PadamCuti(string id, string kod)
        {
            var date = Convert.ToDateTime(kod);
            if (id == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_CUTI_UMUM cutiumum = db.HR_CUTI_UMUM.SingleOrDefault(s => s.HR_KOD_CUTI_UMUM == id && s.HR_TARIKH == date);

            if (cutiumum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_CUTI_UMUM = new SelectList(db.HR_CUTI, "HR_KOD_CUTI", "HR_KETERANGAN");
            return PartialView("_PadamCuti", cutiumum);
        }


        [HttpPost, ActionName("PadamCuti")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_CUTI_UMUM cutiumum)
        {
            cutiumum = db.HR_CUTI_UMUM.SingleOrDefault(s => s.HR_KOD_CUTI_UMUM == cutiumum.HR_KOD_CUTI_UMUM && s.HR_TARIKH == cutiumum.HR_TARIKH);

            db.HR_CUTI_UMUM.Remove(cutiumum);
            db.SaveChanges();
            return RedirectToAction("SenaraiCutiUmum");
        }

        public ActionResult CariCuti (string kod, DateTime tarikh)
        {

         
            List<HR_CUTI_UMUM> cutiumum = db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == kod && s.HR_TARIKH == tarikh).ToList();
            string msg = null;
            if (cutiumum.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditCuti(string kod, string tarikh)
        {

            var tarikhCuti = Convert.ToDateTime(tarikh);
            List<HR_CUTI_UMUM> cutiumum = db.HR_CUTI_UMUM.Where(s => s.HR_KOD_CUTI_UMUM == kod && s.HR_TARIKH == tarikhCuti).ToList();
            string msg = null;
            if (cutiumum.Count() > 0)
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