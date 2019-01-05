using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JenisPeperiksaanModels
    {
        public virtual DbSet<HR_JENIS_PEPERIKSAAN> HR_JENIS_PEPERIKSAAN { get; set; }
    }
    public class HR_JENIS_PEPERIKSAAN
    {
        [Key]
        public string HR_KOD_PEPERIKSAAN { get; set; }
        public string HR_KETERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }

        public virtual ICollection<HR_MATAPELAJARAN> HR_MATAPELAJARAN { get; set; }

    }
}