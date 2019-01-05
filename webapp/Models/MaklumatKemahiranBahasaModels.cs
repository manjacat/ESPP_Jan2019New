using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKemahiranBahasaModels
    {
        public virtual DbSet<HR_MAKLUMAT_KEMAHIRAN_BAHASA> HR_MAKLUMAT_KEMAHIRAN_BAHASA { get; set; }
    }
    public class HR_MAKLUMAT_KEMAHIRAN_BAHASA
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_BAHASA { get; set; }
        public string HR_PEMBACAAN { get; set; }
        public string HR_PENULISAN { get; set; }
        public string HR_PERTUTURAN { get; set; }
    }
}