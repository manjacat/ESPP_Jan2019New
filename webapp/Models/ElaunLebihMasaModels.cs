using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class ElaunLebihMasaModels
    {
        public virtual DbSet<HR_KADAR_ELAUN_LEBIHMASA> HR_KADAR_ELAUN_LEBIHMASA { get; set; }
    }
    public class HR_KADAR_ELAUN_LEBIHMASA

    {
        [Key]
        [Column(Order = 0)]
        public string HR_JENIS_HARI { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_JENIS_WAKTU { get; set; }
        public Nullable<decimal> HR_KADAR_SEJAM { get; set; }
        public string HR_AKTIF_IND { get; set; }
    }
}