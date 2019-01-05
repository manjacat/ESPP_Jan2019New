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
    public class JawatanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HR_JAWATAN
        public ActionResult SenaraiJawatan()
        {
            return View(db.HR_JAWATAN.ToList());
        }
        
        public ActionResult InfoJawatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_JAWATAN jawatan = db.HR_JAWATAN.Find(id);

            if (jawatan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_JAWATAN = db.HR_JAWATAN.ToList();
            return PartialView("_InfoJawatan", jawatan);
        }

        public ActionResult TambahJawatan()
        {

            ViewBag.HR_JAWATAN = db.HR_JAWATAN.ToList();
            return PartialView("_TambahJawatan");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahJawatan([Bind(Include = "HR_KOD_JAWATAN,HR_NAMA_JAWATAN,HR_AKTIF_IND")] HR_JAWATAN jawatan)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_JAWATAN.OrderByDescending(s => s.HR_KOD_JAWATAN).FirstOrDefault().HR_KOD_JAWATAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'J' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodJawatan = Convert.ToString(Increment).PadLeft( 4,'0');
                jawatan.HR_KOD_JAWATAN = "J" + KodJawatan;

                db.HR_JAWATAN.Add(jawatan); 
                db.SaveChanges();
                return RedirectToAction("SenaraiJawatan");
            }

            return View(jawatan);
        }


        public ActionResult EditJawatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JAWATAN jawatan = db.HR_JAWATAN.Find(id);
            if (jawatan == null)
            {
                return HttpNotFound();
            }
            return PartialView("_EditJawatan",jawatan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJawatan([Bind(Include = "HR_KOD_JAWATAN,HR_NAMA_JAWATAN,HR_AKTIF_IND")] HR_JAWATAN jawatan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jawatan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiJawatan");
            }
            return View(jawatan);
        }

        public ActionResult PadamJawatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JAWATAN jawatan = db.HR_JAWATAN.Find(id);
            if (jawatan == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PadamJawatan", jawatan);
        }

        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamJawatan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_JAWATAN jawatan)
        {
            jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == jawatan.HR_KOD_JAWATAN);
            
            db.HR_JAWATAN.Remove(jawatan);
            db.SaveChanges();
            return RedirectToAction("SenaraiJawatan");
        }

        public ActionResult CariJawatan(string nama, string kod)
        {
            List<HR_JAWATAN> jawatan = new List<HR_JAWATAN>();
            if (nama != null)
            {
                jawatan = db.HR_JAWATAN.Where(s => s.HR_KOD_JAWATAN != kod && s.HR_NAMA_JAWATAN == nama).ToList();
            }
            string msg = null;
            if (jawatan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditJawatan(string nama, string kod)
        {
            List<HR_JAWATAN> jawatan = new List<HR_JAWATAN>();
            if (nama != null)
            {
                jawatan = db.HR_JAWATAN.Where(s => s.HR_KOD_JAWATAN != kod && s.HR_NAMA_JAWATAN == nama).ToList();
            }
            string msg = null;
            if (jawatan.Count() > 0)
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
