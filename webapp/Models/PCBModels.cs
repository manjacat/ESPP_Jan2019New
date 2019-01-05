using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PCBModels
    {
        public virtual DbSet<HR_PCB> HR_PCB { get; set; }
    }
    public class HR_PCB
    {
        [Key]
        [Column(Order = 0)]
        public string HR_PCB_KOD { get; set; }
        public string HR_KATEGORI { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
        [Key]
        [Column(Order = 1)]
        public decimal HR_GAJI_DARI { get; set; }
        public Nullable<decimal> HR_GAJI_HINGGA { get; set; }
    }
}