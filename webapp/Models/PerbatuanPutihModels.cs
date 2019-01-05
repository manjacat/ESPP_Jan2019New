using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSPP.Models
{
    public class PerbatuanPutihModels
    {
        public virtual DbSet<HR_PERBATUAN_PUTIH> HR_PERBATUAN_PUTIH { get; set; }

    }

    public class HR_PERBATUAN_PUTIH
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_PERBATUAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KELAS { get; set; }
        public string HR_KM { get; set; }
        public string HR_SEBAB { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKUAN { get; set; }
        public Nullable<System.DateTime> HR_SOKONG_KETUA { get; set; }
        public string HR_KELAYAKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SAH_PTB { get; set; }
        public string HR_ULASAN_TP { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TP { get; set; }
        public string HR_JENIS_IND { get; set; }
        public string HR_IND_KETUA { get; set; }
        public string HR_IND_PTB { get; set; }
        public string HR_IND_TB { get; set; }
        public string HR_MOHON_IND { get; set; }
        public string HR_TANDATANGAN_PENGGUNA { get; set; }
        public string HR_TANDATANGAN_KJ { get; set; }
        public string HR_TANDATANGAN_HR { get; set; }
        public string HR_TANDATANGAN_TP { get; set; }
        public string DOCUMENT_ID { get; set; }
        public string DESC_FILE { get; set; }


    }
}