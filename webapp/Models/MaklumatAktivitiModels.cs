using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatAktivitiModels
    {
        public virtual DbSet<HR_MAKLUMAT_AKTIVITI> HR_MAKLUMAT_AKTIVITI { get; set; }
    }
    public class HR_MAKLUMAT_AKTIVITI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_AKTIVITI { get; set; }
        public string HR_PERINGKAT { get; set; }
        public string HR_NAMA_AKTIVITI { get; set; }
        public string HR_ANJURAN { get; set; }
    }
}