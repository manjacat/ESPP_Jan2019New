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
    public class KuartersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: HR_KUARTERS
        public ActionResult SenaraiKuarters()
        {
            ViewBag.GROUPID  = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).ToList();
          
            return View(db.HR_KUARTERS.ToList());
        }

        public ActionResult InfoKuarters(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KUARTERS kuarters = db.HR_KUARTERS.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            if (kuarters.HR_NEGERI != null)
            {
                kuarters.HR_NEGERI = kuarters.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", kuarters.HR_NEGERI);

            return PartialView("_InfoKuarters", kuarters);
        }

        public ActionResult TambahKuarters()
        {
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahKuarters");
        }

        [HttpPost, ActionName("TambahKuarters")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahKuarters([Bind(Include = "HR_KOD_KUARTERS,HR_BLOK_KUARTERS,HR_ALAMAT1,HR_ALAMAT2,HR_ALAMAT3,HR_BANDAR,HR_POSKOD,HR_NEGERI,HR_AKTIF_IND,SHORT_DESCRIPTION")] HR_KUARTERS kuarters)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_KUARTERS.OrderByDescending(s => s.HR_KOD_KUARTERS).FirstOrDefault().HR_KOD_KUARTERS;
                var LastID = new string (SelectLastID.SkipWhile(x => x == 'K').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodKuarters = Convert.ToString(Increment);
                kuarters.HR_KOD_KUARTERS = "K" + KodKuarters;

                db.HR_KUARTERS.Add(kuarters);
                db.SaveChanges();
                return RedirectToAction("SenaraiKuarters");
                 
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View(kuarters);
        }

        public ActionResult EditKuarters(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KUARTERS kuarters = db.HR_KUARTERS.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            if (kuarters.HR_NEGERI != null)
            {
                kuarters.HR_NEGERI = kuarters.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", kuarters.HR_NEGERI);

            return PartialView("_EditKuarters", kuarters);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditKuarters([Bind(Include = "HR_KOD_KUARTERS,HR_BLOK_KUARTERS,HR_ALAMAT1,HR_ALAMAT2,HR_ALAMAT3,HR_BANDAR,HR_POSKOD,HR_NEGERI,HR_AKTIF_IND,SHORT_DESCRIPTION")] HR_KUARTERS kuarters)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kuarters).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiKuarters");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View(kuarters);
        }

        public ActionResult PadamKuarters(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KUARTERS kuarters = db.HR_KUARTERS.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            if (kuarters.HR_NEGERI != null)
            {
                kuarters.HR_NEGERI = kuarters.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", kuarters.HR_NEGERI);

            return PartialView("_PadamKuarters", kuarters);
        }

        [HttpPost, ActionName("PadamKuarters")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KUARTERS kuarters)
        {
            kuarters = db.HR_KUARTERS.SingleOrDefault(s => s.HR_KOD_KUARTERS == kuarters.HR_KOD_KUARTERS);

            db.HR_KUARTERS.Remove(kuarters);
            db.SaveChanges();
            return RedirectToAction("SenaraiKuarters");
        }

      
        public ActionResult CariKuarters (string kod, string blok)
        {
            List<HR_KUARTERS> kuarters = new List<HR_KUARTERS>();
            if( kod != null)
            {
                kuarters = db.HR_KUARTERS.Where(s => s.HR_KOD_KUARTERS != kod && s.HR_BLOK_KUARTERS == blok ).ToList();
            }
           
            string msg = null;
            if (kuarters.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditKuarters(string kod, string blok)
        {
            List<HR_KUARTERS> kuarters = new List<HR_KUARTERS>();
            if (kod != null)
            {
                kuarters = db.HR_KUARTERS.Where(s => s.HR_KOD_KUARTERS != kod && s.HR_BLOK_KUARTERS == blok).ToList();
            }

            string msg = null;
            if (kuarters.Count() > 0)
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
