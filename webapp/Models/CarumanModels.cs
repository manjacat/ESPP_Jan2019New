using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class CarumanModels
    {
        public virtual DbSet<HR_CARUMAN> HR_CARUMAN { get; set; }
    }
    public class HR_CARUMAN
    {
        [Key]
       
        public string HR_KOD_CARUMAN { get; set; }
        public string HR_PENERANGAN_CARUMAN { get; set; }
        public string HR_VOT_CARUMAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public Nullable<decimal> HR_PERATUS { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
        public string HR_KOD_KREDITOR { get; set; }
        public string HR_KETERANGAN_SLIP { get; set; }
        public string HR_PREFIX { get; set; }
        public string HR_KETERANGAN_LAPORAN { get; set; }
    }
}