using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PendapatanPegawaiModels
    {
        public virtual DbSet<HR_KEWANGAN8> HR_KEWANGAN8 { get; set; }
    }
    public class HR_KEWANGAN8
    {
        [Key]
        [Column(Order = 0)]
        public string HR_KOD_KEW8 { get; set; }
        public string HR_PENERANGAN { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_PENYATA_IND { get; set; }
        public string HR_GAJI_IND { get; set; }
    }
}