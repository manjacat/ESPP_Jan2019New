using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPenilaianPrestasiModels
    {
        public virtual DbSet<HR_PENILAIAN_PRESTASI> HR_PENILAIAN_PRESTASI { get; set; }
    }
    public class HR_PENILAIAN_PRESTASI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public short HR_TAHUN_PRESTASI { get; set; }
        public Nullable<decimal> HR_PENGHASILAN_PPP { get; set; }
        public Nullable<decimal> HR_PENGHASILAN_PPK { get; set; }
        public Nullable<decimal> HR_PENGETAHUAN_PPP { get; set; }
        public Nullable<decimal> HR_PENGETAHUAN_PPK { get; set; }
        public Nullable<decimal> HR_KUALITI_PPP { get; set; }
        public Nullable<decimal> HR_KUALITI_PPK { get; set; }
        public Nullable<decimal> HR_SUMBANGAN_PPP { get; set; }
        public Nullable<decimal> HR_SUMBANGAN_PPK { get; set; }
        public Nullable<decimal> HR_PURATA_PENGHASILAN { get; set; }
        public Nullable<decimal> HR_PURATA_PENGETAHUAN { get; set; }
        public Nullable<decimal> HR_PURATA_KUALITI { get; set; }
        public Nullable<decimal> HR_PURATA_SUMBANGAN { get; set; }
        public Nullable<decimal> HR_PERATUS_PENGHASILAN { get; set; }
        public Nullable<decimal> HR_PERATUS_PENGETAHUAN { get; set; }
        public Nullable<decimal> HR_PERATUS_KUALITI { get; set; }
        public Nullable<decimal> HR_PERATUS_SUMBANGAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_BESAR { get; set; }
        public string HR_CEMERLANG_IND { get; set; }
        public string HR_JENIS_IND { get; set; }
        public Nullable<short> HR_CUTI_KERAJAAN { get; set; }
        public Nullable<short> HR_CUTI_SWASTA { get; set; }
    }
}