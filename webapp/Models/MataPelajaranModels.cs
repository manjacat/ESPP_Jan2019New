using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MataPelajaranModels
    {
        public virtual DbSet<HR_MATAPELAJARAN> HR_MATAPELAJARAN { get; set; }
    }
    public partial class HR_MATAPELAJARAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_MATAPELAJARAN { get; set; }
        public string HR_KETERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_PEPERIKSAAN { get; set; }
    }
}