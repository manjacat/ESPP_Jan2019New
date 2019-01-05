using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class KategoriLatihan
    {
        [Key]
        public string HR_KOD_KATEGORI { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
    }
}