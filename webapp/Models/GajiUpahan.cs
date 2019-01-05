using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class GajiUpahan
    {
        public virtual DbSet<HR_GAJI_UPAHAN> HR_GAJI_UPAHAN { get; set; }
    }
    public class HR_GAJI_UPAHAN
    {
        [Key]
        public string HR_KOD_UPAH { get; set; }
        public string HR_PENERANGAN_UPAH { get; set; }
        public string HR_VOT_UPAH { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_JAWATAN_IND { get; set; }
        public string HR_PELARASAN_IND { get; set; }
        public string HR_TUNGGAKAN_IND { get; set; }
        public string HR_KOD_KREDITOR { get; set; }
        public string HR_KETERANGAN_SLIP { get; set; }
        public string HR_PREFIX { get; set; }
    }
}