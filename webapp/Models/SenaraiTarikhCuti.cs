using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SenaraiTarikhCuti
    {
        public virtual DbSet<HR_SENARAI_TARIKH_CUTI> HR_SENARAI_TARIKH_CUTI { get; set; }
    }
    public class HR_SENARAI_TARIKH_CUTI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_CUTI { get; set; }
        [Key]
        [Column(Order = 2)]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        [Key]
        [Column(Order = 5)]
        public string HR_STATUS_TARIKH_CUTI { get; set; }
        [Key]
        [Column(Order = 4)]
        public System.DateTime HR_SENARAI_TARIKH { get; set; }
        [Key]
        [Column(Order = 3)]
        public System.DateTime HR_TARIKH_MULA_CUTI { get; set; }
        public string HR_LULUS_IND { get; set; }
    }
}