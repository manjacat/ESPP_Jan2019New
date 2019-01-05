using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PendahuluanDiriModels
    {
        public virtual DbSet<HR_PENDAHULUAN_DIRI> HR_PENDAHULUAN_DIRI { get; set; }
    }
    public class HR_PENDAHULUAN_DIRI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_PENDAHULUAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PERMOHONAN { get; set; }
        
        public string HR_PENDAHULUAN { get; set; }
        public string HR_KELULUSAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_SEKARANG { get; set; }
        public Nullable<decimal> HR_JUMLAH_PENUH { get; set; }
        public string HR_TUJUAN { get; set; }
        public string HR_NAMA_PEGAWAI { get; set; }
        public Nullable<long> HR_NO_IC { get; set; }
        public string HR_NO_GAJI { get; set; }
     
        public string HR_BAYARAN_BALIK { get; set; }
        public string HR_IND_HR { get; set; }
        public string HR_CATATAN { get; set; }
        public string HR_FINALISED_IND { get; set; }
        public string HR_NAMA_HR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_HR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
        public string HR_KELULUSAN_1 { get; set; }
        public string HR_KELULUSAN_2 { get; set; }
        public string HR_KELULUSAN_3 { get; set; }
        public string HR_KELULUSAN_4 { get; set; }
        public string HR_PEGAWAI_2 { get; set; }
        public string HR_PEGAWAI_3 { get; set; }
        public string HR_PEGAWAI_4 { get; set; }
        public string HR_PEGAWAI_7 { get; set; }
        public string HR_NO_KELULUSAN { get; set; }
        public string HR_JENIS_DOKUMEN { get; set; }
        public string HR_RUJUKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SELESAI { get; set; }
        public Nullable<decimal> HR_BAKI_SEBELUM { get; set; }
        public string HR_TINDAKAN { get; set; }
        public string HR_IND_PEMOHON { get; set; }
        public string HR_FINAL_PEMOHON { get; set; }
        public string HR_NO_RUJUKAN { get; set; }

    }
}