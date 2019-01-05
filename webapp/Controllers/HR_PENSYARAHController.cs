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
    public class HR_PENSYARAHController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HR_PENSYARAH
        public ActionResult Index()
        {
            var hR_PENSYARAH = db.HR_PENSYARAH.Include(h => h.HR_KUMPULAN_PENSYARAH);
            return View(hR_PENSYARAH.ToList());
        }

        // GET: HR_PENSYARAH/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH hR_PENSYARAH = db.HR_PENSYARAH.Find(id);
            if (hR_PENSYARAH == null)
            {
                return HttpNotFound();
            }
            return View(hR_PENSYARAH);
        }

        // GET: HR_PENSYARAH/Create
        public ActionResult Create()
        {
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return View();
        }

        // POST: HR_PENSYARAH/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TPOSKOD,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH hR_PENSYARAH)
        {
            if (ModelState.IsValid)
            {
                db.HR_PENSYARAH.Add(hR_PENSYARAH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN", hR_PENSYARAH.HR_KOD_KUMPULAN);
            return View(hR_PENSYARAH);
        }

        // GET: HR_PENSYARAH/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH hR_PENSYARAH = db.HR_PENSYARAH.Find(id);
            if (hR_PENSYARAH == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN", hR_PENSYARAH.HR_KOD_KUMPULAN);
            return View(hR_PENSYARAH);
        }

        // POST: HR_PENSYARAH/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TPOSKOD,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH hR_PENSYARAH)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_PENSYARAH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN", hR_PENSYARAH.HR_KOD_KUMPULAN);
            return View(hR_PENSYARAH);
        }

        // GET: HR_PENSYARAH/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH hR_PENSYARAH = db.HR_PENSYARAH.Find(id);
            if (hR_PENSYARAH == null)
            {
                return HttpNotFound();
            }
            return View(hR_PENSYARAH);
        }

        // POST: HR_PENSYARAH/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HR_PENSYARAH hR_PENSYARAH = db.HR_PENSYARAH.Find(id);
            db.HR_PENSYARAH.Remove(hR_PENSYARAH);
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
