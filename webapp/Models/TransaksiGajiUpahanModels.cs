using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiGajiUpahanModels
    {
        DbSet<PA_TRANSAKSI_GAJI_UPAHAN> PA_TRANSAKSI_GAJI_UPAHAN { get; set; }
    }
    public partial class PA_TRANSAKSI_GAJI_UPAHAN
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PA_KOD_GAJI_UPAHAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PA_BULAN_GAJI_UPAHAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public int PA_TAHUN_GAJI_UPAHAN { get; set; }
        public decimal? PA_JUMLAH_GAJI_UPAHAN { get; set; }
        public string PA_PROSES_IND { get; set; }
        public DateTime? PA_TARIKH_PROSES { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PA_VOT_GAJI_UPAHAN { get; set; }
        public DateTime? PA_TARIKH_KEYIN { get; set; }

        public static void InsertSPGGajiUpahan(ApplicationDbContext sppDb, SPGContext spgDb,
            int tahunDibayar, int bulanDibayar)
        {
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

            foreach (string noPekerja in noPekerja_all)
            {
                //get List of transaksi by No Pekerja
                List<HR_TRANSAKSI_SAMBILAN_DETAIL> sppTransData =
                    sppTrans.Where(s => s.HR_NO_PEKERJA == noPekerja).ToList();
                var listGaji = sppTransData.Where(s => s.HR_KOD_IND == "G").ToList();

                foreach (string kodGaji in listGaji.Select(s => s.HR_KOD).ToList())
                {
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> sKod =
                        sppTransData.Where(s => s.HR_KOD == kodGaji).ToList();
                    PA_TRANSAKSI_GAJI_UPAHAN spgTrans = spgDb.PA_TRANSAKSI_GAJI_UPAHAN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_GAJI_UPAHAN == tahunDibayar
                    && s.PA_BULAN_GAJI_UPAHAN == bulanDibayar
                    && s.PA_KOD_GAJI_UPAHAN == kodGaji).FirstOrDefault();
                    var jumlahGaji = sKod.Select(s => s.HR_JUMLAH).Sum();
                    var votGajiUpahan = GetKodVOT(noPekerja, kodGaji);

                    if (spgTrans != null)
                    {
                        //update data
                        //spgTrans.PA_NO_PEKERJA = noPekerja;
                        //spgTrans.PA_KOD_PEMOTONGAN = kodElaun;
                        spgTrans.PA_JUMLAH_GAJI_UPAHAN = jumlahGaji;
                        //spgTrans.PA_BULAN_POTONGAN = (byte)bulanDibayar;
                        //spgTrans.PA_TAHUN_POTONGAN = (short)tahunDibayar;
                        spgTrans.PA_PROSES_IND = "P";
                        spgTrans.PA_TARIKH_PROSES = DateTime.Now;
                        //spgTrans.PA_VOT_PEMOTONGAN = "VOT";
                        spgTrans.PA_TARIKH_KEYIN = DateTime.Now;
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
                    else
                    {
                        //insert data
                        spgTrans = new PA_TRANSAKSI_GAJI_UPAHAN
                        {
                            PA_NO_PEKERJA = noPekerja,
                            PA_KOD_GAJI_UPAHAN = kodGaji,
                            PA_JUMLAH_GAJI_UPAHAN = jumlahGaji,
                            PA_BULAN_GAJI_UPAHAN = (byte)bulanDibayar,
                            PA_TAHUN_GAJI_UPAHAN = (short)tahunDibayar,
                            PA_PROSES_IND = "P",
                            PA_TARIKH_PROSES = DateTime.Now,
                            PA_VOT_GAJI_UPAHAN = votGajiUpahan,
                            PA_TARIKH_KEYIN = DateTime.Now
                        };
                        spgDb.PA_TRANSAKSI_GAJI_UPAHAN.Add(spgTrans);
                        spgDb.SaveChanges();
                    }
                }
            }
        }

        private static string GetKodVOT(string noPekerja, string kodGaji)
        {
            string retString = "11-00-00-00-00000";
            ApplicationDbContext db = new ApplicationDbContext();
            HR_MAKLUMAT_PEKERJAAN mWork = db.HR_MAKLUMAT_PEKERJAAN.Where
                (s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            HR_GAJI_UPAHAN mGaji = db.HR_GAJI_UPAHAN.Where
                (s => s.HR_KOD_UPAH == kodGaji).FirstOrDefault();
            if (mWork != null && mGaji != null)
            {
                string cropString = string.Empty;
                if (mGaji.HR_VOT_UPAH.Length > 5)
                {
                    var indexChar = mGaji.HR_VOT_UPAH.Length - 5;
                    cropString = mGaji.HR_VOT_UPAH.Substring(indexChar, 5);
                }
                else
                {
                    cropString = mGaji.HR_VOT_UPAH;
                }

                if (cropString.Length == 0)
                {
                    cropString = "00000";
                }

                retString = string.Format("{0}-{1}-{2}-{3}-{4}",
                    PageSejarahModel.NoVOTKepala,
                    mWork.HR_JABATAN,
                    mWork.HR_BAHAGIAN,
                    mWork.HR_UNIT,
                    cropString);
            }
            return retString;
        }

    }
}