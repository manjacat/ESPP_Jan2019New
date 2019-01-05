using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace eSPP.Controllers
{
    public class UserInfoesController : Controller
    {
        // GET: UserInfo
        public ActionResult UserProfile()
        {
            return View();
        }
    }
}