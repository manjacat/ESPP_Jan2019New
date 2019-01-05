using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class GredElaunModels
    {
        public virtual DbSet<HR_GRED_ELAUN> HR_GRED_ELAUN { get; set; }
    }
    public partial class HR_GRED_ELAUN
    {
        [Key]
        [Column(Order = 0)]
        public short HR_GRED { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_ELAUN { get; set; }
        public string HR_AKTIF_IND { get; set; }
    }
}