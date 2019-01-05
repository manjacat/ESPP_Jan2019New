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
    public class JadualSOCSOController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: JadualSOCSO
        public ActionResult SenaraiSOCSO()
        {
            return View(db.HR_SOCSO.ToList());
        }

        public ActionResult InfoSOCSO(decimal? dari, decimal? hingga)
        {
            if (dari == null || hingga == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            
            HR_SOCSO socso = db.HR_SOCSO.SingleOrDefault(s  => s.HR_GAJI_DARI == dari && s.HR_GAJI_HINGGA == hingga );

            if (socso == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("_InfoSOCSO", socso);
        }

        public ActionResult TambahSOCSO()
        {

          
            return PartialView("_TambahSOCSO");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahSOCSO([Bind(Include = "HR_GAJI_DARI,HR_GAJI_HINGGA,HR_CARUMAN_PEKERJA,HR_CARUMAN_MAJIKAN,HR_JUMLAH")] HR_SOCSO socso)
        {

            if (ModelState.IsValid)
            {
                List<HR_SOCSO> selectSOCSO = db.HR_SOCSO.Where(s => s.HR_GAJI_DARI == socso.HR_GAJI_DARI && s.HR_GAJI_HINGGA == socso.HR_GAJI_HINGGA).ToList();
                if (selectSOCSO.Count() <= 0)
                {
                    socso.HR_JUMLAH = socso.HR_CARUMAN_MAJIKAN + socso.HR_CARUMAN_PEKERJA; //pangil jadi jumlah
                    db.HR_SOCSO.Add(socso);
                    db.SaveChanges();
                }
                return RedirectToAction("SenaraiSOCSO");
            }

            return View(socso);
        }



        public ActionResult EditSOCSO(decimal? dari, decimal? hingga)
        {
            if (dari == null || hingga == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_SOCSO socso = db.HR_SOCSO.SingleOrDefault(s => s.HR_GAJI_DARI == dari && s.HR_GAJI_HINGGA == hingga);

            if (socso == null)
            {
                return HttpNotFound();
            }

            return PartialView("_EditSOCSO", socso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSOCSO([Bind(Include = "HR_GAJI_DARI,HR_GAJI_HINGGA,HR_CARUMAN_PEKERJA,HR_CARUMAN_MAJIKAN,HR_JUMLAH")] HR_SOCSO socso)
        {
            if (ModelState.IsValid)
            {
                socso.HR_JUMLAH = socso.HR_CARUMAN_MAJIKAN + socso.HR_CARUMAN_PEKERJA; //pangil jadi jumlah
                db.Entry(socso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiSOCSO");
            }
           
            return View(socso);
        }

        public ActionResult PadamSOCSO(decimal? dari, decimal? hingga)
        {
            if (dari == null || hingga == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_SOCSO socso = db.HR_SOCSO.SingleOrDefault(s => s.HR_GAJI_DARI == dari && s.HR_GAJI_HINGGA == hingga);

            if (socso == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PadamSOCSO", socso);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamSOCSO")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SOCSO socso)
        {

            socso = db.HR_SOCSO.SingleOrDefault(s => s.HR_GAJI_DARI == socso.HR_GAJI_DARI && s.HR_GAJI_HINGGA == socso.HR_GAJI_HINGGA);

            db.HR_SOCSO.Remove(socso);
            db.SaveChanges();
            return RedirectToAction("SenaraiSOCSO");
        }

        public ActionResult CariSOCSO ( decimal? dari, decimal? hingga)
        {
            List<HR_SOCSO> socso = new List<HR_SOCSO>();
            if ( dari != null)
            {
                socso = db.HR_SOCSO.Where(s => s.HR_GAJI_DARI == dari).ToList();
            }
            if ( hingga != null)
            {
                socso = db.HR_SOCSO.Where(s => s.HR_GAJI_HINGGA == hingga).ToList();
            }
            string msg = null;
            if (socso.Count() > 0)
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
