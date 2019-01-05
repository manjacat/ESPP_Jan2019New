using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JadualGaji
    {
        public virtual DbSet<HR_JADUAL_GAJI> HR_JADUAL_GAJI { get; set; }
    }
    public partial class HR_JADUAL_GAJI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_SISTEM_SARAAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_GRED_GAJI { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_PERINGKAT { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public Nullable<decimal> HR_GAJI_MIN { get; set; }
        public Nullable<decimal> HR_GAJI_MAX { get; set; }
        public Nullable<decimal> HR_RM_KENAIKAN { get; set; }
        public Nullable<decimal> HR_PERATUS_KENAIKAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_KOD_GAJI { get; set; }
        public string HR_KANAN_IND { get; set; }
    }
}