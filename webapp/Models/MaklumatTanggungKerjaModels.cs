﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatTanggungKerjaModels
    {
        public virtual DbSet<HR_MAKLUMAT_TANGGUNG_KERJA> HR_MAKLUMAT_TANGGUNG_KERJA { get; set; }
    }

    public class HR_MAKLUMAT_TANGGUNG_KERJA
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
        public Nullable<byte> HR_BULAN { get; set; }
        public Nullable<short> HR_TAHUN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_BUTIR_PERUBAHAN { get; set; }
        public string HR_CATATAN { get; set; }
        public string HR_NO_SURAT_KEBENARAN { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_NP_UBAH_HR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH_HR { get; set; }
        public string HR_NP_FINALISED_HR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_FINALISED_HR { get; set; }
        public string HR_FINALISED_IND_HR { get; set; }
        public string HR_NP_UBAH_PA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH_PA { get; set; }
        public string HR_NP_FINALISED_PA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_FINALISED_PA { get; set; }
        public string HR_FINALISED_IND_PA { get; set; }
        public Nullable<decimal> HR_EKA { get; set; }
        public Nullable<decimal> HR_ITP { get; set; }
        public string HR_KEW8_IND { get; set; }
        public Nullable<decimal> HR_BIL { get; set; }
        public string HR_KOD_JAWATAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public int HR_KEW8_ID { get; set; }
        public string HR_LANTIKAN_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SP { get; set; }
        public string HR_SP_IND { get; set; }
        public Nullable<int> HR_JUMLAH_BULAN { get; set; }
        public Nullable<decimal> HR_NILAI_EPF { get; set; }
        public Nullable<decimal> HR_GAJI_LAMA { get; set; }
        public string HR_MATRIKS_GAJI_LAMA { get; set; }
        public string HR_GRED_LAMA { get; set; }
        public string HR_UBAH_IND { get; set; }
        public Nullable<int> HR_ANSURAN_ID { get; set; }
        public Nullable<int> HR_GAJI_MIN_BARU { get; set; }
        public string HR_NO_PEKERJA_PT { get; set; }
        public string HR_NAMA_PT {
            get {
                string nama = null;
                HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.Find(HR_NO_PEKERJA_PT);
                if(pekerja != null)
                {
                    nama = pekerja.HR_NAMA_PEKERJA;
                }
                return nama;
            }
        }
    }
}