using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiCarumanModels
    {
        public virtual DbSet<PA_TRANSAKSI_CARUMAN> PA_TRANSAKSI_CARUMAN { get; set; }
    }
    public partial class PA_TRANSAKSI_CARUMAN
	{
        [Key]
        [Column(Order = 0)]
        public string PA_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PA_KOD_CARUMAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public System.DateTime PA_TARIKH_PROSES { get; set; }
		public Nullable<decimal> PA_JUMLAH_CARUMAN { get; set; }
		public Nullable<byte> PA_BULAN_CARUMAN { get; set; }
		public Nullable<short> PA_TAHUN_CARUMAN { get; set; }
		public string PA_PROSES_IND { get; set; }
		public string PA_VOT_CARUMAN { get; set; }
		public Nullable<System.DateTime> PA_TARIKH_KEYIN { get; set; }

        //TODO InsertData
        public static void InsertSpgCaruman(ApplicationDbContext sppDb, SPGContext spgDb,
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
                var listCaruman = sppTransData.Where(s => s.HR_KOD_IND == "C").ToList();

                foreach (string kodCaruman in listCaruman.Select(s => s.HR_KOD).ToList())
                {
                    List<HR_TRANSAKSI_SAMBILAN_DETAIL> sKod =
                        sppTransData.Where(s => s.HR_KOD == kodCaruman).ToList();
                    PA_TRANSAKSI_CARUMAN spgTrans = spgDb.PA_TRANSAKSI_CARUMAN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_CARUMAN == tahunDibayar
                    && s.PA_BULAN_CARUMAN == bulanDibayar
                    && s.PA_KOD_CARUMAN == kodCaruman).FirstOrDefault();
                    var jumlahCaruman = sKod.Select(s => s.HR_JUMLAH).Sum();
                    var votCaruman = GetKodVOT(noPekerja, kodCaruman);

                    if (spgTrans != null)
                    {
                        //update data
                        //spgTrans.PA_NO_PEKERJA = noPekerja;
                        //spgTrans.PA_KOD_PEMOTONGAN = kodElaun;
                        spgTrans.PA_JUMLAH_CARUMAN = jumlahCaruman;
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
                        spgTrans = new PA_TRANSAKSI_CARUMAN
                        {
                            PA_NO_PEKERJA = noPekerja,
                            PA_KOD_CARUMAN = kodCaruman,
                            PA_JUMLAH_CARUMAN = jumlahCaruman,
                            PA_BULAN_CARUMAN = (byte)bulanDibayar,
                            PA_TAHUN_CARUMAN = (short)tahunDibayar,
                            PA_PROSES_IND = "P",
                            PA_TARIKH_PROSES = DateTime.Now,
                            PA_VOT_CARUMAN = votCaruman,
                            PA_TARIKH_KEYIN = DateTime.Now
                        };
                        spgDb.PA_TRANSAKSI_CARUMAN.Add(spgTrans);
                        spgDb.SaveChanges();
                    }
                }
            }
        }

        private static string GetKodVOT(string noPekerja, string kodCaruman)
        {
            string retString = "11-00-00-00-00000";
            ApplicationDbContext db = new ApplicationDbContext();
            HR_MAKLUMAT_PEKERJAAN mWork = db.HR_MAKLUMAT_PEKERJAAN.Where
                (s => s.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            HR_CARUMAN mCaruman = db.HR_CARUMAN.Where
                (s => s.HR_KOD_CARUMAN == kodCaruman).FirstOrDefault();
            if (mWork != null && mCaruman != null)
            {
                string cropString = string.Empty;
                if (mCaruman.HR_VOT_CARUMAN.Length > 5)
                {
                    var indexChar = mCaruman.HR_VOT_CARUMAN.Length - 5;
                    cropString = mCaruman.HR_VOT_CARUMAN.Substring(indexChar, 5);
                }
                else
                {
                    cropString = mCaruman.HR_VOT_CARUMAN;
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