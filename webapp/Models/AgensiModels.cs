using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class AgensiModels
    {
        public virtual DbSet<HR_AGENSI> HR_AGENSI { get; set; }
    }
    public class HR_AGENSI
    {
        [Key]
        public string HR_KOD_AGENSI { get; set; }
        public string HR_NAMA_AGENSI { get; set; }
        public string HR_ALAMAT1 { get; set; }
        public string HR_ALAMAT2 { get; set; }
        public string HR_ALAMAT3 { get; set; }
        public string HR_BANDAR { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_POSKOD { get; set; }
        public string HR_TELEFON1 { get; set; }
        public string HR_TELEFON2 { get; set; }
        public string HR_FAKS1 { get; set; }
        public string HR_FAKS2 { get; set; }
        public string HR_EMAIL { get; set; }
        public string HR_PERBANKAN { get; set; }
        public string HR_NO_FAIL { get; set; }
        public string HR_AKTIF_IND { get; set; }
        public string HR_SINGKATAN { get; set; }
        public string HR_NO_FAIL_P { get; set; }
        public string HR_NAMA_AHLI { get; set; }
        public string HR_BANK_KOD { get; set; }
    }
}