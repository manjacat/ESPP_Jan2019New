using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatCutiModels
    {
        public virtual DbSet<HR_MAKLUMAT_CUTI> HR_MAKLUMAT_CUTI { get; set; }
    }
    public class HR_MAKLUMAT_CUTI
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_CUTI { get; set; }
        [Key]
        [Column(Order = 2)]
        public System.DateTime HR_TARIKH_PERMOHONAN { get; set; }
        public Nullable<short> HR_BAKI_CUTI_REHAT { get; set; }
        public Nullable<short> HR_JUMLAH_MAKSIMUM { get; set; }
        public Nullable<short> HR_BAKI_TAHUN_LEPAS { get; set; }
        public Nullable<int> HR_BAKI_PENCEN { get; set; }
        public Nullable<short> HR_TAHUN { get; set; }
        public Nullable<short> HR_BIL_CUTI_TEMP { get; set; }
        public Nullable<int> HR_BAKI_PENCEN_TERKUMPUL { get; set; }
        public Nullable<int> HR_KELAYAKAN_BULANAN { get; set; }
        public Nullable<int> HR_BIL_CUTI_DIAMBIL { get; set; }

        public virtual ICollection<HR_MAKLUMAT_CUTI_DETAIL> HR_MAKLUMAT_CUTI_DETAIL { get; set; }
    }
}