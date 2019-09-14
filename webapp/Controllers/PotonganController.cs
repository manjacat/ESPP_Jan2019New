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
    public class PotonganController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //private FMSContext db2 = new FMSContext();
        private ApplicationDbContext db2 = new ApplicationDbContext();

        // GET: Potongan
        public ActionResult SenaraiPemotongan()
        {
            return View(db.HR_POTONGAN.ToList());
        }

        public ActionResult InfoPotongan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_POTONGAN potongan = db.HR_POTONGAN.Find(id);

            if (potongan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db2.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_POTONGAN = db.HR_POTONGAN.ToList();
            return PartialView("_InfoPotongan", potongan);
        }

        public ActionResult TambahPotongan()
        {
            ViewBag.HR_KOD_KREDITOR = new SelectList(db2.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_POTONGAN = db.HR_POTONGAN.ToList();
            return PartialView("_TambahPotongan");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPotongan([Bind(Include = "HR_KOD_POTONGAN,HR_PENERANGAN_POTONGAN,HR_VOT_POTONGAN,HR_SINGKATAN,HR_AKTIF_IND,HR_NILAI,OLD_CODE,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_KETERANGAN_LAPORAN,HR_VOT_POTONGAN_P,HR_INDICATOR,HR_KOD_CARUMAN")] HR_POTONGAN potongan)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_POTONGAN.OrderByDescending(s => s.HR_KOD_POTONGAN).FirstOrDefault().HR_KOD_POTONGAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'P' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodPotongan = Convert.ToString(Increment).PadLeft(4, '0');
                potongan.HR_KOD_POTONGAN = "P" + KodPotongan;

                db.HR_POTONGAN.Add(potongan);
                db.SaveChanges();
                return RedirectToAction("SenaraiPemotongan");
            }

            return View(potongan);
        }

        public ActionResult EditPotongan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_POTONGAN potongan = db.HR_POTONGAN.Find(id);
            if (potongan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db2.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            return PartialView("_EditPotongan", potongan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPotongan([Bind(Include = "HR_KOD_POTONGAN,HR_PENERANGAN_POTONGAN,HR_VOT_POTONGAN,HR_SINGKATAN,HR_AKTIF_IND,HR_NILAI,OLD_CODE,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_KETERANGAN_LAPORAN,HR_VOT_POTONGAN_P,HR_INDICATOR,HR_KOD_CARUMAN")] HR_POTONGAN potongan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(potongan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPemotongan");
            }
            return View(potongan);
        }

        public ActionResult PadamPotongan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_POTONGAN potongan = db.HR_POTONGAN.Find(id);
            if (potongan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db2.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            return PartialView("_PadamPotongan", potongan);
        }

        // POST: Potongan/Delete/5
        [HttpPost, ActionName("PadamPotongan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_POTONGAN potongan)
        {
            potongan = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == potongan.HR_KOD_POTONGAN);

            db.HR_POTONGAN.Remove(potongan);
            db.SaveChanges();
            return RedirectToAction("SenaraiPemotongan");
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_KOD_POTONGAN,HR_PENERANGAN_POTONGAN,HR_VOT_POTONGAN,HR_SINGKATAN,HR_AKTIF_IND,HR_NILAI,OLD_CODE,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_KETERANGAN_LAPORAN,HR_VOT_POTONGAN_P,HR_INDICATOR,HR_KOD_CARUMAN")] HR_POTONGAN potongan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(potongan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPemotongan");
            }
            return View(potongan);
        }

      
       
        public ActionResult CariPotongan ( string penerangan, string vot, string singkatan, string kreditor)
        {
            List<HR_POTONGAN> potongan = new List<HR_POTONGAN>();
            if (penerangan != null)
            {
                potongan = db.HR_POTONGAN.Where(s => s.HR_PENERANGAN_POTONGAN == penerangan).ToList();
            }
            if (vot != null)
            {
                potongan = db.HR_POTONGAN.Where(s => s.HR_VOT_POTONGAN == vot).ToList();
            }
            if ( singkatan != null)
            {
                potongan = db.HR_POTONGAN.Where(s => s.HR_SINGKATAN == singkatan).ToList();
            }
            
            string msg = null;
            if (potongan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult CariEditPotongan (string penerangan, string kod, string singkatan)
        {
            List<HR_POTONGAN> potongan = new List<HR_POTONGAN>();
            if(penerangan != null)
            {
                potongan = db.HR_POTONGAN.Where(s =>s.HR_KOD_POTONGAN != kod && s.HR_PENERANGAN_POTONGAN == penerangan).ToList();
            }
            if(singkatan !=null)
            {
                potongan = db.HR_POTONGAN.Where(s => s.HR_KOD_POTONGAN != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (potongan.Count() > 0)
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
