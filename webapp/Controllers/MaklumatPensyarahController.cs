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
    public class MaklumatPensyarahController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: Pensyarah
        public ActionResult SenaraiPensyarah()
        {
            return View(db.HR_KUMPULAN_PENSYARAH.ToList());
        }

        // GET: Pensyarah/Details/5
        public ActionResult InfoMaklumat(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KUMPULAN_PENSYARAH pensyarah = db.HR_KUMPULAN_PENSYARAH.Find(id);
            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 170), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_InfoMaklumat", pensyarah);
        }

        public ActionResult TambahMaklumat()
        {

            ViewBag.HR_JENIS_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 170), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KUMPULAN_PENSYARAH = db.HR_KUMPULAN_PENSYARAH.ToList();
            return PartialView("_TambahMaklumat");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahMaklumat([Bind(Include = "HR_KOD_KUMPULAN,HR_PENERANGAN,HR_SINGKATAN,HR_KADAR_JAM,HR_NILAI_MAKSIMA,HR_PERATUS,HR_JENIS_IND")] HR_KUMPULAN_PENSYARAH pensyarah)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_KUMPULAN_PENSYARAH.OrderByDescending(s => s.HR_KOD_KUMPULAN).FirstOrDefault().HR_KOD_KUMPULAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'K' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodPensyarah = Convert.ToString(Increment).PadLeft(4, '0');
                pensyarah.HR_KOD_KUMPULAN = "K" + KodPensyarah;

                db.HR_KUMPULAN_PENSYARAH.Add(pensyarah);
                db.SaveChanges();
                return RedirectToAction("SenaraiPensyarah");
            }

            return View(pensyarah);
        }

        public ActionResult EditMaklumat(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KUMPULAN_PENSYARAH pensyarah = db.HR_KUMPULAN_PENSYARAH.Find(id);
            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 170), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_EditMaklumat", pensyarah);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMaklumat([Bind(Include = "HR_KOD_KUMPULAN,HR_PENERANGAN,HR_SINGKATAN,HR_KADAR_JAM,HR_NILAI_MAKSIMA,HR_PERATUS,HR_JENIS_IND")] HR_KUMPULAN_PENSYARAH pensyarah)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pensyarah).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPensyarah");
            }
            ViewBag.HR_JENIS_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 170), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(pensyarah);
        }

        public ActionResult PadamMaklumat(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KUMPULAN_PENSYARAH pensyarah = db.HR_KUMPULAN_PENSYARAH.Find(id);
            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_JENIS_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 170), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_PadamMaklumat", pensyarah);
        }


        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamMaklumat")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KUMPULAN_PENSYARAH pensyarah)
        {
            pensyarah = db.HR_KUMPULAN_PENSYARAH.SingleOrDefault(s => s.HR_KOD_KUMPULAN == pensyarah.HR_KOD_KUMPULAN);

            db.HR_KUMPULAN_PENSYARAH.Remove(pensyarah);
            db.SaveChanges();
            return RedirectToAction("SenaraiPensyarah");
        }

        public ActionResult CariPensyarah (string singkatan, string penerangan)
        {
            List<HR_KUMPULAN_PENSYARAH> pensyarah = new List<HR_KUMPULAN_PENSYARAH>();
            if ( singkatan != null)
            {
                pensyarah = db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_SINGKATAN == singkatan).ToList();
            }
            if( penerangan != null)
            {
                pensyarah = db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_PENERANGAN == penerangan).ToList();
            }
            string msg = null;
            if (pensyarah.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CariEditPensyarah(string singkatan, string penerangan, string kod)
        {
            List<HR_KUMPULAN_PENSYARAH> pensyarah = new List<HR_KUMPULAN_PENSYARAH>();
            if (singkatan != null)
            {
                pensyarah = db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_KOD_KUMPULAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if (penerangan != null)
            {
                pensyarah = db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_KOD_KUMPULAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
          
            string msg = null;
            if (pensyarah.Count() > 0)
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
