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
	}
}