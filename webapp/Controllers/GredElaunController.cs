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
    public class GredElaunController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: GredElaun
        public ActionResult SenaraiGredElaun()
        {
            ViewBag.HR_GRED = (db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList());
            ViewBag.HR_KOD_ELAUN = db.HR_ELAUN.ToList();
            return View(db.HR_GRED_ELAUN.ToList());
        }

        public ActionResult InfoGredElaun(decimal? id, string value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GRED_ELAUN gredElaun = db.HR_GRED_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == value && s.HR_GRED == id);

            if (gredElaun == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM"), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_ELAUN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            return PartialView("_InfoGredElaun", gredElaun);
        }

        public ActionResult TambahGredElaun()
        {
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM"), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_ELAUN = new SelectList (db.HR_ELAUN,"HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            return PartialView("_TambahGredElaun");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahGredElaun([Bind(Include = "HR_GRED,HR_KOD_ELAUN,HR_AKTIF_IND")] HR_GRED_ELAUN gredElaun)
        {
            if (ModelState.IsValid)
            {
                db.HR_GRED_ELAUN.Add(gredElaun);
                db.SaveChanges();
                return RedirectToAction("SenaraiGredElaun");
            }

            return View(gredElaun);
        }

        public ActionResult EditGredElaun(decimal? id, string value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GRED_ELAUN gredElaun = db.HR_GRED_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == value && s.HR_GRED == id);

            if (gredElaun == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM"), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_ELAUN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            return PartialView("_EditGredElaun",gredElaun);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGredElaun([Bind(Include = "HR_GRED,HR_KOD_ELAUN,HR_AKTIF_IND")] HR_GRED_ELAUN gredElaun, decimal gred, string kod_elaun)
        {
            if (ModelState.IsValid)
            {
                db.HR_GRED_ELAUN.RemoveRange(db.HR_GRED_ELAUN.Where(s => s.HR_GRED == gred && s.HR_KOD_ELAUN == kod_elaun));
                //db.Entry(gredElaun).State = EntityState.Modified;
                db.HR_GRED_ELAUN.Add(gredElaun);
                db.SaveChanges();
                return RedirectToAction("SenaraiGredElaun");
            }
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_ELAUN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            return View(gredElaun);
        }

        // GET: GredElaun/Delete/5
        public ActionResult PadamGredElaun(decimal? id, string value)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GRED_ELAUN gredElaun = db.HR_GRED_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == value && s.HR_GRED == id);

            if (gredElaun == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM"), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_ELAUN = new SelectList(db.HR_ELAUN, "HR_KOD_ELAUN", "HR_PENERANGAN_ELAUN");
            return PartialView("_PadamGredElaun",gredElaun);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamGredElaun")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_GRED_ELAUN gredElaun)
        {
            gredElaun = db.HR_GRED_ELAUN.SingleOrDefault(s => s.HR_GRED == gredElaun.HR_GRED && s.HR_KOD_ELAUN == gredElaun.HR_KOD_ELAUN);

            db.HR_GRED_ELAUN.Remove(gredElaun);
            db.SaveChanges();
            return RedirectToAction("SenaraiGredElaun");
        }

        public ActionResult CariElaun (decimal? gred, string elaun)
        {
            List<HR_GRED_ELAUN> gredElaun = new List<HR_GRED_ELAUN>();
            if ( gred != null)
            {
                gredElaun = db.HR_GRED_ELAUN.Where(s => s.HR_GRED == gred && s.HR_KOD_ELAUN == elaun).ToList();
            }
            if ( elaun != null)
            {
                gredElaun = db.HR_GRED_ELAUN.Where(s => s.HR_KOD_ELAUN == elaun && s.HR_GRED == gred).ToList();
            }
            string msg = null;
            if (gredElaun.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditElaun(string elaun, decimal gred)
        {
            List<HR_GRED_ELAUN> gredElaun = new List<HR_GRED_ELAUN>();
            
            if (elaun != null)
            {
                gredElaun = db.HR_GRED_ELAUN.Where(s => s.HR_GRED != gred && s.HR_KOD_ELAUN == elaun).ToList();
            }
            string msg = null;
            if (gredElaun.Count() > 0)
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
