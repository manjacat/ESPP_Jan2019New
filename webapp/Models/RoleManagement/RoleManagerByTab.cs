using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public class RoleManagerByTab
    {
        public RoleManagerByTab()
        {
            HtmlRolesWithTabs = new List<HtmlRoleWithTab>();
        }

        public string RoleId { get; set; }
        public int ModuleId { get; set; }
        public List<HtmlRoleWithTab> HtmlRolesWithTabs { get; set; }

        public static RoleManagerByTab GetByRoleIdAndTabId(string Guid, int moduleId)
        {
            RoleManagerByTab manager = new RoleManagerByTab();

            manager.RoleId = Guid;
            manager.ModuleId = moduleId;

            ApplicationDbContext db = new ApplicationDbContext();
            List<int> tabIdList = db.ASPNETROLESHTML
                .Where(s => s.MODULEID == moduleId
                && s.ROLEID == Guid
                && s.TABID != null)
                .Select(s => s.TABID.Value).Distinct().ToList();

            foreach(int tid in tabIdList)
            {
                HtmlRoleWithTab tab1 = new HtmlRoleWithTab();
                HtmlRole tabHeader = HtmlRole.GetTabHeader(Guid, moduleId, tid);
                List<HtmlRole> roles = HtmlRole.GetHtmlRolesWithSort(Guid, moduleId, tid);
                tab1.TabHeader = tabHeader;
                tab1.HtmlRoles = roles;
                manager.HtmlRolesWithTabs.Add(tab1);
            }

            List<HtmlRoleWithTab> sorted = manager
                .HtmlRolesWithTabs.OrderBy(s => s.TabHeader.TabId).ToList();
            manager.HtmlRolesWithTabs = sorted;
            return manager;
        }
    }
}