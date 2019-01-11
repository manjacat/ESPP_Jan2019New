using System;
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

        private static HtmlRole GetSampleRole(string propertyName, ViewLevel access = ViewLevel.Edit)
        {
            HtmlRole role1 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = propertyName,
                ViewLevel = ViewLevel.Edit,
                EditLevels = new List<EditLevel>
                {
                    EditLevel.Add,
                    EditLevel.Edit
                }
            };

            if(access != ViewLevel.Edit)
            {
                role1.ViewLevel = access;
                role1.EditLevels = new List<EditLevel>();
            }
            return role1;
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
            HtmlRole role2 = GetSampleRole("value");
            HtmlRole role3 = GetSampleRole("search");

            HtmlRole role4 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = "HR_NAMA_PEKERJA",
                ViewLevel = ViewLevel.Edit,
                EditLevels = new List<EditLevel>
                {
                    EditLevel.Add,
                    EditLevel.Edit
                }
            };

            HtmlRole role5 = new HtmlRole
            {
                ModuleName = ModuleConstant.MaklumatKakiTangan,
                PropertyName = "HR_NO_KPBARU",
                ViewLevel = ViewLevel.View,
                EditLevels = new List<EditLevel>()
            };

            HtmlRole role18 = GetSampleRole("Pekerjaan");
            HtmlRole role6 = GetSampleRole("Kemahiran");
            HtmlRole role7 = GetSampleRole("Akademik", ViewLevel.View);
            HtmlRole role8 = GetSampleRole("Pewaris", ViewLevel.View);
            HtmlRole role9 = GetSampleRole("Tanggungan");
            HtmlRole role10 = GetSampleRole("Kuarters", ViewLevel.NoAccess);
            HtmlRole role11 = GetSampleRole("Gaji", ViewLevel.View);
            HtmlRole role12 = GetSampleRole("Anugerah");
            HtmlRole role13 = GetSampleRole("Persaraan", ViewLevel.NoAccess);
            HtmlRole role14 = GetSampleRole("Tatatertib", ViewLevel.NoAccess);
            HtmlRole role15 = GetSampleRole("Kematian", ViewLevel.NoAccess);
            HtmlRole role16 = GetSampleRole("Prestasi", ViewLevel.NoAccess);
            HtmlRole role17 = GetSampleRole("Cuti");          

            List<HtmlRole> htmlRoles = new List<HtmlRole>();
            htmlRoles.Add(role1);
            htmlRoles.Add(role2);
            htmlRoles.Add(role3);
            htmlRoles.Add(role4);
            htmlRoles.Add(role5);
            htmlRoles.Add(role6);
            htmlRoles.Add(role7);
            htmlRoles.Add(role8);
            htmlRoles.Add(role9);
            htmlRoles.Add(role10);
            htmlRoles.Add(role11);
            htmlRoles.Add(role12);
            htmlRoles.Add(role13);
            htmlRoles.Add(role14);
            htmlRoles.Add(role15);
            htmlRoles.Add(role16);
            htmlRoles.Add(role17);
            htmlRoles.Add(role18);
            return htmlRoles;
        }
    }
}