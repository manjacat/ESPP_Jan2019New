using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPekerjaanHistoryModels
    {
        public virtual DbSet<HR_MAKLUMAT_PEKERJAAN_HISTORY> HR_MAKLUMAT_PEKERJAAN_HISTORY { get; set; }
    }
    public partial class HR_MAKLUMAT_PEKERJAAN_HISTORY
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_PERUBAHAN { get; set; }
        public string HR_JABATAN { get; set; }
        public string HR_BAHAGIAN { get; set; }
        public string HR_UNIT { get; set; }
        public string HR_GRED { get; set; }
        public string HR_KATEGORI { get; set; }
        public string HR_KUMP_PERKHIDMATAN { get; set; }
        public string HR_JAWATAN { get; set; }
        public string HR_TARAF_JAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SAH_JAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT_KONTRAK { get; set; }
        public string HR_NO_PENYELIA { get; set; }
        public string HR_MATRIKS_GAJI { get; set; }
        public string HR_KUMPULAN { get; set; }
        public string HR_TINGKATAN { get; set; }
        public string HR_KOD_GAJI { get; set; }
        public string HR_NP_UBAH { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK { get; set; }
        public Nullable<decimal> HR_GAJI { get; set; }
    }
}