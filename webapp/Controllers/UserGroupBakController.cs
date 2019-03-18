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

        public ActionResult EditByModule(string id, string moduleId, string tabId)
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                moduleId = "1";
            }
            int moduleIdInt = Convert.ToInt32(moduleId);
            int tabIdInt = Convert.ToInt32(tabId);
            RoleManagerByTab manager = RoleManagerByTab.GetByRoleIdAndTabId(id, moduleIdInt);
            return View(manager);
        }

        //[HttpPost]
        //public ActionResult EditByModule(RoleManagerByTab manager)
        //{
        //    if (manager.HtmlRolesWithTabs.Count > 0)
        //    {
        //        HtmlRole.EditList(manager.HtmlRolesWithTabs);
        //    }
        //    return View(manager);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditPeribadi(RoleManagerByTab manager)
        //{
        //    string test = "abc";
        //    if (manager.HtmlRolesWithTabs.Count > 0)
        //    {
        //        HtmlRole.EditList(manager.HtmlRolesWithTabs);
        //    }
        //    return RedirectToAction("EditByModule",
        //        new { id = manager.RoleId, moduleId = manager.ModuleId });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPeribadi(RoleManagerByTab manager, string Command)
        {
            string test = "abc";
            if(manager.HtmlRolesWithTabs != null && !string.IsNullOrEmpty(Command))
            {
                int tabIdint = Convert.ToInt32(Command);
                test = "def";
                HtmlRole.EditList(manager.HtmlRolesWithTabs, tabIdint);
            }

            return RedirectToAction("EditByModule",
                new { id = manager.RoleId, moduleId = manager.ModuleId });

            //if (manager.HtmlRoles.Count > 0 && manager.TabHeader != null)
            //{
            //    HtmlRole.EditList(manager);
            //}
            //return RedirectToAction("EditByModule",
            //    new { id = manager.TabHeader.RoleId, moduleId = manager.TabHeader.ModuleId });
        }
    }
}