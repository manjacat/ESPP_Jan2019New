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

        public static void InsertSPGGaji(ApplicationDbContext sppDb, SPGContext spgDb, int tahun, int bulanDibayar)
        {
            //ApplicationDbContext sppDb = new ApplicationDbContext();
            //int tahun = 2019;
            //int bulanDibayar = 4;
            //string noPekerja = "01595";

            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans = 
                HR_TRANSAKSI_SAMBILAN_DETAIL.GetTransaksi(sppDb, tahun, bulanDibayar);

            if(sppTrans != null)
            {
                InsertToSPG(sppDb, spgDb, tahun, bulanDibayar, sppTrans);
            }
        }

        private static void InsertToSPG(ApplicationDbContext sppDb, SPGContext spgDb, int tahun, int bulanDibayar,
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTrans)
        {
            List<string> noPekerja_all = sppTrans.Select(s => s.HR_NO_PEKERJA).Distinct().ToList();

            foreach (string noPekerja in noPekerja_all)
            {
                decimal jumlahHari = sppTrans
                    .Where(s => s.HR_KOD == "GAJPS"
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JAM_HARI).Sum().Value;
                //select elaunKA dan elaunLain - elaunOT
                decimal jumlahElaunSehari = sppTrans
                    .Where(s => s.HR_KOD_IND == "E"
                    && s.HR_KOD != "E0164"
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;
                decimal totalElaunOT = sppTrans
                    .Where(s => s.HR_KOD_IND == "E"
                    && s.HR_KOD == "E0164"
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;

                decimal gajiPokok = sppTrans
                    .Where(s => s.HR_KOD == "GAJPS" 
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;
                decimal jumlahElaun = sppTrans
                    .Where(s => s.HR_KOD_IND == "E"
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;
                decimal jumlahPotongan = sppTrans
                    .Where(s => s.HR_KOD_IND == "P" 
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;
                decimal potonganSocso = PageSejarahModel
                   .GetPotonganSocso(sppDb, gajiPokok, totalElaunOT);
                //tambah potongan Socso dalam calculation sbb potongan socso takda dalam DB
                jumlahPotongan = jumlahPotongan + potonganSocso;

                decimal jumlahCaruman = sppTrans
                    .Where(s => s.HR_KOD == "C0020" 
                    && s.HR_NO_PEKERJA == noPekerja)
                    .Select(s => s.HR_JUMLAH).Sum().Value;
               
                decimal totalElaun = jumlahElaunSehari * jumlahHari;
                decimal gajiKasar = gajiPokok + totalElaun + totalElaunOT;
                decimal gajiBersih = gajiKasar - jumlahPotongan;

                PA_TRANSAKSI_GAJI spgTrans = spgDb.PA_TRANSAKSI_GAJI
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_GAJI == tahun
                    && s.PA_BULAN_GAJI == bulanDibayar).FirstOrDefault();
                if (spgTrans == null)
                {
                    spgTrans = new PA_TRANSAKSI_GAJI
                    {
                        PA_NO_PEKERJA = noPekerja,
                        PA_TARIKH_PROCESS = DateTime.Now,
                        PA_JUMLAH_ELAUN = jumlahElaun,
                        PA_JUMLAH_PEMOTONGAN = jumlahPotongan,
                        PA_JUMLAH_PCB = null,
                        PA_GAJI_POKOK = gajiPokok,
                        PA_GAJI_BERSIH = gajiBersih,
                        PA_NO_SLIP_GAJI = null,
                        PA_TAHUN_GAJI = (short)tahun,
                        PA_BULAN_GAJI = bulanDibayar,
                        PA_JUMLAH_CARUMAN = jumlahCaruman,
                        PA_JUMLAH_PELARASAN = null,
                        PA_PROSES_IND = "P"
                    };
                    spgDb.PA_TRANSAKSI_GAJI.Add(spgTrans);
                    try
                    {
                        spgDb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.ToString());
                    }
                }
                else
                {
                    spgTrans.PA_NO_PEKERJA = noPekerja;
                    spgTrans.PA_TARIKH_PROCESS = DateTime.Now;
                    spgTrans.PA_JUMLAH_ELAUN = jumlahElaun;
                    spgTrans.PA_JUMLAH_PEMOTONGAN = jumlahPotongan;
                    spgTrans.PA_JUMLAH_PCB = null;
                    spgTrans.PA_GAJI_POKOK = gajiPokok;
                    spgTrans.PA_GAJI_BERSIH = gajiBersih;
                    spgTrans.PA_NO_SLIP_GAJI = null;
                    spgTrans.PA_TAHUN_GAJI = (short)tahun;
                    spgTrans.PA_BULAN_GAJI = bulanDibayar;
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