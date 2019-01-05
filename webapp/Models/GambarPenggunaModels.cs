using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class GambarPenggunaModels
    {
        public virtual DbSet<HR_GAMBAR_PENGGUNA> HR_GAMBAR_PENGGUNA { get; set; }
    }
    public partial class HR_GAMBAR_PENGGUNA
    {
        [Key]
        public string HR_NO_PEKERJA { get; set; }
        public string HR_FORMAT_TYPE { get; set; }
        public Nullable<decimal> HR_BYTE_SIZE { get; set; }
        public string HR_PIX_WIDTH { get; set; }
        public string HR_PIX_HEIGHT { get; set; }
        public Nullable<System.Guid> HR_PHOTO { get; set; }


    }
}