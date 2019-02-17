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

        public static List<ASPNETROLESHTML> GetByRoleId(string RoleId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<ASPNETROLESHTML> roles = db.ASPNETROLESHTML
                .Where(s => s.ROLEID == RoleId).ToList();
            return roles;
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
                    existingRole.ISVIEW = role.ISVIEW;
                    existingRole.ISADD = role.ISADD;
                    existingRole.ISEDIT = role.ISEDIT;
                    existingRole.ISDELETE = role.ISDELETE;
                    existingRole.CSSCLASS = role.CSSCLASS;
                    db.Entry(existingRole).State = EntityState.Modified;
                    db.SaveChanges();                    
                }
            }
        }
    }
}