using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatTindakanDisiplinModels
    {
        public virtual DbSet<HR_TINDAKAN_DISIPLIN> HR_TINDAKAN_DISIPLIN { get; set; }
    }
    public class HR_TINDAKAN_DISIPLIN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_KESALAHAN { get; set; }
        public string HR_KESALAHAN { get; set; }
    }
}