using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SejarahPeribadiModels
    {
        public virtual DbSet<HR_SEJARAH_PERIBADI> HR_SEJARAH_PERIBADI { get; set; }
    }
    public class HR_SEJARAH_PERIBADI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NO_KPBARU { get; set; }
        public string HR_NAMA_PEKERJA { get; set; }
        public string HR_NO_KPLAMA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_WARGANEGARA { get; set; }
        public string HR_KETURUNAN { get; set; }
        public string HR_AGAMA { get; set; }
        public string HR_JANTINA { get; set; }
        public string HR_TARAF_KAHWIN { get; set; }
        public string HR_LESEN { get; set; }
        public string HR_KELAS_LESEN { get; set; }
        public string HR_TALAMAT1 { get; set; }
        public string HR_TALAMAT2 { get; set; }
        public string HR_TALAMAT3 { get; set; }
        public string HR_TBANDAR { get; set; }
        public string HR_TPOSKOD { get; set; }
        public string HR_TNEGERI { get; set; }
        public string HR_SALAMAT1 { get; set; }
        public string HR_SALAMAT2 { get; set; }
        public string HR_SALAMAT3 { get; set; }
        public string HR_SBANDAR { get; set; }
        public string HR_SPOSKOD { get; set; }
        public string HR_SNEGERI { get; set; }
        public string HR_TAHUN_SPM { get; set; }
        public string HR_GRED_BM { get; set; }
        public string HR_TELRUMAH { get; set; }
        public string HR_TELPEJABAT { get; set; }
        public string HR_TELBIMBIT { get; set; }
        public string HR_EMAIL { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public Nullable<decimal> HR_CC_KENDERAAN { get; set; }
        public string HR_NO_KENDERAAN { get; set; }
        public string HR_JENIS_KENDERAAN { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_IDPEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal HR_ID_SEJARAH { get; set; }
        public Nullable<DateTime> HR_TARIKH_SEJARAH { get; set; }
        public string HR_TINDAKAN { get; set; }
    }
}