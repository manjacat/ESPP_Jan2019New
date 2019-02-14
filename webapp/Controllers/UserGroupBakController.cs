using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eSPP.Models.RoleManagement;

namespace eSPP.Controllers
{
    public class UserGroupBakController : Controller
    {
        // GET: UserGroupBak
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string id)
        {
            RoleManager manager = RoleManager.GetByRoleId(id);
            return View(manager);
        }

        [HttpPost]
        public ActionResult Edit(RoleManager manager)
        {
            if(manager.HtmlRoles.Count > 0)
            {
                HtmlRole.EditList(manager.HtmlRoles);
            }
            return View(manager);
        }
    }
}