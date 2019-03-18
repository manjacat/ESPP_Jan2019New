using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public class RoleManager
    {
        public RoleManager()
        {
            HtmlRoles = new List<HtmlRole>();
        }
        public string RoleId { get; set; }
        public int ModuleId { get; set; }
        public List<HtmlRole> HtmlRoles { get; set; }

        //sample
        public static RoleManager GetByRoleId(string Guid)
        {
            //default/testing
            int moduleId = ModuleConstant.MaklumatKakiTangan;
            RoleManager manager = GetByRoleId(Guid, moduleId);

            return manager;
        }

        public static RoleManager GetByRoleId(string Guid, int moduleId)
        {
            RoleManager manager = new RoleManager
            {
                RoleId = Guid,
                ModuleId = moduleId
            };

            List<HtmlRole> roles = HtmlRole.GetHtmlRoles(Guid, moduleId);
            manager.HtmlRoles.AddRange(roles);

            return manager;
        }

        public static RoleManager GetByRoleIdWithSort(string Guid, int moduleId, int tabId)
        {
            RoleManager manager = new RoleManager
            {
                RoleId = Guid,
                ModuleId = moduleId
            };

            List<HtmlRole> roles = HtmlRole.GetHtmlRolesWithSort(Guid, moduleId, tabId);
            manager.HtmlRoles.AddRange(roles);

            return manager;
        }

        public HtmlRole GetHtmlRole(string htmlName)
        {
            HtmlRole output = HtmlRole.GetHtmlRoleByHtmlName(HtmlRoles, htmlName, RoleId, ModuleId);
            return output;
        }

        //testing only
        private static RoleManager GetSample()
        {
            RoleManager rm = new RoleManager();
            rm.RoleId = "4a205be5-cadc-446c-87c2-86c7ec5d6559";
            rm.ModuleId = ModuleConstant.MaklumatKakiTangan;
            List<HtmlRole> roles = HtmlRole.GetSampleList();
            foreach(HtmlRole role in roles)
            {
                role.ModuleId = rm.ModuleId;
            }
            rm.HtmlRoles.AddRange(roles);
            return rm;
        }

        //testing
        public RoleManager GetByRoleAndModule(string roleId, int moduleId)
        {
            return null;
        }
    }
}