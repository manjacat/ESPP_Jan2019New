using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatPengalamanKerjaModels
    {
        public virtual DbSet<HR_MAKLUMAT_PENGALAMAN_KERJA> HR_MAKLUMAT_PENGALAMAN_KERJA { get; set; }
    }

    public class HR_MAKLUMAT_PENGALAMAN_KERJA
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_NAMA_SYARIKAT { get; set; }
        public string HR_JAWATAN { get; set; }
        [Key]
        [Column(Order = 1)]
        public System.DateTime HR_TARIKH_MULA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_TAMAT { get; set; }
        public string HR_ALASAN_BERHENTI { get; set; }
    }
}