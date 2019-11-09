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
    public class ElaunController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        private FMSContext FMS = new FMSContext();

        // GET: Elaun
        public ActionResult SenaraiElaun()
        {
            ViewBag.HR_KOD_POTONGAN = db.HR_POTONGAN.ToList();
            ViewBag.HR_KOD_KATEGORI = db.HR_KATEGORI_ELAUN.ToList();
         
           
            return View(db.HR_ELAUN.ToList());
        }

        public ActionResult InfoElaun(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ELAUN maklumatelaun = db.HR_ELAUN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_KATEGORI = new SelectList(db.HR_KATEGORI_ELAUN, "HR_KOD_KATEGORI", "HR_PENERANGAN");
            ViewBag.HR_KOD_POTONGAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            ViewBag.HR_KOD_KREDITOR = new SelectList(FMS.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_ELAUN = db.HR_ELAUN.ToList();
            return PartialView("_InfoElaun", maklumatelaun);
        }

        public ActionResult TambahElaun()
        {
            ViewBag.HR_KOD_KATEGORI = new SelectList(db.HR_KATEGORI_ELAUN, "HR_KOD_KATEGORI", "HR_PENERANGAN");
            ViewBag.HR_KOD_POTONGAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            ViewBag.HR_KOD_KREDITOR = new SelectList(FMS.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_ELAUN = db.HR_ELAUN.ToList();
            return PartialView("_TambahElaun");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahElaun([Bind(Include = "HR_KOD_ELAUN,HR_PENERANGAN_ELAUN,HR_VOT_ELAUN,HR_SINGKATAN,HR_AKTIF_IND,HR_NILAI,HR_JAWATAN_IND,HR_PINDAAN_IND,HR_KOD_KATEGORI,HR_STATUS_CUKAI,HR_TUNGGAKAN_IND,HR_STATUS_KWSP,HR_STATUS_SOCSO,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_KOD_POTONGAN,HR_PREFIX,HR_KOD_TUNGGAKAN,HR_PERATUS_IND,HR_INDICATOR")] HR_ELAUN maklumatelaun)
        {
            if (ModelState.IsValid)
            {


                var SelectLastID = db.HR_ELAUN.OrderByDescending(s => s.HR_KOD_ELAUN).FirstOrDefault().HR_KOD_ELAUN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'E' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodElaun = Convert.ToString(Increment).PadLeft(4, '0');
                maklumatelaun.HR_KOD_ELAUN = "E" + KodElaun;

                db.HR_ELAUN.Add(maklumatelaun);
                db.SaveChanges();
                return RedirectToAction("SenaraiElaun");
            }

            return View(maklumatelaun);
        }

        public ActionResult EditElaun(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ELAUN maklumatelaun = db.HR_ELAUN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_KATEGORI = new SelectList(db.HR_KATEGORI_ELAUN, "HR_KOD_KATEGORI", "HR_PENERANGAN");
            ViewBag.HR_KOD_POTONGAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            ViewBag.HR_KOD_KREDITOR = new SelectList(FMS.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_ELAUN = db.HR_ELAUN.ToList();
            return PartialView("_EditElaun", maklumatelaun);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditElaun([Bind(Include = "HR_KOD_ELAUN,HR_PENERANGAN_ELAUN,HR_VOT_ELAUN,HR_SINGKATAN,HR_AKTIF_IND,HR_NILAI,HR_JAWATAN_IND,HR_PINDAAN_IND,HR_KOD_KATEGORI,HR_STATUS_CUKAI,HR_TUNGGAKAN_IND,HR_STATUS_KWSP,HR_STATUS_SOCSO,HR_KOD_KREDITOR,HR_KETERANGAN_SLIP,HR_KOD_POTONGAN,HR_PREFIX,HR_KOD_TUNGGAKAN,HR_PERATUS_IND,HR_INDICATOR")] HR_ELAUN maklumatelaun)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maklumatelaun).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiElaun");
            }
            ViewBag.HR_KOD_KATEGORI = new SelectList(db.HR_KATEGORI_ELAUN, "HR_KOD_KATEGORI", "HR_PENERANGAN");
            ViewBag.HR_KOD_POTONGAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            ViewBag.HR_KOD_KREDITOR = new SelectList(FMS.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(maklumatelaun);
        }

        public ActionResult PadamElaun(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ELAUN maklumatelaun = db.HR_ELAUN.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_KATEGORI = new SelectList(db.HR_KATEGORI_ELAUN, "HR_KOD_KATEGORI", "HR_PENERANGAN");
            ViewBag.HR_KOD_POTONGAN = new SelectList(db.HR_POTONGAN, "HR_KOD_POTONGAN", "HR_PENERANGAN_POTONGAN");
            ViewBag.HR_KOD_KREDITOR = new SelectList(FMS.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_ELAUN = db.HR_ELAUN.ToList();
            return PartialView("_PadamElaun", maklumatelaun);
        }

        [HttpPost, ActionName("PadamElaun")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_ELAUN maklumatelaun)
        {
            maklumatelaun = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == maklumatelaun.HR_KOD_ELAUN);

            db.HR_ELAUN.Remove(maklumatelaun);
            db.SaveChanges();
            return RedirectToAction("SenaraiElaun");
        }

        
        public ActionResult CariElaun (string kategori, string kod, string penerangan, string vot, string singkatan)
        {
            List<HR_ELAUN> elaun = new List<HR_ELAUN>();
            if(kategori != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == kod && s.HR_KOD_KATEGORI == kategori).ToList();
            }
            if( penerangan != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN != kod && s.HR_KOD_KATEGORI != kategori && s.HR_PENERANGAN_ELAUN == penerangan).ToList();
            }
            if (vot != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_VOT_ELAUN == vot).ToList();
                
            }
            if( singkatan != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_SINGKATAN == singkatan).ToList();
            }
            
            string msg = null;
            if (elaun.Count ()>0)
            {  
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditElaun(string kategori, string kod, string penerangan, string vot, string singkatan)
        {
            List<HR_ELAUN> elaun = new List<HR_ELAUN>();
            if (kod != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == kod).ToList();
            }
            if (penerangan != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN != kod && s.HR_KOD_KATEGORI != kategori && s.HR_PENERANGAN_ELAUN == penerangan).ToList();
            }
            if (vot != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == kod && s.HR_KOD_KATEGORI == kategori && s.HR_VOT_ELAUN != vot).ToList();

            }
            if (singkatan != null)
            {
                elaun = db.HR_ELAUN.Where(s => s.HR_KOD_ELAUN == kod && s.HR_KOD_KATEGORI == kategori && s.HR_SINGKATAN != singkatan).ToList();
            }

            string msg = null;
            if (elaun.Count() > 0)
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
