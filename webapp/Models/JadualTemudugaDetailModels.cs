using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JadualTemudugaDetailModels
    {
        public virtual DbSet<HR_JADUAL_TEMUDUGA_DETAIL> HR_JADUAL_TEMUDUGA_DETAIL { get; set; }
    }

    public class HR_JADUAL_TEMUDUGA_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_KPBARU { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_TEMUDUGA { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_KOD_JAWATAN { get; set; }
        [Key]
        [Column(Order = 3)]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        [Key]
        [Column(Order = 4)]
        public string HR_PENEMUDUGA { get; set; }
        public string HR_CATATAN { get; set; }

        [ForeignKey("HR_NO_KPBARU,HR_TARIKH_TEMUDUGA,HR_KOD_JAWATAN,HR_TARIKH_PERMOHONAN")]
        public virtual HR_JADUAL_TEMUDUGA HR_JADUAL_TEMUDUGA { get; set; }
        public virtual ICollection<HR_MARKAH_TEMUDUGA> HR_MARKAH_TEMUDUGA { get; set; }
    }
}