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
    public class AcaraController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Aktiviti
        public ActionResult SenaraiAcara()
        {
            return View(db.HR_ACARA.ToList());
        }

        public ActionResult TambahAcara()
        {
            
            return PartialView("_TambahAcara");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahAcara([Bind(Include = "HR_KOD_ACARA,HR_TAJUK,HR_WARNA,HR_ICON,HR_AKTIF_IND")] HR_ACARA aktiviti)
        {
            if (ModelState.IsValid)
            {
                HR_ACARA mAktiviti = db.HR_ACARA.OrderByDescending(s => s.HR_KOD_ACARA).FirstOrDefault();
                if (mAktiviti == null)
                {
                    mAktiviti = new HR_ACARA();
                }

                int LastID2 = 0;
                if (mAktiviti.HR_KOD_ACARA != null)
                {
                    var ListID = new string(mAktiviti.HR_KOD_ACARA.SkipWhile(x => x == 'A' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(ListID);
                }

                var Increment = LastID2 + 1;
                var kod = Convert.ToString(Increment).PadLeft(4, '0');
                aktiviti.HR_KOD_ACARA = "A" + kod;
                aktiviti.HR_AKTIF_IND = "Y";
                db.HR_ACARA.Add(aktiviti);
                db.SaveChanges();

                return RedirectToAction("SenaraiAcara");
            }

            return View(aktiviti);
        }



       

        public ActionResult AcaraInfo(string id, string jenis)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_ACARA aktiviti = db.HR_ACARA.Find(id);

            if (aktiviti == null)
            {
                return HttpNotFound();
            }
           
            return PartialView(jenis+ "Acara", aktiviti);
        }

        public ActionResult InfoAcara(string id)
        {
            return AcaraInfo(id, "_Info");
        }

        public ActionResult EditAcara(string id)
        {
            return AcaraInfo(id, "_Edit");
        }

        public ActionResult PadamAcara(string id)
        {
            return AcaraInfo(id, "_Padam");
        }


     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAcara([Bind(Include = "HR_KOD_ACARA,HR_TAJUK,HR_WARNA,HR_ICON,HR_AKTIF_IND")] HR_ACARA aktiviti)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aktiviti).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiAcara");
            }
            return View(aktiviti);
        }

      

     
        [HttpPost, ActionName("PadamAcara")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_ACARA acara)
        {
            HR_ACARA aktiviti = db.HR_ACARA.Find(acara.HR_KOD_ACARA);
            db.HR_ACARA.Remove(aktiviti);
            db.SaveChanges();
            return RedirectToAction("SenaraiAcara");
        }

       
    }
}
