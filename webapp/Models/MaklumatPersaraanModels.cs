using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPersaraanModels
    {
        public virtual DbSet<HR_PERSARAAN> HR_PERSARAAN { get; set; }
    }
    public class HR_PERSARAAN
    {
        [Key]
        [ForeignKey("HR_MAKLUMAT_PEKERJAAN")]
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_BERSARA { get; set; }
        public string HR_ALASAN { get; set; }
        public string HR_BERSARA_IND { get; set; }
        public string HR_BAYARAN_IND { get; set; }
        public Nullable<decimal> HR_JUMLAH_BAYARAN { get; set; }
        public Nullable<int> HR_JUMLAH_CUTI { get; set; }
        public string HR_PALAMAT1 { get; set; }
        public string HR_PALAMAT2 { get; set; }
        public string HR_PALAMAT3 { get; set; }
        public string HR_PBANDAR { get; set; }
        public string HR_PPOSKOD { get; set; }
        public string HR_PNEGERI { get; set; }
        public Nullable<decimal> HR_EKA { get; set; }
        public Nullable<decimal> HR_ITP { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_TERIMA_BAYARAN_IND { get; set; }
        public string HR_NP_PEGAWAI { get; set; }
        public string HR_JAWATAN_PEGAWAI { get; set; }

        public virtual HR_MAKLUMAT_PEKERJAAN HR_MAKLUMAT_PEKERJAAN { get; set; }
    }
}