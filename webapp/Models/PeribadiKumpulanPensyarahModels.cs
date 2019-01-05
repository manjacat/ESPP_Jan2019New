using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PeribadiKumpulanPensyarahModels
    {

        //HR_KUMPULAN_PENSYARAH
        public string HR_KOD_KUMPULAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public Nullable<decimal> HR_KADAR_JAM { get; set; }
        public Nullable<decimal> HR_NILAI_MAKSIMA { get; set; }
        public Nullable<decimal> HR_PERATUS { get; set; }
        public string HR_JENIS_IND { get; set; }


        //HR_PENSYARAH

        public string HR_NO_PENSYARAH { get; set; }
        public string HR_NAMA_PENSYARAH { get; set; }
        //public string HR_KOD_KUMPULAN { get; set; }
        public string HR_NO_KPBARU { get; set; }
        public string HR_NO_KPLAMA { get; set; }
        public string HR_NO_TELPEJABAT { get; set; }
        public string HR_NO_TELBIMBIT { get; set; }
        public string HR_NO_FAKS { get; set; }
        public string HR_JAWATAN { get; set; }
        public string HR_GRED_KELULUSAN { get; set; }
        public Nullable<decimal> HR_GAJI_POKOK { get; set; }
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
        public string HR_NO_PEKERJA { get; set; }


        //HR_PENGALAMAN_KERJA
        public IEnumerable<PeribadiModels> HR_MAKLUMAT_PERIBADI { get; set; }


         }

        //HR_MAKLUMAT_PERIBADI
        public class PeribadiModels
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
    
    }
