using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KategoriLatihanModels
    {
        public virtual DbSet<HR_KATEGORI_KURSUS> HR_KATEGORI_KURSUS { get; set; }
    }
    public partial class HR_KATEGORI_KURSUS
    {
        [Key]
        public string HR_KOD_KATEGORI { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_PENERANGAN { get; set; }
    }
}