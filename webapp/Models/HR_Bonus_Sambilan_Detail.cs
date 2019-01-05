using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace eSPP.Models
{
    public class HR_BONUS_SAMBILAN_DETAIL
    {
        [Key]
        [Column(Order = 0)]
        [MaxLength(5)]
        public string HR_NO_PEKERJA { get; set; }

        [MaxLength(20)]
        public string HR_NO_KPBARU { get; set; }

        [Key]
        [Column(Order = 1)]
        public int HR_TAHUN_BONUS { get; set; }

        [Key]
        [Column(Order = 2)]
        public int HR_BULAN_BONUS { get; set; }

        public Nullable<decimal> HR_JANUARI { get; set; }
        public Nullable<decimal> HR_FEBRUARI { get; set; }
        public Nullable<decimal> HR_MAC { get; set; }
        public Nullable<decimal> HR_APRIL { get; set; }
        public Nullable<decimal> HR_MEI { get; set; }
        public Nullable<decimal> HR_JUN { get; set; }
        public Nullable<decimal> HR_JULAI { get; set; }
        public Nullable<decimal> HR_OGOS { get; set; }
        public Nullable<decimal> HR_SEPTEMBER { get; set; }
        public Nullable<decimal> HR_OKTOBER { get; set; }
        public Nullable<decimal> HR_NOVEMBER { get; set; }
        public Nullable<decimal> HR_DISEMBER { get; set; }

        public Nullable<decimal> HR_JUMLAH_GAJI { get; set; }
        public Nullable<decimal> HR_GAJI_PURATA { get; set; }
        public Nullable<decimal> HR_BONUS_DITERIMA { get; set; }
        public Nullable<decimal> HR_BONUS_LAYAK { get; set; }


        public int HR_BULAN_START { get; set; }
        public int HR_BULAN_END { get; set; }
        public int HR_STATUS { get; set; }

        [MaxLength(1000)]
        public string HR_CATATAN { get; set; }
        public int HR_MUKTAMAD { get; set; }

        #region testing
        public static void TestInsert()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HR_BONUS_SAMBILAN_DETAIL det = new HR_BONUS_SAMBILAN_DETAIL();
            det.HR_NO_PEKERJA = "01410";
            det.HR_NO_KPBARU = "800615085334";
            det.HR_BULAN_BONUS = 1;
            det.HR_TAHUN_BONUS = 2018;
            det.HR_JANUARI = 10;
            det.HR_FEBRUARI = 20;
            det.HR_MAC = 30;
            det.HR_APRIL = 40;
            det.HR_MEI = 50;
            det.HR_JUN = 60;
            det.HR_JULAI = 70;
            det.HR_OGOS = 0;
            det.HR_SEPTEMBER = 0;
            det.HR_OKTOBER = 0;
            det.HR_NOVEMBER = 0;
            det.HR_DISEMBER = 0;
            det.HR_CATATAN = string.Empty;
            det.HR_MUKTAMAD = 0;
            db.HR_BONUS_SAMBILAN_DETAIL.Add(det);
            db.SaveChanges();
        }

        public static void TestingUpdate()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_NO_PEKERJA == "01410"
                && x.HR_BULAN_BONUS == 1
                && x.HR_TAHUN_BONUS == 2018).FirstOrDefault();
            if (det != null)
            {
                det.HR_JUMLAH_GAJI = det.HR_JANUARI + det.HR_FEBRUARI + det.HR_MAC + det.HR_APRIL + det.HR_MEI + det.HR_JUN
                    + det.HR_JULAI + det.HR_OGOS + det.HR_SEPTEMBER + det.HR_OKTOBER + det.HR_NOVEMBER + det.HR_DISEMBER;
                det.HR_GAJI_PURATA = Decimal.Round((decimal)det.HR_JUMLAH_GAJI / 12, 3);
                det.HR_CATATAN = "Ini Kiraan Yang Tepat";
                det.HR_BONUS_DITERIMA = (decimal)12.00;
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void TestDelete()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_NO_PEKERJA == "01410"
                && x.HR_BULAN_BONUS == 1
                && x.HR_TAHUN_BONUS == 2018).FirstOrDefault();
            if (det != null)
            {
                db.Entry(det).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        #endregion

        public static List<HR_BONUS_SAMBILAN_DETAIL> GetBonusSambilanDetailData(int month, int year)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_BONUS_SAMBILAN_DETAIL> list = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BONUS == month
                && x.HR_TAHUN_BONUS == year).ToList();
            return list;
        }

        public static void InsertTambahBonus(List<BonusSambilanDetailModel> listBonus)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            foreach (BonusSambilanDetailModel bonus in listBonus)
            {
                string noPekerja = bonus.NoPekerja;
                HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                    .Where(x => x.HR_BULAN_BONUS == bonus.BulanBonus
                    && x.HR_TAHUN_BONUS == bonus.TahunBonus
                    && x.HR_NO_PEKERJA == bonus.NoPekerja).FirstOrDefault();
                if (det == null)
                {
                    det = new HR_BONUS_SAMBILAN_DETAIL
                    {
                        HR_NO_PEKERJA = bonus.NoPekerja,
                        HR_NO_KPBARU = bonus.NoKadPengenalan,
                        HR_TAHUN_BONUS = bonus.TahunBonus,
                        HR_BULAN_BONUS = bonus.BulanBonus,
                        HR_JANUARI = bonus.Jan,
                        HR_FEBRUARI = bonus.Feb,
                        HR_MAC = bonus.Mac,
                        HR_APRIL = bonus.April,
                        HR_MEI = bonus.Mei,
                        HR_JUN = bonus.Jun,
                        HR_JULAI = bonus.Julai,
                        HR_OGOS = bonus.Ogos,
                        HR_SEPTEMBER = bonus.September,
                        HR_OKTOBER = bonus.Oktober,
                        HR_NOVEMBER = bonus.November,
                        HR_DISEMBER = bonus.Disember,
                        HR_JUMLAH_GAJI = bonus.JumlahGaji,
                        HR_GAJI_PURATA = bonus.GajiPurata,

                        HR_BONUS_LAYAK = bonus.BonusLayak,
                        HR_BONUS_DITERIMA = bonus.BonusDiterima,
                        HR_CATATAN = bonus.Catatan,
                        HR_BULAN_START = bonus.MinBulan,
                        HR_BULAN_END = bonus.MaxBulan,

                        HR_MUKTAMAD = 0, //set tu belum muktamad
                        HR_STATUS = 0 //Not yet confirmed
                    };
                    db.HR_BONUS_SAMBILAN_DETAIL.Add(det);
                    db.SaveChanges();
                }
                else
                {
                    det.HR_NO_PEKERJA = bonus.NoPekerja;
                    det.HR_NO_KPBARU = bonus.NoKadPengenalan;
                    det.HR_TAHUN_BONUS = bonus.TahunBonus;
                    det.HR_BULAN_BONUS = bonus.BulanBonus;
                    det.HR_JANUARI = bonus.Jan;
                    det.HR_FEBRUARI = bonus.Feb;
                    det.HR_MAC = bonus.Mac;
                    det.HR_APRIL = bonus.April;
                    det.HR_MEI = bonus.Mei;
                    det.HR_JUN = bonus.Jun;
                    det.HR_JULAI = bonus.Julai;
                    det.HR_OGOS = bonus.Ogos;
                    det.HR_SEPTEMBER = bonus.September;
                    det.HR_OKTOBER = bonus.Oktober;
                    det.HR_NOVEMBER = bonus.November;
                    det.HR_DISEMBER = bonus.Disember;
                    det.HR_JUMLAH_GAJI = bonus.JumlahGaji;
                    det.HR_GAJI_PURATA = bonus.GajiPurata;
                    det.HR_BONUS_LAYAK = bonus.BonusLayak;
                    det.HR_BONUS_DITERIMA = bonus.BonusDiterima;
                    det.HR_CATATAN = bonus.Catatan;
                    det.HR_BULAN_START = bonus.MinBulan;
                    det.HR_BULAN_END = bonus.MaxBulan;
                    det.HR_MUKTAMAD = 0; //set tu belum muktamad
                    det.HR_STATUS = 0; //Not yet confirmed
                    db.Entry(det).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static void UpdateTambahBonus()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_BONUS_SAMBILAN_DETAIL> listDet = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_STATUS == 0).ToList();
            foreach (HR_BONUS_SAMBILAN_DETAIL det in listDet)
            {
                det.HR_STATUS = 1;
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }           
        }

        public static void DeleteTambahBonus()
        {
            try
            {
                ApplicationDbContext db = new ApplicationDbContext();
                List<HR_BONUS_SAMBILAN_DETAIL> listDet = db.HR_BONUS_SAMBILAN_DETAIL
                    .Where(x => x.HR_STATUS == 0).ToList();
                foreach (HR_BONUS_SAMBILAN_DETAIL det in listDet)
                {
                    db.Entry(det).State = EntityState.Deleted;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        public static void UpdateBonusDiterima(int month, int year, string noPekerja, decimal bonus)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BONUS == month
                && x.HR_TAHUN_BONUS == year
                && x.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            if (det != null)
            {
                det.HR_BONUS_DITERIMA = bonus;
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateCatatan(int month, int year, string noPekerja, string catatan)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(x => x.HR_BULAN_BONUS == month
                && x.HR_TAHUN_BONUS == year
                && x.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
            if (det != null)
            {
                det.HR_CATATAN = catatan;
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateMuktamad(int month, int year, string noPekerja)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (string.IsNullOrEmpty(noPekerja))
            {
                List<HR_BONUS_SAMBILAN_DETAIL> listd;
                listd = db.HR_BONUS_SAMBILAN_DETAIL
                    .Where(x => x.HR_BULAN_BONUS == month
                    && x.HR_TAHUN_BONUS == year).ToList();
                foreach (HR_BONUS_SAMBILAN_DETAIL det in listd)
                {
                    det.HR_MUKTAMAD = 1;
                    db.Entry(det).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                HR_BONUS_SAMBILAN_DETAIL det = db.HR_BONUS_SAMBILAN_DETAIL
                    .Where(x => x.HR_BULAN_BONUS == month
                    && x.HR_TAHUN_BONUS == year
                    && x.HR_NO_PEKERJA == noPekerja).FirstOrDefault();
                det.HR_MUKTAMAD = 1;
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void UpdateBonusDiterima(int month, int year, decimal gandaan)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            List<HR_BONUS_SAMBILAN_DETAIL> listD = db.HR_BONUS_SAMBILAN_DETAIL
                .Where(s => s.HR_BULAN_BONUS == month
                && s.HR_TAHUN_BONUS == year).ToList();
            foreach(HR_BONUS_SAMBILAN_DETAIL det in listD)
            {
                if(det.HR_BONUS_LAYAK != null)
                {
                    decimal bonusDiterima = det.HR_BONUS_LAYAK.Value * gandaan;
                    det.HR_BONUS_DITERIMA = bonusDiterima;
                }
                else
                {
                    det.HR_BONUS_LAYAK = 0;
                    det.HR_BONUS_DITERIMA = 0;
                }
                db.Entry(det).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

    }
}