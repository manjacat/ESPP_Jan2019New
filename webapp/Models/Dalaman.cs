using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class Dalaman
    {
        [Key]
        public string JawatanCalonDalaman { get; set; }
        public string TarafJawatanDalaman { get; set; }
        public string TarikhPermohonanDalaman { get; set; }
        public string StatusDalaman { get; set; }
        
    }
}