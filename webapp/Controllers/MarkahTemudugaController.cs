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
    public class MarkahTemudugaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        // GET: MarkahTemuduga
        public ActionResult SenaraiTemuduga()
        {
            ViewBag.HR_KOD_JENIS = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 122).ToList();
            return View(db.HR_SUBJEK.ToList());
        }

        public ActionResult InfoTemuduga(string jenis, short? kod)
        {
            if (jenis == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_SUBJEK temuduga = db.HR_SUBJEK.SingleOrDefault(s => s.HR_KOD_JENIS == jenis && s.HR_KOD_SUBJEK == kod);


            if (temuduga == null)
            {
                return HttpNotFound();
            }
          
            ViewBag.HR_KOD_JENIS = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 122), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_InfoTemuduga", temuduga);
        }

        public ActionResult TambahTemuduga()
        {

            
            ViewBag.HR_KOD_JENIS = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 122), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_TambahTemuduga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahTemuduga([Bind(Include = "HR_KOD_JENIS,HR_KOD_SUBJEK,HR_SUBJEK1,HR_MARKAH")] HR_SUBJEK temuduga)
        {

            if (ModelState.IsValid)
            {
                List<HR_SUBJEK> selectTemuduga = db.HR_SUBJEK.Where(s => s.HR_KOD_JENIS == temuduga.HR_KOD_JENIS && s.HR_KOD_SUBJEK == temuduga.HR_KOD_SUBJEK).ToList();
                if (selectTemuduga.Count() <= 0)
                {
                    var SelectLastID = db.HR_SUBJEK.OrderByDescending(s => s.HR_KOD_SUBJEK).FirstOrDefault().HR_KOD_SUBJEK;
                    var Increment = SelectLastID + 1 ;
                    temuduga.HR_KOD_SUBJEK = Convert.ToInt16(Increment);
                    

                    db.HR_SUBJEK.Add(temuduga);
                    db.SaveChanges();
                }
                return RedirectToAction("SenaraiTemuduga");
            }

            return View(temuduga);
        }


        public ActionResult EditTemuduga(string jenis, short? kod)
        {
            if (jenis == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_SUBJEK temuduga = db.HR_SUBJEK.SingleOrDefault(s => s.HR_KOD_JENIS == jenis && s.HR_KOD_SUBJEK == kod);


            if (temuduga == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_JENIS = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 122), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_EditTemuduga", temuduga);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTemuduga([Bind(Include = "HR_KOD_JENIS,HR_KOD_SUBJEK,HR_SUBJEK1,HR_MARKAH")] HR_SUBJEK temuduga)
        {
            if (ModelState.IsValid)
            {
                db.HR_SUBJEK.RemoveRange(db.HR_SUBJEK.Where(s =>  s.HR_KOD_SUBJEK == temuduga.HR_KOD_SUBJEK));
                db.HR_SUBJEK.Add(temuduga);
               //db.Entry(temuduga).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("SenaraiTemuduga");
            }
            return View(temuduga);
        }


        public ActionResult PadamTemuduga(string jenis, short? kod)
        {
            if (jenis == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_SUBJEK temuduga = db.HR_SUBJEK.SingleOrDefault(s => s.HR_KOD_JENIS == jenis && s.HR_KOD_SUBJEK == kod);


            if (temuduga == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_JENIS = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 122), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_PadamTemuduga", temuduga);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamTemuduga")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SUBJEK temuduga)
        {

            temuduga = db.HR_SUBJEK.SingleOrDefault(s => s.HR_KOD_SUBJEK == temuduga.HR_KOD_SUBJEK);

            db.HR_SUBJEK.Remove(temuduga);
            db.SaveChanges();
            return RedirectToAction("SenaraiTemuduga");
        }

        public ActionResult CariTemuduga(string kodjenis, short? kodsubjek, string subjek)
        {
            List<HR_SUBJEK> temuduga = new List<HR_SUBJEK>();
            if ( kodjenis != null)
            {
                temuduga = db.HR_SUBJEK.Where(s => s.HR_KOD_JENIS == kodjenis && s.HR_KOD_SUBJEK == kodsubjek).ToList();
            }
            if (subjek != null)
            {
                temuduga = db.HR_SUBJEK.Where(s => s.HR_SUBJEK1 == subjek).ToList();
            }
            string msg = null;
            if (temuduga.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditTemuduga(short? kod, string subjek)
        {
            List<HR_SUBJEK> temuduga = new List<HR_SUBJEK>();
           
            if (subjek != null)
            {
                temuduga = db.HR_SUBJEK.Where(s => s.HR_KOD_SUBJEK != kod &&  s.HR_SUBJEK1 == subjek).ToList();
            }
            string msg = null;
            if (temuduga.Count() > 0)
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