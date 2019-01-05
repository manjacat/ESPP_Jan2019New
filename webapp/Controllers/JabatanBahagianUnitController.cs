using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using static eSPP.Controllers.AccountController;

namespace eSPP.Controllers
{
    public class JabatanBahagianUnitController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: JabatanBahagianUnit
        public ActionResult SenaraiJBU(ManageMessageId? message)
        {

            ViewBag.StatusMessage =
                message == ManageMessageId.DeleteError ? "Terdapat Unit dan Bahagian Di Dalam Jabatan Ini. Sila Buang Unit Dahulu Kemudian Bahagian."
                : message == ManageMessageId.DeleteError1 ? "Terdapat Unit atau Bahagian Di Dalam Jabatan Ini. Sila Buang Unit Dahulu Kemudian Bahagian."
                : message == ManageMessageId.DeleteSuccess ? "Jabatan Telah Berjaya Dibuang."
                : message == ManageMessageId.DeleteBahagian ? "Bahagian Telah Berjaya Dibuang."
                : message == ManageMessageId.DeleteUnit ? "Unit Telah Berjaya Dibuang."
                : "";

            List<GE_BAHAGIAN> bahagian = db2.GE_BAHAGIAN.ToList();
            List<GE_JABATAN> jabatan = db2.GE_JABATAN.ToList();
            List<GE_UNIT> unit = db2.GE_UNIT.ToList();
            JabatanBahagianUnitModels JBU = new JabatanBahagianUnitModels();

            JBU.GE_JABATAN = jabatan;
            JBU.GE_BAHAGIAN = bahagian;
            JBU.GE_UNIT = unit;
            return View(JBU);
        }
          
 //InfoJabatan
        public ActionResult InfoJabatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_JABATAN jabatan = db2.GE_JABATAN.Find(id);

            if (jabatan == null)
            {
                return HttpNotFound();
            }
            if (jabatan.GE_NEGERI != null)
            {
                jabatan.GE_NEGERI = jabatan.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", jabatan.GE_NEGERI);

            ViewBag.GE_JABATAN = db2.GE_JABATAN.ToList();
            return PartialView("_InfoJabatan", jabatan);
        }

        
        //TambahJabatan-GET
        public ActionResult TambahJabatan()
        {

            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahJabatan");
        }

        
        //TambahJabatan-POST

        [HttpPost, ActionName("TambahJabatan")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahJabatan([Bind(Include = "GE_KOD_JABATAN,GE_KETERANGAN_JABATAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_JABATAN jabatan)
        {
            if (ModelState.IsValid)
            {
                List<GE_JABATAN> selectJabatan = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN).ToList();
                if (selectJabatan.Count() <= 0)
                {
                    db2.GE_JABATAN.Add(jabatan);
                    db2.SaveChanges();
                }
                return RedirectToAction("SenaraiJBU");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View();
        }


        //EditJabatan-GET

        public ActionResult EditJabatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_JABATAN jabatan = db2.GE_JABATAN.Find(id);

            if (jabatan == null)
            {
                return HttpNotFound();
            }
            if (jabatan.GE_NEGERI != null)
            {
                jabatan.GE_NEGERI = jabatan.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", jabatan.GE_NEGERI);

            ViewBag.GE_JABATAN = db2.GE_JABATAN.ToList();
            return PartialView("_EditJabatan", jabatan);
        }

        //EditJabatan-POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJabatan([Bind(Include = "GE_KOD_JABATAN,GE_KETERANGAN_JABATAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_JABATAN jabatan)
        {
            if (ModelState.IsValid)
            {
                db2.Entry(jabatan).State = EntityState.Modified;
                db2.SaveChanges();
                return RedirectToAction("SenaraiJBU");
            }
            return View(jabatan);
        }

        //PadamJabatan-GET

        public ActionResult PadamJabatan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_JABATAN jabatan = db2.GE_JABATAN.Find(id);

            if (jabatan == null)
            {
                return HttpNotFound();
            }
            if (jabatan.GE_NEGERI != null)
            {
                jabatan.GE_NEGERI = jabatan.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", jabatan.GE_NEGERI);

            ViewBag.GE_JABATAN = db2.GE_JABATAN.ToList();
            return PartialView("_PadamJabatan", jabatan);
        }

        //PadamJabatan-POST

        // POST: Potongan/Delete/5
        [HttpPost, ActionName("PadamJabatan")]
        [ValidateAntiForgeryToken] 
        public ActionResult DeleteConfirmed(GE_JABATAN jabatan, ManageMessageId? message)
        {
            var bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN).ToList();
            var unit = db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN).ToList();
            if(bahagian.Count > 0 && unit.Count > 0)
            {
                return RedirectToAction("SenaraiJBU", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteError });
            }
            if (unit.Count > 0 || bahagian.Count > 0)
            {
                return RedirectToAction("SenaraiJBU", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteError1 });
            }
            if (unit.Count <= 0 && bahagian.Count <= 0)
            {
                jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == jabatan.GE_KOD_JABATAN);

                db2.GE_JABATAN.Remove(jabatan);
                db2.SaveChanges();
            }
            return RedirectToAction("SenaraiJBU", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteSuccess });
        }


        //InfoBahagian
        public ActionResult InfoBahagian(string id, string kod)
        {
            if (id == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_BAHAGIAN == id && s.GE_KOD_JABATAN == kod);

            if (bahagian == null)
            {
                return HttpNotFound();
            }
            if (bahagian.GE_NEGERI != null)
            {
                bahagian.GE_NEGERI = bahagian.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", bahagian.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
          
            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_InfoBahagian", bahagian);
        }

 

 //TambahBahagian-GET
        public ActionResult TambahBahagian()
        {

            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            return PartialView("_TambahBahagian");
        }


 //TambahBahagian-POST

        [HttpPost, ActionName("TambahBahagian")]
        [ValidateAntiForgeryToken]
        public ActionResult TambahBahagian([Bind(Include = "GE_KOD_BAHAGIAN,GE_KOD_JABATAN,GE_KETERANGAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_BAHAGIAN bahagian)
        {
            if (ModelState.IsValid)
            {
                List<GE_BAHAGIAN> selectBahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.GE_KOD_JABATAN == bahagian.GE_KOD_JABATAN).ToList();
                if (selectBahagian.Count() <= 0)
                {
                    db2.GE_BAHAGIAN.Add(bahagian);
                    db2.SaveChanges();
                }
                return RedirectToAction("SenaraiJBU");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View();
        }


  //EditBahagian
        public ActionResult EditBahagian(string id, string kod)
        {
            if (id == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_BAHAGIAN == id && s.GE_KOD_JABATAN == kod);

            if (bahagian == null)
            {
                return HttpNotFound();
            }
            if (bahagian.GE_NEGERI != null)
            {
                bahagian.GE_NEGERI = bahagian.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", bahagian.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");

            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_EditBahagian", bahagian);
        }


 //EditBahagian-POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBahagian([Bind(Include = "GE_KOD_BAHAGIAN,GE_KOD_JABATAN,GE_KETERANGAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_BAHAGIAN bahagian)
        {
            if (ModelState.IsValid)
            {
                db2.Entry(bahagian).State = EntityState.Modified;
                db2.SaveChanges();
                return RedirectToAction("SenaraiJBU");
            }
            return View(bahagian);
        }

 //PadamBahagian
        public ActionResult PadamBahagian(string id, string kod)
        {
            if (id == null || kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_BAHAGIAN == id && s.GE_KOD_JABATAN == kod);

            if (bahagian == null)
            {
                return HttpNotFound();
            }
            if (bahagian.GE_NEGERI != null)
            {
                bahagian.GE_NEGERI = bahagian.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", bahagian.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");

            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_PadamBahagian", bahagian);
        }

        //PadamBahagian-POST


      /*  [HttpPost, ActionName("PadamBahagian")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(GE_BAHAGIAN bahagian, ManageMessageId? message)
            {
                var unit = db2.GE_UNIT.Where(s => s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN).ToList();
                if (unit.Count > 0 )
                {
                    return RedirectToAction("Index", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteErrorBahagian});
                }
                if (unit.Count <= 0 )
                {
                   bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.GE_KOD_JABATAN == bahagian.GE_KOD_JABATAN);

                    db2.GE_BAHAGIAN.Remove(bahagian);
                    db2.SaveChanges();
                }
                return RedirectToAction("Index", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteBahagian});
            }*/

        // POST: Potongan/Delete/5

         [HttpPost, ActionName("PadamBahagian")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(GE_BAHAGIAN bahagian)
        {
            bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_BAHAGIAN == bahagian.GE_KOD_BAHAGIAN && s.GE_KOD_JABATAN == bahagian.GE_KOD_JABATAN);

            db2.GE_BAHAGIAN.Remove(bahagian);
            db2.SaveChanges();
            return RedirectToAction("SenaraiJBU", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteBahagian });
        }





        //InfoUnit
        public ActionResult InfoUnit(string id, string kod, string kodunit)
        {
            if (id == null || kod == null || kodunit == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_UNIT unit = db2.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == id && s.GE_KOD_BAHAGIAN == kod && s.GE_KOD_UNIT == kodunit);

            if (unit == null)
            {
                return HttpNotFound();
            }
            if (unit.GE_NEGERI != null)
            {
                unit.GE_NEGERI = unit.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", unit.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.GE_KOD_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN, "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_InfoUnit", unit);
        }


        //TambahUnit-GET
        public ActionResult TambahUnit()
        {

            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.GE_KOD_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN, "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            return PartialView("_TambahUnit");
        }

        //TambahUnit-POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahUnit([Bind(Include = "GE_KOD_UNIT,GE_KOD_BAHAGIAN,GE_KOD_JABATAN,GE_KETERANGAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_UNIT unit)
        {
            if (ModelState.IsValid)
            {
                List<GE_UNIT> selectUnit = db2.GE_UNIT.Where(s => s.GE_KOD_UNIT == unit.GE_KOD_UNIT && s.GE_KOD_JABATAN == unit.GE_KOD_JABATAN && s.GE_KOD_BAHAGIAN == unit.GE_KOD_BAHAGIAN).ToList();
                if (selectUnit.Count() <= 0)
                {
                    db2.GE_UNIT.Add(unit);
                    db2.SaveChanges();
                }
                return RedirectToAction("SenaraiJBU");
            }
            ViewBag.HR_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return View();
        }

      

        //EditUnit-GET
        public ActionResult EditUnit(string id, string kod, string kodunit)
        {
            if (id == null || kod == null || kodunit == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_UNIT unit = db2.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == id && s.GE_KOD_BAHAGIAN == kod && s.GE_KOD_UNIT == kodunit);

            if (unit == null)
            {
                return HttpNotFound();
            }
            if (unit.GE_NEGERI != null)
            {
                unit.GE_NEGERI = unit.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", unit.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.GE_KOD_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN, "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_EditUnit", unit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUnit([Bind(Include = "GE_KOD_UNIT,GE_KOD_BAHAGIAN,GE_KOD_JABATAN,GE_KETERANGAN,GE_ALAMAT1,GE_ALAMAT2,GE_ALAMAT3,GE_BANDAR,GE_POSKOD,GE_NEGERI,GE_TELPEJABAT1,GE_TELPEJABAT2,GE_FAKS1,GE_FAKS2,GE_EMAIL,GE_NO_KETUA,GE_SINGKATAN,GE_AKTIF_IND")] GE_UNIT unit)
        {
            if (ModelState.IsValid)
            {
                db2.Entry(unit).State = EntityState.Modified;
                db2.SaveChanges();
                return RedirectToAction("SenaraiJBU");
            }
            return View(unit);
        }


        //PadamUnit-GET
        public ActionResult PadamUnit(string id, string kod, string kodunit)
        {
            if (id == null || kod == null || kodunit == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            GE_UNIT unit = db2.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == id && s.GE_KOD_BAHAGIAN == kod && s.GE_KOD_UNIT == kodunit);

            if (unit == null)
            {
                return HttpNotFound();
            }
            if (unit.GE_NEGERI != null)
            {
                unit.GE_NEGERI = unit.GE_NEGERI.Trim();
            }
            ViewBag.GE_NEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", unit.GE_NEGERI);
            ViewBag.GE_KOD_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.GE_KOD_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN, "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.GE_BAHAGIAN = db2.GE_BAHAGIAN.ToList();
            return PartialView("_PadamUnit", unit);
        }


        //PadamUnit-POST

        // POST: Potongan/Delete/5
        [HttpPost, ActionName("PadamUnit")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(GE_UNIT unit)
        {
            unit = db2.GE_UNIT.SingleOrDefault(s => s.GE_KOD_JABATAN == unit.GE_KOD_JABATAN && s.GE_KOD_BAHAGIAN == unit.GE_KOD_BAHAGIAN && s.GE_KOD_UNIT == unit.GE_KOD_UNIT);

            db2.GE_UNIT.Remove(unit);
            db2.SaveChanges();
            return RedirectToAction("SenaraiJBU", "JabatanBahagianUnit", new { Message = ManageMessageId.DeleteUnit });
        }


        public ActionResult CariJabatan (string kod, string singkatan, string keterangan)
        {
            List<GE_JABATAN> jabatan = new List<GE_JABATAN>();
            if ( kod != null)
            {
                jabatan = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == kod).ToList();
            }
            if (singkatan != null)
            {
                jabatan = db2.GE_JABATAN.Where(s => s.GE_SINGKATAN == singkatan).ToList();
            }
            if (keterangan != null)
            {
                jabatan = db2.GE_JABATAN.Where(s => s.GE_KETERANGAN_JABATAN == keterangan).ToList();
            }
            
            string msg = null;
            if (jabatan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CariEditJabatan(string singkatan, string kod, string keterangan)
        {
            List<GE_JABATAN> jabatan = new List<GE_JABATAN>();
            if (singkatan != null)
            {
                jabatan = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN != kod && s.GE_SINGKATAN == singkatan).ToList();
            }
            if (keterangan != null)
            {
                jabatan = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN != kod && s.GE_KETERANGAN_JABATAN == keterangan).ToList();
            }
            string msg = null;
            if (jabatan.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        
        public ActionResult CariBahagian (string kodbahagian, string kodjabatan, string keterangan)
        {
           
            List<GE_BAHAGIAN> bahagian = new List<GE_BAHAGIAN>();
            if (kodjabatan != null)
            {
                bahagian = db2.GE_BAHAGIAN.Where(s =>  s.GE_KOD_JABATAN == kodjabatan && s.GE_KOD_BAHAGIAN == kodbahagian).ToList();
            }
            if (kodbahagian != null)
            {
                bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == kodjabatan && s.GE_KOD_BAHAGIAN == kodbahagian).ToList();
            }
            
            if (keterangan != null)
            {
                bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN != kodbahagian && s.GE_KOD_JABATAN != kodjabatan && s.GE_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (bahagian.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CariEditBahagian( string keterangan, string singkatan, string kod, string kodjab)
        {

            List<GE_BAHAGIAN> bahagian = new List<GE_BAHAGIAN>();
            if (singkatan != null)
            {
                bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN != kod && s.GE_SINGKATAN == singkatan ).ToList();
            }
            if (keterangan != null)
            {
                bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN != kodjab && s.GE_KETERANGAN == keterangan ).ToList();
            }
            string msg = null;
            if (bahagian.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariUnit(string kodbahagian, string kodjabatan, string kodunit, string singkatan, string keterangan)
        {

            List<GE_UNIT> unit = new List<GE_UNIT>();
            if (kodjabatan != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == kodjabatan && s.GE_KOD_BAHAGIAN == kodbahagian && s.GE_KOD_UNIT == kodunit).ToList();
            }
            if (kodbahagian != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == kodjabatan && s.GE_KOD_BAHAGIAN == kodbahagian && s.GE_KOD_UNIT == kodunit).ToList();
            }
            if (kodunit != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == kodjabatan && s.GE_KOD_BAHAGIAN == kodbahagian && s.GE_KOD_UNIT == kodunit).ToList();
            }
            
            if (singkatan != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_UNIT != kodunit && s.GE_SINGKATAN == singkatan).ToList();
            }
            if (keterangan != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_UNIT != kodunit && s.GE_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (unit.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariEditUnit( string kodunit, string singkatan, string keterangan)
        {

            List<GE_UNIT> unit = new List<GE_UNIT>();
            if (singkatan != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_UNIT != kodunit && s.GE_SINGKATAN == singkatan).ToList();
            }
            if (keterangan != null)
            {
                unit = db2.GE_UNIT.Where(s => s.GE_KOD_UNIT != kodunit && s.GE_KETERANGAN == keterangan).ToList();
            }
            string msg = null;
            if (unit.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public enum ManageMessageId
        {
            DeleteError,
            DeleteSuccess,
            DeleteError1,
            DeleteUnit,
            DeleteBahagian,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ResetPassword,
            Error
        }
    }
}