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
    public class MatapelajaranController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Matapelajaran
        public ActionResult SenaraiMatapelajaran()
        {
           
            ViewBag.HR_KOD_PEPERIKSAAN = db.HR_JENIS_PEPERIKSAAN.ToList();
            

            return View(db.HR_MATAPELAJARAN.ToList());
        }

        public ActionResult InfoMatapelajaran(string id, string kod)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            HR_MATAPELAJARAN matapelajaran = db.HR_MATAPELAJARAN.SingleOrDefault(s => s.HR_KOD_MATAPELAJARAN == id && s.HR_KOD_PEPERIKSAAN == kod);
            if (matapelajaran == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return PartialView("_InfoMatapelajaran", matapelajaran);
        }

        public ActionResult TambahMatapelajaran()
        {
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");

           
            return PartialView("_TambahMatapelajaran");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahMatapelajaran([Bind(Include = "HR_KOD_MATAPELAJARAN,HR_KETERANGAN,HR_SINGKATAN, HR_KOD_PEPERIKSAAN")] HR_MATAPELAJARAN matapelajaran)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_MATAPELAJARAN.OrderByDescending(s => s.HR_KOD_MATAPELAJARAN).FirstOrDefault().HR_KOD_MATAPELAJARAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'M' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodMatapelajaran = Convert.ToString(Increment).PadLeft(4, '0');
                matapelajaran.HR_KOD_MATAPELAJARAN = "M" + KodMatapelajaran;

                db.HR_MATAPELAJARAN.Add(matapelajaran);
                db.SaveChanges();
                return RedirectToAction("SenaraiMatapelajaran");
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return View(matapelajaran);
        }

        public ActionResult EditMatapelajaran(string id, string kod)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           
            HR_MATAPELAJARAN matapelajaran = db.HR_MATAPELAJARAN.SingleOrDefault(s => s.HR_KOD_MATAPELAJARAN == id && s.HR_KOD_PEPERIKSAAN == kod);
            if (matapelajaran == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return PartialView("_EditMatapelajaran", matapelajaran);
        }

   


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMatapelajaran([Bind(Include = "HR_KOD_MATAPELAJARAN,HR_KETERANGAN,HR_SINGKATAN, HR_KOD_PEPERIKSAAN")] HR_MATAPELAJARAN matapelajaran)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matapelajaran).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiMatapelajaran");
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return View(matapelajaran);
        }




        public ActionResult PadamMatapelajaran(string id, string kod)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MATAPELAJARAN matapelajaran = db.HR_MATAPELAJARAN.SingleOrDefault(s => s.HR_KOD_MATAPELAJARAN == id && s.HR_KOD_PEPERIKSAAN == kod);
            if (matapelajaran == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return PartialView("_PadamMatapelajaran", matapelajaran);
        }

         // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamMatapelajaran")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_MATAPELAJARAN matapelajaran)
        {
            matapelajaran = db.HR_MATAPELAJARAN.SingleOrDefault(s => s.HR_KOD_MATAPELAJARAN == matapelajaran.HR_KOD_MATAPELAJARAN);

            db.HR_MATAPELAJARAN.Remove(matapelajaran);
            db.SaveChanges();
            return RedirectToAction("SenaraiMatapelajaran");
        }

        public ActionResult CariMatapelajaran (string kod, string kodmata, string singkatan, string keterangan)
        {
            List<HR_MATAPELAJARAN> matapelajaran = new List<HR_MATAPELAJARAN>();
            
            if (kod != null)
            {
                matapelajaran = db.HR_MATAPELAJARAN.Where(s => s.HR_KOD_PEPERIKSAAN == kod && s.HR_KOD_MATAPELAJARAN == kodmata).ToList();
            }
            if (singkatan != null)
            {
                matapelajaran = db.HR_MATAPELAJARAN.Where(s => s.HR_SINGKATAN == singkatan).ToList();
            }
            if ( keterangan != null)
            {
                matapelajaran = db.HR_MATAPELAJARAN.Where(s =>  s.HR_KOD_PEPERIKSAAN == kod && s.HR_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (matapelajaran.Count() > 0)
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
