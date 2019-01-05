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
    public class PeribadiKumpulanPensyarahController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: PeribadiKumpulanPensyarah

       
        public ActionResult Index()
        {
            //Peribadi("", "");
            List<HR_PENSYARAH> mPensyarah = new List<HR_PENSYARAH>();
            return View(mPensyarah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string action, string value)
        {
            if (action == null || value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<HR_PENSYARAH> mPensyarah = new List<HR_PENSYARAH>();
            if (action == "1")
            {
                mPensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_PEKERJA.Contains(value)).ToList();
            }
            else if (action == "2")
            {
                mPensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH.Contains(value)).ToList();
            }
            else if (action == "3")
            {
                mPensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_KPBARU.Contains(value)).ToList();
            }

            if (mPensyarah == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem { Text = "AKTIF", Value = "Y" });
            status.Add(new SelectListItem { Text = "TIDAK AKTIF", Value = "T" });
            status.Add(new SelectListItem { Text = "PESARA", Value = "P" });
            status.Add(new SelectListItem { Text = "TAHAN GAJI", Value = "N" });
            status.Add(new SelectListItem { Text = "GANTUNG", Value = "G" });
            ViewBag.status = status;

            return View(mPensyarah);
        }

        public ActionResult MaklumatPeribadi (string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH mPensyarah = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PENSYARAH == id);
            HR_MAKLUMAT_PERIBADI mKakitangan = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            List<PeribadiModels> sKakitangan = new List<PeribadiModels>();

            return View(mPensyarah);
        }

        public ActionResult TambahCarian()
        {
           
            return PartialView("_TambahCarian");
        }

       

        public ActionResult InfoDalam(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENSYARAH dalam = db.HR_PENSYARAH.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", dalam.HR_SNEGERI);
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", dalam.HR_TNEGERI);
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_InfoDalam", dalam);
        }

        public ActionResult InfoLuar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENSYARAH luar = db.HR_PENSYARAH.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_SNEGERI);
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_TNEGERI);
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_InfoLuar", luar);
        }

        public ActionResult TambahDalam()
        {
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_JENIS_IND == "D"), "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_TambahDalam");
        }


        public ActionResult TambahLuar()
        {
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_JENIS_IND == "L"), "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_TambahLuar");
        }


        [HttpPost, ActionName("TambahLuar")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahLuar([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH luar)
        {
            if (ModelState.IsValid)
            {
                List<HR_PENSYARAH> selectLuar = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH == luar.HR_NO_PENSYARAH).ToList();
                if (selectLuar.Count() <= 0)
                {
                    var SelectLastID = db.HR_PENSYARAH.OrderByDescending(s => s.HR_NO_PENSYARAH).FirstOrDefault().HR_NO_PENSYARAH;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == 'P' || x == '0').ToArray());
                    var increment = (Convert.ToSingle(LastID) + 1);
                    var KodLuar = Convert.ToString(increment).PadLeft(4, '0');
                    luar.HR_NO_PENSYARAH = "P" + KodLuar;
                    db.HR_PENSYARAH.Add(luar);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return View(luar);
        }

        public ActionResult EditLuar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENSYARAH luar = db.HR_PENSYARAH.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_SNEGERI);
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_TNEGERI);
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_JENIS_IND == "L"), "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_EditLuar", luar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLuar([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TNEGERI,HR_TPOSKOD,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SNEGERI,HR_SPOSKOD,HR_NO_PEKERJA")] HR_PENSYARAH luar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(luar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(luar);
        }

        public ActionResult PadamLuar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENSYARAH luar = db.HR_PENSYARAH.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_SNEGERI);
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", luar.HR_TNEGERI);
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_PadamLuar", luar);
        }

        
        [HttpPost, ActionName("PadamLuar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_PENSYARAH luar)
        {
            luar = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PENSYARAH == luar.HR_NO_PENSYARAH);

            db.HR_PENSYARAH.Remove(luar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult CariLuar(string nama, string kod)
        {
            List<HR_PENSYARAH> luar = new List<HR_PENSYARAH>();
            if ( nama != null)
            {
                luar = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH != kod && s.HR_NAMA_PENSYARAH == nama).ToList();
            }
            string msg = null;
            if (luar.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditLuar (string nama,string kod)
        {
            List<HR_PENSYARAH> luar = new List<HR_PENSYARAH>();
            if (nama != null)
            {
                luar = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH != kod && s.HR_NAMA_PENSYARAH == nama).ToList();
            }
            string msg = null;
            if (luar.Count() > 0)
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