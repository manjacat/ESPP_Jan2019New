using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatAnugerahHajiModels
    {
        public virtual DbSet<HR_ANUGERAH_HAJI> HR_ANUGERAH_HAJI { get; set; }
    }
    public class HR_ANUGERAH_HAJI
    {
        [Key]
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TAHUN_PERGI { get; set; }
        public string HR_STATUS_HAJI { get; set; }
        public string HR_NP_YDP { get; set; }
        public string HR_LULUS_IND { get; set; }
        public string HR_NP_UP { get; set; }
        public string HR_NP_PEG { get; set; }
    }
}