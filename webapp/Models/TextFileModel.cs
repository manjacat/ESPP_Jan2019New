using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TextFileModel
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }
        public string String4 { get; set; }
        public string String5 { get; set; }
        public string String6 { get; set; }

        public static List<TextFileModel> GetPerkesoSambilanSukan(int bulan, int tahun)
        {
            List<TextFileModel> report = new List<TextFileModel>();

            SPGContext spgDb = new SPGContext();
            ApplicationDbContext db = new ApplicationDbContext();
            string[] kwspSambilan = new string[]
            {
                "P0160",
                "P0163"
            };

            //N = sambilan, A = sukan
            List<string> list_pekerja = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_TARAF_JAWATAN == "A").Select(x => x.HR_NO_PEKERJA).ToList();

            var potongList = spgDb.PA_TRANSAKSI_PEMOTONGAN
                .Where(s => s.PA_BULAN_POTONGAN == bulan
                && s.PA_TAHUN_POTONGAN == tahun
                && kwspSambilan.Contains(s.PA_KOD_PEMOTONGAN)
                && list_pekerja.Contains(s.PA_NO_PEKERJA))
                .GroupBy(s => new { s.PA_NO_PEKERJA, s.PA_BULAN_POTONGAN, s.PA_TAHUN_POTONGAN })
                .Select(g => new
                {
                    PA_No_Pekerja = g.Key.PA_NO_PEKERJA,
                    PA_Jumlah_Pemontongan = g.Sum(x => x.PA_JUMLAH_PEMOTONGAN),
                    PA_Bulan = g.Key.PA_BULAN_POTONGAN,
                    PA_Tahun = g.Key.PA_TAHUN_POTONGAN
                })
                .ToList();

            foreach (var item in potongList)
            {
                HR_MAKLUMAT_PERIBADI maklumatPeribat = db.HR_MAKLUMAT_PERIBADI
                   .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == item.PA_No_Pekerja).FirstOrDefault();
                string tarikhKerja = string.Format("{0:ddMMyyyy}", DateTime.Now);
                if (kerja != null)
                {
                    tarikhKerja = string.Format("{0:ddMMyyyy}", kerja.HR_TARIKH_MASUK);
                }

                if (maklumatPeribat != null)
                {
                    TextFileModel m = new TextFileModel();
                    m.String1 = "B3200000538V";
                    m.String2 = maklumatPeribat.HR_NO_KPBARU;
                    m.String3 = maklumatPeribat.HR_NAMA_PEKERJA;
                    m.String4 = string.Format("{0}{1}", bulan.ToString().PadLeft(2, '0'), tahun);
                    try
                    {
                        decimal totalCaruman = Convert.ToDecimal(spgDb.PA_TRANSAKSI_CARUMAN
                            .Where(p => p.PA_NO_PEKERJA == item.PA_No_Pekerja
                            && p.PA_BULAN_CARUMAN == item.PA_Bulan
                            && p.PA_TAHUN_CARUMAN == item.PA_Tahun
                            && p.PA_KOD_CARUMAN == "C0034")
                            .Sum(p => p.PA_JUMLAH_CARUMAN));
                        decimal totalPemotongan = Convert.ToDecimal(item.PA_Jumlah_Pemontongan);
                        decimal totalSum = decimal.Round((totalCaruman + totalPemotongan) * 100);
                        m.String5 = string.Format("{0}{1}", totalSum, tarikhKerja);
                    }
                    catch
                    {
                        m.String5 = string.Empty;
                    }
                    report.Add(m);
                }
            }
            return report;
        }

        public static List<TextFileModel> GetPerkesoSambilan(int bulan, int tahun)
        {
            List<TextFileModel> report = new List<TextFileModel>();

            SPGContext spgDb = new SPGContext();
            ApplicationDbContext db = new ApplicationDbContext();
            string[] kwspSambilan = new string[]
            {
                "P0160",
                "P0163"
            };

            //N = sambilan, A = sukan
            List<string> list_pekerja = db.HR_MAKLUMAT_PEKERJAAN
                .Where(s => s.HR_TARAF_JAWATAN == "N").Select(x => x.HR_NO_PEKERJA).ToList();

            var potongList = spgDb.PA_TRANSAKSI_PEMOTONGAN
                .Where(s => s.PA_BULAN_POTONGAN == bulan
                && s.PA_TAHUN_POTONGAN == tahun
                && kwspSambilan.Contains(s.PA_KOD_PEMOTONGAN)
                && list_pekerja.Contains(s.PA_NO_PEKERJA))
                .GroupBy(s => new { s.PA_NO_PEKERJA, s.PA_BULAN_POTONGAN, s.PA_TAHUN_POTONGAN })
                .Select(g => new
                {
                    PA_No_Pekerja = g.Key.PA_NO_PEKERJA,
                    PA_Jumlah_Pemontongan = g.Sum(x => x.PA_JUMLAH_PEMOTONGAN),
                    PA_Bulan = g.Key.PA_BULAN_POTONGAN,
                    PA_Tahun = g.Key.PA_TAHUN_POTONGAN
                })
                .ToList();

            foreach (var item in potongList)
            {
                HR_MAKLUMAT_PERIBADI maklumatPeribat = db.HR_MAKLUMAT_PERIBADI
                   .Where(s => s.HR_NO_PEKERJA == item.PA_No_Pekerja).FirstOrDefault();
                HR_MAKLUMAT_PEKERJAAN kerja = db.HR_MAKLUMAT_PEKERJAAN
                    .Where(m => m.HR_NO_PEKERJA == item.PA_No_Pekerja).FirstOrDefault();
                string tarikhKerja = string.Format("{0:ddMMyyyy}", DateTime.Now);
                if (kerja != null)
                {
                    tarikhKerja = string.Format("{0:ddMMyyyy}", kerja.HR_TARIKH_MASUK);
                }

                if (maklumatPeribat != null)
                {
                    TextFileModel m = new TextFileModel();
                    m.String1 = "B3200000538V";
                    m.String2 = maklumatPeribat.HR_NO_KPBARU;
                    m.String3 = maklumatPeribat.HR_NAMA_PEKERJA;
                    m.String4 = string.Format("{0}{1}", bulan.ToString().PadLeft(2,'0'), tahun);
                    try
                    {
                        decimal totalCaruman = Convert.ToDecimal(spgDb.PA_TRANSAKSI_CARUMAN
                            .Where(p => p.PA_NO_PEKERJA == item.PA_No_Pekerja
                            && p.PA_BULAN_CARUMAN == item.PA_Bulan
                            && p.PA_TAHUN_CARUMAN == item.PA_Tahun
                            && p.PA_KOD_CARUMAN == "C0034")
                            .Sum(p => p.PA_JUMLAH_CARUMAN));
                        decimal totalPemotongan = Convert.ToDecimal(item.PA_Jumlah_Pemontongan);
                        decimal totalSum = decimal.Round((totalCaruman + totalPemotongan) * 100);
                        m.String5 = string.Format("{0}{1}", totalSum, tarikhKerja);
                    }
                    catch
                    {
                        m.String5 = string.Empty;
                    }
                    report.Add(m);
                }
            }
            return report;
        }
    }
}