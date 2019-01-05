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
    public class AlasanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: Alasan
        public ActionResult SenaraiAlasan()
        {
            return View(db.HR_ALASAN.ToList());
        }

        public ActionResult InfoAlasan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ALASAN alasan = db.HR_ALASAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SISTEM_IND = new SelectList(db2.GE_SYSTEM, "GE_SYS_ID", "GE_SYS_NAME");
            ViewBag.HR_ALASAN = db.HR_ALASAN.ToList();
            return PartialView("_InfoAlasan", alasan);
        }

        public ActionResult TambahAlasan()
        {
           
            ViewBag.HR_SISTEM_IND = new SelectList(db2.GE_SYSTEM, "GE_SYS_ID", "GE_SYS_NAME");
            
            return PartialView("_TambahAlasan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahAlasan([Bind(Include = "HR_KOD_ALASAN,HR_PENERANGAN,HR_SISTEM_IND")] HR_ALASAN alasan)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_ALASAN.OrderByDescending(s => s.HR_KOD_ALASAN).FirstOrDefault().HR_KOD_ALASAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'A' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodAlasan = Convert.ToString(Increment).PadLeft(2, '0');
                alasan.HR_KOD_ALASAN = "A" + KodAlasan;

                db.HR_ALASAN.Add(alasan);
                db.SaveChanges();
                return RedirectToAction("SenaraiAlasan");
            }
            ViewBag.HR_KOD_PEPERIKSAAN = new SelectList(db.HR_JENIS_PEPERIKSAAN, "HR_KOD_PEPERIKSAAN", "HR_SINGKATAN");
            return View(alasan);
        }

        public ActionResult EditAlasan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ALASAN alasan = db.HR_ALASAN.Find(id);

            if (alasan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SISTEM_IND = new SelectList(db2.GE_SYSTEM, "GE_SYS_ID", "GE_SYS_NAME");
            ViewBag.HR_ALASAN = db.HR_ALASAN.ToList();
            return PartialView("_EditAlasan", alasan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAlasan([Bind(Include = "HR_KOD_ALASAN,HR_PENERANGAN,HR_SISTEM_IND")] HR_ALASAN alasan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alasan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiAlasan");
            }
            return View(alasan);
        }

        public ActionResult PadamAlasan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ALASAN alasan = db.HR_ALASAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SISTEM_IND = new SelectList(db2.GE_SYSTEM, "GE_SYS_ID", "GE_SYS_NAME");
            ViewBag.HR_ALASAN = db.HR_ALASAN.ToList();
            return PartialView("_PadamAlasan", alasan);
        }


        // POST: Potongan/Delete/5
        [HttpPost, ActionName("PadamAlasan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_ALASAN alasan)
        {
            alasan = db.HR_ALASAN.SingleOrDefault(s => s.HR_KOD_ALASAN == alasan.HR_KOD_ALASAN);

            db.HR_ALASAN.Remove(alasan);
            db.SaveChanges();
            return RedirectToAction("SenaraiAlasan");
        }

        public ActionResult CariAlasan(string penerangan, string kod)
        {
            List<HR_ALASAN> alasan = new List<HR_ALASAN>();
            if (penerangan != null)
            {
                alasan = db.HR_ALASAN.Where(s => s.HR_KOD_ALASAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (alasan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CariEditAlasan(string penerangan, string kod)
        {
            List<HR_ALASAN> alasan = new List<HR_ALASAN>();
            if (penerangan != null)
            {
                alasan = db.HR_ALASAN.Where(s => s.HR_KOD_ALASAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (alasan.Count() > 0)
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
