using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class AcaraModels
    {
        public virtual DbSet<HR_ACARA> HR_ACARA { get; set; }
    }

    public class HR_ACARA
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_ACARA { get; set; }
        public string HR_TAJUK { get; set; }
        public string HR_WARNA { get; set; }
        public string HR_ICON { get; set; }
        public string HR_AKTIF_IND { get; set; }
      
    }
}