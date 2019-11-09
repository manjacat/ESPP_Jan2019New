using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPemohonModels
    {
        public virtual DbSet<HR_MAKLUMAT_PEMOHON> HR_MAKLUMAT_PEMOHON { get; set; }
    }

    public class HR_MAKLUMAT_PEMOHON
    {
        [Key]
        public string HR_NO_KPBARU { get; set; }
        public string HR_NAMA_PEMOHON { get; set; }
        public string HR_NAMA_LAIN { get; set; }
        public string HR_NO_KPLAMA { get; set; }
        public string HR_WARNA_KP { get; set; }
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
        public string HR_TELRUMAH { get; set; }
        public string HR_TELPEJABAT { get; set; }
        public string HR_TELBIMBIT { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Email tidak sah")]
        public string HR_EMAIL { get; set; }
        public string HR_NAMA_BAPA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR_BAPA { get; set; }
        public string HR_TEMPAT_LAHIR_BAPA { get; set; }
        public string HR_NAMA_IBU { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR_IBU { get; set; }
        public string HR_TEMPAT_LAHIR_IBU { get; set; }


        public virtual ICollection<HR_PERMOHONAN_JAWATAN> HR_PERMOHONAN_JAWATAN { get; set; }
    }
}