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
    public class AgensiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: Agensi
        public ActionResult SenaraiAgensi()
        {
           
            return View(db.HR_AGENSI.ToList());
        }

        public ActionResult InfoAgensi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_AGENSI agensi = db.HR_AGENSI.Find(id);

            if (agensi == null)
            {
                return HttpNotFound();
            }
            if(agensi.HR_NEGERI != null)
            {
                agensi.HR_NEGERI = agensi.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", agensi.HR_NEGERI);
            
            return PartialView("_InfoAgensi", agensi);
        }

        public ActionResult TambahAgensi()
        {
           
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahAgensi");
        }

        [HttpPost, ActionName("TambahAgensi")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahAgensi([Bind(Include = "HR_KOD_AGENSI,HR_NAMA_AGENSI,HR_ALAMAT1,HR_ALAMAT2,HR_ALAMAT3,HR_BANDAR,HR_NEGERI,HR_POSKOD,HR_TELEFON1,HR_TELEFON2,HR_FAKS1,HR_FAKS2,HR_EMAIL,HR_PERBANKAN,HR_NO_FAIL,HR_AKTIF_IND,HR_SINGKATAN,HR_NO_FAIL_P,HR_NAMA_AHLI,HR_BANK_KOD")] HR_AGENSI agensi)
        {
            if (ModelState.IsValid)
            {
                
                    var SelectLastID = db.HR_AGENSI.OrderByDescending(s => s.HR_KOD_AGENSI).FirstOrDefault().HR_KOD_AGENSI;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == 'A' || x == '0').ToArray());
                    var Increment = Convert.ToSingle(LastID) + 1;
                    var KodAgensi = Convert.ToString(Increment).PadLeft(4, '0');
                    agensi.HR_KOD_AGENSI = "A" + KodAgensi;

                    db.HR_AGENSI.Add(agensi);
                    db.SaveChanges();
                return RedirectToAction("SenaraiAgensi");

                }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View(agensi);
        }


        public ActionResult EditAgensi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_AGENSI agensi = db.HR_AGENSI.Find(id);

            if (agensi == null)
            {
                return HttpNotFound();
            }
            if (agensi.HR_NEGERI != null)
            {
                agensi.HR_NEGERI = agensi.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", agensi.HR_NEGERI);


            return PartialView("_EditAgensi", agensi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAgensi([Bind(Include = "HR_KOD_AGENSI, HR_NAMA_AGENSI, HR_ALAMAT1, HR_ALAMAT2, HR_ALAMAT3, HR_BANDAR, HR_NEGERI, HR_POSKOD, HR_TELEFON1, HR_TELEFON2, HR_FAKS1, HR_FAKS2, HR_EMAIL, HR_PERBANKAN, HR_NO_FAIL, HR_AKTIF_IND, HR_SINGKATAN, HR_NO_FAIL_P, HR_NAMA_AHLI, HR_BANK_KOD")] HR_AGENSI agensi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agensi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiAgensi");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View(agensi);
        }

        public ActionResult PadamAgensi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_AGENSI agensi = db.HR_AGENSI.Find(id);

            if (agensi == null)
            {
                return HttpNotFound();
            }
            if (agensi.HR_NEGERI != null)
            {
                agensi.HR_NEGERI = agensi.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", agensi.HR_NEGERI);

            return PartialView("_PadamAgensi", agensi);
        }

        
        [HttpPost, ActionName("PadamAgensi")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_AGENSI agensi)
        {
            agensi = db.HR_AGENSI.SingleOrDefault(s => s.HR_KOD_AGENSI == agensi.HR_KOD_AGENSI);

            db.HR_AGENSI.Remove(agensi);
            db.SaveChanges();
            return RedirectToAction("SenaraiAgensi");
        }

        public ActionResult CariAgensi(string nama, string singkatan)
        {
            List<HR_AGENSI> agensi = new List<HR_AGENSI>();
            if ( nama != null)
            {
                agensi = db.HR_AGENSI.Where(s => s.HR_NAMA_AGENSI == nama).ToList();
            }
            if (singkatan !=null)
            {
                agensi = db.HR_AGENSI.Where(s => s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg= null;
            if (agensi.Count() > 0)
            {
                msg = "Data telah wujud"; 
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditAgensi( string singkatan, string kod, string nama)
        {
            List<HR_AGENSI> agensi = new List<HR_AGENSI>();
            if (singkatan != null)
            {
                agensi = db.HR_AGENSI.Where(s => s.HR_KOD_AGENSI != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            if (nama != null)
            {
                agensi = db.HR_AGENSI.Where(s => s.HR_KOD_AGENSI != kod && s.HR_NAMA_AGENSI == nama).ToList();
            }
            string msg = null;
            if(agensi.Count()>0)
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