using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SoalanMaklumBalasModels
    {
        public virtual DbSet<HR_SOALAN_MB>HR_SOALAN_MB { get; set; }
    }

    public class HR_SOALAN_MB
    {
        [Key]
        public string HR_KOD_MB { get; set; }
        public string HR_PENERANGAN_MB { get; set; }
        public string HR_MB_IND { get; set; }
    }

   
}