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

        public static RoleManager GetByRoleId(string Guid)
        {
            int moduleId = ModuleConstant.MaklumatKakiTangan;
            RoleManager manager = new RoleManager
            {
                RoleId = Guid,
                ModuleId = moduleId
            };

            List<HtmlRole> roles = HtmlRole.GetHtmlRoles(Guid, moduleId);
            manager.HtmlRoles.AddRange(roles);

            return manager;
        }
        
        //testing
        public RoleManager GetByRoleAndModule(string roleId, int moduleId)
        {
            return null;        
        }

        public void Edit()
        {

        }

        //testing
        public HtmlRole GetHtmlRole(string htmlName)
        {
            if(htmlName == "Gaji")
            {
                Console.Write("gaji");
            }
            HtmlRole role = HtmlRoles
                .Where(s => s.HtmlName == htmlName).FirstOrDefault();
            if(role == null)
            {
                role = new HtmlRole
                {
                    RoleId = RoleId,
                    ModuleId = ModuleId,
                    HtmlName = htmlName,
                    IsView = false,
                    IsAdd = false,
                    IsEdit = false,
                    IsDelete = false
                };
            }
            return role;
        }

        //testing
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
    }
}