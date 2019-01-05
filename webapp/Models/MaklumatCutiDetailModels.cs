using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatCutiDetailModels
    {
        public virtual DbSet<HR_MAKLUMAT_CUTI_DETAIL> HR_MAKLUMAT_CUTI_DETAIL { get; set; }
    }
    public partial class HR_MAKLUMAT_CUTI_DETAIL
    {
        [Key]
        [ForeignKey("HR_MAKLUMAT_CUTI")]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [ForeignKey("HR_MAKLUMAT_CUTI")]
        [Column(Order = 1)]
        public string HR_KOD_CUTI { get; set; }
        [Key]
        [ForeignKey("HR_MAKLUMAT_CUTI")]
        [Column(Order = 2)]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public System.DateTime HR_TARIKH_MULA_CUTI { get; set; }
        public System.DateTime HR_TARIKH_TAMAT_CUTI { get; set; }
        public string HR_CALAMAT1 { get; set; }
        public string HR_CALAMAT2 { get; set; }
        public string HR_CALAMAT3 { get; set; }
        public string HR_CBANDAR { get; set; }
        public string HR_CPOSKOD { get; set; }
        public string HR_CNEGERI { get; set; }
        public string HR_TEL { get; set; }
        public string HR_NP_PENGGANTI { get; set; }
        public string HR_NAMA_PROGRAM { get; set; }
        public string HR_TEMPAT_PROGRAM { get; set; }
        public string HR_ANJURAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_ISTERI_BERSALIN { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_NO_SIRI { get; set; }
        public Nullable<short> HR_BIL_HARI_CUTI { get; set; }
        public string HR_SOKONG_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SOKONG { get; set; }
        public string HR_LULUS_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LULUS { get; set; }
        public string HR_NP_KJ { get; set; }
        public string HR_HR_LULUS_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LULUS_HR { get; set; }
        public string HR_NO_PEKERJA_LULUS { get; set; }
        public string HR_ULASAN { get; set; }
        public string HR_LULUS_YDP_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_YDP { get; set; }
        public string HR_NO_PEKERJA_YDP { get; set; }
        public string HR_HARI_CUTI { get; set; }
        public string HR_NAMA_KLINIK { get; set; }

        public virtual HR_MAKLUMAT_CUTI HR_MAKLUMAT_CUTI { get; set; }
    }
}