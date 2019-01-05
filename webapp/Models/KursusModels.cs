using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KursusModels
    {
        public virtual DbSet<HR_KURSUS> HR_KURSUS { get; set; }
    }
    public class HR_KURSUS
    {
        [Key]
        public string HR_KOD_KURSUS { get; set; }
        public string HR_NAMA_KURSUS { get; set; }
        public string HR_ANJURAN { get; set; }
        public string HR_KOD_KATEGORI { get; set; }
        public string HR_KOD_ANJURAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR_PERMOHONAN { get; set; }
        public Nullable<short> HR_PESERTA_MAKSIMUM { get; set; }
        public string HR_STATUS_KURSUS { get; set; }
    }
}