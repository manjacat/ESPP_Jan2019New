using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TarikhAcaraModels
    {
        public virtual DbSet<HR_TARIKH_ACARA> HR_TARIKH_ACARA { get; set; }
    }

    public class HR_TARIKH_ACARA
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_ACARA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_TARIKH_ACARA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH { get; set; }
        public string HR_WAKTU_MULA { get; set; }
        public string HR_WAKTU_AKHIR { get; set; }
        public string HR_KETERANGAN { get; set; }
    }
}