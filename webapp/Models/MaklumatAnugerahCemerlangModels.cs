using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatAnugerahCemerlangModels
    {
        public virtual DbSet<HR_ANUGERAH_CEMERLANG> HR_ANUGERAH_CEMERLANG { get; set; }
    }

    public class HR_ANUGERAH_CEMERLANG
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_NAMA_ANUGERAH { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PENERIMAAN { get; set; }
    }
}