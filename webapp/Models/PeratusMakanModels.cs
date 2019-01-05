using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PeratusMakanModels
    {
        public virtual DbSet<HR_PERATUS_MAKAN> HR_PERATUS_MAKAN { get; set; }
        
    }
    public class HR_PERATUS_MAKAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_PERATUS { get; set; }
        public string HR_KETERANGAN { get; set; }
        public Nullable<decimal> HR_NILAI { get; set; }
    }
}