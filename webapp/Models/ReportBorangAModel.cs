using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PekerjaReportModel
    {
        public int Bil { get; set; }
        public string NoKadPengenalan { get; set; }
        public string NoKesSosial { get; set; }
        public string NamaPekerja { get; set; }
        private decimal _carumanRM;
        public decimal CarumanRM
        {
            get
            {
                try
                {
                    return Decimal.Round(_carumanRM, 2);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                _carumanRM = value;
            }
        }

        public static string[] kwspSambilan
        {
            get
            {
                return new string[]
                {
                    "P0035",
                    "P0050"
                };
            }
        }

        public static string[] perkesoSambilan
        {
            get
            {
                return new string[]
                {
                    "P0160",
                    "P0163"
                };
            }
        }

        //a.k.a. Kod caruman majikan
        public static string[] kodBorangA
        {
            get
            {
                return new string[]
                {
                        "C0034",
                        "C0020"
                };
            }
        }

        public static List<PekerjaReportModel> GetPerkesoSambilanSukan
            (int bulanProses, int tahunProses, bool isSukan)
        {
            SPGContext spgDb = new SPGContext();
            ApplicationDbContext db = new ApplicationDbContext();

            //N = sambilan, A = sukan
            List<string> list_pekerja = new List<string>();
            if (isSukan)
            {
                list_pekerja = PageSejarahModel.ListPekerjaSukan;
            }
            else
            {
                list_pekerja = PageSejarahModel.ListPekerjaSambilan;
            }

            var listGaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_BULAN_DIBAYAR == bulanProses
                && s.HR_TAHUN == tahunProses
                && s.HR_KOD_IND == "G"
                && list_pekerja.Contains(s.HR_NO_PEKERJA)).ToList();

            List<PekerjaReportModel> pekerja = new List<PekerjaReportModel>();

            int counter = 0;
            foreach (var item in listGaji)
            {
                var bulanBekerja = item.HR_BULAN_BEKERJA;
                var tahunBekerja = item.HR_TAHUN_BEKERJA;
                var bulanDibayar = item.HR_BULAN_DIBAYAR;
                var tahunDibayar = item.HR_TAHUN;

                HR_MAKLUMAT_PERIBADI maklumatPeribat = db.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA)
                    .FirstOrDefault();

                var detail = PageSejarahModel.GetTRANSAKSI_SAMBILAN_DETAIL
                    (db, item.HR_NO_PEKERJA, tahunDibayar, bulanDibayar,
                    tahunBekerja, bulanBekerja);
                var sosco = PageSejarahModel.GetPotonganSocso(db, detail);

                if (maklumatPeribat != null)
                {
                    PekerjaReportModel m = new PekerjaReportModel();
                    counter++;
                    m.Bil = counter;
                    m.NamaPekerja = maklumatPeribat.HR_NAMA_PEKERJA;
                    m.NoKesSosial = string.Empty;
                    m.NoKadPengenalan = maklumatPeribat.HR_NO_KPBARU;
                    try
                    {
                        decimal totalVal = Convert.ToDecimal
                        (sosco.HR_CARUMAN_PEKERJA);
                        totalVal = decimal.Round(totalVal);
                        m.CarumanRM = totalVal;
                    }
                    catch
                    {
                        m.CarumanRM = 0;
                    }
                    pekerja.Add(m);
                }
            }
            return pekerja;
        }

        public static List<PekerjaReportModel> BorangASambilanSukan
            (int bulanProses, int tahunProses, bool isSukan)
        {
            SPGContext spgDb = new SPGContext();
            ApplicationDbContext db = new ApplicationDbContext();

            //N = sambilan, A = sukan
            List<string> list_pekerja = new List<string>();
            if (isSukan)
            {
                list_pekerja = PageSejarahModel.ListPekerjaSukan;
            }
            else
            {
                list_pekerja = PageSejarahModel.ListPekerjaSambilan;
            }

            var listGaji = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_BULAN_DIBAYAR == bulanProses
                && s.HR_TAHUN == tahunProses
                && s.HR_KOD_IND == "G"
                && list_pekerja.Contains(s.HR_NO_PEKERJA)).ToList();

            List<PekerjaReportModel> pekerja = new List<PekerjaReportModel>();

            int counter = 0;
            foreach (var item in listGaji)
            {
                var bulanBekerja = item.HR_BULAN_BEKERJA;
                var tahunBekerja = item.HR_TAHUN_BEKERJA;
                var bulanDibayar = item.HR_BULAN_DIBAYAR;
                var tahunDibayar = item.HR_TAHUN;

                HR_MAKLUMAT_PERIBADI maklumatPeribat = db.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA)
                    .FirstOrDefault();

                var detail = PageSejarahModel.GetTRANSAKSI_SAMBILAN_DETAIL
                    (db, item.HR_NO_PEKERJA, tahunDibayar, bulanDibayar,
                    tahunBekerja, bulanBekerja);
                var gajiKasar = PageSejarahModel.GetGajiKasar(db, detail);
                HR_KWSP kwsp = PageSejarahModel.GetPotonganKWSP(db, gajiKasar);

                if (maklumatPeribat != null)
                {
                    PekerjaReportModel m = new PekerjaReportModel();
                    counter++;
                    m.Bil = counter;
                    m.NamaPekerja = maklumatPeribat.HR_NAMA_PEKERJA;
                    m.NoKesSosial = string.Empty;
                    m.NoKadPengenalan = maklumatPeribat.HR_NO_KPBARU;
                    try
                    {
                        decimal totalVal = kwsp.HR_CARUMAN_PEKERJA;
                        totalVal = decimal.Round(totalVal);
                        m.CarumanRM = totalVal;
                    }
                    catch
                    {
                        m.CarumanRM = 0;
                    }
                    pekerja.Add(m);
                }
            }
            return pekerja;
        }

        //public static List<PekerjaReportModel> GetBorangASambilan(int bulan, int tahun)
        //{
        //    SPGContext spgDb = new SPGContext();
        //    ApplicationDbContext db = new ApplicationDbContext();

        //    //N = sambilan, A = sukan
        //    List<string> list_pekerja = PageSejarahModel.ListPekerjaSambilan;

        //    var potongList = spgDb.PA_TRANSAKSI_PEMOTONGAN
        //        .Where(s => s.PA_BULAN_POTONGAN == bulan
        //        && s.PA_TAHUN_POTONGAN == tahun
        //        && kwspSambilan.Contains(s.PA_KOD_PEMOTONGAN)
        //        && list_pekerja.Contains(s.PA_NO_PEKERJA))
        //        .GroupBy(s => new
        //        {
        //            s.PA_NO_PEKERJA,
        //            s.PA_BULAN_POTONGAN,
        //            s.PA_TAHUN_POTONGAN
        //        })
        //        .Select(g => new
        //        {
        //            PA_No_Pekerja = g.Key.PA_NO_PEKERJA,
        //            PA_Jumlah_Pemontongan = g.Sum(x => x.PA_JUMLAH_PEMOTONGAN),
        //            PA_Bulan = g.Key.PA_BULAN_POTONGAN,
        //            PA_Tahun = g.Key.PA_TAHUN_POTONGAN
        //        })
        //        .ToList();


        //    List<PekerjaReportModel> pekerja = new List<PekerjaReportModel>();

        //    int counter = 0;
        //    foreach (var item in potongList)
        //    {
        //        HR_MAKLUMAT_PERIBADI maklumatPeribat =
        //            db.HR_MAKLUMAT_PERIBADI
        //            .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja)
        //            .FirstOrDefault();

        //        if (maklumatPeribat != null)
        //        {
        //            PekerjaReportModel m = new PekerjaReportModel();
        //            counter++;
        //            m.Bil = counter;
        //            m.NamaPekerja = maklumatPeribat.HR_NAMA_PEKERJA;
        //            m.NoKesSosial = string.Empty;
        //            m.NoKadPengenalan = maklumatPeribat.HR_NO_KPBARU;
        //            try
        //            {
        //                decimal totalVal = Convert.ToDecimal
        //                    (spgDb.PA_TRANSAKSI_CARUMAN
        //                    .Where(p => p.PA_NO_PEKERJA == item.PA_No_Pekerja
        //                    && p.PA_BULAN_CARUMAN == item.PA_Bulan
        //                    && p.PA_TAHUN_CARUMAN == item.PA_Tahun
        //                    && kodBorangA.Contains(p.PA_KOD_CARUMAN))
        //                    .Sum(p => p.PA_JUMLAH_CARUMAN));

        //                //decimal totalVal = Convert.ToDecimal(item.PA_Jumlah_Pemontongan); //* Convert.ToDecimal(caruman);
        //                totalVal = decimal.Round(totalVal);
        //                m.CarumanRM = totalVal;
        //            }
        //            catch
        //            {
        //                m.CarumanRM = 0;
        //            }
        //            pekerja.Add(m);
        //        }
        //    }
        //    return pekerja;
        //}

    }

    public class ReportBorangAModel
    {
        public ReportBorangAModel()
        {
            PekerjaSambilan = new List<PekerjaReportModel>();
        }

        public List<PekerjaReportModel> PekerjaSambilan { get; set; }

        public static ReportBorangAModel GetReport(int bulanProses, 
            int tahunProses, string jenisLaporan)
        {
            ReportBorangAModel reportData = new ReportBorangAModel();
            switch (jenisLaporan)
            {
                case ("1"): //borangASambilan
                    reportData.PekerjaSambilan = 
                        PekerjaReportModel.BorangASambilanSukan
                        (bulanProses, tahunProses, false);
                    break;
                case ("2"): //borangATunggakanSambilan
                    reportData.PekerjaSambilan = 
                        PekerjaReportModel
                        .BorangASambilanSukan
                        (bulanProses, tahunProses,false);
                    break;
                case ("3"): //borangASukan
                    reportData.PekerjaSambilan = PekerjaReportModel
                        .BorangASambilanSukan
                        (bulanProses, tahunProses,true);
                    break;
                case ("4"): //borangATunggakanSukan
                    reportData.PekerjaSambilan = 
                        PekerjaReportModel.BorangASambilanSukan
                        (bulanProses, tahunProses,true);
                    break;
            }
            return reportData;
        }
    }
}

/*
 * #region get Elaun & Caruman
    HR_ELAUN elaun1 = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == item.HR_KOD);
    if (elaun1 != null)
    {
        maklumatelaunpotongan = elaun1.HR_PENERANGAN_ELAUN;
    }
    HR_POTONGAN potongan1 = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == item.HR_KOD);
    if (potongan1 != null)
    {
        maklumatelaunpotongan = potongan1.HR_PENERANGAN_POTONGAN;
    }
    HR_CARUMAN caruman1 = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == item.HR_KOD);
    if (caruman1 != null)
    {
        maklumatelaunpotongan = caruman1.HR_PENERANGAN_CARUMAN;
    }
    HR_GAJI_UPAHAN gaji1 = db.HR_GAJI_UPAHAN.SingleOrDefault(s => s.HR_KOD_UPAH == item.HR_KOD);
    if (gaji1 != null)
    {
        maklumatelaunpotongan = gaji1.HR_PENERANGAN_UPAH;
    }
#endregion
 */
