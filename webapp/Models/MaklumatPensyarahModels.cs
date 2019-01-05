using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPensyarahModels
    {
        public virtual DbSet<HR_KUMPULAN_PENSYARAH> HR_KUMPULAN_PENSYARAH { get; set; }
    }

    public class HR_KUMPULAN_PENSYARAH
    {
        
        [Key]
        public string HR_KOD_KUMPULAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public Nullable<decimal> HR_KADAR_JAM { get; set; }
        public Nullable<decimal> HR_NILAI_MAKSIMA { get; set; }
        public Nullable<decimal> HR_PERATUS { get; set; }
        public string HR_JENIS_IND { get; set; }

        public virtual ICollection<HR_PENSYARAH> HR_PENSYARAH { get; set; }
    }
}