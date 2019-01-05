using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PinkTujuanModels
    {
        public virtual DbSet<HR_PERBATUAN_TUJUAN> HR_PERBATUAN_TUJUAN { get; set; }
    }
    public partial class HR_PERBATUAN_TUJUAN
    {
        [Key, ForeignKey("HR_PERBATUAN_PINK"), Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key, ForeignKey("HR_PERBATUAN_PINK"), Column(Order = 1)]
        public string HR_KOD_PERBATUAN { get; set; }
        [Key, Column(Order = 2)]
        public string HR_KOD_TUJUAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH { get; set; }
        public string HR_WAKTU_BERTOLAK { get; set; }
        public string HR_WAKTU_SAMPAI { get; set; }
        public string HR_TUJUAN { get; set; }
        public Nullable<int> HR_JARAK { get; set; }
        public Nullable<decimal> HR_JUMLAH { get; set; }
        public string HR_LOKASI { get; set; }
        public Nullable<decimal> HR_RM_MAKAN_P { get; set; }
        public Nullable<decimal> HR_RM_MAKAN_T { get; set; }
        public Nullable<decimal> HR_RM_MAKAN_M { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
        public Nullable<decimal> HR_RM_HOTEL { get; set; }
        public Nullable<bool> HR_IND_HOTEL { get; set; }

        public Nullable<decimal> HR_RM_LOJING { get; set; }

        public Nullable<decimal> HR_NILAI_LOJING { get; set; }
        public Nullable<decimal> HR_NILAI_HOTEL { get; set; }

   
        public virtual HR_PERBATUAN_PINK HR_PERBATUAN_PINK { get; set; }
    }
}