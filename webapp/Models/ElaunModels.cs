using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ElaunModels
    {
        public virtual DbSet<HR_ELAUN> HR_ELAUN { get; set; }
    }
    public class HR_ELAUN
    {
        [Key]
        public string HR_KOD_ELAUN { get; set; }
        public string HR_PENERANGAN_ELAUN { get; set; }
        public string HR_VOT_ELAUN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
        public string HR_JAWATAN_IND { get; set; }
        public string HR_PINDAAN_IND { get; set; }
        [ForeignKey("HR_KATEGORI_ELAUN")]
        public string HR_KOD_KATEGORI { get; set; }
        public string HR_STATUS_CUKAI { get; set; }
        public string HR_TUNGGAKAN_IND { get; set; }
        public string HR_STATUS_KWSP { get; set; }
        public string HR_STATUS_SOCSO { get; set; }
        public string HR_KOD_KREDITOR { get; set; }
        public string HR_KETERANGAN_SLIP { get; set; }
        public string HR_KOD_POTONGAN { get; set; }
        public string HR_PREFIX { get; set; }
        public string HR_KOD_TUNGGAKAN { get; set; }
        public string HR_PERATUS_IND { get; set; }
        public string HR_INDICATOR { get; set; }

        public virtual HR_KATEGORI_ELAUN HR_KATEGORI_ELAUN { get; set; }
    }
}