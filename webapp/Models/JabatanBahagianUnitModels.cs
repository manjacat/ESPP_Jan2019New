using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class JabatanBahagianUnitModels
    {


        public IEnumerable <GE_JABATAN> GE_JABATAN { get; set; }
        public IEnumerable <GE_BAHAGIAN> GE_BAHAGIAN { get; set; }
        public IEnumerable <GE_UNIT> GE_UNIT { get; set; }
    }

}