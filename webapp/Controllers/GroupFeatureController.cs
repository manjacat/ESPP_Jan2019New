using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eSPP.Controllers
{
    public class GroupFeaturesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GroupFeatures
        public ActionResult Index()
        {
            List<SystemFeature> SystemFeaturesList = db.SystemFeatures.ToList<SystemFeature>();
            List<IdentityRole> RoleList = db.Roles.ToList<IdentityRole>();

            ViewBag.SystemFeaturesList = SystemFeaturesList;
            ViewBag.RoleList = RoleList;

            return View(db.GroupFeatures.ToList());
        }

        // GET: GroupFeatures/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupFeature groupFeature = db.GroupFeatures.Find(id);
            if (groupFeature == null)
            {
                return HttpNotFound();
            }
            return View(groupFeature);
        }

        // GET: GroupFeatures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupFeatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupFeatureID,UserGroupID,SystemFeatureID,CreateDateTime")] GroupFeature groupFeature)
        {
            if (ModelState.IsValid)
            {
                groupFeature.GroupFeatureID = Guid.NewGuid();
                db.GroupFeatures.Add(groupFeature);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupFeature);
        }

        // GET: GroupFeatures/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupFeature groupFeature = db.GroupFeatures.Find(id);
            if (groupFeature == null)
            {
                return HttpNotFound();
            }
            return View(groupFeature);
        }

        // POST: GroupFeatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupFeatureID,UserGroupID,SystemFeatureID,CreateDateTime")] GroupFeature groupFeature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupFeature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupFeature);
        }

        // GET: GroupFeatures/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupFeature groupFeature = db.GroupFeatures.Find(id);
            if (groupFeature == null)
            {
                return HttpNotFound();
            }
            return View(groupFeature);
        }

        // POST: GroupFeatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            GroupFeature groupFeature = db.GroupFeatures.Find(id);
            db.GroupFeatures.Remove(groupFeature);
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
