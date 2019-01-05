using eSPP.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class MaklumatPeribadiController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: MaklumatPeribadi

        // GET: MaklumatKakitangan
        public ActionResult SenaraiPeribadi(string key, string value, int? kategori)
        {
            if(value == null)
            {
                value = "";
            }
            if (key == null)
            {
                key = "";
            }
            List<HR_PENSYARAH> mPeribadi = db.HR_PENSYARAH.ToList();
            List<HR_PENSYARAH> sPensyarah = new List<HR_PENSYARAH>();
            foreach (HR_PENSYARAH peribadi in mPeribadi)
            {
                HR_PENSYARAH pensyarah = new HR_PENSYARAH();
                pensyarah.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
                if (peribadi.HR_NO_PEKERJA != null)
                {
                    HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.Find(peribadi.HR_NO_PEKERJA);
                    HR_MAKLUMAT_PEKERJAAN pekerja2 = db.HR_MAKLUMAT_PEKERJAAN.Find(peribadi.HR_NO_PEKERJA);
                    pensyarah.HR_NO_PENSYARAH = peribadi.HR_NO_PENSYARAH;
                    pensyarah.HR_KUMPULAN_PENSYARAH = peribadi.HR_KUMPULAN_PENSYARAH;
                    pensyarah.HR_KOD_KUMPULAN = peribadi.HR_KOD_KUMPULAN;
                    pensyarah.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
                    pensyarah.HR_NAMA_PENSYARAH = pekerja.HR_NAMA_PEKERJA;
                    pensyarah.HR_NO_KPBARU = pekerja.HR_NO_KPBARU;
                }
                else
                {
                    pensyarah = peribadi;
                    pensyarah.HR_KUMPULAN_PENSYARAH = peribadi.HR_KUMPULAN_PENSYARAH;
                }
                sPensyarah.Add(pensyarah);
            }

            List<HR_PENSYARAH> data = new List<HR_PENSYARAH>();
            data = sPensyarah.ToList();
            if (kategori == 1)
            {
                data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D").ToList();
                if(value != "")
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D" && (s.HR_NO_PEKERJA == value || s.HR_NAMA_PENSYARAH.Contains(value) || s.HR_NO_KPBARU == value)).ToList();
                }
            }
            else if (kategori == 2)
            {
                data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "L").ToList();
                if (value != "")
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "L" && (s.HR_NO_PEKERJA == value || s.HR_NAMA_PENSYARAH.Contains(value) || s.HR_NO_KPBARU == value)).ToList();
                }
            }

            if(key == "" && kategori == null && value != null)
            {
                data = data.Where(s => s.HR_NO_PEKERJA == value || s.HR_NAMA_PENSYARAH.Contains(value) || s.HR_NO_KPBARU == value).ToList();
            }

            if (key == "1")
            {
                data = new List<HR_PENSYARAH>();
                data = sPensyarah.ToList();
                
                if (kategori == 1)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D" && s.HR_NO_PEKERJA == value).ToList();
                }
                else if (kategori == 2)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "L" && s.HR_NO_PEKERJA == value).ToList();
                }
                else
                {
                    data = data.Where(s => s.HR_NO_PEKERJA == value).ToList();
                }

            }
            else if (key == "2")
            {
                data = new List<HR_PENSYARAH>();
                data = sPensyarah.ToList();
                
                if (kategori == 1)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D" && s.HR_NAMA_PENSYARAH.Contains(value)).ToList();
                }
                else if (kategori == 2)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "L" && s.HR_NAMA_PENSYARAH.Contains(value)).ToList();
                }
                else
                {
                    data = data.Where(s => s.HR_NAMA_PENSYARAH.Contains(value)).ToList();
                }
            }
            else if (key == "3")
            {
                data = new List<HR_PENSYARAH>();
                data = sPensyarah.ToList();
                
                if (kategori == 1)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "D" && s.HR_NO_KPBARU == value).ToList();
                }
                else if (kategori == 2)
                {
                    data = data.Where(s => s.HR_KUMPULAN_PENSYARAH.HR_JENIS_IND == "L" && s.HR_NO_KPBARU == value).ToList();
                }
                else
                {
                    data = data.Where(s => s.HR_NO_KPBARU == value).ToList();
                }
            }

            
            return View(data.ToList());
        }

        public ActionResult InfoPeribadi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH pensyarah = db.HR_PENSYARAH.Find(id);

            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            if (pensyarah.HR_TNEGERI != null)
            {
                pensyarah.HR_TNEGERI = pensyarah.HR_TNEGERI.Trim();
            }
            if (pensyarah.HR_SNEGERI != null)
            {
                pensyarah.HR_SNEGERI = pensyarah.HR_SNEGERI.Trim();
            }


            if (pensyarah.HR_NO_PEKERJA != null)
            {
                HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.Find(pensyarah.HR_NO_PEKERJA);
                pensyarah.HR_NO_PENSYARAH = pensyarah.HR_NO_PENSYARAH;
                pensyarah.HR_KUMPULAN_PENSYARAH = pensyarah.HR_KUMPULAN_PENSYARAH;
                pensyarah.HR_KOD_KUMPULAN = pensyarah.HR_KOD_KUMPULAN;
                pensyarah.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
                pensyarah.HR_NAMA_PENSYARAH = pekerja.HR_NAMA_PEKERJA;
                pensyarah.HR_NO_KPBARU = pekerja.HR_NO_KPBARU;

                pensyarah.HR_NO_KPLAMA = pekerja.HR_NO_KPLAMA;
                pensyarah.HR_NO_TELPEJABAT = pekerja.HR_TELPEJABAT;
                pensyarah.HR_NO_TELBIMBIT = pekerja.HR_TELBIMBIT;
                //pensyarah.HR_NO_FAKS = pekerja.HR_NO_FAKS;

                HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(pensyarah.HR_NO_PEKERJA);
                if (mPekerjaan == null)
                {
                    mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }

                pensyarah.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                var gred = Convert.ToInt32(mPekerjaan.HR_GRED);
                GE_PARAMTABLE Gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                if (Gred == null)
                {
                    Gred = new GE_PARAMTABLE();
                }
                pensyarah.HR_GRED_KELULUSAN = Gred.SHORT_DESCRIPTION;
                //pensyarah.HR_GRED_KELULUSAN = pekerja.HR_GRED_KELULUSAN;
                pensyarah.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                pensyarah.HR_TALAMAT1 = pekerja.HR_TALAMAT1;
                pensyarah.HR_TALAMAT2 = pekerja.HR_TALAMAT2;
                pensyarah.HR_TALAMAT3 = pekerja.HR_TALAMAT3;
                pensyarah.HR_TBANDAR = pekerja.HR_TBANDAR;
                pensyarah.HR_TPOSKOD = pekerja.HR_TPOSKOD;
                pensyarah.HR_TNEGERI = pekerja.HR_TNEGERI;
                pensyarah.HR_SALAMAT1 = pekerja.HR_SALAMAT1;
                pensyarah.HR_SALAMAT2 = pekerja.HR_SALAMAT2;
                pensyarah.HR_SALAMAT3 = pekerja.HR_SALAMAT3;
                pensyarah.HR_SBANDAR = pekerja.HR_SBANDAR;
                pensyarah.HR_SPOSKOD = pekerja.HR_SPOSKOD;
                pensyarah.HR_SNEGERI = pekerja.HR_SNEGERI;

            }

            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");

            return PartialView("_InfoPeribadi", pensyarah);
        }

        public ActionResult TambahPeribadi ()
        {
            HR_PENSYARAH peribadi = new HR_PENSYARAH();
            peribadi.HR_KUMPULAN_PENSYARAH = new HR_KUMPULAN_PENSYARAH();
    
        
            ViewBag.Msg = "";
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            return PartialView("_TambahPeribadi");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPeribadi([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TPOSKOD,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH pensyarah)
        {
            if (ModelState.IsValid)
            {               
                HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.SingleOrDefault(s => (s.HR_NO_PEKERJA == pensyarah.HR_NO_PEKERJA && s.HR_NO_PEKERJA != null) || (s.HR_NAMA_PENSYARAH == pensyarah.HR_NAMA_PENSYARAH && s.HR_NAMA_PENSYARAH != null));


                if (Pensyarah == null)
                {
                    HR_PENSYARAH selectLastID = db.HR_PENSYARAH.OrderByDescending(s => s.HR_NO_PENSYARAH).FirstOrDefault();
                    string LastID = new string(selectLastID.HR_NO_PENSYARAH.SkipWhile(x => !char.IsDigit(x)).TakeWhile(x => char.IsDigit(x)).ToArray());
                    int incrementID = Convert.ToInt32(LastID) + 1;
                    string pensyarahID = Convert.ToString(incrementID);
                    pensyarahID = "P" + pensyarahID.PadLeft(4, '0');
                    pensyarah.HR_NO_PENSYARAH = pensyarahID;

                    db.HR_PENSYARAH.Add(pensyarah);
                    db.SaveChanges();
                }
                
                
                return RedirectToAction("SenaraiPeribadi");
            }
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            //ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahPeribadi", pensyarah);
        }

        public ActionResult EditPeribadi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH pensyarah = db.HR_PENSYARAH.Find(id);

            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            if (pensyarah.HR_TNEGERI != null)
            {
                pensyarah.HR_TNEGERI = pensyarah.HR_TNEGERI.Trim();
            }
            if (pensyarah.HR_SNEGERI != null)
            {
                pensyarah.HR_SNEGERI = pensyarah.HR_SNEGERI.Trim();
            }


            if (pensyarah.HR_NO_PEKERJA != null)
            {
                HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.Find(pensyarah.HR_NO_PEKERJA);
                pensyarah.HR_NO_PENSYARAH = pensyarah.HR_NO_PENSYARAH;
                pensyarah.HR_KUMPULAN_PENSYARAH = pensyarah.HR_KUMPULAN_PENSYARAH;
                pensyarah.HR_KOD_KUMPULAN = pensyarah.HR_KOD_KUMPULAN;
                pensyarah.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
                pensyarah.HR_NAMA_PENSYARAH = pekerja.HR_NAMA_PEKERJA;
                pensyarah.HR_NO_KPBARU = pekerja.HR_NO_KPBARU;

                pensyarah.HR_NO_KPLAMA = pekerja.HR_NO_KPLAMA;
                pensyarah.HR_NO_TELPEJABAT = pekerja.HR_TELPEJABAT;
                pensyarah.HR_NO_TELBIMBIT = pekerja.HR_TELBIMBIT;
                //pensyarah.HR_NO_FAKS = pekerja.HR_NO_FAKS;

                HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(pensyarah.HR_NO_PEKERJA);
                if (mPekerjaan == null)
                {
                    mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }

                pensyarah.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                var gred = Convert.ToInt32(mPekerjaan.HR_GRED);
                GE_PARAMTABLE Gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                if (Gred == null)
                {
                    Gred = new GE_PARAMTABLE();
                }
                pensyarah.HR_GRED_KELULUSAN = Gred.SHORT_DESCRIPTION;
                //pensyarah.HR_GRED_KELULUSAN = pekerja.HR_GRED_KELULUSAN;
                pensyarah.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                pensyarah.HR_TALAMAT1 = pekerja.HR_TALAMAT1;
                pensyarah.HR_TALAMAT2 = pekerja.HR_TALAMAT2;
                pensyarah.HR_TALAMAT3 = pekerja.HR_TALAMAT3;
                pensyarah.HR_TBANDAR = pekerja.HR_TBANDAR;
                pensyarah.HR_TPOSKOD = pekerja.HR_TPOSKOD;
                pensyarah.HR_TNEGERI = pekerja.HR_TNEGERI;
                pensyarah.HR_SALAMAT1 = pekerja.HR_SALAMAT1;
                pensyarah.HR_SALAMAT2 = pekerja.HR_SALAMAT2;
                pensyarah.HR_SALAMAT3 = pekerja.HR_SALAMAT3;
                pensyarah.HR_SBANDAR = pekerja.HR_SBANDAR;
                pensyarah.HR_SPOSKOD = pekerja.HR_SPOSKOD;
                pensyarah.HR_SNEGERI = pekerja.HR_SNEGERI;

            }

            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");

            return PartialView("_EditPeribadi", pensyarah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPeribadi([Bind(Include = "HR_NO_PENSYARAH,HR_NAMA_PENSYARAH,HR_KOD_KUMPULAN,HR_NO_KPBARU,HR_NO_KPLAMA,HR_NO_TELPEJABAT,HR_NO_TELBIMBIT,HR_NO_FAKS,HR_JAWATAN,HR_GRED_KELULUSAN,HR_GAJI_POKOK,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TBANDAR,HR_TPOSKOD,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_NO_PEKERJA")] HR_PENSYARAH pensyarah)
        {
            if (ModelState.IsValid)
            {
                HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.SingleOrDefault(s => (s.HR_NO_PENSYARAH != pensyarah.HR_NO_PENSYARAH && (s.HR_NO_PEKERJA == pensyarah.HR_NO_PEKERJA && s.HR_NO_PEKERJA != null)) || (s.HR_NO_PENSYARAH != pensyarah.HR_NO_PENSYARAH && (s.HR_NAMA_PENSYARAH == pensyarah.HR_NAMA_PENSYARAH && s.HR_NAMA_PENSYARAH != null)));
                if (Pensyarah == null)
                {
                    db.Entry(pensyarah).State = EntityState.Modified;
                    db.SaveChanges();
                }


                return RedirectToAction("SenaraiPeribadi");
            }
            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");
            //ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            return PartialView("_TambahPeribadi", pensyarah);
        }

        public ActionResult PadamPeribadi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENSYARAH pensyarah = db.HR_PENSYARAH.Find(id);

            if (pensyarah == null)
            {
                return HttpNotFound();
            }
            if (pensyarah.HR_TNEGERI != null)
            {
                pensyarah.HR_TNEGERI = pensyarah.HR_TNEGERI.Trim();
            }
            if (pensyarah.HR_SNEGERI != null)
            {
                pensyarah.HR_SNEGERI = pensyarah.HR_SNEGERI.Trim();
            }


            if (pensyarah.HR_NO_PEKERJA != null)
            {
                HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.Find(pensyarah.HR_NO_PEKERJA);
                pensyarah.HR_NO_PENSYARAH = pensyarah.HR_NO_PENSYARAH;
                pensyarah.HR_KUMPULAN_PENSYARAH = pensyarah.HR_KUMPULAN_PENSYARAH;
                pensyarah.HR_KOD_KUMPULAN = pensyarah.HR_KOD_KUMPULAN;
                pensyarah.HR_NO_PEKERJA = pekerja.HR_NO_PEKERJA;
                pensyarah.HR_NAMA_PENSYARAH = pekerja.HR_NAMA_PEKERJA;
                pensyarah.HR_NO_KPBARU = pekerja.HR_NO_KPBARU;

                pensyarah.HR_NO_KPLAMA = pekerja.HR_NO_KPLAMA;
                pensyarah.HR_NO_TELPEJABAT = pekerja.HR_TELPEJABAT;
                pensyarah.HR_NO_TELBIMBIT = pekerja.HR_TELBIMBIT;
                //pensyarah.HR_NO_FAKS = pekerja.HR_NO_FAKS;

                HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(pensyarah.HR_NO_PEKERJA);
                if (mPekerjaan == null)
                {
                    mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                }

                pensyarah.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                var gred = Convert.ToInt32(mPekerjaan.HR_GRED);
                GE_PARAMTABLE Gred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gred);
                if (Gred == null)
                {
                    Gred = new GE_PARAMTABLE();
                }
                pensyarah.HR_GRED_KELULUSAN = Gred.SHORT_DESCRIPTION;
                //pensyarah.HR_GRED_KELULUSAN = pekerja.HR_GRED_KELULUSAN;
                pensyarah.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                pensyarah.HR_TALAMAT1 = pekerja.HR_TALAMAT1;
                pensyarah.HR_TALAMAT2 = pekerja.HR_TALAMAT2;
                pensyarah.HR_TALAMAT3 = pekerja.HR_TALAMAT3;
                pensyarah.HR_TBANDAR = pekerja.HR_TBANDAR;
                pensyarah.HR_TPOSKOD = pekerja.HR_TPOSKOD;
                pensyarah.HR_TNEGERI = pekerja.HR_TNEGERI;
                pensyarah.HR_SALAMAT1 = pekerja.HR_SALAMAT1;
                pensyarah.HR_SALAMAT2 = pekerja.HR_SALAMAT2;
                pensyarah.HR_SALAMAT3 = pekerja.HR_SALAMAT3;
                pensyarah.HR_SBANDAR = pekerja.HR_SBANDAR;
                pensyarah.HR_SPOSKOD = pekerja.HR_SPOSKOD;
                pensyarah.HR_SNEGERI = pekerja.HR_SNEGERI;

            }

            ViewBag.HR_TNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_SNEGERI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_KOD_KUMPULAN = new SelectList(db.HR_KUMPULAN_PENSYARAH, "HR_KOD_KUMPULAN", "HR_PENERANGAN");

            return PartialView("_PadamPeribadi", pensyarah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PadamPeribadi(HR_PENSYARAH model)
        {
            HR_PENSYARAH pensyarah = db.HR_PENSYARAH.Find(model.HR_NO_PENSYARAH);
            db.HR_PENSYARAH.Remove(pensyarah);
            db.SaveChanges();
            return RedirectToAction("SenaraiPeribadi");
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
            if (KATEGORI == "D")
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
                }
            }

            HR_PENSYARAH Pensyarah = db.HR_PENSYARAH.FirstOrDefault(s => (s.HR_NAMA_PENSYARAH == DATA && s.HR_NAMA_PENSYARAH != null) || (s.HR_NO_PEKERJA == DATA && s.HR_NO_PEKERJA != null));
            if (Pensyarah != null)
            {
                if (KATEGORI == "L")
                {
                    item.HR_PENSYARAH = Pensyarah;
                }
                else
                {
                    item.HR_PENSYARAH.HR_NO_PENSYARAH = Pensyarah.HR_NO_PENSYARAH;
                    item.HR_PENSYARAH.HR_KOD_KUMPULAN = Pensyarah.HR_KOD_KUMPULAN;
                }
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult CariData(string DATA, string KATEGORI, string ID)
        {
            List<HR_PENSYARAH> Pensyarah = new List<HR_PENSYARAH>();
            if (KATEGORI == "D")
            {
                
                if (ID == null)
                {
                    Pensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_PEKERJA == DATA && s.HR_NO_PEKERJA != null).ToList();
                }
                else
                {
                    Pensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH != ID && (s.HR_NO_PEKERJA == DATA && s.HR_NO_PEKERJA != null)).ToList();
                }
                if (Pensyarah.Count() > 0)
                {
                    var data = "Data Telah Wujud";
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                if (ID == null)
                {
                    Pensyarah = db.HR_PENSYARAH.Where(s => s.HR_NAMA_PENSYARAH == DATA && s.HR_NAMA_PENSYARAH != null).ToList();
                }
                else
                {
                    Pensyarah = db.HR_PENSYARAH.Where(s => s.HR_NO_PENSYARAH != ID && (s.HR_NAMA_PENSYARAH == DATA && s.HR_NAMA_PENSYARAH != null)).ToList();
                }
                if (Pensyarah.Count() > 0)
                {
                    var data = "Data Telah Wujud";
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            
        }

       
    }



}