using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
		public string PA_NO_PEKERJA { get; set; }
		public string PA_KOD_CARUMAN { get; set; }
		public System.DateTime PA_TARIKH_PROSES { get; set; }
		public Nullable<decimal> PA_JUMLAH_CARUMAN { get; set; }
		public Nullable<byte> PA_BULAN_CARUMAN { get; set; }
		public Nullable<short> PA_TAHUN_CARUMAN { get; set; }
		public string PA_PROSES_IND { get; set; }
		public string PA_VOT_CARUMAN { get; set; }
		public Nullable<System.DateTime> PA_TARIKH_KEYIN { get; set; }
	}
}