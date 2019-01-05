using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PencalonanKurniaanModels
    {
        public virtual DbSet<HR_PENCALONAN_KURNIAAN> HR_PENCALONAN_KURNIAAN { get; set; }
    }
    public class HR_PENCALONAN_KURNIAAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_PENCALONAN { get; set; }
        public string HR_KOD_KURNIAAN { get; set; }
        public string HR_KURNIAAN_IND { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_NP_PENCALON { get; set; }
        public string HR_STATUS { get; set; }
    }
}