using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPekerjaanModels
    {
        public virtual DbSet<HR_MAKLUMAT_PEKERJAAN> HR_MAKLUMAT_PEKERJAAN { get; set; }
    }

    [Table("HR_MAKLUMAT_PEKERJAAN")]
    public class HR_MAKLUMAT_PEKERJAAN
    {

        [Key]
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

        
        public virtual ICollection<HR_MAKLUMAT_ELAUN_POTONGAN> HR_MAKLUMAT_ELAUN_POTONGAN { get; set; }
        [ForeignKey("HR_NO_PEKERJA")]

        public virtual HR_MAKLUMAT_PERIBADI HR_MAKLUMAT_PERIBADI { get; set; }
        [ForeignKey("HR_NO_PEKERJA")]
        public virtual HR_PERSARAAN HR_PERSARAAN { get; set; }
    }
}