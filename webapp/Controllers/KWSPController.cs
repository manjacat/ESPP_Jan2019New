using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class KWSPController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: KWSP
        public ActionResult SenaraiKWSP()
        {
            return View(db.HR_KWSP.ToList());
        }

        public ActionResult InfoKWSP(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_KWSP kwsp = db.HR_KWSP.Find(id);

            if (id == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_KWSP = db.HR_KWSP.ToList();
            return PartialView("_InfoKWSP", kwsp);
        }

        

    }
}
