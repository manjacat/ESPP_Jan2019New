using Microsoft.AspNet.Identity;
using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class AuditController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Pengguna
        public ActionResult Audit()
        {
            List<AuditTrail> audit = db.AuditTrails.Where(s => s.audittype == "Login").ToList();
            ViewBag.audit = audit;
            List<AuditTrail> audit1 = db.AuditTrails.Where(s => s.audittype == "Logout").ToList();
            ViewBag.audit1 = audit1;
            List<AuditTrail> audit2 = db.AuditTrails.Where(s => s.audittype == "ChangePassword").ToList();
            ViewBag.audit2 = audit2;
			List<AuditTrail> audit3 = db.AuditTrails.Where(s => s.audittype == "Register").ToList();
			ViewBag.audit3 = audit3;
			List<AuditTrail> audit4 = db.AuditTrails.Where(s => s.audittype == "TransaksiSambilan").ToList();
			ViewBag.audit3 = audit4;
			return View();
        }

        public ActionResult Logout()
        {
            return View(db.AuditTrails.ToList().Where(s => s.audittype == "Logout"));
        }

        public ActionResult Aktiviti()
        {
            string username = User.Identity.GetUserId();
            string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            string pcname = System.Net.Dns.GetHostName();
            ViewBag.UserIPAddress = ip;
            return View(db.AuditTrails.ToList());
        }

        // POST: AuditTrail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "user_id,date,time,user_roles,user_ipaddress,changes,areaaccessed")] AuditTrail auditTrail)
        {
            if (ModelState.IsValid)
            {
                db.AuditTrails.Add(auditTrail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auditTrail);
        }

    }
}