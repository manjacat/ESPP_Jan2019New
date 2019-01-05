using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class Kurniaan
    {
        public virtual DbSet<HR_KURNIAAN> HR_KURNIAAN { get; set; }
    }
    public class HR_KURNIAAN
    {
        [Key]
        public string HR_KOD_KURNIAAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_DARJAH_IND { get; set; }
    }
}