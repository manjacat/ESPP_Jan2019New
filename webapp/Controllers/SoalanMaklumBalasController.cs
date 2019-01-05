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
    public class SoalanMaklumBalasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: SoalanMaklumBalas
        public ActionResult SenaraiSoalan()
        {
            ViewBag.HR_MB_IND = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 141).ToList();
            
            return View(db.HR_SOALAN_MB.ToList());
        }

        public ActionResult InfoMaklumBalas(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SOALAN_MB maklumbalas = db.HR_SOALAN_MB.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_MB_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 141), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_InfoMaklumBalas", maklumbalas);
        }
        
        public ActionResult TambahMaklumBalas()
        {
            ViewBag.HR_MB_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 141), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_TambahMaklumBalas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahMaklumBalas([Bind(Include = "HR_KOD_MB,HR_PENERANGAN_MB,HR_MB_IND")] HR_SOALAN_MB maklumbalas)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_SOALAN_MB.OrderByDescending(s => s.HR_KOD_MB).FirstOrDefault().HR_KOD_MB;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'S' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodMaklum = Convert.ToString(Increment).PadLeft(4, '0');
                maklumbalas.HR_KOD_MB = "S" + KodMaklum;

                db.HR_SOALAN_MB.Add(maklumbalas);
                db.SaveChanges();
                return RedirectToAction("SenaraiSoalan");
            }

            return View(maklumbalas);
        }

        public ActionResult EditMaklumBalas(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SOALAN_MB maklumbalas = db.HR_SOALAN_MB.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_MB_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 141), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_EditMaklumBalas", maklumbalas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMaklumBalas([Bind(Include = "HR_KOD_MB, HR_PENERANGAN_MB, HR_MB_IND")] HR_SOALAN_MB maklumbalas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maklumbalas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SenaraiSoalan");
            }
            return View(maklumbalas);
        }


        public ActionResult PadamMaklumBalas(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SOALAN_MB maklumbalas = db.HR_SOALAN_MB.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_MB_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 141), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_PadamMaklumBalas", maklumbalas);
        }

        // POST: HR_JAWATAN/Delete/5
        [HttpPost, ActionName("PadamMaklumBalas")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SOALAN_MB maklumbalas)
        {
            maklumbalas = db.HR_SOALAN_MB.SingleOrDefault(s => s.HR_KOD_MB == maklumbalas.HR_KOD_MB);

            db.HR_SOALAN_MB.Remove(maklumbalas);
            db.SaveChanges();
            return RedirectToAction("SenaraiSoalan");
        }

        public ActionResult CariBalas (string mb, string kod, string penerangan)
        {
            List<HR_SOALAN_MB> maklumbalas = new List<HR_SOALAN_MB>();
            if ( mb != null)
            {
                maklumbalas = db.HR_SOALAN_MB.Where(s => s.HR_KOD_MB == kod && s.HR_MB_IND == mb).ToList();
            }
            if(penerangan != null )
            {
                maklumbalas = db.HR_SOALAN_MB.Where(s => s.HR_PENERANGAN_MB == penerangan).ToList();
            }
            string msg = null;
            if (maklumbalas.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CariEditBalas (string penerangan, string kod, string mb)
        {
            List<HR_SOALAN_MB> maklumbalas = new List<HR_SOALAN_MB>();
            if ( mb != null)
            {
                maklumbalas = db.HR_SOALAN_MB.Where(s => s.HR_KOD_MB == kod && s.HR_MB_IND == mb).ToList();
            }
            if ( penerangan != null)
            {
                maklumbalas = db.HR_SOALAN_MB.Where(s => s.HR_KOD_MB != kod && s.HR_PENERANGAN_MB == penerangan).ToList();
            }
            string msg = null;
            if (maklumbalas.Count() > 0)
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