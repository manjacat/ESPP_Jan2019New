using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KWSPModels
    {
        public virtual DbSet<HR_KWSP> HR_KWSP { get; set; }
    }
    public partial class HR_KWSP
    {
        [Key]
        public decimal HR_UPAH_DARI { get; set; }
        public decimal HR_UPAH_HINGGA { get; set; }
        public decimal HR_CARUMAN_MAJIKAN { get; set; }
        public decimal HR_CARUMAN_PEKERJA { get; set; }
        public Nullable<decimal> HR_TOTAL_CARUMAN { get; set; }
    }
}