using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SenaraiTemudugaModels
    {
        public virtual DbSet<HR_SENARAI_TEMUDUGA> HR_SENARAI_TEMUDUGA { get; set; }
    }

    public class HR_SENARAI_TEMUDUGA 
    {
        [Key]
        [Column(Order = 0)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime HR_TARIKH_TEMUDUGA { get; set; }
        public string HR_MASA_MULA { get; set; }
        public string HR_MASA_AKHIR { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_JAWATAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_TARAF_JAWATAN { get; set; }
        public string HR_GRED_GAJI { get; set; }
        public string HR_TEMPAT { get; set; }

        public virtual ICollection<HR_MAKLUMAT_CALON_TEMUDUGA> HR_MAKLUMAT_CALON_TEMUDUGA { get; set; }
        public virtual ICollection<HR_MAKLUMAT_PENEMUDUGA> HR_MAKLUMAT_PENEMUDUGA { get; set; }
    }
}