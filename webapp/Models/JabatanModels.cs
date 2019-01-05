using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JabatanModels
    {
        public virtual DbSet<GE_JABATAN> GE_JABATAN { get; set; }
    }
    public class GE_JABATAN
    {
        [Key]
        public string GE_KOD_JABATAN { get; set; }
        public string GE_KETERANGAN_JABATAN { get; set; }
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

        public virtual ICollection<GE_BAHAGIAN> GE_BAHAGIAN { get; set; }
    }
}