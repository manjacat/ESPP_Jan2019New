using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PemohonanPinkModels
    {

        public IEnumerable<HR_PERBATUAN_TUJUAN> HR_PERBATUAN_TUJUAN { get; set; }
        public HR_PERBATUAN_TUNTUTAN HR_PERBATUAN_TUNTUTAN { get; set; }
        public HR_PERBATUAN_PINK HR_PERBATUAN_PINK { get; set; }
    }

}