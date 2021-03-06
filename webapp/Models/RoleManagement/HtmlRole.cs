﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace eSPP.Models.RoleManagement
{
    public class HtmlRole
    {
        public HtmlRole()
        {
            IsView = false;
            IsDelete = false;
            IsAdd = false;
            IsEdit = false;            
        }

        public string RoleId { get; set; }
        public int ModuleId { get; set; }
        public string HtmlName { get; set; }
        public string CSSClass { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }

        //only for editing
        public int TabId { get; set; }

        public static HtmlRole GetHtmlRoleByHtmlName(List<HtmlRole> HtmlRoles, string htmlName, string RoleId, int ModuleId)
        {
            HtmlRole role = HtmlRoles
                .Where(s => s.HtmlName == htmlName).FirstOrDefault();
            //kalau takde role dalam DB,
            //create satu temp Value yang takde access
            if (role == null)
            {
                role = new HtmlRole
                {
                    RoleId = RoleId,
                    ModuleId = ModuleId,
                    HtmlName = htmlName,
                    CSSClass = "noclass",
                    IsView = false,
                    IsAdd = false,
                    IsEdit = false,
                    IsDelete = false
                };
            }
            return role;
        }

        public static HtmlRole GetTabHeader(string roleId, int moduleId, int tabId)
        {
            HtmlRole tabHeader = GetHtmlRoles(roleId, moduleId, tabId)
                .Where(s => s.CSSClass == "tab").FirstOrDefault();
            if(tabHeader == null)
            {
                tabHeader = new HtmlRole
                {
                    RoleId = roleId,
                    ModuleId = moduleId,
                    HtmlName = "",
                    TabId = tabId,
                    CSSClass = "tab",
                    IsView = false,
                    IsAdd = false,
                    IsEdit = false,
                    IsDelete = false
                };
            }
            return tabHeader;
        }


        //convert ASPNETROLESHTML kepada List<HtmlRole>
        //css-class kalau null, kita defaultkan valuenya ke "noclass"
        //display kat View
        public static List<HtmlRole> GetHtmlRoles(string roleId, int moduleId, int tabId = 0)
        {
            List<ASPNETROLESHTML> roles = new List<ASPNETROLESHTML>();
            if(tabId == 0)
            {
                roles = ASPNETROLESHTML.GetByRoleId(roleId);
            }
            else
            {
                roles = ASPNETROLESHTML.GetByRoleId(roleId, moduleId, tabId);
            }
            
            List<HtmlRole> output = new List<HtmlRole>();
            foreach(ASPNETROLESHTML role in roles)
            {
                HtmlRole single = new HtmlRole();
                single.RoleId = role.ROLEID;
                single.ModuleId = role.MODULEID.Equals(DBNull.Value) ? 0 : moduleId;
                single.TabId = role.TABID.Equals(DBNull.Value) ? 0 : tabId;
                single.HtmlName = role.HTMLNAME;
                single.CSSClass = role.CSSCLASS == null ? "noclass": role.CSSCLASS;

                if (!role.ISVIEW.Equals(DBNull.Value))
                {
                    if(role.ISVIEW > 0)
                    {
                        single.IsView = true;
                    }
                }
                if (!role.ISADD.Equals(DBNull.Value))
                {
                    if (role.ISADD > 0)
                    {
                        single.IsAdd = true;
                    }
                }
                if (!role.ISEDIT.Equals(DBNull.Value))
                {
                    if (role.ISEDIT > 0)
                    {
                        single.IsEdit = true;
                    }
                }
                if (!role.ISDELETE.Equals(DBNull.Value))
                {
                    if (role.ISDELETE > 0)
                    {
                        single.IsDelete = true;
                    }
                }

                output.Add(single);
            }            
            return output;
        }

        //masa nak edit role je kita sort
        //masa nak search role nak display kat view, tak payah sort
        public static List<HtmlRole> GetHtmlRolesWithSort(string roleId, int moduleId, int tabId = 0)
        {
            List<HtmlRole> output = GetHtmlRoles(roleId, moduleId, tabId);
            HtmlRole tab = output.Where(s => s.CSSClass == "tab").FirstOrDefault();
            if(tab != null)
            {
                output.Remove(tab);
            }
            output = output.OrderBy(o => o.CSSClass).ToList();
            return output;
        }

        //convert HtmlRole kepada ASPNETROLESHTML
        //Update/Edit dalam Oracle DB
        public static int EditList(List<HtmlRoleWithTab> output, int tabId)
        {
            try
            {
                //sepatutnya output ni dapat 1 je sebab tabId ada 1 je utk setiap output
                output = output
                    .Where(s => s.TabHeader.TabId == tabId).ToList();

                //sebenarnya edit 1 je. 1 tab daripada semua tab yang tersenarai
                foreach(HtmlRoleWithTab single in output)
                {
                    List<ASPNETROLESHTML> roles = new List<ASPNETROLESHTML>();

                    ASPNETROLESHTML tabHeader = new ASPNETROLESHTML();
                    tabHeader.ROLEID = single.TabHeader.RoleId;
                    tabHeader.MODULEID = single.TabHeader.ModuleId;
                    tabHeader.HTMLNAME = single.TabHeader.HtmlName;
                    tabHeader.CSSCLASS = single.TabHeader.CSSClass;
                    tabHeader.ISVIEW = single.TabHeader.IsView ? 1 : 0;
                    tabHeader.ISADD = single.TabHeader.IsAdd ? 1 : 0;
                    tabHeader.ISEDIT = single.TabHeader.IsEdit ? 1 : 0;
                    tabHeader.ISDELETE = single.TabHeader.IsDelete ? 1 : 0;
                    tabHeader.TABID = single.TabHeader.TabId;

                    //add header to TOEDIT list;
                    roles.Add(tabHeader);

                    foreach (HtmlRole inSingle in single.HtmlRoles)
                    {
                        ASPNETROLESHTML role = new ASPNETROLESHTML();
                        role.ROLEID = inSingle.RoleId;
                        role.MODULEID = inSingle.ModuleId;
                        role.HTMLNAME = inSingle.HtmlName;
                        role.CSSCLASS = inSingle.CSSClass;
                        role.ISVIEW = inSingle.IsView ? 1 : 0;
                        role.ISADD = inSingle.IsAdd ? 1 : 0;
                        role.ISEDIT = inSingle.IsEdit ? 1 : 0;
                        role.ISDELETE = inSingle.IsDelete ? 1 : 0;
                        role.TABID = inSingle.TabId;
                        //add others to roles
                        roles.Add(role);                        
                    }
                    //Edit all the htmlRoles
                    ASPNETROLESHTML.EditList(roles);
                }



                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return 1;
            }
        }


        //convert HtmlRole kepada ASPNETROLESHTML
        //Update/Edit dalam Oracle DB
        public static int EditList(List<HtmlRole> output)
        {
            try
            {
                List<ASPNETROLESHTML> roles = new List<ASPNETROLESHTML>();
                foreach(HtmlRole single in output)
                {
                    ASPNETROLESHTML role = new ASPNETROLESHTML();
                    role.ROLEID = single.RoleId;
                    role.MODULEID = single.ModuleId;
                    role.HTMLNAME = single.HtmlName;
                    role.CSSCLASS = single.CSSClass;
                    role.ISVIEW = single.IsView ? 1 : 0;
                    role.ISADD = single.IsAdd ? 1 : 0;
                    role.ISEDIT = single.IsEdit ? 1 : 0;
                    role.ISDELETE = single.IsDelete ? 1 : 0;
                    role.TABID = single.TabId;
                    roles.Add(role);
                }
                ASPNETROLESHTML.EditList(roles);
                return 0;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return 1;
            }
        }

        //testing
        #region TestData
        private static HtmlRole GetSampleRole(string htmlName, ViewLevel vl = ViewLevel.Edit)
        {
            HtmlRole role1 = new HtmlRole
            {
                RoleId = ModuleConstant.roleAdmin,
                ModuleId = ModuleConstant.MaklumatKakiTangan,
                HtmlName = htmlName,
                IsView = true,
                IsAdd = false,
                IsEdit = false,
                IsDelete = false
            };

            switch (vl)
            {
                case (ViewLevel.NoAccess):
                    role1.IsView = false;
                    role1.IsAdd = false;
                    role1.IsEdit = false;
                    role1.IsDelete = false;
                    break;
                case (ViewLevel.View):
                    role1.IsView = true;
                    role1.IsAdd = false;
                    role1.IsEdit = false;
                    role1.IsDelete = false;
                    break;
                case (ViewLevel.Add):
                    role1.IsView = true;
                    role1.IsAdd = true;
                    role1.IsEdit = false;
                    role1.IsDelete = false;
                    break;
                case (ViewLevel.Edit):
                    role1.IsView = true;
                    role1.IsAdd = false;
                    role1.IsEdit = true;
                    role1.IsDelete = false;
                    break;
                case (ViewLevel.Delete):
                    role1.IsView = true;
                    role1.IsAdd = false;
                    role1.IsEdit = false;
                    role1.IsDelete = true;
                    break;
            }

            return role1;
        }

        public static List<HtmlRole> GetSampleList()
        {
            HtmlRole role1 = GetSampleRole("key");                
            HtmlRole role2 = GetSampleRole("value");
            HtmlRole role3 = GetSampleRole("search");
            HtmlRole role4 = GetSampleRole("HR_NAMA_PEKERJA");
            HtmlRole role5 = GetSampleRole("HR_NO_KPBARU");
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
            HtmlRole edit1 = GetSampleRole("mKakitangan-btn");

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
            htmlRoles.Add(edit1);

            htmlRoles = htmlRoles.OrderBy(o => o.HtmlName).ToList();
            return htmlRoles;
        }
        #endregion
    }
}