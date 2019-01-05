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
    public class TindakanDisiplinController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TindakanDisiplin
        public ActionResult SenaraiJenis()
        {
            ViewBag.HR_TINDAKAN = db.HR_TINDAKAN.ToList();
            return View(db.HR_TINDAKAN.ToList());
        }

        public ActionResult InfoTindakanDisiplin (string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_TINDAKAN tindakan = db.HR_TINDAKAN.Find(id);

            if (tindakan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_TINDAKAN = db.HR_TINDAKAN.ToList();
            return PartialView("_InfoTindakanDisiplin",tindakan);
        }

        public ActionResult TambahTindakanDisiplin ()
        {
            ViewBag.HR_TINDAKAN = db.HR_TINDAKAN.ToList();
            return PartialView("_TambahTindakanDisiplin");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahTindakanDisiplin ([Bind(Include = " HR_KOD_TINDAKAN, HR_PENERANGAN, HR_SINGKATAN")] HR_TINDAKAN tindakan)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_TINDAKAN.OrderByDescending(s => s.HR_KOD_TINDAKAN).FirstOrDefault().HR_KOD_TINDAKAN;
                var LastID = new string(SelectLastID.SkipWhile(x =>  x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodTindakan = Convert.ToString(Increment).PadLeft(5, '0');
                tindakan.HR_KOD_TINDAKAN = KodTindakan;

                db.HR_TINDAKAN.Add(tindakan);
                db.SaveChanges();
                return RedirectToAction("SenaraiJenis");
            }
            return View(tindakan);
        }

        public ActionResult EditTindakanDisiplin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_TINDAKAN tindakan = db.HR_TINDAKAN.Find(id);

            if (tindakan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_TINDAKAN = db.HR_TINDAKAN.ToList();
            return PartialView("_EditTindakanDisiplin", tindakan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTindakanDisiplin([Bind(Include = " HR_KOD_TINDAKAN, HR_PENERANGAN, HR_SINGKATAN")] HR_TINDAKAN tindakan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tindakan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiJenis");
            }
            return View(tindakan);
        }

        public ActionResult PadamTindakanDisiplin(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_TINDAKAN tindakan = db.HR_TINDAKAN.Find(id);

            if (tindakan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_TINDAKAN = db.HR_TINDAKAN.ToList();
            return PartialView("_PadamTindakanDisiplin", tindakan);
        }

        [HttpPost, ActionName("PadamTindakanDisiplin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_TINDAKAN tindakan)
        {
            tindakan = db.HR_TINDAKAN.SingleOrDefault(s => s.HR_KOD_TINDAKAN == tindakan.HR_KOD_TINDAKAN);

            db.HR_TINDAKAN.Remove(tindakan);
            db.SaveChanges();
            return RedirectToAction("SenaraiJenis");
        }


        public ActionResult CariDisiplin ( string penerangan, string kod, string singkatan)
        {
            List<HR_TINDAKAN> tindakan = new List<HR_TINDAKAN>();
            if ( penerangan != null)
            {
                tindakan = db.HR_TINDAKAN.Where(s => s.HR_KOD_TINDAKAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if ( singkatan != null)
            {
                tindakan = db.HR_TINDAKAN.Where(s => s.HR_KOD_TINDAKAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (tindakan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditDisiplin(string penerangan, string kod, string singkatan)
        {
            List<HR_TINDAKAN> tindakan = new List<HR_TINDAKAN>();
            if (penerangan != null)
            {
                tindakan = db.HR_TINDAKAN.Where(s => s.HR_KOD_TINDAKAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if (singkatan != null)
            {
                tindakan = db.HR_TINDAKAN.Where(s => s.HR_KOD_TINDAKAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (tindakan.Count() > 0)
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