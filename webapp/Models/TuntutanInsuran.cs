using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TuntutanInsuran
    {
        public virtual DbSet<HR_TUNTUTAN_INSURAN> HR_TUNTUTAN_INSURAN { get; set; }
    }

    public partial class HR_TUNTUTAN_INSURAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_TUNTUTAN { get; set; }
        public string HR_NO_POLISI { get; set; }
        public string HR_PELAN_KATEGORI { get; set; }
        public string HR_JENIS_TUNTUTAN { get; set; }
        public string HR_NAMA_PENYAKIT { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SIMPTOM { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_NASIHAT { get; set; }
        public string HR_KECEDERAAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASA_CEDERA { get; set; }
        public string HR_KERJA_IND { get; set; }
        public string HR_RAWATAN_IND { get; set; }
        public string HR_KAITAN_RAWATAN_IND { get; set; }
        public string HR_NAMA_DOKTOR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_RAWATAN { get; set; }
        public string HR_KOD_AGENSI_DOKTOR { get; set; }
        public string HR_KOD_AGENSI_RAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK_HOSP { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_BEDAH { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KELUAR_HOSP { get; set; }
        public string HR_CLAIM_IND { get; set; }
        public string HR_SYKT_INSURANS { get; set; }
        public string HR_NO_POLISI_LAIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR_TUGAS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEMATIAN { get; set; }
        public string HR_PUNCA_KEMATIAN { get; set; }
        public string HR_PESAKIT_IND { get; set; }
        public string HR_NO_KP_PESAKIT { get; set; }
        public string HR_MASA_CEDERA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PEMBAYARAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_TUNTUTAN { get; set; }
        public string HR_BAYAR_IND { get; set; }
    }
    public class CARI_PEKERJA
    {
        public HR_TUNTUTAN_INSURAN HR_TUNTUTAN_INSURAN { get; set; }
        public string HR_MESEJ { get; set; }
    }

}