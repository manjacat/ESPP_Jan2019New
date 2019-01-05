using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPeribadiPensyarahModels
    {
        public virtual DbSet<HR_PENSYARAH> HR_PENSYARAH { get; set; }
    }

    public class HR_PENSYARAH
    {
        [Key]
        public string HR_NO_PENSYARAH { get; set; }
        public string HR_NAMA_PENSYARAH { get; set; }

        [ForeignKey("HR_KUMPULAN_PENSYARAH")]
        public string HR_KOD_KUMPULAN { get; set; }
        public string HR_NO_KPBARU { get; set; }
        public string HR_NO_KPLAMA { get; set; }
        public string HR_NO_TELPEJABAT { get; set; }
        public string HR_NO_TELBIMBIT { get; set; }
        public string HR_NO_FAKS { get; set; }
        public string HR_JAWATAN { get; set; }
        public string HR_GRED_KELULUSAN { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_TALAMAT1 { get; set; }
        public string HR_TALAMAT2 { get; set; }
        public string HR_TALAMAT3 { get; set; }
        public string HR_TBANDAR { get; set; }
        public string HR_TPOSKOD { get; set; }
        public string HR_TNEGERI { get; set; }
        public string HR_SALAMAT1 { get; set; }
        public string HR_SALAMAT2 { get; set; }
        public string HR_SALAMAT3 { get; set; }
        public string HR_SBANDAR { get; set; }
        public string HR_SPOSKOD { get; set; }
        public string HR_SNEGERI { get; set; }
        public string HR_NO_PEKERJA { get; set; }

        public virtual HR_KUMPULAN_PENSYARAH HR_KUMPULAN_PENSYARAH { get; set; }
    }

    public class CARI_PENSYARAH
    {
        public HR_PENSYARAH HR_PENSYARAH { get; set; }
        public string HR_MESEJ { get; set; }
    }
}