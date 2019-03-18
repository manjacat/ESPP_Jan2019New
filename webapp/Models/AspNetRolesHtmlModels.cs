using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class AspNetRolesHtmlModels
    {
        public virtual DbSet<ASPNETROLESHTML> ASPNETROLESHTML { get; set; }
    }

    [Table("ASPNETROLESHTML")]
    public partial class ASPNETROLESHTML
    {
        [Key]
        [Column(Order = 0)]
        public string ROLEID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int MODULEID { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HTMLNAME { get; set; }
        public string CSSCLASS { get; set; }
        public int? ISVIEW { get; set; }
        public int? ISADD{ get; set; }
        public int? ISEDIT { get; set; }
        public int? ISDELETE { get; set; }
        public int? TABID { get; set; }

        public static List<ASPNETROLESHTML> GetByRoleId(string RoleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ASPNETROLESHTML> roles = db.ASPNETROLESHTML
                .Where(s => s.ROLEID == RoleId).ToList();
            return roles;
        }

        public static List<ASPNETROLESHTML> GetByRoleId(string RoleId, int moduleId, int tabId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ASPNETROLESHTML> roles = db.ASPNETROLESHTML
                .Where(s => s.ROLEID == RoleId
                && s.MODULEID == moduleId
                && s.TABID == tabId)
                .ToList();
            return roles;
        }

        //tak pakai. combine dgn EditList
        public static void EditHeader(ASPNETROLESHTML role)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ASPNETROLESHTML existingRole = db.ASPNETROLESHTML
                    .Where(s => s.MODULEID == role.MODULEID
                    && s.ROLEID == role.ROLEID
                    && s.HTMLNAME == role.HTMLNAME
                    && s.CSSCLASS == "tab"
                    && s.TABID != null).FirstOrDefault();
            if (existingRole != null)
            {
                int changeCheck = 0;
                if (role.ISVIEW != existingRole.ISVIEW)
                {
                    existingRole.ISVIEW = role.ISVIEW;
                    changeCheck++;
                }
                if (changeCheck > 0)
                {
                    db.Entry(existingRole).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static void EditList(List<ASPNETROLESHTML> roles)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            foreach(ASPNETROLESHTML role in roles)
            {
                ASPNETROLESHTML existingRole = db.ASPNETROLESHTML
                    .Where(s => s.MODULEID == role.MODULEID
                    && s.ROLEID == role.ROLEID
                    && s.HTMLNAME == role.HTMLNAME).FirstOrDefault();
                if(existingRole != null)
                {
                    int changeCheck = 0;
                    if(role.ISVIEW != existingRole.ISVIEW)
                    {
                        existingRole.ISVIEW = role.ISVIEW;
                        changeCheck++;
                    }
                    if(existingRole.ISADD != role.ISADD)
                    {
                        existingRole.ISADD = role.ISADD;
                        changeCheck++;
                    }
                    if(existingRole.ISEDIT != role.ISEDIT)
                    {
                        existingRole.ISEDIT = role.ISEDIT;
                        changeCheck++;
                    }
                    if (existingRole.ISDELETE != role.ISDELETE)
                    {
                        existingRole.ISDELETE = role.ISDELETE;
                        changeCheck++;
                    }
                    if (existingRole.CSSCLASS != role.CSSCLASS)
                    {
                        existingRole.CSSCLASS = role.CSSCLASS;
                        changeCheck++;
                    }
                    //existingRole.TABID = role.TABID;
                    if(changeCheck > 0)
                    {
                        //just tukar kalau ada change je.
                        //kalau takde change, takyah update DB
                        db.Entry(existingRole).State = EntityState.Modified;
                        db.SaveChanges();
                    }                                  
                }
            }
        }
    }
}