using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class UserInfoModels
    {
        public UserInfoModels() { }
        public virtual DbSet<UserInfo> UserInfoes { get; set; }
    }

    [Table("USERINFOES")]
    public class UserInfo
    {
        [Column("USERINFOID")]
        public Guid UserInfoID { get; set; }
        [Column("USERID")]
        public Guid UserID { get; set; }
        [Column("POSITION")]
        public string Position { get; set; }
        [Column("EMAIL")]
        public string Email { get; set; }
        [Column("PHONENO")]
        public string PhoneNo { get; set; }
        [Column("NOKP")]
        public string NoKP { get; set; }
        [Column("DOB")]
        public string DOB { get; set; }
        [Column("AGE")]
        public string Age { get; set; }
        [Column("SEX")]
        public string Sex { get; set; }
        [Column("RELIGION")]
        public string Religion { get; set; }
        [Column("RACE")]
        public string Race { get; set; }
        [Column("MSTATUS")]
        public string MStatus { get; set; }
        [Column("FAXNO")]
        public string FaxNo { get; set; }
        [Column("USERNAME")]
        public string username { get; set; }
        [Column("ADDRESS2")]
        public string Address2 { get; set; }
        [Column("ADDRESS3")]
        public string Address3 { get; set; }
        [Column("MAILADDRESS")]
        public string MailAddress { get; set; }
        [Column("MAILADDRESS2")]
        public string MailAddress2 { get; set; }
        [Column("MAILADDRESS3")]
        public string MailAddress3 { get; set; }
        [Column("MAILPOSTCODE")]
        public string MailPostCode { get; set; }
        [Column("MAILSTATE")]
        public string MailState { get; set; }
        [Column("MAILCITY")]
        public string MailCity { get; set; }
        [Column("LICENSE")]
        public string License { get; set; }
        [Column("CARMODEL")]
        public string CarModel { get; set; }
        [Column("CARCC")]
        public string CarCC { get; set; }
        [Column("PLATNO")]
        public string PlatNo { get; set; }
        [Column("STATUS")]
        public string Status { get; set; }
        [Column("ADDRESSS")]
        public string Addresss { get; set; }
        [Column("CITY")]
        public string City { get; set; }
        [Column("POSTCODE")]
        public string PostCode { get; set; }
        [Column("STATE")]
        public string State { get; set; }
        [Column("CREATEUSERID")]
        public Guid CreateUserID { get; set; }
        [Column("CREATEDATETIME")]
        public DateTime? CreateDateTime { get; set; }
        [Column("ROWGUID")]
        public string rowguid { get; set; }
        [Column("PASSWORDUPDATE")]
        public DateTime PasswordUpdate { get; set; }

        public bool CreateUserInfo()
        {
            bool bReturn = false;
            ApplicationDbContext db = new ApplicationDbContext();
            db.UserInfoes.Add(this);
            db.SaveChanges();
            return bReturn;
        }
    }
    public class UserInfoView
    {
        
        public Guid UserInfoID { get; set; }
        
        public Guid UserID { get; set; }
        
        public string username { get; set; }
        
        public string Email { get; set; }

        public string UserName { get; set; }
        public string Username { get; set; }

        public string Position { get; set; }
        
        public string PhoneNo { get; set; }
        
        public string FaxNo { get; set; }
        
        public string Status { get; set; }
        
        public string Addresss { get; set; }
        
        public string City { get; set; }
        
        public string PostCode { get; set; }
        
        public string State { get; set; }
        
        public Guid CreateUserID { get; set; }
        
        public DateTime? CreateDateTime { get; set; }
        public string rowguid { get; set; }
        public string NoKP { get; set; }
        public string DOB { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Religion { get; set; }
        public string Race { get; set; }
        public string MStatus { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string MailAddress { get; set; }
        public string MailAddress2 { get; set; }
        public string MailAddress3 { get; set; }
        public string MailPostCode { get; set; }
        public string MailState { get; set; }
        public string MailCity { get; set; }
        public string License { get; set; }
        public string CarModel { get; set; }
        public string CarCC { get; set; }
        public string PlatNo { get; set; }

    }
}