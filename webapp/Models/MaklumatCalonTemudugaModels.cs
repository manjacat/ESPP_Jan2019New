using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatCalonTemudugaModels
    {
        public virtual DbSet<HR_MAKLUMAT_CALON_TEMUDUGA> HR_MAKLUMAT_CALON_TEMUDUGA { get; set; }
    }

    public class HR_MAKLUMAT_CALON_TEMUDUGA
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
        public string HR_NO_KPBARU { get; set; }
        public string HR_NAMA_CALON { get; set; }
        public string HR_PEKERJA_IND { get; set; }
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<int> HR_KEMAHIRAN_KOMUNIKASI { get; set; }
        public Nullable<int> HR_PENGETAHUAN_AM { get; set; }
        public Nullable<int> HR_SIFAT_SAHSIAH { get; set; }
        public Nullable<int> HR_MARKAH_PENUH { get; set; }
        public string HR_STATUS_TEMUDUGA { get; set; }

        [ForeignKey("HR_TARIKH_TEMUDUGA,HR_KOD_JAWATAN,HR_TARAF_JAWATAN")]
        public virtual HR_SENARAI_TEMUDUGA HR_SENARAI_TEMUDUGA { get; set; }
    }
}