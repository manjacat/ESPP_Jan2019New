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
    public class SewaanAlatanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SewaanAlatan
        public ActionResult SenaraiSewaan()
        {
            return View(db.HR_SEWAAN_ALATAN.ToList());
        }

        public ActionResult InfoSewaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEWAAN_ALATAN sewaan = db.HR_SEWAAN_ALATAN.Find(id);

            if (sewaan == null)
            {
                return HttpNotFound();
            }
            
            return PartialView("_InfoSewaan", sewaan);
        }


        public ActionResult TambahSewaan()
        {
            
            ViewBag.HR_SEWAAN_ALATAN = db.HR_SEWAAN_ALATAN.ToList();
            return PartialView("_TambahSewaan");
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahSewaan([Bind(Include = "HR_KOD_ALAT,HR_PENERANGAN,HR_SINGKATAN,HR_HARGA_SEWAAN")] HR_SEWAAN_ALATAN sewaan)
        {
            if (ModelState.IsValid)
            {
                HR_SEWAAN_ALATAN mSewaan = db.HR_SEWAAN_ALATAN.OrderByDescending(s => s.HR_KOD_ALAT).FirstOrDefault();
                if (mSewaan == null)
                {
                    mSewaan = new HR_SEWAAN_ALATAN();
                }

                int LastID2 = 0;
                if (mSewaan.HR_KOD_ALAT != null)
                {
                    var ListID = new string(mSewaan.HR_KOD_ALAT.SkipWhile(x => x == 'A' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(ListID);
                }

                var Increment = LastID2 + 1;
                var kod = Convert.ToString(Increment).PadLeft(3, '0');
                sewaan.HR_KOD_ALAT = "A" + kod;
                db.HR_SEWAAN_ALATAN.Add(sewaan);
                db.SaveChanges();

                return RedirectToAction("SenaraiSewaan");
            }

            return View(sewaan);
        }




        public ActionResult EditSewaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEWAAN_ALATAN sewaan = db.HR_SEWAAN_ALATAN.Find(id);

            if (sewaan == null)
            {
                return HttpNotFound();
            }

            return PartialView("_EditSewaan", sewaan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSewaan([Bind(Include = "HR_KOD_ALAT,HR_PENERANGAN,HR_SINGKATAN,HR_HARGA_SEWAAN")] HR_SEWAAN_ALATAN sewaan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sewaan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiSewaan");
            }
            return View(sewaan);
        }

        public ActionResult PadamSewaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEWAAN_ALATAN sewaan = db.HR_SEWAAN_ALATAN.Find(id);

            if (sewaan == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PadamSewaan", sewaan);
        }

        [HttpPost, ActionName("PadamSewaan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SEWAAN_ALATAN sewaan)
        {
            sewaan = db.HR_SEWAAN_ALATAN.SingleOrDefault(s => s.HR_KOD_ALAT == sewaan.HR_KOD_ALAT);

            db.HR_SEWAAN_ALATAN.Remove(sewaan);
            db.SaveChanges();
            return RedirectToAction("SenaraiSewaan");
        }

        public ActionResult CariSewaan (string singkatan, string kod, string penerangan, decimal? harga)
        {
            List<HR_SEWAAN_ALATAN> sewaan = new List<HR_SEWAAN_ALATAN>();
            if ( singkatan != null)
            {
                sewaan = db.HR_SEWAAN_ALATAN.Where(s => s.HR_KOD_ALAT != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if( penerangan != null)
            {
                sewaan = db.HR_SEWAAN_ALATAN.Where(s => s.HR_KOD_ALAT != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
           
            string msg = null;
            if (sewaan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditSewaan(string singkatan, string kod, string penerangan, decimal? harga)
        {
            List<HR_SEWAAN_ALATAN> sewaan = new List<HR_SEWAAN_ALATAN>();
            if (singkatan != null)
            {
                sewaan = db.HR_SEWAAN_ALATAN.Where(s => s.HR_KOD_ALAT != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if (penerangan != null)
            {
                sewaan = db.HR_SEWAAN_ALATAN.Where(s => s.HR_KOD_ALAT != kod && s.HR_PENERANGAN == penerangan).ToList();
            }

            string msg = null;
            if (sewaan.Count() > 0)
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
