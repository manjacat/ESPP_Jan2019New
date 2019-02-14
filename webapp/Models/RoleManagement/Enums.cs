using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public enum ViewLevel
    {
        NoAccess = 0,
        View = 1,
        Add = 2,
        Edit = 3,
        Delete = 4
    }
}