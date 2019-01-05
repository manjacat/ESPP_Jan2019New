using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PinkTuntutanModels
    {
        public virtual DbSet<HR_PERBATUAN_TUNTUTAN> HR_PERBATUAN_TUNTUTAN { get; set; }
    }
    public partial class HR_PERBATUAN_TUNTUTAN
    {
        [Key, ForeignKey("HR_PERBATUAN_PINK"), Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key, ForeignKey("HR_PERBATUAN_PINK"), Column(Order = 1)]
        public string HR_KOD_PERBATUAN { get; set; }

        public Nullable<decimal> HR_RM_PENGANGKUTAN { get; set; }
        public Nullable<decimal> HR_RM_CUKAI { get; set; }
        public Nullable<decimal> HR_RM_TELEFON { get; set; }
        public Nullable<decimal> HR_RM_DOBI { get; set; }

        public Nullable<decimal> HR_RM_GANTIRUGI { get; set; }
        public Nullable<decimal> HR_RM_PENDAHULUAN { get; set; }
        public Nullable<decimal> HR_RM_BAKI { get; set; }
        public Nullable<decimal> HR_RM_TIP { get; set; }
        public Nullable<decimal> HR_RM_TOLAK { get; set; }



        //public Nullable<int> HR_JUMLAH { get { return HR_NILAI * HR_KILOMETER; } }

        public virtual HR_PERBATUAN_PINK HR_PERBATUAN_PINK { get; set; }
         
    }

}