using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class MaklumatKematianModels
    {
        public virtual DbSet<HR_MAKLUMAT_KEMATIAN> HR_MAKLUMAT_KEMATIAN { get; set; }
    }
    public  class HR_MAKLUMAT_KEMATIAN
    {
        [Key]
        public string HR_NO_PEKERJA { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_KEMATIAN { get; set; }
        public string HR_NO_KP_PEWARIS { get; set; }
        public string HR_ALAMAT1 { get; set; }
        public string HR_ALAMAT2 { get; set; }
        public string HR_ALAMAT3 { get; set; }
        public string HR_BANDAR { get; set; }
        public string HR_NO_TELRUMAH { get; set; }
        public string HR_HUBUNGAN { get; set; }
        public string HR_POSKOD { get; set; }
        public string HR_NEGERI { get; set; }
        public string HR_NAMA_PEWARIS { get; set; }
        public string HR_NO_TELPEJABAT { get; set; }
        public string HR_NO_TELBIMBIT { get; set; }
        public string HR_NO_VOUCHER { get; set; }
        public string HR_NAMA_PEGAWAI { get; set; }
        public string HR_JAWATAN_PEGAWAI { get; set; }
        public Nullable<System.DateTime> HR_TARIKH_BAYAR { get; set; }
        public string HR_MAKLUMAT_KHIDMAT { get; set; }
        public Nullable<decimal> HR_JUMLAH_WANG { get; set; }
        public string HR_VOT { get; set; }
    }
}