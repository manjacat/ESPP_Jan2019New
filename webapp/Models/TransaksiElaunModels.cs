using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiElaunModels
    {
        DbSet<PA_TRANSAKSI_ELAUN> PA_TRANSAKSI_ELAUN { get; set; }
    }
    public partial class PA_TRANSAKSI_ELAUN
    {
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PA_KOD_ELAUN { get; set; }
        public decimal? PA_JUMLAH_ELAUN { get; set; }
        [Key]
        [Column(Order = 2)]
        public int PA_BULAN_TUNTUTAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public int PA_TAHUN_TUNTUTAN { get; set; }
        public string PA_PROSES_IND { get; set; }
        public DateTime? PA_TARIKH_PROSES { get; set; }
        [Key]
        [Column(Order = 4)]
        public string PA_VOT_ELAUN { get; set; }
        public DateTime? PA_TARIKH_KEYIN { get; set; }
        public decimal? PA_JUMLAH_MASA { get; set; }
        public string PA_UBAH_OLEH { get; set; }
        public DateTime? PA_TARIKH_UBAH { get; set; }

        public static void InsertSPGElaun(ApplicationDbContext sppDb, SPGContext spgDb,
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
                var listElaun = sppTransData.Where(s => s.HR_KOD_IND == "E").ToList();

                var jumlahMasa = sppTransData.Where
                    (s => s.HR_KOD == "E0164").Select(s => s.HR_JAM_HARI).Sum();

                foreach (string kodElaun in listElaun.Select(s => s.HR_KOD).ToList())
                {
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> sKod =
                        sppTransData.Where(s => s.HR_KOD == kodElaun).ToList();
                    PA_TRANSAKSI_ELAUN spgTrans = spgDb.PA_TRANSAKSI_ELAUN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_TUNTUTAN == tahunDibayar
                    && s.PA_BULAN_TUNTUTAN == bulanDibayar
                    && s.PA_KOD_ELAUN == kodElaun).FirstOrDefault();
                    var jumlahElaun = sKod.Select(s => s.HR_JUMLAH).Sum();
                    var votElaun = GetKodVOT(noPekerja, kodElaun);

                    if (spgTrans != null)
                    {
                        //update data
                        //spgTrans.PA_NO_PEKERJA = noPekerja;
                        //spgTrans.PA_KOD_ELAUN = kodElaun;
                        spgTrans.PA_JUMLAH_ELAUN = jumlahElaun;
                        //spgTrans.PA_BULAN_TUNTUTAN = (short)bulanDibayar;
                        //spgTrans.PA_TAHUN_TUNTUTAN = (short)tahunDibayar;
                        spgTrans.PA_PROSES_IND = "P";
                        spgTrans.PA_TARIKH_PROSES = DateTime.Now;
                        //spgTrans.PA_VOT_ELAUN = votElaun;
                        spgTrans.PA_TARIKH_KEYIN = DateTime.Now;
                        spgTrans.PA_JUMLAH_MASA = jumlahMasa;
                        spgTrans.PA_UBAH_OLEH = null;
                        spgTrans.PA_TARIKH_UBAH = null;
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
                        spgTrans = new PA_TRANSAKSI_ELAUN
                        {
                            PA_NO_PEKERJA = noPekerja,
                            PA_KOD_ELAUN = kodElaun,
                            PA_JUMLAH_ELAUN = jumlahElaun,
                            PA_BULAN_TUNTUTAN = (short)bulanDibayar,
                            PA_TAHUN_TUNTUTAN = (short)tahunDibayar,
                            PA_PROSES_IND = "P",
                            PA_TARIKH_PROSES = DateTime.Now,
                            PA_VOT_ELAUN = votElaun,
                            PA_TARIKH_KEYIN = DateTime.Now,
                            PA_JUMLAH_MASA = jumlahMasa,
                            PA_UBAH_OLEH = null,
                            PA_TARIKH_UBAH = null
                        };
                        spgDb.PA_TRANSAKSI_ELAUN.Add(spgTrans);
                        spgDb.SaveChanges();
                    }
                }
            }
        }

        private static string GetKodVOT(string noPekerja, string kodElaun)
        {
            string retString = "11-00-00-00-0000";
            ApplicationDbContext db = new ApplicationDbContext();
            HR_MAKLUMAT_PEKERJAAN mWork = db.HR_MAKLUMAT_PEKERJAAN.Where
                (s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            HR_ELAUN mElaun = db.HR_ELAUN.Where
                (s => s.HR_KOD_ELAUN == kodElaun).FirstOrDefault();
            if (mWork != null && mElaun != null)
            {
                retString = string.Format("{0}-{1}-{2}-{3}-{4}",
                    PageSejarahModel.NoVOTKepala,
                    mWork.HR_JABATAN,
                    mWork.HR_BAHAGIAN,
                    mWork.HR_UNIT,
                    mElaun.HR_VOT_ELAUN);
            }
            return retString;
        }
    }
}