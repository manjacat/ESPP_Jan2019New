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
    public class LantikanBaruController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: LantikanBaru
        public ActionResult Index()
        {
            return View(db.HR_JUSTIFIKASI_JAWATAN_BARU.ToList());
        }
        public ActionResult Luaran()
        {
            return View();
        }
        public ActionResult Dalaman()
        {
            return View();
        }

        public ActionResult PenilaianTemuduga()
        {
            return View();
        }

        // GET: LantikanBaru/Details/5
        public ActionResult Details(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU = db.HR_JUSTIFIKASI_JAWATAN_BARU.Find(id);
            if (hR_JUSTIFIKASI_JAWATAN_BARU == null)
            {
                return HttpNotFound();
            }
            return View(hR_JUSTIFIKASI_JAWATAN_BARU);
        }

        // GET: LantikanBaru/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LantikanBaru/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HR_TARIKH_PERMOHONAN,HR_JAWATAN_DIPOHON,HR_JAWATAN_BARU_IND,HR_MAKSUD,HR_KOD_JABATAN,HR_PROGRAM,HR_AKTIVITI,HR_OBJ_AKTIVITI,HR_GRED_GAJI,HR_KLAS_PERKHIDMATAN,HR_BIDANG,HR_SKIM,HR_KOD_SKIM,HR_AGENSI_SEDIAADA,HR_AGENSI_KOSONG,HR_AGENSI_DIPOHON,HR_AKTIVITI_SEDIAADA,HR_AKTIVITI_KOSONG,HR_AKTIVITI_DIPOHON,HR_KEMUKA_IND,HR_LULUS_IND,HR_BIL_DIPOHON,HR_BIL_DIISI,HR_TARIKH_KEMUKA")] HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU)
        {
            if (ModelState.IsValid)
            {
                db.HR_JUSTIFIKASI_JAWATAN_BARU.Add(hR_JUSTIFIKASI_JAWATAN_BARU);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hR_JUSTIFIKASI_JAWATAN_BARU);
        }

        // GET: LantikanBaru/Edit/5
        public ActionResult Edit(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU = db.HR_JUSTIFIKASI_JAWATAN_BARU.Find(id);
            if (hR_JUSTIFIKASI_JAWATAN_BARU == null)
            {
                return HttpNotFound();
            }
            return View(hR_JUSTIFIKASI_JAWATAN_BARU);
        }

        // POST: LantikanBaru/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_TARIKH_PERMOHONAN,HR_JAWATAN_DIPOHON,HR_JAWATAN_BARU_IND,HR_MAKSUD,HR_KOD_JABATAN,HR_PROGRAM,HR_AKTIVITI,HR_OBJ_AKTIVITI,HR_GRED_GAJI,HR_KLAS_PERKHIDMATAN,HR_BIDANG,HR_SKIM,HR_KOD_SKIM,HR_AGENSI_SEDIAADA,HR_AGENSI_KOSONG,HR_AGENSI_DIPOHON,HR_AKTIVITI_SEDIAADA,HR_AKTIVITI_KOSONG,HR_AKTIVITI_DIPOHON,HR_KEMUKA_IND,HR_LULUS_IND,HR_BIL_DIPOHON,HR_BIL_DIISI,HR_TARIKH_KEMUKA")] HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_JUSTIFIKASI_JAWATAN_BARU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hR_JUSTIFIKASI_JAWATAN_BARU);
        }

        // GET: LantikanBaru/Delete/5
        public ActionResult Delete(DateTime id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU = db.HR_JUSTIFIKASI_JAWATAN_BARU.Find(id);
            if (hR_JUSTIFIKASI_JAWATAN_BARU == null)
            {
                return HttpNotFound();
            }
            return View(hR_JUSTIFIKASI_JAWATAN_BARU);
        }

        // POST: LantikanBaru/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(DateTime id)
        {
            HR_JUSTIFIKASI_JAWATAN_BARU hR_JUSTIFIKASI_JAWATAN_BARU = db.HR_JUSTIFIKASI_JAWATAN_BARU.Find(id);
            db.HR_JUSTIFIKASI_JAWATAN_BARU.Remove(hR_JUSTIFIKASI_JAWATAN_BARU);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
