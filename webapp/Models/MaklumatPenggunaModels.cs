using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPenggunaModels
	{
		public MaklumatPenggunaModels() { }
		public virtual DbSet<PRUSER> PRUSER { get; set; }
    }
	[Table("PRUSER")]
	public class PRUSER
    {
        [Key]
		[Column("USERID")]
		public string USERID { get; set; }
		[Column("USERNAME")]
		public string USERNAME { get; set; }
		[Column("USERPASSWORD")]
		public string USERPASSWORD { get; set; }
		[Column("USERGROUPCODE")]
		public string USERGROUPCODE { get; set; }
		[Column("USERTYPECODE")]
		public string USERTYPECODE { get; set; }
		[Column("CUSTOMERID")]
		public string CUSTOMERID { get; set; }
		[Column("DEPARTMENTCODE")]
		public string DEPARTMENTCODE { get; set; }
		[Column("OFFICERID")]
		public string OFFICERID { get; set; }
		[Column("IMAGEFILE")]
		public string IMAGEFILE { get; set; }
		[Column("CREATEDDATE")]
		public Nullable<System.DateTime> CREATEDDATE { get; set; }
		[Column("USERLOGINSTATUSCODE")]
		public string USERLOGINSTATUSCODE { get; set; }
		[Column("STATUSCODE")]
		public string STATUSCODE { get; set; }
		[Column("USERLOGINTIMESTAMP")]
		public Nullable<System.DateTime> USERLOGINTIMESTAMP { get; set; }
		[Column("NIRC")]
		public string NIRC { get; set; }
		[Column("NAME")]
		public string NAME { get; set; }
		[Column("MOBILE_PHONE")]
		public string MOBILE_PHONE { get; set; }
		[Column("OFFICE_PHONE")]
		public string OFFICE_PHONE { get; set; }
		[Column("OFFICE_EXT")]
		public string OFFICE_EXT { get; set; }
		[Column("HOME_PHONE")]
		public string HOME_PHONE { get; set; }
		[Column("ADDRESS")]
		public string ADDRESS { get; set; }
		[Column("POSCODE")]
		public string POSCODE { get; set; }
		[Column("STATE")]
		public string STATE { get; set; }
		[Column("CITY")]
		public string CITY { get; set; }
		[Column("JANTINA")]
		public string JANTINA { get; set; }
		[Column("BANGSA")]
		public string BANGSA { get; set; }
		[Column("AGAMA")]
		public string AGAMA { get; set; }
		[Column("LASTUPDATE")]
		public string LASTUPDATE { get; set; }
		[Column("DOB")]
		public Nullable<System.DateTime> DOB { get; set; }
		[Column("DESIGNATION")]
		public string DESIGNATION { get; set; }
		[Column("EMAIL")]
		public string EMAIL { get; set; }
		[Column("USERCHANGEPASSWORDTIMESTAMP")]
		public Nullable<System.DateTime> USERCHANGEPASSWORDTIMESTAMP { get; set; }
    }
}