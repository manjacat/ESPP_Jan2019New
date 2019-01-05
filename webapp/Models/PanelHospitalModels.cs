using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PanelHospitalModels
    {
        public virtual DbSet<HR_PANEL_HOSPITAL> HR_PANEL_HOSPITAL { get; set; }
    }
    public partial class HR_PANEL_HOSPITAL
    {
        [Key]
        public string HR_KOD_HOSPITAL { get; set; }
        public string HR_NAMA_HOSPITAL { get; set; }
        public string HR_NEGERI { get; set; }
    }
}