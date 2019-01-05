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
    }
}