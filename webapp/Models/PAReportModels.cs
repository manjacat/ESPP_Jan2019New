using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{

    public class PAReportModels
    {
        DbSet<PA_REPORT> PA_REPORT { get; set; }
    }
    public partial class PA_REPORT
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public byte PA_BULAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public short PA_TAHUN { get; set; }
        public string PA_JABATAN { get; set; }
        public string PA_BAHAGIAN { get; set; }
        public string PA_UNIT { get; set; }
        public string PA_KAKITANGAN_IND { get; set; }
        public string PA_TARAF_JAWATAN { get; set; }
        public string PA_STATUS_KWSP { get; set; }
        public string PA_STATUS_SOCSO { get; set; }
        public string PA_STATUS_PENCEN { get; set; }
        public string PA_STATUS_PCB { get; set; }
        public string PA_GRED { get; set; }
        public string PA_MATRIKS_GAJI { get; set; }
        public string PA_KOD_BANK { get; set; }
        //number 8,2
        public decimal? PA_CC_KENDERAAN { get; set; }
        public string PA_NO_KENDERAAN { get; set; }
        public string PA_JENIS_KENDERAAN { get; set; }
        public string PA_NO_AKAUN_BANK { get; set; }
        //date
        public DateTime? PA_BULAN_KENAIKAN_GAJI { get; set; }
        public string PA_JAWATAN { get; set; }
        //number 8,2
        public decimal? PA_GAJI_POKOK { get; set; }

        public static List<PA_REPORT> TestSelect(SPGContext spgDb)
        {
            string noPekerja = "03807";
            int tahun = 2018;
            int bulan = 1;
            List<PA_REPORT> report_all = new List<PA_REPORT>();
            report_all = spgDb.PA_REPORT
                .Where(s => s.PA_NO_PEKERJA == noPekerja
                && s.PA_TAHUN == tahun
                && s.PA_BULAN == bulan).ToList();
            return report_all;
        }

        //TODO InsertData
        public static void InsertSpgReport(ApplicationDbContext sppDb, SPGContext spgDb,
            int tahun, int bulan)
        {
            //int tahun = 2019;
            //int bulan = 4;

            //List<string> listPekerja = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
            //    .Where(s => s.HR_TAHUN == tahun
            //    && s.HR_BULAN_DIBAYAR == bulan)
            //    .Select(s => s.HR_NO_PEKERJA).Distinct().ToList();
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> gajiAll =
               sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
               .Where(s => s.HR_KOD == "GAJPS"
               && s.HR_TAHUN == tahun
               && s.HR_BULAN_DIBAYAR == bulan).ToList();

            foreach (var item in gajiAll)
            {
                string noPekerja = item.HR_NO_PEKERJA;
                int bulanBekerja = item.HR_BULAN_BEKERJA;
                int bulanDibayar = item.HR_BULAN_DIBAYAR;
                int tahunBekerja = item.HR_TAHUN_BEKERJA;
                int tahunDibayar = item.HR_TAHUN;

                HR_MAKLUMAT_PEKERJAAN mPekerjaan = sppDb.HR_MAKLUMAT_PEKERJAAN
                    .Where(s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
                HR_MAKLUMAT_PERIBADI mPeribadi = sppDb.HR_MAKLUMAT_PERIBADI
                    .Where(s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
                PA_REPORT spgReport = spgDb.PA_REPORT
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN == tahunBekerja
                    && s.PA_BULAN == bulanBekerja).FirstOrDefault();

                decimal gajiPokok = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_KOD == "GAJPS"
                    && s.HR_NO_PEKERJA == noPekerja
                    && s.HR_BULAN_DIBAYAR == bulanDibayar
                    && s.HR_TAHUN == tahunDibayar
                    && s.HR_BULAN_BEKERJA == bulanBekerja
                    && s.HR_TAHUN_BEKERJA == tahunBekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;

                if (spgReport == null)
                {
                    //insert
                    spgReport = new PA_REPORT
                    {
                        PA_NO_PEKERJA = noPekerja,
                        PA_BULAN = (byte)bulanBekerja,
                        PA_TAHUN = (short)tahunBekerja,
                        PA_JABATAN = mPekerjaan.HR_JABATAN,
                        PA_BAHAGIAN = mPekerjaan.HR_BAHAGIAN,
                        PA_UNIT = mPekerjaan.HR_UNIT,
                        PA_KAKITANGAN_IND = mPekerjaan.HR_KAKITANGAN_IND,
                        PA_TARAF_JAWATAN = mPekerjaan.HR_TARAF_JAWATAN,
                        PA_STATUS_KWSP = mPekerjaan.HR_STATUS_KWSP,
                        PA_STATUS_SOCSO = mPekerjaan.HR_STATUS_SOCSO,
                        PA_STATUS_PENCEN = mPekerjaan.HR_STATUS_PENCEN,
                        PA_STATUS_PCB = mPekerjaan.HR_STATUS_PCB,
                        PA_GRED = mPekerjaan.HR_GRED,
                        PA_MATRIKS_GAJI = mPekerjaan.HR_MATRIKS_GAJI,
                        PA_KOD_BANK = mPekerjaan.HR_KOD_BANK,
                        //number 8,2
                        PA_CC_KENDERAAN = mPeribadi.HR_CC_KENDERAAN,
                        PA_NO_KENDERAAN = mPeribadi.HR_NO_KENDERAAN,
                        PA_JENIS_KENDERAAN = mPeribadi.HR_JENIS_KENDERAAN,
                        PA_NO_AKAUN_BANK = mPekerjaan.HR_NO_AKAUN_BANK,
                        //date
                        PA_BULAN_KENAIKAN_GAJI = mPekerjaan.HR_BULAN_KENAIKAN_GAJI,
                        PA_JAWATAN = mPekerjaan.HR_JAWATAN,
                        //number 8,2
                        PA_GAJI_POKOK = gajiPokok
                    };
                    try
                    {
                        spgDb.PA_REPORT.Add(spgReport);
                        spgDb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                    }
                }
                else
                {
                    //update
                    spgReport.PA_NO_PEKERJA = noPekerja;
                    spgReport.PA_BULAN = (byte)bulanBekerja;
                    spgReport.PA_TAHUN = (short)tahunBekerja;
                    spgReport.PA_JABATAN = mPekerjaan.HR_JABATAN;
                    spgReport.PA_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;
                    spgReport.PA_UNIT = mPekerjaan.HR_UNIT;
                    spgReport.PA_KAKITANGAN_IND = mPekerjaan.HR_KAKITANGAN_IND;
                    spgReport.PA_TARAF_JAWATAN = mPekerjaan.HR_TARAF_JAWATAN;
                    spgReport.PA_STATUS_KWSP = mPekerjaan.HR_STATUS_KWSP;
                    spgReport.PA_STATUS_SOCSO = mPekerjaan.HR_STATUS_SOCSO;
                    spgReport.PA_STATUS_PENCEN = mPekerjaan.HR_STATUS_PENCEN;
                    spgReport.PA_STATUS_PCB = mPekerjaan.HR_STATUS_PCB;
                    spgReport.PA_GRED = mPekerjaan.HR_GRED;
                    spgReport.PA_MATRIKS_GAJI = mPekerjaan.HR_MATRIKS_GAJI;
                    spgReport.PA_KOD_BANK = mPekerjaan.HR_KOD_BANK;
                    //number 8,2
                    spgReport.PA_CC_KENDERAAN = mPeribadi.HR_CC_KENDERAAN;
                    spgReport.PA_NO_KENDERAAN = mPeribadi.HR_NO_KENDERAAN;
                    spgReport.PA_JENIS_KENDERAAN = mPeribadi.HR_JENIS_KENDERAAN;
                    spgReport.PA_NO_AKAUN_BANK = mPekerjaan.HR_NO_AKAUN_BANK;
                    //date
                    spgReport.PA_BULAN_KENAIKAN_GAJI = mPekerjaan.HR_BULAN_KENAIKAN_GAJI;
                    spgReport.PA_JAWATAN = mPekerjaan.HR_JAWATAN;
                    //number 8,2
                    spgReport.PA_GAJI_POKOK = gajiPokok;

                    try
                    {
                        spgDb.Entry(spgReport).State = EntityState.Modified;
                        spgDb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}