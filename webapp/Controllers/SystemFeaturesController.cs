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
    public class SystemFeaturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SystemFeatures
        public ActionResult SystemFeatures()
        {
            return View(db.SystemFeatures.ToList());
        }

        // GET: SystemFeatures/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemFeature systemFeature = db.SystemFeatures.Find(id);
            if (systemFeature == null)
            {
                return HttpNotFound();
            }
            return View(systemFeature);
        }

        // GET: SystemFeatures/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: SystemFeatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SystemFeatureID,FeatureName,ControllerName,ActionName,CreateDateTime")] SystemFeature systemFeature)
        {
            if (ModelState.IsValid)
            {
                systemFeature.SystemFeatureID = Guid.NewGuid();
                systemFeature.CreateDateTime = DateTime.Now;
                db.SystemFeatures.Add(systemFeature);
                db.SaveChanges();
                return RedirectToAction("SystemFeatures");
            }

            return View(systemFeature);
        }

        // GET: SystemFeatures/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemFeature systemFeature = db.SystemFeatures.Find(id);
            if (systemFeature == null)
            {
                return HttpNotFound();
            }
            return View(systemFeature);
        }

        // POST: SystemFeatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SystemFeatureID,FeatureName,ControllerName,ActionName,CreateDateTime")] SystemFeature systemFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SystemFeatures");
            }
            return View(systemFeature);
        }

        // GET: SystemFeatures/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemFeature systemFeature = db.SystemFeatures.Find(id);
            if (systemFeature == null)
            {
                return HttpNotFound();
            }
            return View(systemFeature);
        }

        // POST: SystemFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            SystemFeature systemFeature = db.SystemFeatures.Find(id);
            db.SystemFeatures.Remove(systemFeature);
            db.SaveChanges();
            return RedirectToAction("SystemFeatures");
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
