﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public class HtmlRole
    {
        public HtmlRole()
        {
            EditLevels = new List<EditLevel>();
        }

        public string ModuleName { get; set; }
        public string PropertyName { get; set; }
        public ViewLevel ViewLevel { get; set; }
        public List<EditLevel> EditLevels { get; set; }        

        public bool IsCanAdd()
        {
            bool check = EditLevels.Contains(EditLevel.Add);
            return check;
        }

        public static List<HtmlRole> GetSampleList()
        {
            HtmlRole role1 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = "key",
                ViewLevel = ViewLevel.Edit,
                EditLevels = new List<EditLevel>
                {
                    EditLevel.Add,
                    EditLevel.Edit
                }
            };
            HtmlRole role2 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = "value",
                ViewLevel = ViewLevel.Edit,
                EditLevels = new List<EditLevel>
                {
                    EditLevel.Add,
                    EditLevel.Edit
                }
            };
            HtmlRole role3 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = "search",
                ViewLevel = ViewLevel.Edit,
                EditLevels = new List<EditLevel>
                {
                    EditLevel.Add,
                    EditLevel.Edit
                }
            };
            List<HtmlRole> htmlRoles = new List<HtmlRole>();
            htmlRoles.Add(role1);
            htmlRoles.Add(role2);
            htmlRoles.Add(role3);
            return htmlRoles;
        }
    }
}