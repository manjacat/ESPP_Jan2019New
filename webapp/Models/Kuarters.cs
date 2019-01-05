using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class Kuarters
    {
        public virtual DbSet<HR_KUARTERS> HR_KUARTERS { get; set; }
    }
    public class HR_KUARTERS
    {
        [Key]
        public string HR_KOD_KUARTERS { get; set; }
        public string HR_BLOK_KUARTERS { get; set; }
        public string HR_ALAMAT1 { get; set; }
        public string HR_ALAMAT2 { get; set; }
        public string HR_ALAMAT3 { get; set; }
        public string HR_BANDAR { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_POSKOD { get; set; }
        public string HR_AKTIF_IND { get; set; }

        
        public virtual ICollection<HR_MAKLUMAT_KUARTERS> HR_MAKLUMAT_KUARTERS { get; set; }
    }
}