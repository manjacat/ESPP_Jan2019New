using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class SuratPengesahanHospitalModel
    {
        public SuratPengesahanHospitalModel()
        {
            SearchResult = new List<CariPekerjaModel>();
        }

        public List<CariPekerjaModel> SearchResult { get; set; }
    }
}