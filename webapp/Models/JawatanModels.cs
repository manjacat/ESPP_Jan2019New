using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JawatanModels
    {
        public virtual DbSet<HR_JAWATAN> HR_JAWATAN { get; set; }
    }
    public class HR_JAWATAN
    {
        [Key]
        public string HR_KOD_JAWATAN { get; set; }
        public string HR_NAMA_JAWATAN { get; set; }
        public string HR_AKTIF_IND { get; set; }

       
    }
}