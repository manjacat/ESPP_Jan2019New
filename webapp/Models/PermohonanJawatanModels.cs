using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PermohonanJawatanModels
    {
    }

    public class HR_PERMOHONAN_JAWATAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_KPBARU { get; set; }
        [Key]
        [Column(Order = 3)]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        [Key]
        [Column(Order = 2)]
        public string HR_KOD_JAWATAN { get; set; }
        public string HR_TARAF_JAWATAN { get; set; }
        public string HR_STATUS_TEMUDUGA { get; set; }
        public string HR_PEKERJA_IND { get; set; }
        public string HR_NO_PEKERJA { get; set; }


        public virtual ICollection<HR_JADUAL_TEMUDUGA> HR_JADUAL_TEMUDUGA { get; set; }
        [ForeignKey("HR_NO_KPBARU")]
        public virtual HR_MAKLUMAT_PEMOHON HR_MAKLUMAT_PEMOHON { get; set; }
    }
}