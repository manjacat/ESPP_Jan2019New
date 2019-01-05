using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKelayakanModels
    {
        public virtual DbSet<HR_MAKLUMAT_KELAYAKAN> HR_MAKLUMAT_KELAYAKAN { get; set; }
    }
    public class HR_MAKLUMAT_KELAYAKAN
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public short HR_SEQ_NO { get; set; }
        public string HR_KEPUTUSAN { get; set; }
        public string HR_PANGKAT { get; set; }
        public string HR_TAHUN_MULA { get; set; }
        public string HR_TAHUN_TAMAT { get; set; }
        public string HR_SEKOLAH_INSTITUSI { get; set; }
    }
}