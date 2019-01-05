using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatTanggunganModels
    { 
        public virtual DbSet<HR_MAKLUMAT_TANGGUNGAN> HR_MAKLUMAT_TANGGUNGAN { get; set; }
    }
    public class HR_MAKLUMAT_TANGGUNGAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_TANGGUNGAN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_NO_KP { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_SEK_IPT { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_JANTINA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
    }
}