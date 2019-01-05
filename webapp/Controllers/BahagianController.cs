using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class BahagianController : Controller
    {
        private MajlisContext db2 = new MajlisContext();


        // GET: Bahagian
        public ActionResult Index()
        {
            return View();
           
           
        }
    }
}