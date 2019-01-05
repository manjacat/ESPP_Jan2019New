using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PelarasanModels
    {
        public virtual DbSet<PA_PELARASAN> PA_PELARASAN { get; set; }
    }
    public partial class PA_PELARASAN
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public int PA_BULAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public short PA_TAHUN { get; set; }
        [Key]
        [Column(Order = 3)]
        public string PA_KOD_PELARASAN { get; set; }
        public Nullable<decimal> PA_PERATUS { get; set; }
        public Nullable<decimal> PA_NILAI { get; set; }
        public Nullable<decimal> PA_NILAI_MAKSIMUM { get; set; }
        public Nullable<decimal> PA_NILAI_MINIMUM { get; set; }
        public Nullable<System.DateTime> PA_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> PA_TARIKH_AKHIR { get; set; }
        public string PA_JENIS_PELARASAN { get; set; }
        public string PA_PROSES_IND { get; set; }
        public string PA_VOT_PELARASAN { get; set; }
        public string PA_SINGKATAN { get; set; }
        public Nullable<System.DateTime> PA_TARIKH_KEYIN { get; set; }
        public Nullable<System.DateTime> PA_TARIKH_PROSES { get; set; }
        public string PA_LAPORAN_IND { get; set; }
        public Nullable<int> HR_KEW8_ID { get; set; }
    }
}