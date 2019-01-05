using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class GelaranJawatanModels
    {
        public virtual DbSet<HR_GELARAN_JAWATAN> HR_GELARAN_JAWATAN { get; set; }
    }
    public class HR_GELARAN_JAWATAN
    {
        [Key]
        public string HR_KOD_GELARAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_KOD_JAWATAN { get; set; }
        public string HR_GRED { get; set; }

       
    }
}