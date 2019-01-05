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
    public class CarumanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Caruman
        public ActionResult SenaraiCaruman()
        {
           
            return View(db.HR_CARUMAN.ToList());
        }

        public ActionResult Caruman()
        {
            return View();
        }

        public ActionResult InfoCaruman(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CARUMAN caruman = db.HR_CARUMAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_CARUMAN = db.HR_CARUMAN.ToList();
            return PartialView("_InfoCaruman", caruman);
        }

        public ActionResult TambahCaruman()
        {
            ViewBag.HR_KOD_KREDITOR = new SelectList(db.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_CARUMAN = db.HR_CARUMAN.ToList();
            return PartialView("_TambahCaruman");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahCaruman([Bind(Include = "HR_KOD_CARUMAN,HR_PENERANGAN_CARUMAN,HR_VOT_CARUMAN,HR_SINGKATAN,HR_PERATUS,HR_AKTIF_IND,HR_NILAI,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_PREFIX,HR_KETERANGAN_LAPORAN")] HR_CARUMAN caruman)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_CARUMAN.OrderByDescending(s => s.HR_KOD_CARUMAN).FirstOrDefault().HR_KOD_CARUMAN;
                var LastID = new string(SelectLastID.SkipWhile( x => x == 'C' || x == '0' ).ToArray());
                var increment = (Convert.ToSingle(LastID) + 1);
                var KodCaruman = Convert.ToString(increment).PadLeft(4,'0');
                caruman.HR_KOD_CARUMAN = "C" + KodCaruman;
                db.HR_CARUMAN.Add(caruman);
                db.SaveChanges();
                return RedirectToAction("SenaraiCaruman");
            }

            return View(caruman);
        }

        public ActionResult EditCaruman(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CARUMAN caruman = db.HR_CARUMAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_CARUMAN = db.HR_CARUMAN.ToList();
            return PartialView("_EditCaruman", caruman);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCaruman([Bind(Include = "HR_KOD_CARUMAN,HR_PENERANGAN_CARUMAN,HR_VOT_CARUMAN,HR_SINGKATAN,HR_PERATUS,HR_AKTIF_IND,HR_NILAI,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_PREFIX,HR_KETERANGAN_LAPORAN")] HR_CARUMAN caruman)
        {
            if (ModelState.IsValid)
            {
                db.Entry(caruman).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiCaruman");
            }
            return View(caruman);
        }

        public ActionResult PadamCaruman(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CARUMAN caruman = db.HR_CARUMAN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_CARUMAN = db.HR_CARUMAN.ToList();
            return PartialView("_PadamCaruman", caruman);
        }


        // POST: Potongan/Delete/5
        [HttpPost, ActionName("PadamCaruman")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_CARUMAN caruman)
        {
            caruman = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == caruman.HR_KOD_CARUMAN);

            db.HR_CARUMAN.Remove(caruman);
            db.SaveChanges();
            return RedirectToAction("SenaraiCaruman");
        }


        public ActionResult CariCaruman (string penerangan)
        {
            List<HR_CARUMAN> caruman = db.HR_CARUMAN.Where(s => s.HR_PENERANGAN_CARUMAN == penerangan).ToList();

            string msg = null;
            if (caruman.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CariEditCaruman (string penerangan, string kod)
        {
            List<HR_CARUMAN> caruman = new List<HR_CARUMAN>();
            if ( penerangan != null)
                
            caruman = db.HR_CARUMAN.Where(s => s.HR_KOD_CARUMAN == kod && s.HR_PENERANGAN_CARUMAN != penerangan).ToList();

            string msg = null;
            if (caruman.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
