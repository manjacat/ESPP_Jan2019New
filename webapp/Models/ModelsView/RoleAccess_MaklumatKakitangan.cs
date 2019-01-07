using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.ModelsView
{
    public enum RoleAccess
    {
        NoAccess = 0,
        View = 1,
        Add = 2,
        Edit  = 3,
        Delete = 4
    }

    public class HtmlRole
    {
        public string Name { get; set; }
        public RoleAccess RoleAccess { get; set; }
    }

    public class RoleList
    {
        public RoleList()
        {
            HtmlRoles = new List<HtmlRole>();
        }
        public List<HtmlRole> HtmlRoles { get; set; }

        public HtmlRole GetHtmlRole(string propertyName)
        {
            HtmlRole role = HtmlRoles.Where(s => s.Name.ToLower() == propertyName.ToLower()).FirstOrDefault();
            if(role == null)
            {
                role = new HtmlRole
                {
                    Name = propertyName,
                    RoleAccess = RoleAccess.NoAccess
                };
            }
            return role;
    }

        public RoleList GetSample()
        {
            RoleList roleList = new RoleList();
            HtmlRole role1 = new HtmlRole
            {
                Name = "key",
                RoleAccess = RoleAccess.Edit
            };
            HtmlRole role2 = new HtmlRole
            {
                Name = "value",
                RoleAccess = RoleAccess.Edit
            };
            HtmlRole role3 = new HtmlRole
            {
                Name = "search",
                RoleAccess = RoleAccess.Edit
            };
            roleList.HtmlRoles.Add(role1);
            roleList.HtmlRoles.Add(role2);
            roleList.HtmlRoles.Add(role3);
            return roleList;
        }
    }

    //public class RoleAccess_MaklumatKakitangan
    //{
    //    public RoleAccess_MaklumatKakitangan()
    //    {

    //    }

    //    public RoleList roleAccess { get; set; }
    //    public MaklumatKakitanganModels maklumatKakitanganModels { get; set; }
    //}
}