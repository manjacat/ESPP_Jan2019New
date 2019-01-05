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
    public class TarikhAcaraController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TarikhAcara
        public ActionResult SenaraiTarikhAcara()
        {
            ViewBag.HR_KOD_ACARA = db.HR_ACARA.ToList();
            return View(db.HR_TARIKH_ACARA.ToList());
        }

        
        public ActionResult TambahTarikh()
        {
            
            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return PartialView("_TambahTarikh");
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahTarikh([Bind(Include = "HR_KOD_ACARA,HR_KOD_TARIKH_ACARA,HR_TARIKH,HR_WAKTU_MULA,HR_WAKTU_AKHIR,HR_KETERANGAN")] HR_TARIKH_ACARA tarikh)
        {
            if (ModelState.IsValid)
            {
                HR_TARIKH_ACARA mTarikh = db.HR_TARIKH_ACARA.OrderByDescending(s => s.HR_KOD_TARIKH_ACARA).FirstOrDefault();
                if (mTarikh == null)
                {
                    mTarikh = new HR_TARIKH_ACARA();
                }

                int LastID2 = 0;
                if (mTarikh.HR_KOD_TARIKH_ACARA != null)
                {
                    var ListID = new string(mTarikh.HR_KOD_TARIKH_ACARA.SkipWhile(x => x == 'T' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(ListID);
                }

                var Increment = LastID2 + 1;
                var kod = Convert.ToString(Increment).PadLeft(4, '0');
                tarikh.HR_KOD_TARIKH_ACARA = "T" + kod;
                
                db.HR_TARIKH_ACARA.Add(tarikh);
                db.SaveChanges();

                return RedirectToAction("SenaraiTarikhAcara");
            }

            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA","HR_TAJUK");
            return View(tarikh);
        }



        public ActionResult TarikhInfo(string id,string kod, string jenis)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            
            HR_TARIKH_ACARA tacara = db.HR_TARIKH_ACARA.SingleOrDefault(s => s.HR_KOD_ACARA == id && s.HR_KOD_TARIKH_ACARA == kod);

            if (tacara == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return PartialView(jenis + "Tarikh", tacara);
        }

        public ActionResult InfoTarikh(string id, string kod)
        {
            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return TarikhInfo(id,kod, "_Info");
        }

        public ActionResult EditTarikh(string id, string kod)
        {

            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return TarikhInfo(id,kod, "_Edit");
        }

        public ActionResult PadamTarikh(string id,string kod)
        {

            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return TarikhInfo(id,kod, "_Padam");
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTarikh([Bind(Include = "HR_KOD_ACARA,HR_KOD_TARIKH_ACARA,HR_TARIKH,HR_WAKTU_MULA,HR_WAKTU_AKHIR,HR_KETERANGAN")] HR_TARIKH_ACARA tarikh)
        {
            if (ModelState.IsValid)
            {
                

                db.Entry(tarikh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiTarikhAcara");


            }
            ViewBag.HR_KOD_ACARA = new SelectList(db.HR_ACARA, "HR_KOD_ACARA", "HR_TAJUK");
            return View(tarikh);
        }

       


        [HttpPost, ActionName("PadamTarikh")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_TARIKH_ACARA tarikh)
        {

            db.HR_TARIKH_ACARA.RemoveRange(db.HR_TARIKH_ACARA.Where(s => s.HR_KOD_ACARA == tarikh.HR_KOD_ACARA && s.HR_KOD_TARIKH_ACARA == tarikh.HR_KOD_TARIKH_ACARA));
            db.SaveChanges();
            return RedirectToAction("SenaraiTarikhAcara");
        }
    }
}
