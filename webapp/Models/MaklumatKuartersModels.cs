using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKuartersModels
    {
        public virtual DbSet<HR_MAKLUMAT_KUARTERS> HR_MAKLUMAT_KUARTERS { get; set; }
    }
    public class HR_MAKLUMAT_KUARTERS
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [ForeignKey("HR_KUARTERS")]
        public string HR_KOD_KUARTERS { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_MASUK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KELUAR { get; set; }
        public string HR_NO_UNIT { get; set; }
        public string HR_GANDAAN2X { get; set; }
        public string HR_GERAI { get; set; }
        public string HR_CATATAN { get; set; }
        public string HR_IDP { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_GANDAAN5X { get; set; }
        public Nullable<decimal> HR_JUMLAH_POTONGAN { get; set; }

        public virtual HR_KUARTERS HR_KUARTERS { get; set; }
    }
}