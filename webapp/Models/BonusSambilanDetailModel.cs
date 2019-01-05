using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class BonusSambilanDetailModel
    {
        public int BulanBonus { get; set; }
        public int TahunBonus { get; set; }
        public string Nama { get; set; }
        public string NoPekerja { get; set; }
        public string NoKadPengenalan { get; set; }
        public string NoAkaunBank { get; set; }
        public string NoKWSP { get; set; }
        public DateTime? TarikhLantikan { get; set; }
        /// <summary>
        /// returns TarikhLantikan in dd-MM-yyyy format
        /// </summary>
        public string TarikhLantikanString
        {
            get
            {
                if (TarikhLantikan != null)
                {
                    return string.Format("{0:dd-MM-yyyy}", TarikhLantikan);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public decimal? Jan { get; set; }
        public decimal? Feb { get; set; }
        public decimal? Mac { get; set; }
        public decimal? April { get; set; }
        public decimal? Mei { get; set; }
        public decimal? Jun { get; set; }
        public decimal? Julai { get; set; }
        public decimal? Ogos { get; set; }
        public decimal? September { get; set; }
        public decimal? Oktober { get; set; }
        public decimal? November { get; set; }
        public decimal? Disember { get; set; }
        public decimal? JumlahGaji { get; set; }
        public decimal? GajiPurata { get; set; }
        public decimal? BonusDiterima { get; set; }
        public decimal? BonusLayak { get; set; }
        public string Catatan { get; set; }
        public bool IsMuktamad { get; set; }

        //new column sebab nak tengok bulan start dgn bulan end
        //berguna utk kira purata dan nak tengok bulan mana kita nak display kat report
        //hide semua bulan yang di luar dari range MinBulan - MaxBulan
        public int MinBulan { get; set; } 
        public int MaxBulan { get; set; }

        public static List<BonusSambilanDetailModel> GetBonusSambilanDetailData(int month, int year)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_BONUS_SAMBILAN_DETAIL> tableList = HR_BONUS_SAMBILAN_DETAIL.GetBonusSambilanDetailData(month, year);

            List<BonusSambilanDetailModel> outputList = new List<BonusSambilanDetailModel>();
            foreach (HR_BONUS_SAMBILAN_DETAIL y in tableList)
            {
                BonusSambilanDetailModel d = new BonusSambilanDetailModel();
                HR_MAKLUMAT_PERIBADI maklumat = db.HR_MAKLUMAT_PERIBADI
                    .Where(m => m.HR_NO_PEKERJA == y.HR_NO_PEKERJA).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == y.HR_NO_PEKERJA).FirstOrDefault();
                d.BulanBonus = y.HR_BULAN_BONUS;
                d.TahunBonus = y.HR_TAHUN_BONUS;
                d.Nama = maklumat.HR_NAMA_PEKERJA;
                d.NoPekerja = y.HR_NO_PEKERJA;
                d.NoKadPengenalan = y.HR_NO_KPBARU;
                d.NoAkaunBank = kerja.HR_NO_AKAUN_BANK;
                d.NoKWSP = kerja.HR_NO_KWSP;
                d.TarikhLantikan = kerja.HR_TARIKH_LANTIKAN;
                d.Jan = y.HR_JANUARI;
                d.Feb = y.HR_FEBRUARI;
                d.Mac = y.HR_MAC;
                d.April = y.HR_APRIL;
                d.Mei = y.HR_MEI;
                d.Jun = y.HR_JUN;
                d.Julai = y.HR_JULAI;
                d.Ogos = y.HR_OGOS;
                d.September = y.HR_SEPTEMBER;
                d.Oktober = y.HR_OKTOBER;
                d.November = y.HR_NOVEMBER;
                d.Disember = y.HR_DISEMBER;
                d.JumlahGaji = y.HR_JUMLAH_GAJI;
                d.GajiPurata = y.HR_GAJI_PURATA;
                d.BonusDiterima = y.HR_BONUS_DITERIMA;
                d.BonusLayak = y.HR_BONUS_LAYAK;
                d.Catatan = y.HR_CATATAN;
                d.MinBulan = y.HR_BULAN_START;
                d.MaxBulan = y.HR_BULAN_END;
                if (y.HR_MUKTAMAD > 0)
                {
                    d.IsMuktamad = true;
                }
                else
                {
                    d.IsMuktamad = false;
                }
                outputList.Add(d);
            }
            //sort by nama
            outputList = outputList.OrderBy(x => x.Nama).ToList();

            return outputList;
        }

        /// <summary>
        /// Output BONUS_SAMBILAN_DETAIL data from HR_TRANSAKSI_SAMBILAN_DETAIL
        /// </summary>
        /// <param name="startMonth">start of month (if not Jan)</param>
        /// <param name="month">bulan bonus dibayar</param>
        /// <param name="year">tahun bonus dibayar</param>
        /// <returns>List BonusSambilanDetailModel</returns>
        public static List<BonusSambilanDetailModel> GetDetailsFromTransaksi
            (int startMonth, int month, int year, int endMonth)
        {
           
            ApplicationDbContext db = new ApplicationDbContext();

            //get list of pekerja tidak aktif
            List<string> listHR_TidakAktif = db.HR_MAKLUMAT_PERIBADI
                .Where(s => s.HR_AKTIF_IND == "T"
                && s.HR_ALASAN != "A17").Select(s => s.HR_NO_PEKERJA).Distinct().ToList();

            //exclude bulan dibayar (month) and filter out tidak aktif
            List<string> listHR_PEKERJA = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_BULAN_BEKERJA >= startMonth
                && s.HR_BULAN_BEKERJA <= endMonth
                && s.HR_TAHUN == year
                && !listHR_TidakAktif.Contains(s.HR_NO_PEKERJA))                
                .Select(s => s.HR_NO_PEKERJA).Distinct().ToList();

            List<BonusSambilanDetailModel> outputList = new List<BonusSambilanDetailModel>();
            foreach (string hr_pekerja in listHR_PEKERJA)
            {
                BonusSambilanDetailModel det = new BonusSambilanDetailModel();
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunlain =
                    db.HR_TRANSAKSI_SAMBILAN_DETAIL.
                    Where(x => x.HR_NO_PEKERJA == hr_pekerja
                    && x.HR_TAHUN == year
                    && x.HR_BULAN_BEKERJA >= startMonth
                    && x.HR_BULAN_BEKERJA <= endMonth
                    //&& x.HR_BULAN_DIBAYAR == month 
                    ).ToList();

                HR_MAKLUMAT_PERIBADI maklumat = db.HR_MAKLUMAT_PERIBADI
                    .Where(m => m.HR_NO_PEKERJA == hr_pekerja).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == hr_pekerja).FirstOrDefault();
                det.BulanBonus = month;
                det.TahunBonus = year;
                det.Nama = maklumat.HR_NAMA_PEKERJA;
                det.NoPekerja = hr_pekerja;
                det.NoKadPengenalan = maklumat.HR_NO_KPBARU;
                det.NoAkaunBank = kerja.HR_NO_AKAUN_BANK;
                det.NoKWSP = kerja.HR_NO_KWSP;
                det.TarikhLantikan = kerja.HR_TARIKH_LANTIKAN;

                int totalBulanBerkhidmat = 0;

                det.Jan = GetGajiPokok(db, elaunlain, 1);
                if(det.Jan > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Feb = GetGajiPokok(db, elaunlain, 2);
                if (det.Feb > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Mac = GetGajiPokok(db, elaunlain, 3);
                if (det.Mac > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.April = GetGajiPokok(db, elaunlain, 4);
                if (det.April > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Mei = GetGajiPokok(db, elaunlain, 5);
                if (det.Mei > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Jun = GetGajiPokok(db, elaunlain, 6);
                if (det.Jun > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Julai = GetGajiPokok(db, elaunlain, 7);
                if (det.Julai > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Ogos = GetGajiPokok(db, elaunlain, 8);
                if (det.Ogos > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.September = GetGajiPokok(db, elaunlain, 9);
                if (det.September > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Oktober = GetGajiPokok(db, elaunlain, 10);
                if (det.Oktober > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.November = GetGajiPokok(db, elaunlain, 11);
                if (det.November > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.Disember = GetGajiPokok(db, elaunlain, 12);
                if (det.Disember > 0)
                {
                    totalBulanBerkhidmat++;
                }
                det.JumlahGaji = GetSumGajiPokok(elaunlain);
                det.MinBulan = startMonth;
                det.MaxBulan = endMonth;

                int totalBulanDikira = endMonth - startMonth + 1;
                if (totalBulanBerkhidmat > 0)
                {
                    det.GajiPurata = det.JumlahGaji == null ?
                        0 : decimal.Round(Convert.ToDecimal(det.JumlahGaji) / totalBulanBerkhidmat, 3);
                }
                //new changes: 
                decimal totalGajiPokok = GetSumGajiPokok(elaunlain);
                decimal bonusLayak = totalGajiPokok / totalBulanDikira;
                det.BonusLayak = bonusLayak;
                det.BonusDiterima = bonusLayak * (decimal)1.0; //temp start with decimal 1.0
                det.IsMuktamad = false;

                if (!IsNoData(det))
                {
                    outputList.Add(det);
                }
            }
            //sort by Nama
            outputList = outputList.OrderBy(x => x.Nama).ToList();
            return outputList;
        }

        private static bool IsNoData(BonusSambilanDetailModel det)
        {
            bool retbool = false;

            if(det.Jan == 0 && det.Feb == 0 && det.Mac == 0 && det.April == 0 && det.Mei == 0 && det.Jun == 0
                && det.Julai == 0 && det.Ogos == 0 && det.September == 0 && det.Oktober == 0 
                && det.November == 0 && det.Disember == 0)
            {
                retbool = true;
            }
            return retbool;
        }

        //this will be easier if kod potongan sosco masuk dlm transaksi
        private static decimal GetGajiPokok(ApplicationDbContext db, List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunList, int bulan)
        {
            decimal gajiPokok = elaunList
                .Where(s => s.HR_KOD_IND == "G" && s.HR_BULAN_BEKERJA == bulan).Sum(c => c.HR_JUMLAH).Value;
            //if(gajiPokok > 0)
            //{
            //    decimal elaun = elaunList
            //        .Where(s => s.HR_KOD_IND == "E" && s.HR_BULAN_BEKERJA == bulan).Sum(c => c.HR_JUMLAH).Value;
            //    decimal elaunOT = elaunList
            //        .Where(s => s.HR_KOD == "E0164" && s.HR_BULAN_BEKERJA == bulan).Sum(c => c.HR_JUMLAH).Value;
            //    decimal potongan = elaunList
            //        .Where(s => s.HR_KOD_IND == "P" && s.HR_BULAN_BEKERJA == bulan).Sum(c => c.HR_JUMLAH).Value;
            //    decimal potonganSocso = PageSejarahModel.GetPotonganSocso(db, gajiPokok, elaunOT);
            //    decimal gajiBersih = gajiPokok + elaun - potongan - potonganSocso;                
            //    return gajiBersih;
            //}
            //else
            //{
            //    return gajiPokok;
            //}
            return gajiPokok;
        }

        //this will be easier if kod potongan sosco masuk dlm transaksi
        private static decimal GetGajiBersihSum(ApplicationDbContext db, List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunList, int minBulan, int maxBulan)
        {
            decimal sumGajiBersih = 0;
            for(int i = minBulan; i <= maxBulan; i++)
            {
                decimal gajiBersihBulan = GetGajiPokok(db, elaunList, i);
                sumGajiBersih = sumGajiBersih + gajiBersihBulan;
            }
            return sumGajiBersih;
        }

        private static decimal GetSumGajiPokok(List<HR_TRANSAKSI_SAMBILAN_DETAIL> elaunList)
        {
            decimal gaji = elaunList
                .Where(s => s.HR_KOD_IND == "G").Sum(c => c.HR_JUMLAH).Value;
            return gaji;
        }
    }
}