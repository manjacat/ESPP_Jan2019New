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
            ViewBag.RoleId = id;
            return View();
        }

        public ActionResult EditByModule(string id, string moduleId)
        {
            ViewBag.RoleId = id;
            if (string.IsNullOrEmpty(moduleId))
            {
                moduleId = "1";
            }
            int moduleIdInt = Convert.ToInt32(moduleId);
            RoleManagerByTab manager = RoleManagerByTab.GetByRoleIdAndTabId(id, moduleIdInt);
            return View(manager);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPeribadi(RoleManagerByTab manager, string Command)
        {
            if(manager.HtmlRolesWithTabs != null && !string.IsNullOrEmpty(Command))
            {
                //convert tabId to int
                int tabIdint = Convert.ToInt32(Command);
                //save changes
                HtmlRole.EditList(manager.HtmlRolesWithTabs, tabIdint);
            }

            return RedirectToAction("EditByModule",
                new { id = manager.RoleId, moduleId = manager.ModuleId });
        }
    }
}