using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PemohonanLuarNegaraDetailsModel
    {
        public virtual DbSet<HR_SEMINAR_LUAR_DETAIL> HR_SEMINAR_LUAR_DETAIL { get; set; }
    }
    public class HR_SEMINAR_LUAR_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_LAWATAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KERAP_IND { get; set; }
        public string HR_LAPORAN_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_CUTI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_CUTI_AKH { get; set; }
        public string HR_JUMLAH_CUTI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEMBALI { get; set; }
        public string HR_ALAMAT_CUTI { get; set; }
        public string HR_TIKET_KAPAL { get; set; }
        public string HR_PENGINAPAN { get; set; }
        public string HR_LAIN { get; set; }
        public string HR_JUMLAH_BELANJA { get; set; }
        public string HR_NAMA_PEGAWAI { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_ALAMAT_PEGAWAI { get; set; }
        public string HR_NOTEL_PEGAWAI { get; set; }
        public string HR_EMAIL_PEGAWAI { get; set; }
        public string HR_ALASAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MANGKU_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MANGKU_AKHIR { get; set; }
        public string HR_NO_TEL { get; set; }
        public string HR_ALAMAT_EMEL { get; set; }
        public string HR_LULUS { get; set; }
        public string HR_LULUS_PEKERJA { get; set; }


        public virtual HR_SEMINAR_LUAR HR_SEMINAR_LUAR { get; set; }
    }
    public class CARI_NOPEKERJA
    {
        public HR_SEMINAR_LUAR_DETAIL HR_SEMINAR_LUAR_DETAIL { get; set; }
        public string HR_MESEJ { get; set; }
    }

}