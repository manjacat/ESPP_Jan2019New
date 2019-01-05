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
    public class PendapatanPegawaiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PendapatanPegawai
        public ActionResult SenaraiPerubahan()
        {
            return View(db.HR_KEWANGAN8.ToList());
        }

        public ActionResult PerubahanPendapatanPegawai()
        {
            return View();
        }

        public ActionResult InfoPendapatanPegawai(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KEWANGAN8 pegawai = db.HR_KEWANGAN8.Find(id);

            if (pegawai == null)
            {
                return HttpNotFound();
            }
           
            return PartialView("_InfoPendapatanPegawai", pegawai);
        }

        public ActionResult TambahPendapatanPegawai()
        {

            
            return PartialView("_TambahPendapatanPegawai");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPendapatanPegawai([Bind(Include = "HR_KOD_KEW8,HR_PENERANGAN,HR_SINGKATAN,HR_PENYATA_IND,HR_GAJI_IND")] HR_KEWANGAN8 pegawai)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_KEWANGAN8.OrderByDescending(s => s.HR_KOD_KEW8).FirstOrDefault().HR_KOD_KEW8;
                var LastID = new string(SelectLastID.SkipWhile(x => x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodPegawai = Convert.ToString(Increment).PadLeft(5, '0');
                pegawai.HR_KOD_KEW8 = KodPegawai;

                db.HR_KEWANGAN8.Add(pegawai);
                db.SaveChanges();
                return RedirectToAction("SenaraiPerubahan");
            }

            return View(pegawai);
        }

        public ActionResult EditPendapatanPegawai(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KEWANGAN8 pegawai = db.HR_KEWANGAN8.Find(id);

            if (pegawai == null)
            {
                return HttpNotFound();
            }

            return PartialView("_EditPendapatanPegawai", pegawai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPendapatanPegawai([Bind(Include = "HR_KOD_KEW8,HR_PENERANGAN,HR_SINGKATAN,HR_PENYATA_IND,HR_GAJI_IND")] HR_KEWANGAN8 pegawai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pegawai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiPerubahan");
            }
            return View(pegawai);
        }

        public ActionResult PadamPendapatanPegawai(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_KEWANGAN8 pegawai = db.HR_KEWANGAN8.Find(id);

            if (pegawai == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PadamPendapatanPegawai", pegawai);
        }

        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamPendapatanPegawai")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_KEWANGAN8 pegawai)
        {
            pegawai = db.HR_KEWANGAN8.SingleOrDefault(s => s.HR_KOD_KEW8 == pegawai.HR_KOD_KEW8);

            db.HR_KEWANGAN8.Remove(pegawai);
            db.SaveChanges();
            return RedirectToAction("SenaraiPerubahan");
        }

        public ActionResult CariPendapatan ( string penerangan, string kod, string singkatan)
        {
            List<HR_KEWANGAN8> pegawai = new List<HR_KEWANGAN8>();
            if ( penerangan != null)
            {
                pegawai = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if ( singkatan != null)
            {
                pegawai = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if ( pegawai.Count> 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditPendapatan(string penerangan, string kod, string singkatan)
        {
            List<HR_KEWANGAN8> pegawai = new List<HR_KEWANGAN8>();
            if (penerangan != null)
            {
                pegawai = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 != kod && s.HR_PENERANGAN == penerangan).ToList();
            }
            if (singkatan != null)
            {
                pegawai = db.HR_KEWANGAN8.Where(s => s.HR_KOD_KEW8 != kod && s.HR_SINGKATAN == singkatan).ToList();
            }
            string msg = null;
            if (pegawai.Count > 0)
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