﻿using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eSPP.Controllers;
using System.Data;
using System.Net;
using System.Data.Entity;

namespace eSPP.Controllers
{
    public class JadualGajiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: JadualGaji
        public ActionResult SenaraiGaji()
        {
            return View(db.HR_JADUAL_GAJI.ToList());
        }

        public ActionResult InfoJadual(string saraan, string gaji, string peringkat)
        {
            if (saraan == null || gaji == null || peringkat == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_SISTEM_SARAAN == saraan && s.HR_GRED_GAJI == gaji && s.HR_PERINGKAT == peringkat);
           
            if (jadual == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.GroupBy(c => c.HR_PERINGKAT).Select(c => c.FirstOrDefault()).OrderBy(c => c.HR_PERINGKAT), "HR_PERINGKAT", "HR_PERINGKAT");
            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");


            return PartialView("_InfoJadual", jadual);
        }

        public ActionResult TambahJadual()
        {
            ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.GroupBy(c => c.HR_PERINGKAT).Select(c => c.FirstOrDefault()).OrderBy(c => c.HR_PERINGKAT), "HR_PERINGKAT", "HR_PERINGKAT");

            List<String> disableditem = new List<string>(); //newobject
            List<HR_JADUAL_GAJI> jad = db.HR_JADUAL_GAJI.GroupBy(s => s.HR_GRED_GAJI).Select(s => s.FirstOrDefault()).ToList(); //selectandgroupby
            foreach (var item in jad) //loop 
            {
                var selectjadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == item.HR_GRED_GAJI).Count(); // selectgredgaji

                if (selectjadual >= 9)
                {
                    disableditem.Add(item.HR_GRED_GAJI); //masukkandata
                    jad = new List<HR_JADUAL_GAJI>();

                }
            }
            string[] gred = disableditem.ToArray(); //masukkandatadalamgred



            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION", null, null, gred);


            return PartialView("_TambahJadual");
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahJadual([Bind(Include = "HR_SISTEM_SARAAN, HR_GRED_GAJI, HR_PERINGKAT, HR_GAJI_POKOK, HR_GAJI_MIN, HR_GAJI_MAX, HR_RM_KENAIKAN, HR_PERATUS_KENAIKAN, HR_KOD_GAJI, HR_AKTIF_IND, HR_KANAN_IND")] HR_JADUAL_GAJI jadual)
        {
            if (ModelState.IsValid)
            {
                   var selectJadual = db.HR_JADUAL_GAJI.Where(s=> s.HR_GRED_GAJI == jadual.HR_GRED_GAJI).Count(); //selectandgroupby
                    //var SelectLastID = db.HR_JADUAL_GAJI.GroupBy(s=> s.HR_PERINGKAT ).Select(c => c.FirstOrDefault()).Count();
                   var Increment = selectJadual + 1 ;
                   jadual.HR_PERINGKAT = "P" + Increment;
                   db.HR_JADUAL_GAJI.Add(jadual);
                   db.SaveChanges();
           
                return RedirectToAction("SenaraiGaji");
            }
            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");

            return View();
        }

        
        
        public ActionResult EditJadual(string saraan, string gaji, string peringkat)
        {
            if (saraan == null || gaji == null || peringkat == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_SISTEM_SARAAN == saraan && s.HR_GRED_GAJI == gaji && s.HR_PERINGKAT == peringkat);

            if (jadual == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.GroupBy(c => c.HR_PERINGKAT).Select(c => c.FirstOrDefault()).OrderBy(c => c.HR_PERINGKAT), "HR_PERINGKAT", "HR_PERINGKAT");
            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            return PartialView("_EditJadual", jadual);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJadual([Bind(Include = "HR_SISTEM_SARAAN, HR_GRED_GAJI, HR_PERINGKAT, HR_GAJI_POKOK, HR_GAJI_MIN, HR_GAJI_MAX, HR_RM_KENAIKAN, HR_PERATUS_KENAIKAN, HR_KOD_GAJI, HR_AKTIF_IND, HR_KANAN_IND")] HR_JADUAL_GAJI jadual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jadual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiGaji");
            }
           
            return View(jadual);
        }
           
        
        public ActionResult PadamJadual(string saraan, string gaji, string peringkat)
        {
            if (saraan == null || gaji == null || peringkat == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_JADUAL_GAJI jadual = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_SISTEM_SARAAN == saraan && s.HR_GRED_GAJI == gaji && s.HR_PERINGKAT == peringkat);

            if (jadual == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_PERINGKAT = new SelectList(db.HR_JADUAL_GAJI.GroupBy(c => c.HR_PERINGKAT).Select(c => c.FirstOrDefault()).OrderBy(c => c.HR_PERINGKAT), "HR_GRED_GAJI", "HR_PERINGKAT");
            ViewBag.HR_GRED_GAJI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == "SSM"), "ORDINAL", "SHORT_DESCRIPTION");
            return PartialView("_PadamJadual", jadual);
        }

        // POST: JenisPeperiksaan/Delete/5
        [HttpPost, ActionName("PadamJadual")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_JADUAL_GAJI jadual)
        {
            jadual = db.HR_JADUAL_GAJI.SingleOrDefault(s => s.HR_SISTEM_SARAAN == jadual.HR_SISTEM_SARAAN && s.HR_GRED_GAJI == jadual.HR_GRED_GAJI && s.HR_PERINGKAT == jadual.HR_PERINGKAT);

            db.HR_JADUAL_GAJI.Remove(jadual);
            db.SaveChanges();
            return RedirectToAction("SenaraiGaji");
        }

        public ActionResult CariGaji ( string gaji, string peringkat, string kod)
        {
            List<HR_JADUAL_GAJI> jadual = new List<HR_JADUAL_GAJI>();
            if (gaji != null)
            {
                jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gaji).ToList();
            }
            if ( peringkat != null)
            {
                jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == gaji && s.HR_PERINGKAT == peringkat).ToList();
            }
            if (kod != null)
            {
                jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_KOD_GAJI == kod).ToList();
            }
            string msg = null;
            if (jadual.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditGaji(string gred, string kod)
        {
            List<HR_JADUAL_GAJI> jadual = new List<HR_JADUAL_GAJI>();

            if (kod != null)
            {
                jadual = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI != gred && s.HR_KOD_GAJI == kod).ToList();
            }
            string msg = null;
            if (jadual.Count() > 0)
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