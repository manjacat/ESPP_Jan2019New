using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class TindakanModels
    {
        public virtual DbSet<HR_TINDAKAN> HR_TINDAKAN { get; set; }
    }
    public class HR_TINDAKAN
    {
        [Key]
        public string HR_KOD_TINDAKAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
    }
}