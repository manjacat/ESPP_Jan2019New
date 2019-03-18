using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public class HtmlRoleWithTab
    {
        public HtmlRoleWithTab()
        {
            HtmlRoles = new List<HtmlRole>();
        }

        public HtmlRole TabHeader { get; set; }
        public List<HtmlRole> HtmlRoles { get; set; }
    }
}