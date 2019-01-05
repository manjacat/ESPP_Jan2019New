using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KlasifikasiPerkhidmatanModels
    {
        public virtual DbSet<HR_KLAS_PERKHIDMATAN> HR_KLAS_PERKHIDMATAN { get; set; }
    }
        public class HR_KLAS_PERKHIDMATAN
        {
        [Key]
        public string HR_GRED { get; set; }
        public string HR_PENERANGAN { get; set; }

    }
}