using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SejarahElaunPotongan
    {
        public virtual DbSet<HR_SEJARAH_ELAUN_POTONGAN> HR_SEJARAH_ELAUN_POTONGAN { get; set; }
    }

    public class HR_SEJARAH_ELAUN_POTONGAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_ELAUN_POTONGAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_NO_FAIL { get; set; }
        public Nullable<decimal> HR_JUMLAH { get; set; }
        public string HR_ELAUN_POTONGAN_IND { get; set; }
        public string HR_MOD_BAYARAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
        public Nullable<decimal> HR_TUNTUTAN_MAKSIMA { get; set; }
        public Nullable<decimal> HR_BAKI { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<int> HR_HARI_BEKERJA { get; set; }
        public string HR_NO_PEKERJA_PT { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_UBAH_IND { get; set; }
        public string HR_GRED_PT { get; set; }
        public string HR_MATRIKS_GAJI_PT { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string HR_AUTO_IND { get; set; }
        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal HR_ID_ELAUN_POTONGAN { get; set; }
        public Nullable<DateTime> HR_TARIKH_SEJARAH { get; set; }
        public string HR_TINDAKAN { get; set; }
    }
}