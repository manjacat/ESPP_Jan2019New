using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKemahiranTeknikalModels
    {
        public virtual DbSet<HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL> HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL { get; set; }
    }
    public class HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public short HR_SEQ_NO { get; set; }
        public string HR_KEMAHIRAN { get; set; }
        public string HR_TAHAP { get; set; }
    }
}