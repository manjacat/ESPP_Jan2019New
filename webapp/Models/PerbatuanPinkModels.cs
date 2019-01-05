using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PerbatuanPinkModels
    {
        public virtual DbSet<HR_PERBATUAN_PINK> HR_PERBATUAN_PINK { get; set; }
    }
    
    public  class HR_PERBATUAN_PINK
    {
        [Key]
        [Column(Order = 0)]
        public string HR_NO_PEKERJA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string HR_KOD_PERBATUAN { get; set; }
        public string HR_KENDERAAN_JENIS { get; set; }
        public string HR_KENDERAAN_NOMBOR { get; set; }
        public string HR_KENDERAAN_KUASA { get; set; }
        public string HR_KENDERAAN_KELAS { get; set; }
        public Nullable<int> HR_MAKSIMA_TUNTUTAN { get; set; }
      
        public Nullable<System.DateTime> HR_TARIKH_PERMOHONAN { get; set; }
        public string HR_IND_PEMOHON { get; set; }
        public string HR_TANDATANGAN_PEMOHON { get; set; }
        public string HR_TANDATANGAN_KB { get; set; }
        public string HR_NAMA_KB { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KB { get; set; }
        public string HR_TANDATANGAN_KJ { get; set; }
        public string HR_NAMA_KJ { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KJ { get; set; }
        public string HR_IND_KB { get; set; }
        public string HR_IND_KJ { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KERANIP { get; set; }
        public string HR_IND_KERANIP { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KERANIS { get; set; }
        public string HR_IND_KERANIS { get; set; }
        public string DOCUMENT_ID { get; set; }
        public string DESC_FILE { get; set; }
        public string HR_IND_HR { get; set; }
        public string HR_NAMA_HR { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_HR { get; set; }
        public string HR_CATATAN { get; set; }
        public string HR_FINALISED_IND { get; set; }
       


        public virtual ICollection<HR_PERBATUAN_TUJUAN> HR_PERBATUAN_TUJUAN { get; set; }
        public virtual HR_PERBATUAN_TUNTUTAN HR_PERBATUAN_TUNTUTAN { get; set; }
       
       
        
    }

} 