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
    public class PCBController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: PCB
        public ActionResult SenaraiPCB()
        {
            ViewBag.HR_PCB_KOD = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102).ToList();
            ViewBag.HR_KATEGORI = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101).ToList();
            return View(db.HR_PCB.ToList());
        }

        // GET: PCB/Details/5
        public ActionResult InfoPCB(string kod, decimal? dari)
        {
            if (kod == null || dari == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            HR_PCB pcb = db.HR_PCB.SingleOrDefault(s => s.HR_PCB_KOD == kod && s.HR_GAJI_DARI == dari);

            if (pcb == null)
            {
                return HttpNotFound();
            }

            pcb.HR_PCB_KOD = new string(pcb.HR_PCB_KOD.SkipWhile(x => char.IsDigit(x)).TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());
          
            ViewBag.HR_PCB_KOD = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_InfoPCB", pcb);
        }

        public ActionResult TambahPCB()
        {
          
            ViewBag.HR_PCB_KOD = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102),"STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101),"STRING_PARAM", "SHORT_DESCRIPTION");
           
            return PartialView("_TambahPCB");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPCB([Bind(Include = "HR_PCB_KOD,HR_KATEGORI,HR_NILAI,HR_GAJI_DARI,HR_GAJI_HINGGA")] HR_PCB pcb)
        {

            if (ModelState.IsValid)
            {
                List<HR_PCB> selectpcb = db.HR_PCB.Where(s => s.HR_PCB_KOD == pcb.HR_PCB_KOD && s.HR_GAJI_DARI == pcb.HR_GAJI_DARI).ToList();
                if (selectpcb.Count() <= 0)
                {
                    pcb.HR_PCB_KOD = pcb.HR_KATEGORI + pcb.HR_PCB_KOD ;
                    db.HR_PCB.Add(pcb);
                    db.SaveChanges(); 
                }
                return RedirectToAction("SenaraiPCB");
            }
           
            return View(pcb);
        }

        // GET: PCB/Details/5
        public ActionResult EditPCB(string kod, decimal? dari)
        {
            if (kod == null || dari == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_PCB pcb = db.HR_PCB.SingleOrDefault(s => s.HR_PCB_KOD == kod && s.HR_GAJI_DARI == dari);

            if (pcb == null)
            {
                return HttpNotFound();
            }

            pcb.HR_PCB_KOD = new string(pcb.HR_PCB_KOD.SkipWhile(x => char.IsDigit(x)).TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());

            ViewBag.HR_PCB_KOD = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_EditPCB", pcb);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public ActionResult EditPCB([Bind(Include = "HR_PCB_KOD,HR_KATEGORI,HR_NILAI,HR_GAJI_DARI,HR_GAJI_HINGGA")] HR_PCB pcb)
        {
            if (ModelState.IsValid)
            {
                pcb.HR_PCB_KOD = pcb.HR_KATEGORI + pcb.HR_PCB_KOD;
                pcb.HR_PCB_KOD = pcb.HR_PCB_KOD.PadRight(5, ' ');
                List<HR_PCB> selectpcb = db.HR_PCB.Where(s => s.HR_PCB_KOD == pcb.HR_PCB_KOD && s.HR_GAJI_DARI == pcb.HR_GAJI_DARI).ToList();

                if (selectpcb.Count() <= 1)
                {
                    HR_PCB pcb2 = db.HR_PCB.SingleOrDefault(s => s.HR_PCB_KOD == pcb.HR_PCB_KOD && s.HR_GAJI_DARI == pcb.HR_GAJI_DARI);
                    pcb2.HR_NILAI = pcb.HR_NILAI; 
                    pcb2.HR_GAJI_HINGGA = pcb.HR_GAJI_HINGGA;
                    db.Entry(pcb2).State = EntityState.Modified;
                    db.SaveChanges();
                  
                }
                return RedirectToAction("SenaraiPCB");
            }
            ViewBag.HR_PCB_KOD = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(pcb);
        }



        public ActionResult PadamPCB(string kod, decimal? dari)
        {
            if (kod == null || dari == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_PCB pcb = db.HR_PCB.SingleOrDefault(s => s.HR_PCB_KOD == kod && s.HR_GAJI_DARI == dari);

            if (pcb == null)
            {
                return HttpNotFound();
            }

            ViewBag.kodPCB = pcb.HR_PCB_KOD;
            pcb.HR_PCB_KOD = new string(pcb.HR_PCB_KOD.SkipWhile(x => char.IsDigit(x)).TakeWhile(x => char.IsLetterOrDigit(x)).ToArray());

            ViewBag.HR_PCB_KOD = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_PadamPCB", pcb);
        }

        // POST: GredElaun/Delete/5
        [HttpPost, ActionName("PadamPCB")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_PCB pcb, string KodPCB)
        {

            pcb = db.HR_PCB.SingleOrDefault(s => s.HR_PCB_KOD == KodPCB && s.HR_GAJI_DARI == pcb.HR_GAJI_DARI);
            
            db.HR_PCB.Remove(pcb);
            db.SaveChanges();
            return RedirectToAction("SenaraiPCB");
        }

        public ActionResult CariPCB ( decimal? gaji,  string kod)
        {
            kod = kod.PadRight(5, ' ');
            List<HR_PCB> pcb = db.HR_PCB.Where(s => s.HR_GAJI_DARI == gaji && s.HR_PCB_KOD == kod).ToList();

            string msg = null;
            if (pcb.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult CariEditPCB(decimal? hingga, string kod, decimal? nilai)
        {
            kod = kod.PadRight(5, ' ');
            List<HR_PCB> pcb = new List<HR_PCB>();
           
            if (hingga != null)
            {
                pcb = db.HR_PCB.Where(s => s.HR_PCB_KOD == kod && s.HR_GAJI_HINGGA == hingga).ToList();
            }
            if (nilai != null)
            {
                pcb = db.HR_PCB.Where(s => s.HR_PCB_KOD == kod && s.HR_NILAI == nilai).ToList();
            }
            string msg = null;
            if (pcb.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        //JSON
        public ActionResult Kategori(int id)
        {
            db2.Configuration.ProxyCreationEnabled = false;
            List<GE_PARAMTABLE> item = new List<GE_PARAMTABLE>();
            if (id == 1)
            {
                item = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102 && s.ORDINAL == id).ToList();
            }
            else
            {
                item = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102 && s.ORDINAL != 1).ToList();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

       

    }
}
