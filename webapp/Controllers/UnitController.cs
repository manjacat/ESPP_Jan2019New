using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class UnitController : Controller
    {
        private MajlisContext db2 = new MajlisContext();

        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
    }
}