using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class CutiUmum
    {
        public virtual DbSet<HR_CUTI_UMUM> HR_CUTI_UMUM { get; set; }
    }
    public class HR_CUTI_UMUM
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_CUTI_UMUM { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH { get; set; }
        
        public string HR_CATATAN { get; set; }
    }
}