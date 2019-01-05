using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JadualSOCSO
    {
        public virtual DbSet<HR_SOCSO> HR_SOCSO { get; set; }
    }

    public partial class HR_SOCSO
    {

        [Key]
        [Column(Order = 0)]
        public decimal HR_GAJI_DARI { get; set; }
        [Key]
        [Column(Order = 1)]
        public decimal HR_GAJI_HINGGA { get; set; }
        public decimal HR_CARUMAN_PEKERJA { get; set; }
        public decimal HR_CARUMAN_MAJIKAN { get; set; }
        public decimal HR_JUMLAH { get; set; }

    }
}