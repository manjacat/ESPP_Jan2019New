using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PeratusKWSPModels
    {
        public virtual DbSet<HR_PERATUS_KWSP> HR_PERATUS_KWSP { get; set; }
    }

    public class HR_PERATUS_KWSP
    {
        [Key]
        public string HR_KOD_PERATUS { get; set; }
        public Nullable<decimal> HR_NILAI_PERATUS { get; set; }
    }
}