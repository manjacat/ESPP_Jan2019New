using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiSambilanDetailModels
    {
        public TransaksiSambilanDetailModels() { }
        public virtual DbSet<HR_TRANSAKSI_SAMBILAN_DETAIL> HR_TRANSAKSI_SAMBILAN_DETAIL { get; set; }
    }
    public partial class HR_TRANSAKSI_SAMBILAN_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public int HR_BULAN_DIBAYAR { get; set; }
        [Key]
        [Column(Order = 2)]
        public int HR_TAHUN { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HR_KOD { get; set; }
        [Key]
        [Column(Order = 4)]
        public int HR_BULAN_BEKERJA { get; set; }
        public Nullable<decimal> HR_JUMLAH { get; set; }
        public string HR_KOD_IND { get; set; }
        public string HR_TUNGGAKAN_IND { get; set; }
        public decimal? HR_JAM_HARI { get; set; }
        public int HR_TAHUN_BEKERJA { get; set; }
        public string HR_YDP_LULUS_IND { get; set; }
        public string HR_POTONGAN_IND { get; set; }
        public int? HR_MUKTAMAD { get; set; }

        public static List<HR_TRANSAKSI_SAMBILAN_DETAIL> GetTransaksiBekerja
        (ApplicationDbContext db, int tahunBekerja, int bulanBekerja)
        {
            if (tahunBekerja == 0 || bulanBekerja == 0)
            {
                return null;
            }
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> allTransaksi 
                = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();
            try
            {
                allTransaksi = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_TAHUN_BEKERJA == tahunBekerja
                    && s.HR_BULAN_BEKERJA == bulanBekerja).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return allTransaksi;
        }

        public static List<HR_TRANSAKSI_SAMBILAN_DETAIL> GetTransaksiDibayar
        (ApplicationDbContext db, int tahunDibayar, int bulanDibayar)
        {
            if (tahunDibayar == 0 || bulanDibayar == 0)
            {
                return null;
            }
            List<HR_TRANSAKSI_SAMBILAN_DETAIL> allTransaksi
                = new List<HR_TRANSAKSI_SAMBILAN_DETAIL>();
            try
            {
                allTransaksi = db.HR_TRANSAKSI_SAMBILAN_DETAIL
                    .Where(s => s.HR_TAHUN == tahunDibayar
                    && s.HR_BULAN_DIBAYAR == bulanDibayar).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return allTransaksi;
        }
    }
}