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
    public class GelaranJawatanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: GelaranJawatan
        public ActionResult SenaraiGelaran()
        {
            ViewBag.HR_KOD_JAWATAN = db.HR_JAWATAN.ToList();
            ViewBag.HR_GRED = db2.GE_PARAMTABLE.ToList();
            return View(db.HR_GELARAN_JAWATAN.ToList());
        }

        public ActionResult InfoGelaran(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GELARAN_JAWATAN gelaran = db.HR_GELARAN_JAWATAN.Find(id);

            if (gelaran == null)
            {
                return HttpNotFound();
            }
            if (gelaran.HR_GRED != null)
            {
                gelaran.HR_GRED = gelaran.HR_GRED.Trim();
            }
           
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "LONG_DESCRIPTION", gelaran.HR_GRED);

            ViewBag.HR_KOD_JAWATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            
            return PartialView("_InfoGelaran", gelaran);
        }

        public ActionResult TambahGelaran()
        {
            ViewBag.HR_KOD_JAWATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "LONG_DESCRIPTION");
            ViewBag.HR_GELARAN_JAWATAN = db.HR_GELARAN_JAWATAN.ToList();
            return PartialView("_TambahGelaran");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahGelaran([Bind(Include = "HR_KOD_GELARAN,HR_PENERANGAN,HR_SINGKATAN,HR_KOD_JAWATAN,HR_GRED")] HR_GELARAN_JAWATAN gelaran)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_GELARAN_JAWATAN.OrderByDescending(s => s.HR_KOD_GELARAN).FirstOrDefault().HR_KOD_GELARAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'G' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodGelaran = Convert.ToString(Increment).PadLeft(4, '0');
                gelaran.HR_KOD_GELARAN = "G" + KodGelaran;

                db.HR_GELARAN_JAWATAN.Add(gelaran);
                db.SaveChanges();
                return RedirectToAction("SenaraiGelaran");
            }
          
            return View(gelaran);
        }

        public ActionResult EditGelaran(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GELARAN_JAWATAN gelaran = db.HR_GELARAN_JAWATAN.Find(id);

            if (gelaran == null)
            {
                return HttpNotFound();
            }
            if (gelaran.HR_GRED != null)
            {
                gelaran.HR_GRED = gelaran.HR_GRED.Trim();
            }

            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "LONG_DESCRIPTION", gelaran.HR_GRED);

            ViewBag.HR_KOD_JAWATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_EditGelaran", gelaran);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGelaran([Bind(Include = "HR_KOD_GELARAN,HR_PENERANGAN,HR_SINGKATAN,HR_KOD_JAWATAN,HR_GRED")] HR_GELARAN_JAWATAN gelaran)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gelaran).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiGelaran");
            }
            return View(gelaran);
        }

        public ActionResult PadamGelaran(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GELARAN_JAWATAN gelaran = db.HR_GELARAN_JAWATAN.Find(id);

            if (gelaran == null)
            {
                return HttpNotFound();
            }
            if (gelaran.HR_GRED != null)
            {
                gelaran.HR_GRED = gelaran.HR_GRED.Trim();
            }

            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "LONG_DESCRIPTION", gelaran.HR_GRED);

            ViewBag.HR_KOD_JAWATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_PadamGelaran", gelaran);
        }

        [HttpPost, ActionName("PadamGelaran")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_GELARAN_JAWATAN gelaran)
        {

            gelaran = db.HR_GELARAN_JAWATAN.SingleOrDefault(s => s.HR_KOD_GELARAN == gelaran.HR_KOD_GELARAN);
            
            db.HR_GELARAN_JAWATAN.Remove(gelaran);
            db.SaveChanges();
            return RedirectToAction("SenaraiGelaran");
        }

        public ActionResult CariGelaran (string penerangan, string singkatan, string kod)
        {
            List<HR_GELARAN_JAWATAN> gelaran = new List<HR_GELARAN_JAWATAN>();
            if (penerangan !=null)
            {
                gelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if (singkatan != null)
            {
                gelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (gelaran.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CariEditGelaran(string penerangan, string singkatan, string kod)
        {
            List<HR_GELARAN_JAWATAN> gelaran = new List<HR_GELARAN_JAWATAN>();
            if (penerangan != null)
            {
                gelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if (singkatan != null)
            {
                gelaran = db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (gelaran.Count() > 0)
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
