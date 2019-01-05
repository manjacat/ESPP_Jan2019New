using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
	public class BONUSSAMBILANMODELS
	{
		public BONUSSAMBILANMODELS() { }
		public virtual DbSet<HR_BONUS_SAMBILAN> HR_BONUS_SAMBILAN { get; set; }
	}
	public partial class HR_BONUS_SAMBILAN
	{
		[Key]
		[Column(Order = 0)]
		public string HR_NO_PEKERJA { get; set; }
		[Key]
		[Column(Order = 1)]
		public int? HR_BULAN_NOTIS { get; set; }
		[Key]
		[Column(Order = 2)]
		public int? HR_TAHUN_NOTIS { get; set; }
		public int? HR_BULAN_BONUS { get; set; }
		public Nullable<decimal> HR_JUMLAH { get; set; }
		public string HR_KWSP_IND { get; set; }
		public string HR_PCB_IND { get; set; }
		public string HR_SOCSO_IND { get; set; }
		public string HR_FINALISED_IND { get; set; }
		public string HR_NP_FINALISED { get; set; }
		public Nullable<System.DateTime> HR_TARIKH_FINALISED { get; set; }
		public Nullable<decimal> HR_JUMLAH_KWSP { get; set; }
		public Nullable<decimal> HR_CARUMAN_KWSP { get; set; }
		public int? HR_TAHUN_BONUS { get; set; }
		public string HR_KOD_ELAUN { get; set; }
		public string HR_BONUS_BERSIH { get; set; }
	}
}