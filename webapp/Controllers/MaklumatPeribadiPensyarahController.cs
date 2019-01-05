using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;

namespace eSPP.Controllers
{
    public class MaklumatPeribadiPensyarahController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        // GET: MaklumatPeribadiPensyarah
       
        [HttpGet]
        public ActionResult PeribadiPensyarah()
        {
            HR_PENSYARAH hR_PENSYARAH = new HR_PENSYARAH();
            hR_PENSYARAH.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.Msg = "";
            return View(hR_PENSYARAH);
        }

        [HttpPost]
        public ActionResult PeribadiPensyarah([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TPOSKOD,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH hR_PENSYARAH, string sub)
        {
            if (ModelState.IsValid)
            {
                ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
                ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
                ViewBag.Msg = "Data Tidak Berjaya Diproses";
                HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.SingleOrDefault(s => (s.HR_NO_PEKERJA == hR_PENSYARAH.HR_NO_PEKERJA && s.HR_NO_PEKERJA != null) || (s.HR_NAMA_PENSYARAH == hR_PENSYARAH.HR_NAMA_PENSYARAH && s.HR_NAMA_PENSYARAH != null));
                HR_PENSYARAH item = new HR_PENSYARAH();
                if (sub == "tambah")
                {
                    if (Pensyarah == null)
                    {
                        HR_PENSYARAH selectLastID = db.HR_PENSYARAH.OrderByDescending(s => s.HR_NO_PENSYARAH).FirstOrDefault();
                        string LastID = new string(selectLastID.HR_NO_PENSYARAH.SkipWhile(x => !char.IsDigit(x)).TakeWhile(x => char.IsDigit(x)).ToArray());
                        int incrementID = Convert.ToInt32(LastID) + 1;
                        string pensyarahID = Convert.ToString(incrementID);
                        pensyarahID = "P" + pensyarahID.PadLeft(4, '0');
                        hR_PENSYARAH.HR_NO_PENSYARAH = pensyarahID;
                        
                        db.HR_PENSYARAH.Add(hR_PENSYARAH);
                        ViewBag.Msg = "Data Berjaya Di Masukkan";
                    }
                    else
                    {
                        if ( Pensyarah != null)
                        {
                            hR_PENSYARAH = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PENSYARAH == Pensyarah.HR_NO_PENSYARAH);
                            // hR_PENSYARAH.HR_NO_PENSYARAH = Pensyarah.HR_NO_PENSYARAH;
                            db.Entry(hR_PENSYARAH).State = EntityState.Modified;

                            ViewBag.Msg = "Data Berjaya Di Kemaskini";
                        }

                          
                    }
                }

                else
                {
                    if (Pensyarah != null)
                    {
                        hR_PENSYARAH = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PENSYARAH == Pensyarah.HR_NO_PENSYARAH);
                        //hR_PENSYARAH.HR_NO_PENSYARAH = Pensyarah.HR_NO_PENSYARAH;
                        db.HR_PENSYARAH.Remove(hR_PENSYARAH);
                        db.SaveChanges();
                        HR_PENSYARAH hR_PENSYARAH2 = new HR_PENSYARAH();
                        hR_PENSYARAH2.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
                        ViewBag.Msg = "Data Berjaya Di Padam";
                        return View(hR_PENSYARAH2);
                    }
                        
                }
                
                db.SaveChanges();

                HR_KUMPULAN_PENSYARAH kPensyarah = db.HR_KUMPULAN_PENSYARAH.SingleOrDefault(s => s.HR_KOD_KUMPULAN == hR_PENSYARAH.HR_KOD_KUMPULAN);
                if(kPensyarah == null)
                {
                    kPensyarah = new HR_KUMPULAN_PENSYARAH();
                }
                hR_PENSYARAH.HR_KUMPULAN_PENSYARAH = kPensyarah;

                
                ViewBag.HR_MESEJ = "T";
                if (hR_PENSYARAH.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D")
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == hR_PENSYARAH.HR_NO_PEKERJA && s.HR_NO_PEKERJA != null);
                    if (mPeribadi != null)
                    {
                        
                        item.HR_NAMA_PENSYARAH = mPeribadi.HR_NAMA_PEKERJA;
                        //item.HR_NO_PENSYARAH = mPeribadi.HR_NO_PENSYARAH;
                        //item.HR_KOD_KUMPULAN = mPeribadi.HR_KOD_KUMPULAN;
                        item.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
                        item.HR_NO_KPLAMA = mPeribadi.HR_NO_KPLAMA;
                        item.HR_NO_TELPEJABAT = mPeribadi.HR_TELPEJABAT;
                        item.HR_NO_TELBIMBIT = mPeribadi.HR_TELBIMBIT;
                        //item.HR_NO_FAKS = mPeribadi.HR_NO_FAKS;

                        HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == hR_PENSYARAH.HR_NO_PEKERJA);
                        if (mPekerjaan == null)
                        {
                            mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                        }

                        item.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                        var gred = Convert.ToInt32(mPekerjaan.HR_GRED);
                        GE_PARAMTABLE Gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                        if (Gred == null)
                        {
                            Gred = new GE_PARAMTABLE();
                        }
                        item.HR_GRED_KELULUSAN = Gred.SHORT_DESCRIPTION;
                        //item.HR_GRED_KELULUSAN = mPeribadi.HR_GRED_KELULUSAN;
                        item.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                        item.HR_TALAMAT1 = mPeribadi.HR_TALAMAT1;
                        item.HR_TALAMAT2 = mPeribadi.HR_TALAMAT2;
                        item.HR_TALAMAT3 = mPeribadi.HR_TALAMAT3;
                        item.HR_TBANDAR = mPeribadi.HR_TBANDAR;
                        item.HR_TPOSKOD = mPeribadi.HR_TPOSKOD;
                        item.HR_TNEGERI = mPeribadi.HR_TNEGERI;
                        item.HR_SALAMAT1 = mPeribadi.HR_SALAMAT1;
                        item.HR_SALAMAT2 = mPeribadi.HR_SALAMAT2;
                        item.HR_SALAMAT3 = mPeribadi.HR_SALAMAT3;
                        item.HR_SBANDAR = mPeribadi.HR_SBANDAR;
                        item.HR_SPOSKOD = mPeribadi.HR_SPOSKOD;
                        item.HR_SNEGERI = mPeribadi.HR_SNEGERI;
                        item.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;

                        HR_PENSYARAH Pensyarah3 = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PEKERJA == hR_PENSYARAH.HR_NO_PEKERJA && s.HR_NO_PEKERJA != null);
                        if (Pensyarah3 == null)
                        {
                            Pensyarah3 = new HR_PENSYARAH();
                        }

                        item.HR_NO_PENSYARAH = Pensyarah3.HR_NO_PENSYARAH;
                        item.HR_KOD_KUMPULAN = Pensyarah3.HR_KOD_KUMPULAN;
                        item.HR_NO_PENSYARAH = Pensyarah3.HR_NO_PENSYARAH;
                        item.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
                        item.HR_KUMPULAN_PENSYARAH = Pensyarah3.HR_KUMPULAN_PENSYARAH;

                        hR_PENSYARAH = item;

                    }
                }

                if (hR_PENSYARAH.HR_NO_PENSYARAH == null)
                {
                    ViewBag.HR_MESEJ = "T";
                }
                else
                {
                    ViewBag.HR_MESEJ = "Y";
                }

                return View(hR_PENSYARAH);
                
            }
            return View(hR_PENSYARAH);
        }

        public ActionResult JenisPensyarah(string HR_JENIS_IND)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<HR_KUMPULAN_PENSYARAH> item = db.HR_KUMPULAN_PENSYARAH.Where(s => s.HR_JENIS_IND == HR_JENIS_IND).ToList();
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CariPensyarah(string DATA, string KATEGORI)
        {
            db.Configuration.ProxyCreationEnabled = false;
            CARI_PENSYARAH item = new CARI_PENSYARAH();
            item.HR_PENSYARAH = new HR_PENSYARAH();
            item.HR_MESEJ = "T";
            if(KATEGORI == "D")
            {
                HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == DATA && s.HR_NO_PEKERJA != null);
                if (mPeribadi != null)
                {
                    
                    item.HR_PENSYARAH.HR_NAMA_PENSYARAH = mPeribadi.HR_NAMA_PEKERJA;
                    //item.HR_PENSYARAH.HR_NO_PENSYARAH = mPeribadi.HR_NO_PENSYARAH;
                    //item.HR_PENSYARAH.HR_KOD_KUMPULAN = mPeribadi.HR_KOD_KUMPULAN;
                    item.HR_PENSYARAH.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
                    item.HR_PENSYARAH.HR_NO_KPLAMA = mPeribadi.HR_NO_KPLAMA;
                    item.HR_PENSYARAH.HR_NO_TELPEJABAT = mPeribadi.HR_TELPEJABAT;
                    item.HR_PENSYARAH.HR_NO_TELBIMBIT = mPeribadi.HR_TELBIMBIT;
                    //item.HR_PENSYARAH.HR_NO_FAKS = mPeribadi.HR_NO_FAKS;

                    HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == DATA);
                    if (mPekerjaan == null)
                    {
                        mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                    }

                    item.HR_PENSYARAH.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                    var gred = Convert.ToInt32(mPekerjaan.HR_GRED);
                    GE_PARAMTABLE Gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                    if (Gred == null)
                    {
                        Gred = new GE_PARAMTABLE();
                    }
                    item.HR_PENSYARAH.HR_GRED_KELULUSAN = Gred.SHORT_DESCRIPTION;
                    //item.HR_PENSYARAH.HR_GRED_KELULUSAN = mPeribadi.HR_GRED_KELULUSAN;
                    item.HR_PENSYARAH.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                    item.HR_PENSYARAH.HR_TALAMAT1 = mPeribadi.HR_TALAMAT1;
                    item.HR_PENSYARAH.HR_TALAMAT2 = mPeribadi.HR_TALAMAT2;
                    item.HR_PENSYARAH.HR_TALAMAT3 = mPeribadi.HR_TALAMAT3;
                    item.HR_PENSYARAH.HR_TBANDAR = mPeribadi.HR_TBANDAR;
                    item.HR_PENSYARAH.HR_TPOSKOD = mPeribadi.HR_TPOSKOD;
                    item.HR_PENSYARAH.HR_TNEGERI = mPeribadi.HR_TNEGERI;
                    item.HR_PENSYARAH.HR_SALAMAT1 = mPeribadi.HR_SALAMAT1;
                    item.HR_PENSYARAH.HR_SALAMAT2 = mPeribadi.HR_SALAMAT2;
                    item.HR_PENSYARAH.HR_SALAMAT3 = mPeribadi.HR_SALAMAT3;
                    item.HR_PENSYARAH.HR_SBANDAR = mPeribadi.HR_SBANDAR;
                    item.HR_PENSYARAH.HR_SPOSKOD = mPeribadi.HR_SPOSKOD;
                    item.HR_PENSYARAH.HR_SNEGERI = mPeribadi.HR_SNEGERI;
                    item.HR_PENSYARAH.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;

                    HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NO_PEKERJA == DATA && s.HR_NO_PEKERJA != null);
                    if (Pensyarah == null)
                    {
                        Pensyarah = new HR_PENSYARAH();
                        item.HR_MESEJ = "T";
                    }
                    else
                    {
                        item.HR_MESEJ = "Y";
                    }

                    item.HR_PENSYARAH.HR_NO_PENSYARAH = Pensyarah.HR_NO_PENSYARAH;
                    item.HR_PENSYARAH.HR_KOD_KUMPULAN = Pensyarah.HR_KOD_KUMPULAN;
                    item.HR_PENSYARAH.HR_NO_PENSYARAH = Pensyarah.HR_NO_PENSYARAH;

                }
                else
                {
                    item.HR_MESEJ = "T";
                }
            }
            else
            {
                HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.SingleOrDefault(s => s.HR_NAMA_PENSYARAH == DATA && s.HR_NAMA_PENSYARAH != null);
                
                if (Pensyarah == null)
                {
                    Pensyarah = new HR_PENSYARAH();
                    Pensyarah.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
                    item.HR_MESEJ = "T";
                }
                else
                {
                    item.HR_MESEJ = "Y";
                }
                item.HR_PENSYARAH = Pensyarah;
            }

            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}