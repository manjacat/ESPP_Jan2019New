using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class Luaran
    {
            [Key]
            public string NoIC { get; set; }
            public string Nama { get; set; }
            public string Keturunan { get; set; }
             public string NamaLain { get; set; }
            public string NoICLama { get; set; }
            public string WarnaIC { get; set; }
            public string LesenMemandu { get; set; }
            public string Kelas { get; set; }
            public string Alamat { get; set; }
            public string TempatLahir { get; set; }
            public string TarikhLahir { get; set; }
            public string Agama { get; set; }
            public string TarafPerkahwinan { get; set; }
            public string Jantina { get; set; }
            public string Warganegara { get; set; }
            public string Poskod { get; set; }
            public string Negeri { get; set; }
            public string Bandar { get; set; }
            public string TelPejabat { get; set; }
            public string TelRumah { get; set; }
            public string TelBimbit { get; set; }
            public string Email { get; set; }
            public string NamaBapa { get; set; }
            public string TarikhLahirBapa { get; set; }
            public string TempatLahirBapa { get; set; }
            public string NamaIbu { get; set; }
            public string TarikhLahirIbu { get; set; }
            public string TempatLahirIbu { get; set; }
            public string TarikhdanAktiviti { get; set; }
            public string Peringkat { get; set; }
            public string NamaAktiviti { get; set; }
            public string Anjuran { get; set; }
            public string Bahasa { get; set; }
            public string Pembacaan { get; set; }
            public string Penulisan { get; set; }
            public string Pertuturan { get; set; }
            public string TarikhMula { get; set; }
            public string TarikhTamat { get; set; }
            public string NamaSekolah { get; set; }
            public string Darjah { get; set; }
            public string Kemahiran { get; set; }
            public string Tahap { get; set; }
            public string KodPeperiksaan { get; set; }
            public string AngkGiliran { get; set; }
            public string Pangkat { get; set; }
            public string Jawatan { get; set; }
            public string GajiPokok { get; set; }
            public string Majikan { get; set; }
            public string TarikhDari { get; set; }
            public string AlasanBerhenti { get; set; }
            public string TarikhHingga { get; set; }
            public string TarikhLulus { get; set; }
            public string NamaInstitusi { get; set; }
            public string DiplomaIjazah { get; set; }
            public string BidangPengkhususan { get; set; }
            public string CGPA { get; set; }
            public string JawatanPermohonan { get; set; }
            public string TarafJawatan { get; set; }
            public string TarikhPermohonan { get; set; }
            public string Status { get; set; }
    }

}
