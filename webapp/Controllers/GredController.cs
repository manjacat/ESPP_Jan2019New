using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace eSPP.Controllers
{
    public class GredController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        // GET: Gred
        public ActionResult SenaraiGred()
        {
            return View(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList());
        }

        public ActionResult InfoGred(int? id)
        { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_PARAMTABLE gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == id);

            if (id == null)
            {
                return HttpNotFound();
            }
           
       
            return PartialView("_InfoGred",gred);
        }

       
        public ActionResult TambahGred(GE_PARAMTABLE gred)
        {
           
            gred.GROUPID = 109;
            gred.AUDIT_WHEN = DateTime.Now;
            ViewBag.GE_PARAMTABLE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            return PartialView("_TambahGred", gred);
        }

        [HttpPost, ActionName("TambahGred")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahGred2([Bind(Include = "GROUPID,ORDINAL,SHORT_DESCRIPTION,LONG_DESCRIPTION,STRING_PARAM,AUDIT_WHEN")] GE_PARAMTABLE gred)
        {
            if (ModelState.IsValid)
            {
                List<GE_PARAMTABLE> selectGred = db2.GE_PARAMTABLE.Where(s => s.SHORT_DESCRIPTION == gred.SHORT_DESCRIPTION && s.STRING_PARAM == gred.STRING_PARAM && s.GROUPID == 109 ).ToList();
                if(selectGred.Count() <= 0)
                {
                    int Increment = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).OrderByDescending(s => s.ORDINAL).FirstOrDefault().ORDINAL;
                    gred.LONG_DESCRIPTION = gred.SHORT_DESCRIPTION;
                    gred.AUDIT_WHEN = DateTime.Now;
                    gred.ORDINAL = Increment + 1;
                    db2.GE_PARAMTABLE.Add(gred);
                    db2.SaveChanges();
                }
                return RedirectToAction("SenaraiGred");
            }
            ViewBag.GE_PARAMTABLE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            return View(gred);
        }

        public ActionResult EditGred(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GE_PARAMTABLE gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID ==109 && s.ORDINAL == id) ;

            if (gred == null)
            {
                return HttpNotFound();
            }
            ViewBag.GE_PARAMTABLE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            return PartialView("_EditGred", gred);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGred([Bind(Include = "GROUPID,ORDINAL,SHORT_DESCRIPTION,LONG_DESCRIPTION,STRING_PARAM,AUDIT_WHEN")] GE_PARAMTABLE gred)
        {
            if (ModelState.IsValid)
            {
                
                List<GE_PARAMTABLE> selectGred = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == gred.STRING_PARAM && s.SHORT_DESCRIPTION == gred.SHORT_DESCRIPTION && s.ORDINAL != gred.ORDINAL).ToList();
                if (selectGred.Count() <= 0)
                {
                    gred.LONG_DESCRIPTION = gred.SHORT_DESCRIPTION;
                    db2.Entry(gred).State = EntityState.Modified;
                    db2.SaveChanges();
                }
                return RedirectToAction("SenaraiGred");
            }
            ViewBag.GE_PARAMTABLE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            return View(gred);
        }

        // GET: GredElaun/Delete/5
        public ActionResult PadamGred(int? id)
        { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GE_PARAMTABLE gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == id);
            if (gred == null)
            {
                return HttpNotFound();
            }
            ViewBag.GE_PARAMTABLE = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109).ToList();
            return PartialView("_PadamGred", gred);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamGred")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(GE_PARAMTABLE gred)
        {

            GE_PARAMTABLE item = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred.ORDINAL);
           
            db2.GE_PARAMTABLE.Remove(item);
            db2.SaveChanges();
            return RedirectToAction("SenaraiGred");
        }


        public ActionResult CariGred ( string penerangan)
        {
            List<GE_PARAMTABLE> gred = new List<GE_PARAMTABLE>();
            if ( penerangan != null)
            {
                gred = db2.GE_PARAMTABLE.Where(s => s.SHORT_DESCRIPTION == penerangan).ToList();
            }
            string msg = null;
            if (gred.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditGred(string penerangan, int kod)
        {
            List<GE_PARAMTABLE> gred = new List<GE_PARAMTABLE>();
            if (penerangan != null)
            {
                gred = db2.GE_PARAMTABLE.Where(s => s.ORDINAL != kod && s.SHORT_DESCRIPTION == penerangan).ToList();
            }
            string msg = null;
            if (gred.Count() > 0)
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