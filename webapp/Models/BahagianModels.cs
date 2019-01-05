using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class BahagianModels
    {
        public virtual DbSet<GE_BAHAGIAN> GE_BAHAGIAN { get; set; }
    }
    public  class GE_BAHAGIAN
    {
        [Key]
        [Column(Order = 0)]
        public string GE_KOD_BAHAGIAN { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("GE_JABATAN")]
        public string GE_KOD_JABATAN { get; set; }
        public string GE_KETERANGAN { get; set; }
        public string GE_ALAMAT1 { get; set; }
        public string GE_ALAMAT2 { get; set; }
        public string GE_ALAMAT3 { get; set; }
        public string GE_BANDAR { get; set; }
        public string GE_POSKOD { get; set; }
        public string GE_NEGERI { get; set; }
        public string GE_TELPEJABAT1 { get; set; }
        public string GE_TELPEJABAT2 { get; set; }
        public string GE_FAKS1 { get; set; }
        public string GE_FAKS2 { get; set; }
        public string GE_EMAIL { get; set; }
        public string GE_NO_KETUA { get; set; }
        public string GE_SINGKATAN { get; set; }
        public string GE_AKTIF_IND { get; set; }

        public virtual GE_JABATAN GE_JABATAN { get; set; }

        public virtual ICollection<GE_UNIT> GE_UNIT { get; set; }
    }
}