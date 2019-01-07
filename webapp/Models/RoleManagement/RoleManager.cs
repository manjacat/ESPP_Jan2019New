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

        public string RoleName { get; set; }
        public string RoleId { get; set; }
        List<HtmlRole> HtmlRoles { get; set; }

        public HtmlRole GetHtmlRole(string htmlName, string moduleName)
        {
            HtmlRole role = HtmlRoles
                .Where(s => s.EditLevels.Contains(EditLevel.Add)
                && s.ModuleName == moduleName
                && s.PropertyName == htmlName).FirstOrDefault();
            if(role == null)
            {
                role = new HtmlRole
                {
                    ModuleName = moduleName,
                    PropertyName = htmlName,
                    ViewLevel = ViewLevel.NoAccess,
                    EditLevels = new List<EditLevel>()
                };
            }
            return role;
        }

        public RoleManager GetSample()
        {
            RoleManager rm = new RoleManager();
            rm.RoleName = "Administrator";
            rm.RoleId = "4a205be5-cadc-446c-87c2-86c7ec5d6559";
            List<HtmlRole> roles = HtmlRole.GetSampleList();
            rm.HtmlRoles.AddRange(roles);
            return rm;
        }
    }
}