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
        public static void TestInsert(ApplicationDbContext sppDb, SPGContext spgDb)
        {
            int tahun = 2019;
            int bulan = 4;

            List<string> listPekerja = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
                .Where(s => s.HR_TAHUN == tahun
                && s.HR_BULAN_DIBAYAR == bulan)
                .Select(s => s.HR_NO_PEKERJA).Distinct().ToList();

            foreach(string noPekerja in listPekerja)
            {
                PA_TRANSAKSI_CARUMAN spgCaruman = spgDb.PA_TRANSAKSI_CARUMAN
                    .Where(s => s.PA_NO_PEKERJA == noPekerja
                    && s.PA_TAHUN_CARUMAN == tahun
                    && s.PA_BULAN_CARUMAN == bulan
                    && s.PA_KOD_CARUMAN == "C0020").FirstOrDefault();

                HR_TRANSAKSI_SAMBILAN_DETAIL sppCaruman = sppDb.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_NO_PEKERJA == noPekerja
                    && s.HR_TAHUN == tahun
                    && s.HR_BULAN_DIBAYAR == bulan
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
                        PA_BULAN_CARUMAN = (byte)bulan,
                        PA_PROSES_IND = "P",
                        PA_TAHUN_CARUMAN = (short)tahun,
                        PA_TARIKH_KEYIN = DateTime.Now,
                        PA_VOT_CARUMAN = "11-03-01-00-13102"
                    };
                    try
                    {
                        spgDb.PA_TRANSAKSI_CARUMAN.Add(spgCaruman);
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
                    //spgCaruman.PA_NO_PEKERJA = noPekerja;
                    spgCaruman.PA_KOD_CARUMAN = "C0020";
                    //spgCaruman.PA_TARIKH_PROSES = DateTime.Now;
                    spgCaruman.PA_JUMLAH_CARUMAN = jumlahCaruman;
                    spgCaruman.PA_BULAN_CARUMAN = (byte)bulan;
                    spgCaruman.PA_PROSES_IND = "P";
                    spgCaruman.PA_TAHUN_CARUMAN = (short)tahun;
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