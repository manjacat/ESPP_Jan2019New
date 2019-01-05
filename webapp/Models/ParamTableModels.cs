using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ParamTableModels
    {
        public virtual DbSet<GE_PARAMTABLE> GE_PARAMTABLE { get; set; }
    }
    public class GE_PARAMTABLE
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("GE_PARAMTABLE_GROUP")]
        public int GROUPID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ORDINAL { get; set; }
        public Nullable<int> INTEGER_PARAM { get; set; }
        public Nullable<decimal> MONEY { get; set; }
        public Nullable<decimal> RATE { get; set; }
        public string STRING_PARAM { get; set; }
        public Nullable<System.DateTime> DATE_PARAM { get; set; }
        public string SHORT_DESCRIPTION { get; set; }
        public string LONG_DESCRIPTION { get; set; }
        public string AUDIT_ACTION { get; set; }
        public System.DateTime AUDIT_WHEN { get; set; }

        public virtual GE_PARAMTABLE_GROUP GE_PARAMTABLE_GROUP { get; set; }

       
    }
}