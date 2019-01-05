using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PergerakanGajiModels
    {
        //public IEnumerable<MaklumatPeribadi> HR_MAKLUMAT_PERIBADI { get; set; }

        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_PEKERJA { get; set; }
        public string HR_NO_KPBARU { get; set; }

        public string HR_KOD_PERUBAHAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_AKHIR { get; set; }
        public Nullable<int> HR_BULAN { get; set; }
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

        public Nullable<int> HR_KEW8_ID { get; set; }
        public string HR_LANTIKAN_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SP { get; set; }
        public string HR_SP_IND { get; set; }
        public Nullable<int> HR_JUMLAH_BULAN { get; set; }
        public Nullable<decimal> HR_NILAI_EPF { get; set; }

        public string HR_KOD_PELARASAN { get; set; }
        public string HR_MATRIKS_GAJI { get; set; }
        public string HR_MATRIKS_GAJI_LAMA { get; set; }
        public string HR_GRED { get; set; }
        public string HR_GRED_LAMA { get; set; }
        public Nullable<decimal> HR_JUMLAH_PERUBAHAN { get; set; }
        public Nullable<decimal> HR_GAJI_BARU { get; set; }
        public string HR_JENIS_PERGERAKAN { get; set; }
        public Nullable<decimal> HR_JUMLAH_PERUBAHAN_ELAUN { get; set; }
        public string HR_STATUS_IND { get; set; }
        public Nullable<decimal> HR_ELAUN_KRITIKAL_BARU { get; set; }
        public string HR_NO_PEKERJA_PT { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_EKAL { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_EWIL { get; set; }
        public Nullable<decimal> HR_GAJI_LAMA { get; set; }

        public Nullable<decimal> HR_GAJI_MIN { get; set; }
        public Nullable<decimal> HR_GAJI_MAX { get; set; }

        public string HR_NAMA_GRED { get; set; }

        //LAIN-LAIN
        public string HR_JAWATAN { get; set; }
        public string HR_KOD_GAJI { get; set; }
        public string HR_SISTEM { get; set; }
        public Nullable<decimal> HR_GAJI_MIN_P { get; set; }
        public Nullable<decimal> HR_GAJI_MAX_P { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_KOD_KRITIKAL { get; set; }
        public Nullable<float> HR_KRITIKAL { get; set; }
        public string HR_JENIS_PERUBAHAN { get; set; }
        public string HR_KOD_WILAYAH { get; set; }
        public Nullable<float> HR_WILAYAH { get; set; }
        public Nullable<decimal> HR_PERUBAHAN_KRITIKAL { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_KRITIKAL { get; set; }
        public Nullable<decimal> HR_PERUBAHAN_WILAYAH { get; set; }
        public Nullable<decimal> HR_PERGERAKAN_WILAYAH { get; set; }
        public string HR_NAMA_PEGAWAI { get; set; }
        public Nullable<int> COUNTLIST { get; set; }
        public Nullable<int> CHECKED { get; set; }
        public Nullable<decimal> HR_KENAIKAN { get; set; }

        public string HR_UBAH_IND { get; set; }
    }
}