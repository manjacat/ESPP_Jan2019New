using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPenemudugaModels
    {
        public virtual DbSet<HR_MAKLUMAT_PENEMUDUGA> HR_MAKLUMAT_PENEMUDUGA { get; set; }
    }

    public class HR_MAKLUMAT_PENEMUDUGA
    {
        [Key]
        [Column(Order = 0)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime HR_TARIKH_TEMUDUGA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_JAWATAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_TARAF_JAWATAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public string HR_PENEMUDUGA { get; set; }
        public string HR_CATATAN { get; set; }

        [ForeignKey("HR_TARIKH_TEMUDUGA,HR_KOD_JAWATAN,HR_TARAF_JAWATAN")]
        public virtual HR_SENARAI_TEMUDUGA HR_SENARAI_TEMUDUGA { get; set; }
    }
}