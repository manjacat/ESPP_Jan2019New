using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class Cuti
    {
        public virtual DbSet<HR_CUTI> HR_CUTI { get; set; }
    }
    public class HR_CUTI
    {
        [Key]
        public string HR_KOD_CUTI { get; set; }
        public string HR_KETERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public Nullable<int> HR_JUMLAH_CUTI { get; set; }
        public string HR_KATEGORI { get; set; }
        public string HR_CUTI_IND { get; set; }
        public Nullable<int> HR_KEKERAPAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_ICON { get; set; }
        public string HR_COLOR { get; set;}
    }
}