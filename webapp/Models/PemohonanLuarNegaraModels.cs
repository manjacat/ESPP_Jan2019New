using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PemohonanLuarNegaraModels
    {
        public virtual DbSet<HR_SEMINAR_LUAR> HR_SEMINAR_LUAR { get; set; }
    }
    public class HR_SEMINAR_LUAR
    {
        [Key]
        public string HR_KOD_LAWATAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_PERMOHONAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_NAMA_SEMINAR { get; set; }
        public string HR_TUJUAN { get; set; }
        public string HR_TEMPAT { get; set; }
        public string HR_FAEDAH { get; set; }
        public string HR_LULUS_IND { get; set; }
        public string HR_PERBELANJAAN { get; set; }
        public string HR_LULUS_MENTERI_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LULUS_MENTERI { get; set; }
        public string HR_PERBELANJAAN_LAIN { get; set; }
        public string HR_SOKONG_IND { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_SOKONG { get; set; }
        public string HR_NP_SOKONG { get; set; }
        public string HR_JENIS_IND { get; set; }


        [Newtonsoft.Json.JsonIgnoreAttribute]
        public virtual ICollection<HR_SEMINAR_LUAR_DETAIL> HR_SEMINAR_LUAR_DETAIL { get; set; }
    }

}