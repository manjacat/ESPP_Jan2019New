using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public enum ViewLevel
    {
        NoAccess = 0, //tak nampak apa2
        View = 1, //read only
        Edit = 2 //bole nampak/interact
    }

    public enum EditLevel
    {
        Add = 0,
        Edit = 1,
        Delete = 2
    }

    public static class ModuleConstant
    {
        public const string MaklumatKakiTangan = "MaklumatKakitangan";
    }
}