using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TransaksiModels
    {
		public HR_MAKLUMAT_PERIBADI peribadi { get; set; }
		public HR_MAKLUMAT_PEKERJAAN pekerjaan { get; set; }
		public IEnumerable<HR_TRANSAKSI_SAMBILAN_DETAIL> detail { get; set; }

		public string BONUS { get; set; }
		public string PURATA { get; set; }
		public string TOTAL { get; set; }
	}
}