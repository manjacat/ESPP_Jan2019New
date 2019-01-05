using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPewarisModels
    {
        public virtual DbSet<HR_MAKLUMAT_PEWARIS> HR_MAKLUMAT_PEWARIS { get; set; }
    }
    public class HR_MAKLUMAT_PEWARIS
    {
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_PEWARIS { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_LAHIR { get; set; }
        public string HR_TEMPAT_LAHIR { get; set; }
        public string HR_JANTINA { get; set; }
        public string HR_PALAMAT1 { get; set; }
        public string HR_PALAMAT2 { get; set; }
        public string HR_PALAMAT3 { get; set; }
        public string HR_PBANDAR { get; set; }
        public string HR_PPOSKOD { get; set; }
        public string HR_PNEGERI { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_TELRUMAH { get; set; }
        public string HR_TELPEJABAT { get; set; }
        public string HR_TELBIMBIT { get; set; }
        [Key]
        public string HR_NO_KP { get; set; }
        public string HR_NO_KP_LAMA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEYIN { get; set; }
        public string HR_NP_KEYIN { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_UBAH { get; set; }
        public string HR_NP_UBAH { get; set; }
        public string HR_PEWARIS_IND { get; set; }
    }
}