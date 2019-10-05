using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiPemotonganModels
    {
        public virtual DbSet<PA_TRANSAKSI_PEMOTONGAN> PA_TRANSAKSI_PEMOTONGAN { get; set; }
    }
    public partial class PA_TRANSAKSI_PEMOTONGAN
	{
        [Key]
        [Column(Order = 0)]
		public string PA_NO_PEKERJA { get; set; }
		public string PA_KOD_PEMOTONGAN { get; set; }
		public System.DateTime PA_TARIKH_PROSES { get; set; }
		public Nullable<decimal> PA_JUMLAH_PEMOTONGAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public byte PA_BULAN_POTONGAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public short PA_TAHUN_POTONGAN { get; set; }
		public string PA_PROSES_IND { get; set; }
		public string PA_VOT_PEMOTONGAN { get; set; }
		public Nullable<System.DateTime> PA_TARIKH_KEYIN { get; set; }

        //TODO InsertData
        public static void TestInsert(ApplicationDbContext sppDb, SPGContext spgDb)
        {

        }
	}
}