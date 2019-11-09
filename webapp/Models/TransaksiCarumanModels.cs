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

                PA_TRANSAKSI_CARUMAN spgCaruman = spgDb.PA_TRANSAKSI_CARUMAN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_CARUMAN == tahunBekerja
                    && s.PA_BULAN_CARUMAN == bulanBekerja
                    && s.PA_KOD_CARUMAN == "C0020").FirstOrDefault();

                HR_TRANSAKSI_SAMBILAN_DETAIL sppCaruman = 
                    sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == noPekerja
                    && s.HR_BULAN_BEKERJA == bulanBekerja
                    && s.HR_TAHUN_BEKERJA == tahunBekerja
                    && s.HR_TAHUN == tahunDibayar
                    && s.HR_BULAN_DIBAYAR == bulanDibayar
                    && s.HR_KOD == "C0020").FirstOrDefault();
                decimal jumlahCaruman = sppCaruman.HR_JUMLAH == null 
                    ? 0 : sppCaruman.HR_JUMLAH.Value;

                if(spgCaruman == null)
                {
                    //insert
                    spgCaruman = new PA_TRANSAKSI_CARUMAN
                    {
                        PA_NO_PEKERJA = noPekerja,
                        PA_KOD_CARUMAN = "C0020",
                        PA_TARIKH_PROSES = DateTime.Now,
                        PA_JUMLAH_CARUMAN = jumlahCaruman,
                        PA_BULAN_CARUMAN = (byte)bulanBekerja,
                        PA_PROSES_IND = "P",
                        PA_TAHUN_CARUMAN = (short)tahunBekerja,
                        PA_TARIKH_KEYIN = DateTime.Now,
                        //ambik dari HR_CARUMAN.HR_VOT_CARUMAN
                        PA_VOT_CARUMAN = "11-03-01-00-29301"
                    };
                    spgDb.PA_TRANSAKSI_CARUMAN.Add(spgCaruman);
                    spgDb.SaveChanges();
                }
                else
                {
                    //update
                    //spgCaruman.PA_NO_PEKERJA = noPekerja;
                    spgCaruman.PA_KOD_CARUMAN = "C0020";
                    //spgCaruman.PA_TARIKH_PROSES = DateTime.Now;
                    spgCaruman.PA_JUMLAH_CARUMAN = jumlahCaruman;
                    spgCaruman.PA_BULAN_CARUMAN = (byte)bulanBekerja;
                    spgCaruman.PA_PROSES_IND = "P";
                    spgCaruman.PA_TAHUN_CARUMAN = (short)tahunBekerja;
                    spgCaruman.PA_TARIKH_KEYIN = DateTime.Now;
                    //spgCaruman.PA_VOT_CARUMAN = null;
                    try
                    {
                        spgDb.Entry(spgCaruman).State = EntityState.Modified;
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