using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class CreditorMasterModels
    {
        public virtual DbSet<AP_CREDITORMASTER> AP_CREDITORMASTER { get; set; }
    }
    public partial class AP_CREDITORMASTER
    {
        public string CREDITORTYPE { get; set; }
        [Key]
        public string CREDITORCODE { get; set; }
        public string CREDITORNAME { get; set; }
        public string ICNEW { get; set; }
        public string ICOLD { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string ADDRESS3 { get; set; }
        public string POSCODE { get; set; }
        public string STATE { get; set; }
        public string TEL1 { get; set; }
        public string TEL2 { get; set; }
        public string BANK { get; set; }
        public string ACCTNO { get; set; }
        public Nullable<decimal> CURRAMOUNT { get; set; }
        public Nullable<decimal> PREVAMOUNT { get; set; }
        public string CREDITORSTATUS { get; set; }
        public string CREDITORCOMMENT { get; set; }
        public string SYSUSERCREATED { get; set; }
        public Nullable<System.DateTime> SYSDATECREATED { get; set; }
        public Nullable<System.DateTime> SYSLASTMODIFIED { get; set; }
        public string SYSLASTUSER { get; set; }
        public string SHORTNAME { get; set; }
        public string CONTACT_PERSON { get; set; }
        public Nullable<System.DateTime> LAST_INV_DATE { get; set; }
        public Nullable<decimal> PAYMENT_PREVYEAR { get; set; }
        public Nullable<decimal> PAYMENT_CURRYEAR { get; set; }
        public string NAME_ON_CHEQUE { get; set; }
        public string CREDITOR_OLD_CODE { get; set; }
        public Nullable<short> PERIOD_YEAR { get; set; }
        public Nullable<byte> PERIOD_MONTH { get; set; }
        public string BANK2 { get; set; }
        public string BANK3 { get; set; }
        public string BANK2ACCTNO { get; set; }
        public string BANK3ACCTNO { get; set; }
        public string THIRDPARTY { get; set; }
        public string CATEGORY { get; set; }
        public string PR_SUPPLIER_ID { get; set; }
        public string COMPANY_ID { get; set; }
        public string COMPANY_REGNO { get; set; }
        public string EMAIL { get; set; }
        public string BANKCODE { get; set; }
        public string BANKCODE2 { get; set; }
        public string BANKCODE3 { get; set; }
        public Nullable<bool> FLAGVALIDATE { get; set; }
        public Nullable<System.DateTime> COMMENCE_DATE { get; set; }
        public string GSTNO { get; set; }
        public string GST_ACTIVE { get; set; }
        public Nullable<System.DateTime> GSTREG_DATE { get; set; }
        public string STATUS_GST { get; set; }
    }
}