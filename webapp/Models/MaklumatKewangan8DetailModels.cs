using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKewangan8DetailModels
    {
        virtual public DbSet<HR_MAKLUMAT_KEWANGAN8_DETAIL> HR_MAKLUMAT_KEWANGAN8_DETAIL { get; set; }
    }

    public class HR_MAKLUMAT_KEWANGAN8_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_PERUBAHAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public System.DateTime HR_TARIKH_MULA { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HR_KOD_PELARASAN { get; set; }
        public string HR_MATRIKS_GAJI { get; set; }
        public string HR_GRED { get; set; }
        public Nullable<decimal> HR_JUMLAH_PERUBAHAN { get; set; }
        public Nullable<decimal> HR_GAJI_BARU { get; set; }
        public string HR_JENIS_PERGERAKAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_PERUBAHAN_ELAUN { get; set; }
        public string HR_STATUS_IND { get; set; }
        public Nullable<decimal> HR_ELAUN_KRITIKAL_BARU { get; set; }
        [Key]
        [Column(Order = 4)]
        public int HR_KEW8_ID { get; set; }
        public string HR_NO_PEKERJA_PT { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_EKAL { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_EWIL { get; set; }
        public Nullable<decimal> HR_GAJI_LAMA { get; set; }
        public string HR_JAWATAN_BARU { get; set; }
        public string HR_KOD_ELAUN { get; set; }
    }
}