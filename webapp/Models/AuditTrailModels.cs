using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSPP.Models
{
    public class AuditTrailModels
    {
        public virtual DbSet<AuditTrail> AuditTrails { get; set; }

        public void Log(string Email, string UserName, string IpAddress, string UserRole, string Reason, string PcName, string PhoneNumber, string area, string type)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            //AuditTrail Search = db.AuditTrail.Where(i => i.login == Email).SingleOrDefault();
            //if (Search != null)
            //{
            //Search.time = DateTime.Now.ToShortTimeString();
            //Search.date = DateTime.Now.ToShortDateString();
            //Search.user_ipaddress = IpAddress;
            //Search.changes = Reason;
            //db.Entry(Search).State = EntityState.Modified;
            //db.SaveChanges();
            //} else {
            AuditTrail tbl = new AuditTrail();
            tbl.user_email = Email;
            tbl.username = UserName;
            tbl.phonenumber = PhoneNumber;
            tbl.masa = DateTime.Now.ToShortDateString();
            tbl.tarikh = DateTime.Now.ToShortTimeString();
            tbl.user_ipaddress = IpAddress;
            tbl.pcname = PcName;
            tbl.user_roles = UserRole;
            tbl.changes = Reason;
            tbl.AreaAccessed = area;
            tbl.audittype = type;
            db.AuditTrails.Add(tbl);
            db.SaveChanges();
            //}
        }

    }

    [Table("AUDITTRAILS")]
    public class AuditTrail
    {
        [Key]
        [Column("USER_ID")]
        public int user_id { get; set; }
        [Column("TARIKH")]
        public string tarikh { get; set; }
        [Column("MASA")]
        public string masa { get; set; }
        [Column("USER_ROLES")]
        public string user_roles { get; set; }
        [Column("USER_IPADDRESS")]
        public string user_ipaddress { get; set; }
        [Column("CHANGES")]
        public string changes { get; set; }
        [Column("USER_EMAIL")]
        public string user_email { get; set; }
        [Column("USERNAME")]
        public string username { get; set; }
        [Column("PCNAME")]
        public string pcname { get; set; }
        [Column("PHONENUMBER")]
        public string phonenumber { get; set; }
        [Column("AREAACCESSED")]
        public string AreaAccessed { get; set; }
        [Column("AUDITTYPE")]
        public string audittype { get; set; }
    }
}