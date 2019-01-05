using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PotonganModels
    {
        public virtual DbSet<HR_POTONGAN> HR_POTONGAN { get; set; }
    }
    public partial class HR_POTONGAN
    {
        [Key]
        public string HR_KOD_POTONGAN { get; set; }
        public string HR_PENERANGAN_POTONGAN { get; set; }
        public string HR_VOT_POTONGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
        public string OLD_CODE { get; set; }
        public string HR_KOD_KREDITOR { get; set; }
        public string HR_KETERANGAN_SLIP { get; set; }
        public string HR_KETERANGAN_LAPORAN { get; set; }
        public string HR_VOT_POTONGAN_P { get; set; }
        public string HR_INDICATOR { get; set; }
        public string HR_KOD_CARUMAN { get; set; }
    }
}