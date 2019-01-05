using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class PenggunaModels
    {
        public PenggunaModels() { }
        public virtual DbSet<GE_PENGGUNA> GE_PENGGUNA { get; set; }
    }

    [Table("GE_PENGGUNA")]
    public class GE_PENGGUNA
    {
        [Key]
        [Column("USER_ID")]
        public string USER_ID { get; set; }
        [Column("USER_LEVEL")]
        public byte USER_LEVEL { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Column("USER_PASSWORD")]
        public string USER_PASSWORD { get; set; }
        [Column("USER_GROUP")]
        public string USER_GROUP { get; set; }
        [Column("PEGAWAI_TAMBAH")]
        public string PEGAWAI_TAMBAH { get; set; }
        [Column("TKH_TAMBAH")]
        public Nullable<System.DateTime> TKH_TAMBAH { get; set; }
        [Column("PEGAWAI_UBAH")]
        public string PEGAWAI_UBAH { get; set; }
        [Column("TKH_UBAH")]
        public Nullable<System.DateTime> TKH_UBAH { get; set; }
        [Column("NAMA_PENUH_PENGGUNA")]
        public string NAMA_PENUH_PENGGUNA { get; set; }
        [Column("PENGGUNAAN")]
        public string PENGGUNAAN { get; set; }
        [Column("DEFAULT_WINDOW")]
        public string DEFAULT_WINDOW { get; set; }
        [Column("BANTUAN_BUTANG")]
        public decimal BANTUAN_BUTANG { get; set; }
        [Column("SARING_SENARAI_TUGAS")]
        public decimal SARING_SENARAI_TUGAS { get; set; }
        [Column("NOTA")]
        public string NOTA { get; set; }
        [Column("LOGIN_AKHIR")]
        public Nullable<System.DateTime> LOGIN_AKHIR { get; set; }
        [Column("ID_KAKITANGAN")]
        public string ID_KAKITANGAN { get; set; }
        [Column("ROWGUID")]
        public string ROWGUID { get; set; }
        public bool CreateUserInfo()
        {
            bool bReturn = false;
            ApplicationDbContext db = new ApplicationDbContext();
            db.GE_PENGGUNA.Add(this);
            db.SaveChanges();
            return bReturn;
        }

    }
}