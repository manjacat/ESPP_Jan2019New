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
    public class PanelHospitalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: HR_PANEL_HOSPITAL
        public ActionResult SenaraiPanel()
        {
          
            ViewBag.HR_NEGERI = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).ToList();

            return View(db.HR_PANEL_HOSPITAL.ToList());
        }

        public ActionResult InfoPanel(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_PANEL_HOSPITAL panel = db.HR_PANEL_HOSPITAL.SingleOrDefault(s => s.HR_KOD_HOSPITAL == id);
            if (panel == null)
            {
                return HttpNotFound();
            }
            if (panel.HR_NEGERI != null)
            {
                panel.HR_NEGERI = panel.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
     
            return PartialView("_InfoPanel", panel);
        }



        public ActionResult TambahPanel()
        {
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahPanel");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPanel([Bind(Include = "HR_KOD_HOSPITAL,HR_NAMA_HOSPITAL,HR_NEGERI")] HR_PANEL_HOSPITAL panel)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_PANEL_HOSPITAL.OrderByDescending(s => s.HR_KOD_HOSPITAL).FirstOrDefault().HR_KOD_HOSPITAL;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'H' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodHospital = Convert.ToString(Increment).PadLeft(4, '0');
                panel.HR_KOD_HOSPITAL = "H" + KodHospital;

                db.HR_PANEL_HOSPITAL.Add(panel);
                db.SaveChanges();
                return RedirectToAction("SenaraiPanel");
            }

            return View(panel);
        }


        public ActionResult EditPanel(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_PANEL_HOSPITAL panel = db.HR_PANEL_HOSPITAL.SingleOrDefault(s => s.HR_KOD_HOSPITAL == id);
            if (panel == null)
            {
                return HttpNotFound();
            }
            if (panel.HR_NEGERI != null)
            {
                panel.HR_NEGERI = panel.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_EditPanel", panel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPanel([Bind(Include = "HR_KOD_HOSPITAL,HR_NAMA_HOSPITAL,HR_NEGERI")] HR_PANEL_HOSPITAL panel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(panel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPanel");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View(panel);
        }




        public ActionResult PadamPanel(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_PANEL_HOSPITAL panel = db.HR_PANEL_HOSPITAL.SingleOrDefault(s => s.HR_KOD_HOSPITAL == id);
            if (panel == null)
            {
                return HttpNotFound();
            }
            if (panel.HR_NEGERI != null)
            {
                panel.HR_NEGERI = panel.HR_NEGERI.Trim();
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_PadamPanel", panel);
        }

  
        [HttpPost, ActionName("PadamPanel")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_PANEL_HOSPITAL panel)
        {
            panel = db.HR_PANEL_HOSPITAL.SingleOrDefault(s => s.HR_KOD_HOSPITAL == panel.HR_KOD_HOSPITAL);

            db.HR_PANEL_HOSPITAL.Remove(panel);
            db.SaveChanges();
            return RedirectToAction("SenaraiPanel");
        }



        public ActionResult CariPanel(string keterangan, string kategori)
        {
            List<HR_PANEL_HOSPITAL> panel = new List<HR_PANEL_HOSPITAL>();
            if (keterangan != null)
            {
                panel = db.HR_PANEL_HOSPITAL.Where(s => s.HR_NAMA_HOSPITAL == keterangan).ToList();
            }
            if (kategori != null)
            {
                panel = db.HR_PANEL_HOSPITAL.Where(s => s.HR_NEGERI == kategori).ToList();
            }
            string msg = null;
            if (panel.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditPanel(string keterangan, string kategori, string kod)
        {
            List<HR_PANEL_HOSPITAL> panel = new List<HR_PANEL_HOSPITAL>();
            if (keterangan != null)
            {
                panel = db.HR_PANEL_HOSPITAL.Where(s => s.HR_KOD_HOSPITAL != kod && s.HR_NAMA_HOSPITAL == keterangan).ToList();
            }
            if (kategori != null)
            {
                panel = db.HR_PANEL_HOSPITAL.Where(s => s.HR_NEGERI == kategori).ToList();
            }
            string msg = null;
            if (panel.Count() > 0)
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