using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace eSPP.Controllers
{
    public class ElaunPerjalananController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: ElaunTuntutanPerjalanan
        public ActionResult SenaraiKadarTuntutan()
        {
            return View(db.HR_KADAR_PERBATUAN.ToList());
        }


        public ActionResult InfoElaunPerjalanan(string kelas, decimal? mula)
        {
            if (kelas == null || mula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_PERBATUAN kadar = db.HR_KADAR_PERBATUAN.SingleOrDefault(s => s.HR_KELAS == kelas && s.HR_KM_MULA == mula);


            if (kadar == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("_InfoElaunPerjalanan", kadar);
        }

        public ActionResult TambahElaunPerjalanan()
        {

           
            return PartialView("_TambahElaunPerjalanan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahElaunPerjalanan([Bind(Include = "HR_KELAS,HR_KM_MULA,HR_KM_AKHIR,HR_NILAI,HR_AKTIF_IND")] HR_KADAR_PERBATUAN kadar)
        {

            if (ModelState.IsValid)
            {
                List<HR_KADAR_PERBATUAN> selectPerjalanan = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KELAS == kadar.HR_KELAS && s.HR_KM_MULA == kadar.HR_KM_MULA).ToList();
                if (selectPerjalanan.Count() <= 0)
                {
                    db.HR_KADAR_PERBATUAN.Add(kadar);
                    db.SaveChanges();
                }
                return RedirectToAction("SenaraiKadarTuntutan");
            }

            return View(kadar);
        }

       
        
        public ActionResult EditElaunPerjalanan(string kelas, decimal? mula)
        {
            if (kelas == null || mula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_PERBATUAN kadar = db.HR_KADAR_PERBATUAN.SingleOrDefault(s => s.HR_KELAS == kelas && s.HR_KM_MULA == mula);


            if (kadar == null)
            {
                return HttpNotFound();
            }
           
            return PartialView("_EditElaunPerjalanan", kadar);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditElaunPerjalanan([Bind(Include ="HR_KELAS,HR_KM_MULA,HR_KM_AKHIR,HR_NILAI,HR_AKTIF_IND")] HR_KADAR_PERBATUAN kadar)
        {
            if (ModelState.IsValid)
            {
              
                db.Entry(kadar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiKadarTuntutan");
            }

            return View(kadar);
        }


        public ActionResult PadamElaunPerjalanan(string kelas, decimal? mula)
        {
            if (kelas == null || mula == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_KADAR_PERBATUAN kadar = db.HR_KADAR_PERBATUAN.SingleOrDefault(s => s.HR_KELAS == kelas && s.HR_KM_MULA == mula);


            if (kadar == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PadamElaunPerjalanan", kadar);
        }


        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamElaunPerjalanan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KADAR_PERBATUAN kadar)
        {

            kadar = db.HR_KADAR_PERBATUAN.SingleOrDefault(s => s.HR_KELAS == kadar.HR_KELAS && s.HR_KM_MULA == kadar.HR_KM_MULA);

            db.HR_KADAR_PERBATUAN.Remove(kadar);
            db.SaveChanges();
            return RedirectToAction("SenaraiKadarTuntutan");
        }

        public ActionResult CariPerjalanan (string kelas, decimal? mula, decimal? akhir)
        {
            List<HR_KADAR_PERBATUAN> kadar = new List<HR_KADAR_PERBATUAN>();
            if (kelas != null)
            {
                kadar = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KELAS == kelas && s.HR_KM_MULA == mula).ToList();
            }
            if (mula != null)
            {
                kadar = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KM_MULA == mula && s.HR_KELAS == kelas).ToList();
            }
            if (akhir != null)
            {
                kadar = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KELAS == kelas && s.HR_KM_AKHIR == akhir).ToList();
            }
            string msg = null;
            if (kadar.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }



        public ActionResult CariEditPerjalanan(string kelas, decimal? akhir, decimal? mula)
        {
            List<HR_KADAR_PERBATUAN> kadar = new List<HR_KADAR_PERBATUAN>();
            
            if (akhir != null)
            {
                kadar = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KELAS != kelas && s.HR_KM_MULA != mula && s.HR_KM_AKHIR == akhir).ToList();
            }  
            string msg = null;
            if (kadar.Count() > 0)
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