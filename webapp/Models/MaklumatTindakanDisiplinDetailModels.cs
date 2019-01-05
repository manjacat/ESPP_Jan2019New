using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatTindakanDisiplinDetailModels
    {
        public virtual DbSet<HR_TINDAKAN_DISIPLIN_DETAIL> HR_TINDAKAN_DISIPLIN_DETAIL { get; set; }
    }
    public class HR_TINDAKAN_DISIPLIN_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_KESALAHAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_KOD_TINDAKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
    }
}