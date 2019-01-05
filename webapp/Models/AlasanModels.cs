using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class AlasanModels
    {
        public virtual DbSet<HR_ALASAN> HR_ALASAN { get; set; }
    }

    public class HR_ALASAN
    {
        [Key]
        public string HR_KOD_ALASAN { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SISTEM_IND { get; set; }
    }
}