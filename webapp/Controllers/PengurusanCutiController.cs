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
    public class PengurusanCutiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: PengurusanCuti
        public ActionResult SenaraiPengurusan()
        {
          
            return View(db.HR_CUTI.ToList());
        }

        //InfoJabatan
        public ActionResult InfoTahunan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI tahunan = db.HR_CUTI.Find(id);

            if (tahunan == null)
            {
                return HttpNotFound();
            }
            if (tahunan.HR_KATEGORI != null)
            {
                tahunan.HR_KATEGORI = tahunan.HR_KATEGORI.Trim();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION", tahunan.HR_KATEGORI);

            
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "B"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();

            return PartialView("_InfoTahunan",tahunan);
        }


        //TambahJabatan-GET
        public ActionResult TambahTahunan()
        {

            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_TambahTahunan");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahTahunan([Bind(Include = "HR_KOD_CUTI,HR_KETERANGAN,HR_SINGKATAN,HR_JUMLAH_CUTI,HR_KATEGORI,HR_CUTI_IND,HR_KEKERAPAN,HR_AKTIF_IND")] HR_CUTI tahunan)
        {
            
            if (ModelState.IsValid)
            {
                HR_CUTI mCuti = db.HR_CUTI.OrderByDescending(s => s.HR_KOD_CUTI).FirstOrDefault();
                if (mCuti == null)
                {
                    mCuti = new HR_CUTI();
                }

                int LastID2 = 0;
                if (mCuti.HR_KOD_CUTI != null)
                {
                    var ListID = new string(mCuti.HR_KOD_CUTI.SkipWhile(x => x == 'C' || x == 'U' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(ListID);
                }

                var Increment = LastID2 + 1;
                var KodGaji = Convert.ToString(Increment).PadLeft(3, '0');
                tahunan.HR_KOD_CUTI = "C" + "U" + KodGaji;
                tahunan.HR_CUTI_IND = "B";
                db.HR_CUTI.Add(tahunan);
                db.SaveChanges();

                return RedirectToAction("SenaraiPengurusan");
            }

            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "B"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            return View(tahunan);
        }


        public ActionResult EditTahunan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI tahunan = db.HR_CUTI.Find(id);

            if (tahunan == null)
            {
                return HttpNotFound();
            }
            if (tahunan.HR_KATEGORI != null)
            {
                tahunan.HR_KATEGORI = tahunan.HR_KATEGORI.Trim();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION", tahunan.HR_KATEGORI);


            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "B"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();

            return PartialView("_EditTahunan", tahunan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTahunan([Bind(Include = "HR_KOD_CUTI,HR_KETERANGAN,HR_SINGKATAN,HR_JUMLAH_CUTI,HR_KATEGORI,HR_CUTI_IND,HR_KEKERAPAN,HR_AKTIF_IND")] HR_CUTI tahunan)
        {
            if (ModelState.IsValid)
            {
                //MASUKKAN NILAI DALAM COLUMN
                tahunan.HR_CUTI_IND = "B";

                db.Entry(tahunan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPengurusan");
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "B"), "HR_CUTI_IND", "HR_KETERANGAN");
            return View(tahunan);
        }


        public ActionResult PadamTahunan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI tahunan = db.HR_CUTI.Find(id);

            if (tahunan == null)
            {
                return HttpNotFound();
            }
            if (tahunan.HR_KATEGORI != null)
            {
                tahunan.HR_KATEGORI = tahunan.HR_KATEGORI.Trim();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION", tahunan.HR_KATEGORI);


            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "B"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();

            return PartialView("_PadamTahunan", tahunan);
        }


        [HttpPost, ActionName("PadamTahunan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_CUTI tahunan)
        {
            tahunan = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == tahunan.HR_KOD_CUTI);

            //MASUKKAN NILAI DALAM COLUMN
            tahunan.HR_CUTI_IND = "B";

            db.HR_CUTI.Remove(tahunan);
            db.SaveChanges();
            return RedirectToAction("SenaraiPengurusan");
        }

        //InfoJabatan
        public ActionResult InfoUmum(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI umum = db.HR_CUTI.Find(id);

            if (umum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "C"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();

            return PartialView("_InfoUmum", umum);
        }

        //TambahJabatan-GET
        public ActionResult TambahUmum()
        {

            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_TambahUmum");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahUmum([Bind(Include = "HR_KOD_CUTI,HR_KETERANGAN,HR_SINGKATAN,HR_JUMLAH_CUTI,HR_KATEGORI,HR_CUTI_IND,HR_KEKERAPAN,HR_AKTIF_IND")] HR_CUTI umum)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_CUTI.OrderByDescending(s => s.HR_KOD_CUTI).FirstOrDefault().HR_KOD_CUTI;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'C' || x == 'U' || x == '0').ToArray());
                var increment = (Convert.ToSingle(LastID) + 1);
                var KodGaji = Convert.ToString(increment).PadLeft(3, '0');
                umum.HR_KOD_CUTI = "C" + "U" + KodGaji;

                //MASUKKAN NILAI DALAM COLUMN
                umum.HR_CUTI_IND = "C";

                
                db.HR_CUTI.Add(umum);
                db.SaveChanges();
                return RedirectToAction("SenaraiPengurusan");
            }
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "C"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            return View(umum);
        }

        //EditTahunan
        public ActionResult EditUmum(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI umum = db.HR_CUTI.Find(id);

            if (umum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "C"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();
            return PartialView("_EditUmum", umum);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUmum([Bind(Include = "HR_KOD_CUTI,HR_KETERANGAN,HR_SINGKATAN,HR_JUMLAH_CUTI,HR_KATEGORI,HR_CUTI_IND,HR_KEKERAPAN,HR_AKTIF_IND")] HR_CUTI umum)
        {
            if (ModelState.IsValid)
            {
                //MASUKKAN NILAI DALAM COLUMN
                umum.HR_CUTI_IND = "C";

                db.Entry(umum).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPengurusan");
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "C"), "HR_CUTI_IND", "HR_KETERANGAN");
            return View(umum);
        }

        //PadamTahunan
        public ActionResult PadamUmum(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_CUTI umum = db.HR_CUTI.Find(id);

            if (umum == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_CUTI_IND = new SelectList(db.HR_CUTI.Where(s => s.HR_CUTI_IND == "C"), "HR_CUTI_IND", "HR_KETERANGAN");
            ViewBag.HR_CUTI = db.HR_CUTI.ToList();
            return PartialView("_PadamUmum", umum);
        }

        [HttpPost, ActionName("PadamUmum")]
        [ValidateAntiForgeryToken]
        public ActionResult PadamConfirmed(HR_CUTI umum)
        {
            umum = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == umum.HR_KOD_CUTI);

            //MASUKKAN NILAI DALAM COLUMN
            umum.HR_CUTI_IND = "C";

            db.HR_CUTI.Remove(umum);
            db.SaveChanges();
            return RedirectToAction("SenaraiPengurusan");
        }

        public ActionResult CariCuti ( string keterangan, string kategori, string kod)
        {
            List<HR_CUTI> tahunan = new List<HR_CUTI>();
            if ( kategori != null)
            {
                tahunan = db.HR_CUTI.Where(s => s.HR_KOD_CUTI == kod && s.HR_KATEGORI == kategori).ToList();
            }
            if( keterangan != null)
            {
                tahunan = db.HR_CUTI.Where(s => s.HR_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (tahunan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditCuti(string keterangan, string kod)
        {
            List<HR_CUTI> tahunan = new List<HR_CUTI>();  
            
            if (keterangan != null)
            {
                tahunan = db.HR_CUTI.Where(s => s.HR_KOD_CUTI != kod && s.HR_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (tahunan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariUmumCuti( string keterangan, string kod, string singkatan)
        {
            List<HR_CUTI> umum = new List<HR_CUTI>();
            if ( keterangan != null )
            {
                umum = db.HR_CUTI.Where(s => s.HR_KOD_CUTI != kod && s.HR_KETERANGAN == keterangan).ToList();
            }
            if ( singkatan != null)
            {
                umum = db.HR_CUTI.Where(s => s.HR_KOD_CUTI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (umum.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

       public ActionResult CariEditUmumCuti ( string keterangan, string singkatan, string kod)
        {
            List<HR_CUTI> umum = new List<HR_CUTI>();
            if ( keterangan != null)
            {
                umum = db.HR_CUTI.Where(s => s.HR_KOD_CUTI != kod && s.HR_KETERANGAN == keterangan).ToList();
            }
            if( singkatan != null)
            {
                umum = db.HR_CUTI.Where(s => s.HR_KOD_CUTI != kod && s.HR_SINGKATAN == singkatan).ToList();

            }
            string msg = null;
            if (umum.Count() > 0)
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