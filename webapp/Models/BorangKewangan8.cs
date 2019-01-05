using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class BorangKewangan8
    {
        public  Kewangan8Model HR_MAKLUMAT_KEWANGAN8 { get; set; }
        public  PeribadiModel HR_MAKLUMAT_PERIBADI { get; set; }
        public  PekerjaanModel HR_MAKLUMAT_PEKERJAAN { get; set; }
        

    }

    public class PeribadiModel
    {
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
    }

    public class PekerjaanModel
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_GELARAN { get; set; }
        public string HR_JABATAN { get; set; }
        public string HR_BAHAGIAN { get; set; }
        public string HR_JAWATAN { get; set; }
        public string HR_GRED { get; set; }
        public string HR_KATEGORI { get; set; }
        public string HR_KUMP_PERKHIDMATAN { get; set; }
        public string HR_TARAF_JAWATAN { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
        public string HR_NO_AKAUN_BANK { get; set; }
        public Nullable<System.DateTime> HR_BULAN_KENAIKAN_GAJI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SAH_JAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT_KONTRAK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_SISTEM { get; set; }
        public string HR_NO_PENYELIA { get; set; }
        public string HR_STATUS_KWSP { get; set; }
        public string HR_STATUS_SOCSO { get; set; }
        public string HR_STATUS_PCB { get; set; }
        public string HR_STATUS_PENCEN { get; set; }
        public Nullable<decimal> HR_NILAI_KWSP { get; set; }
        public Nullable<decimal> HR_NILAI_SOCSO { get; set; }
        public string HR_KOD_PCB { get; set; }
        public string HR_GAJI_PRORATA { get; set; }
        public string HR_MATRIKS_GAJI { get; set; }
        public string HR_UNIT { get; set; }
        public string HR_KUMPULAN { get; set; }
        public string HR_KOD_BANK { get; set; }
        public string HR_TINGKATAN { get; set; }
        public string HR_KAKITANGAN_IND { get; set; }
        public string HR_FAIL_PERKHIDMATAN { get; set; }
        public string HR_NO_SIRI { get; set; }
        public string HR_BAYARAN_CEK { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KE_JABATAN { get; set; }
        public string HR_KOD_GAJI { get; set; }
        public string HR_KELAS_PERJALANAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LANTIKAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TIDAK_AKTIF { get; set; }
        public string HR_GAJI_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_GAJI { get; set; }
        public Nullable<System.DateTime> HR_PCB_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_PCB_TARIKH_AKHIR { get; set; }
        public Nullable<decimal> HR_NILAI_PCB { get; set; }
        public string HR_KOD_GELARAN_J { get; set; }
        public string HR_TANGGUH_GERAKGAJI_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string HR_SKIM { get; set; }
        public string HR_PERGERAKAN_GAJI { get; set; }
        public string HR_NO_KWSP { get; set; }
        public string HR_NO_PENCEN { get; set; }
        public string HR_NO_SOCSO { get; set; }
        public string HR_NO_PCB { get; set; }
        public string HR_INITIAL { get; set; }
        public string HR_AM_YDP { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MASUK_KERAJAAN { get; set; }
        public string HR_UNIFORM { get; set; }
        public string HR_TEKNIKAL { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KELUAR_MBPJ { get; set; }
    }

    public class Kewangan8Model
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_KOD_PERUBAHAN { get; set; }
        public Nullable<System.DateTime>  HR_TARIKH_MULA { get; set; }
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
        public int HR_KEW8_ID { get; set; }
        public string HR_LANTIKAN_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SP { get; set; }
        public string HR_SP_IND { get; set; }
    }
}
