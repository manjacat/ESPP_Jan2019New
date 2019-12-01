using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiGajiModels
    {
        DbSet<PA_TRANSAKSI_GAJI> PA_TRANSAKSI_GAJI { get; set; }
    }
    public partial class PA_TRANSAKSI_GAJI
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        public System.DateTime PA_TARIKH_PROCESS { get; set; }
        public Nullable<decimal> PA_JUMLAH_ELAUN { get; set; }
        public Nullable<decimal> PA_JUMLAH_PEMOTONGAN { get; set; }
        public Nullable<decimal> PA_JUMLAH_PCB { get; set; }
        public Nullable<decimal> PA_GAJI_POKOK { get; set; }
        public Nullable<decimal> PA_GAJI_BERSIH { get; set; }
        public string PA_NO_SLIP_GAJI { get; set; }
        [Key]
        [Column(Order = 1)]
        public short PA_TAHUN_GAJI { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PA_BULAN_GAJI { get; set; }
        public Nullable<decimal> PA_JUMLAH_CARUMAN { get; set; }
        public Nullable<decimal> PA_JUMLAH_PELARASAN { get; set; }
        public string PA_PROSES_IND { get; set; }

        public static void InsertSPGGaji(ApplicationDbContext sppDb, SPGContext spgDb,
            int tahunDibayar, int bulanDibayar)
        {
            //ApplicationDbContext sppDb = new ApplicationDbContext();
            //int tahun = 2019;
            //int bulanDibayar = 4;
            //string noPekerja = "01595";

            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans =
                HR_TRANSAKSI_SAMBILAN_DETAIL
                .GetTransaksiDibayar(sppDb, tahunDibayar, bulanDibayar);

            if (sppTrans != null)
            {
                InsertToSPG(sppDb, spgDb, sppTrans);
            }
        }

        private static void InsertToSPG(ApplicationDbContext sppDb, SPGContext spgDb,
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans)
        {
            List<string> noPekerja_all =
                sppTrans.Select(s => s.HR_NO_PEKERJA).Distinct().ToList();
            int bulanDibayar = sppTrans.Where(s => s.HR_KOD == "GAJPS").Select(s => s.HR_BULAN_DIBAYAR).FirstOrDefault();
            int tahunDibayar = sppTrans.Where(s => s.HR_KOD == "GAJPS").Select(s => s.HR_TAHUN).FirstOrDefault();

            foreach (var noPekerja in noPekerja_all)
            {
                //get List of transaksi by No Pekerja
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTransData =
                    sppTrans.Where(s => s.HR_NO_PEKERJA == noPekerja).ToList();
                var jumlahElaun = sppTransData.Where(s => s.HR_KOD_IND == "E")
                    .Select(s => s.HR_JUMLAH).Sum();
                jumlahElaun = jumlahElaun == null ? 0 : jumlahElaun;
                var jumlahPemotongan = sppTransData.Where(s => s.HR_KOD_IND == "P")
                    .Select(s => s.HR_JUMLAH).Sum();
                jumlahPemotongan = jumlahPemotongan == null ? 0 : jumlahPemotongan;
                var gajiPokok = sppTransData.Where(s => s.HR_KOD == "GAJPS")
                    .Select(s => s.HR_JUMLAH).Sum();
                gajiPokok = gajiPokok == null ? 0 : gajiPokok;
                var gajiBersih = gajiPokok + jumlahElaun - jumlahPemotongan;
                var jumlahCaruman = sppTransData.Where(s => s.HR_KOD_IND == "C")
                    .Select(s => s.HR_JUMLAH).Sum();
                jumlahCaruman = jumlahCaruman == null ? 0 : jumlahCaruman;

                PA_TRANSAKSI_GAJI spgTrans = spgDb.PA_TRANSAKSI_GAJI
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_GAJI == tahunDibayar
                    && s.PA_BULAN_GAJI == bulanDibayar).FirstOrDefault();
                if (spgTrans == null)
                {
                    spgTrans = new PA_TRANSAKSI_GAJI
                    {
                        PA_NO_PEKERJA = noPekerja,
                        PA_TARIKH_PROCESS = DateTime.Now,
                        PA_JUMLAH_ELAUN = jumlahElaun,
                        PA_JUMLAH_PEMOTONGAN = jumlahPemotongan,
                        PA_JUMLAH_PCB = null,
                        PA_GAJI_POKOK = gajiPokok,
                        PA_GAJI_BERSIH = gajiBersih,
                        PA_NO_SLIP_GAJI = null,
                        PA_TAHUN_GAJI = (short)tahunDibayar,
                        PA_BULAN_GAJI = (byte)bulanDibayar,
                        PA_JUMLAH_CARUMAN = jumlahCaruman,
                        PA_JUMLAH_PELARASAN = null,
                        PA_PROSES_IND = "P"
                    };
                    spgDb.PA_TRANSAKSI_GAJI.Add(spgTrans);
                    spgDb.SaveChanges();
                }
                else
                {
                    //spgTrans.PA_NO_PEKERJA = noPekerja;
                    spgTrans.PA_TARIKH_PROCESS = DateTime.Now;
                    spgTrans.PA_JUMLAH_ELAUN = jumlahElaun;
                    spgTrans.PA_JUMLAH_PEMOTONGAN = jumlahPemotongan;
                    spgTrans.PA_JUMLAH_PCB = null;
                    spgTrans.PA_GAJI_POKOK = gajiPokok;
                    spgTrans.PA_GAJI_BERSIH = gajiBersih;
                    spgTrans.PA_NO_SLIP_GAJI = null;
                    //spgTrans.PA_TAHUN_GAJI = (short)tahunDibayar;
                    //spgTrans.PA_BULAN_GAJI = (byte)bulanDibayar;
                    spgTrans.PA_JUMLAH_CARUMAN = jumlahCaruman;
                    spgTrans.PA_JUMLAH_PELARASAN = null;
                    spgTrans.PA_PROSES_IND = "P";
                    try
                    {
                        spgDb.Entry(spgTrans).State = EntityState.Modified;
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