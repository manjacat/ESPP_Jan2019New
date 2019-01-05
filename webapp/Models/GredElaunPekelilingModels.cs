using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class GredElaunPekelilingModels
    {
        public virtual DbSet<HR_GRED_ELAUN_PEKELILING> HR_GRED_ELAUN_PEKELILING { get; set; }
    }
    public class HR_GRED_ELAUN_PEKELILING
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_GREDELAUN { get; set; }
        public string HR_GRED { get; set; }
        public Nullable<int> HR_NILAI { get; set; }
        public string HR_KATEGORI { get; set; }
        public Nullable<int> HR_LOJING { get; set; }
        public string HR_JENIS { get; set; }
    }

}
