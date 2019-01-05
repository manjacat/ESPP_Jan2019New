using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MarkahTemudugaModels
    {
        public virtual DbSet<HR_SUBJEK> HR_SUBJEK { get; set; }
    }
    public partial class HR_SUBJEK
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_JENIS { get; set; }
        [Key]
        [Column(Order = 1)]
        public short HR_KOD_SUBJEK { get; set; }
        [Column("HR_SUBJEK")]
        public string HR_SUBJEK1 { get; set; }
        public Nullable<int> HR_MARKAH { get; set; }
    }
}