using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ElaunPerjalananModels
    {
        public virtual DbSet<HR_KADAR_PERBATUAN> HR_KADAR_PERBATUAN { get; set; }
    }
    public partial class HR_KADAR_PERBATUAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KELAS { get; set; }
        [Key]
        [Column(Order = 1)]
        public decimal HR_KM_MULA { get; set; }
        public decimal HR_KM_AKHIR { get; set; }
        public decimal HR_NILAI { get; set; }
        public string HR_AKTIF_IND { get; set; }
    }
}