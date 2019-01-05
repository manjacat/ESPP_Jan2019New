using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatSijilModels
    {
        public virtual DbSet<HR_MAKLUMAT_SIJIL> HR_MAKLUMAT_SIJIL { get; set; }
    }
    public partial class HR_MAKLUMAT_SIJIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_DIPEROLEHI { get; set; }
        public string HR_NAMA_SIJIL_PEPERIKSAAN { get; set; }
        public string HR_ANJURAN { get; set; }
        public string HR_KEPUTUSAN { get; set; }
    }
}