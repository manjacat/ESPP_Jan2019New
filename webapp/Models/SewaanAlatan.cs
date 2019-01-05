using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SewaanAlatan
    {
        public virtual DbSet<HR_SEWAAN_ALATAN> HR_SEWAAN_ALATAN { get; set; }
    }
    
    public partial class HR_SEWAAN_ALATAN
    {
      
        [Key]
        public string HR_KOD_ALAT { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public Nullable<decimal> HR_HARGA_SEWAAN { get; set; }

    }
}