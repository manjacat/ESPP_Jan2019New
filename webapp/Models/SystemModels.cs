using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SystemModels
    {
        public virtual DbSet<GE_SYSTEM> GE_SYSTEM { get; set; }
    }
    public partial class GE_SYSTEM
    {
        [Key]
        public string GE_SYS_ID { get; set; }
        public string GE_SYS_NAME { get; set; }
        public string GE_SYARIKAT { get; set; }
        public Nullable<decimal> GE_KOS { get; set; }
        public string GE_LO { get; set; }
        public Nullable<short> GE_TAHUN { get; set; }
        public string GE_VOT { get; set; }
    }
}