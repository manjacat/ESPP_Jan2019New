using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class LantikanBaruModel
    {
       
        public virtual DbSet<HR_JUSTIFIKASI_JAWATAN_BARU> HR_JUSTIFIKASI_JAWATAN_BARU { get; set; }
    }

    [Table("HR_JUSTIFIKASI_JAWATAN_BARU")]
    public class HR_JUSTIFIKASI_JAWATAN_BARU
    {
        [Key]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        public string HR_JAWATAN_DIPOHON { get; set; }
        public string HR_JAWATAN_BARU_IND { get; set; }
        public string HR_MAKSUD { get; set; }
        public string HR_KOD_JABATAN { get; set; }
        public string HR_PROGRAM { get; set; }
        public string HR_AKTIVITI { get; set; }
        public string HR_OBJ_AKTIVITI { get; set; }
        public string HR_GRED_GAJI { get; set; }
        public string HR_KLAS_PERKHIDMATAN { get; set; }
        public string HR_BIDANG { get; set; }
        public string HR_SKIM { get; set; }
        public string HR_KOD_SKIM { get; set; }
        public Nullable<int> HR_AGENSI_SEDIAADA { get; set; }
        public Nullable<int> HR_AGENSI_KOSONG { get; set; }
        public Nullable<int> HR_AGENSI_DIPOHON { get; set; }
        public Nullable<int> HR_AKTIVITI_SEDIAADA { get; set; }
        public Nullable<int> HR_AKTIVITI_KOSONG { get; set; }
        public Nullable<int> HR_AKTIVITI_DIPOHON { get; set; }
        public string HR_KEMUKA_IND { get; set; }
        public string HR_LULUS_IND { get; set; }
        public Nullable<int> HR_BIL_DIPOHON { get; set; }
        public Nullable<int> HR_BIL_DIISI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEMUKA { get; set; }
    }
}
     


