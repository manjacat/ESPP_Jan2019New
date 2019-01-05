using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKursusLatihanModels
    {
        public virtual DbSet<HR_MAKLUMAT_KURSUS_LATIHAN> HR_MAKLUMAT_KURSUS_LATIHAN { get; set; }
    }
    public class HR_MAKLUMAT_KURSUS_LATIHAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_KURSUS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_ANJURAN { get; set; }
        public string HR_KEPUTUSAN { get; set; }
    }
}