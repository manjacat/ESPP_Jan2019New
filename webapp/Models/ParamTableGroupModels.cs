using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ParamTableGroupModels
    {
        public virtual DbSet<GE_PARAMTABLE_GROUP> GE_PARAMTABLE_GROUP { get; set; }
    }
    public class GE_PARAMTABLE_GROUP
    {
        [Key]
        public int GROUPID { get; set; }
        public string DESCRIPTION { get; set; }
        public bool UPDATE_FLAG { get; set; }
        public Nullable<int> LAST_ORDINAL { get; set; }
        public Nullable<int> PRIMEDESC { get; set; }
        public string AUDIT_ACTION { get; set; }
        public System.DateTime AUDIT_WHEN { get; set; }

        public virtual ICollection<GE_PARAMTABLE> GE_PARAMTABLE { get; set; }
    }
}