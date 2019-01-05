using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KategoriElaun
    {
        public virtual DbSet<HR_KATEGORI_ELAUN> HR_KATEGORI_ELAUN { get; set; }
    }
    public class HR_KATEGORI_ELAUN
    {
        [Key]
        public string HR_KOD_KATEGORI { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }

        public virtual ICollection<HR_ELAUN> HR_ELAUN { get; set; }
    }
}