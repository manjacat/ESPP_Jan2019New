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
    public class GajiUpahanController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        //private FMSContext db3 = new FMSContext();
        private ApplicationDbContext db3 = new ApplicationDbContext();

        // GET: HR_GAJI_UPAHAN
        public ActionResult SenaraiGaji()
        {
            
            return View(db.HR_GAJI_UPAHAN.ToList());
        }

        public ActionResult InfoGajiUpahan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_GAJI_UPAHAN upahan = db.HR_GAJI_UPAHAN.Find(id);
            if (upahan == null)
            {
                ViewBag.HR_GAJI_UPAHAN = db.HR_GAJI_UPAHAN.ToList();
                return HttpNotFound();
            }
            ViewBag.HR_KOD_KREDITOR = new SelectList(db3.AP_CREDITORMASTER, "CREDITORCODE", "CREDITORNAME");
            ViewBag.HR_JAWATAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 110), "STRING_PARAM", "SHORT_DESCRIPTION");
            return PartialView("_InfoGajiUpahan",upahan);
        }


    }
}
