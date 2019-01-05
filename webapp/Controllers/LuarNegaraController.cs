using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class LuarNegaraController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        // GET: LuarNegara


        public ActionResult Index()
        {
            List<HR_SEMINAR_LUAR> Msem = new List<HR_SEMINAR_LUAR>();
            List<HR_SEMINAR_LUAR_DETAIL> Mdata = new List<HR_SEMINAR_LUAR_DETAIL>();
               
            List<HR_SEMINAR_LUAR> seminaran = db.HR_SEMINAR_LUAR.ToList();
            List<HR_SEMINAR_LUAR_DETAIL> seminarandetails = db.HR_SEMINAR_LUAR_DETAIL.ToList();

            LuarNegaraModels LuarNegara = new LuarNegaraModels();

            LuarNegara.HR_SEMINAR_LUAR = seminaran;
            LuarNegara.HR_SEMINAR_LUAR_DETAIL = seminarandetails;
            
            return View(LuarNegara);
        }

        public ActionResult CariPekerja(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            List<HR_SEMINAR_LUAR_DETAIL> model = new List<HR_SEMINAR_LUAR_DETAIL>();
            if (key == "1" || key == "4")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == value).ToList();

            }
            else if (key == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper())).ToList();
            }
            else if (key == "3")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == value).ToList();
            }

            List<HR_GAMBAR_PENGGUNA> gambar = db.HR_GAMBAR_PENGGUNA.ToList();
            ViewBag.gambar = gambar;
            ViewBag.senaraiPeribadi = mPeribadi;
            ViewBag.key = key;
            ViewBag.value = value;
            ViewBag.HR_SENARAI_PERIBADI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GAMBAR_PENGGUNA = new SelectList(db.HR_GAMBAR_PENGGUNA, "HR_NO_PEKERJA", "HR_PHOTO");

            if (key == "4")
            {
                ViewBag.key = key;
                ViewBag.value = mPeribadi.ElementAt(0).HR_NO_PEKERJA;
                model = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA == value).ToList();

            }
            return View(model.ToList());
        }



        public ActionResult InfoLuarNegara(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);
           

            if (seminar == null)
            {
                return HttpNotFound();
            }
            
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");
            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");
            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();
            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;
            return PartialView("_InfoLuarNegara", seminar);
        }


        public ActionResult TambahNegara(string id, string Jenis)
        {
            if (id == null || Jenis == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_SEMINAR_LUAR seminar = new HR_SEMINAR_LUAR();

            seminar.HR_KOD_LAWATAN = id;
            seminar.HR_JENIS_IND = Jenis;
            seminar.HR_TARIKH_PERMOHONAN = DateTime.Now;

            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;


            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");
            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");


            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();

            return PartialView("_TambahNegara", seminar);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahNegara([Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND,HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI")] HR_SEMINAR_LUAR seminar)
        {
            if (ModelState.IsValid)
            {
                HR_SEMINAR_LUAR seminarluar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => (s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN));

                if (seminarluar == null)
                {
                    var SelectLastID = db.HR_SEMINAR_LUAR.OrderByDescending(s => s.HR_KOD_LAWATAN).FirstOrDefault().HR_KOD_LAWATAN;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == '0').ToArray());
                    var Increment = Convert.ToSingle(LastID) + 1;
                    var KodLawatan = Convert.ToString(Increment).PadLeft(5, '0');
                    seminar.HR_KOD_LAWATAN = KodLawatan;

                    db.HR_SEMINAR_LUAR.Add(seminar);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return PartialView("_TambahNegara", seminar);
        }






        //public ActionResult TambahPengesahan()
        //{
            
        //    return PartialView("_TambahPengesahan");
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult TambahPengesahan([Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND,HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI")] HR_SEMINAR_LUAR seminar)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        HR_SEMINAR_LUAR seminarluar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => (s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN));

        //        if (seminarluar == null)
        //        {
        //            var SelectLastID = db.HR_SEMINAR_LUAR.OrderByDescending(s => s.HR_KOD_LAWATAN).FirstOrDefault().HR_KOD_LAWATAN;
        //            var LastID = new string(SelectLastID.SkipWhile(x => x == '0').ToArray());
        //            var Increment = Convert.ToSingle(LastID) + 1;
        //            var KodLawatan = Convert.ToString(Increment).PadLeft(5, '0');
        //            seminar.HR_KOD_LAWATAN = KodLawatan;

        //            db.HR_SEMINAR_LUAR.Add(seminar);
        //            db.SaveChanges();
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    return PartialView("_TambahPengesahan", seminar);
        //}



        public ActionResult EditLuarNegara(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);

            if (seminar == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");

            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");

            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;

            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();
       
            return PartialView("_EditLuarNegara", seminar);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLuarNegara([Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND,HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI")] HR_SEMINAR_LUAR seminar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seminar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(seminar);
        }


        public ActionResult PadamLuarNegara(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.Find(id);

            if (seminar == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> bulan = new List<SelectListItem>
            {
                //new SelectListItem { Text = "JANUARI", Value = "1" },
                //new SelectListItem { Text = "FEBRUARI", Value = "2" },
                //new SelectListItem { Text = "MAC", Value = "3" },
                //new SelectListItem { Text = "APRIL", Value = "4" },
                //new SelectListItem { Text = "MAY", Value = "5" },
                //new SelectListItem { Text = "JUN", Value = "6" },
                //new SelectListItem { Text = "JULAI", Value = "7" },
                //new SelectListItem { Text = "OGOS", Value = "8" },
                //new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                //new SelectListItem { Text = "OKTOBER", Value = "10" },
                //new SelectListItem { Text = "NOVEMBER", Value = "11" },
                //new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulan = new SelectList(bulan, "Value", "Text", DateTime.Now.Month + 1);

            List<SelectListItem> belanja = new List<SelectListItem>
            {
                new SelectListItem { Text = "MBPJ", Value = "M" },
                new SelectListItem { Text = "SENDIRI", Value = "S" },
                new SelectListItem { Text = "LAIN-LAIN", Value = "L" },
            };
            ViewBag.belanja = new SelectList(belanja, "Value", "Text");

            List<SelectListItem> luluskementerian = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.luluskementerian = new SelectList(luluskementerian, "Value", "Text");

            ViewBag.HR_SEMINAR_LUAR = db.HR_SEMINAR_LUAR.ToList();

            var tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", seminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpermohonan;
            return PartialView("_PadamLuarNegara", seminar);
        }


        [HttpPost, ActionName("PadamLuarNegara")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SEMINAR_LUAR seminar, HR_SEMINAR_LUAR_DETAIL seminardetails)
        {

            //ViewBag.seminarluar = db.HR_SEMINAR_LUAR_DETAIL.Include(s => s.HR_SEMINAR_LUAR).Where(s => s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();

            //  db.HR_SEMINAR_LUAR.RemoveRange(db.HR_SEMINAR_LUAR.Where(s => s.HR_KOD_LAWATAN == seminar.kod);
            //seminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN && s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN);

            db.HR_SEMINAR_LUAR.RemoveRange(db.HR_SEMINAR_LUAR.Include(s => s.HR_SEMINAR_LUAR_DETAIL).Where(s => s.HR_KOD_LAWATAN == seminar.HR_KOD_LAWATAN && s.HR_KOD_LAWATAN == seminardetails.HR_KOD_LAWATAN));
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult InfoAddPekerja(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            List<HR_SEMINAR_LUAR_DETAIL> details = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_KOD_LAWATAN == id).ToList();

            if (details == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.id = id;
            return PartialView("_InfoAddPekerja", details);
        }


        public ActionResult InfoSenaraiPekerja(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            List<HR_SEMINAR_LUAR_DETAIL> luar = db.HR_SEMINAR_LUAR_DETAIL.Include(s=>s.HR_SEMINAR_LUAR).Where(s => s.HR_NO_PEKERJA == id).ToList();

            if (luar == null)
            {
                return HttpNotFound();
            }
           
            return PartialView("_InfoSenaraiPekerja", luar);
        }



        public ActionResult InfoPekerja(string id, string value)
        {

            if (id == null && value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR_DETAIL seminarpekerja = new HR_SEMINAR_LUAR_DETAIL();
            seminarpekerja.HR_KOD_LAWATAN = id;
            seminarpekerja.HR_NO_PEKERJA = value;
            HR_SEMINAR_LUAR_DETAIL seminardetails = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_KOD_LAWATAN == id && s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == id);

            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_JAWATAN = mPekerjaan.HR_JAWATAN;

            ViewBag.HR_JABATAN = mPekerjaan.HR_JABATAN;


            ViewBag.HR_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;

            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");


            db.Configuration.ProxyCreationEnabled = false;

            //ViewBag.seminarluar = db.HR_SEMINAR_LUAR.Include(s => s.HR_SEMINAR_LUAR_DETAIL).Where(s => s.HR_KOD_LAWATAN == seminardetails.HR_KOD_LAWATAN).ToList();
            ViewBag.seminarluar = db.HR_SEMINAR_LUAR_DETAIL.Include(s => s.HR_SEMINAR_LUAR).Where(s => s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();

            // ViewBag.seminarluar = db.HR_SEMINAR_LUAR_DETAIL.Where(s=> s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();


            if (seminardetails == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> kerap = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },

            };
            ViewBag.kerap = new SelectList(kerap, "Value", "Text");


            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();


            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            return PartialView("_InfoPekerja", seminardetails);
        }



        public ActionResult TambahPekerja(string id)
        {
            
            HR_SEMINAR_LUAR_DETAIL seminardetails = new HR_SEMINAR_LUAR_DETAIL();
            seminardetails.HR_KOD_LAWATAN = id;

            List<SelectListItem> kerap = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },
            };
            ViewBag.kerap = new SelectList(kerap, "Value", "Text");

            
            return PartialView("_TambahPekerja", seminardetails);
          
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPekerja([Bind(Include = "HR_KOD_LAWATAN,HR_NO_PEKERJA,HR_KERAP_IND,HR_LAPORAN_IND")] HR_SEMINAR_LUAR_DETAIL seminardetails)
        {
            if (ModelState.IsValid)
            {
                List<HR_SEMINAR_LUAR_DETAIL> selectdetail = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_KOD_LAWATAN == seminardetails.HR_KOD_LAWATAN && s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();
                if (selectdetail.Count() <= 0)
                {
                   
                    db.HR_SEMINAR_LUAR_DETAIL.Add(seminardetails);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return PartialView("_TambahPekerja", seminardetails);
        }


        public JsonResult CariTanggungan(string HR_NO_PEKERJA)
        {
            MaklumatKakitanganModels model = new MaklumatKakitanganModels();
           // Test test = new Test();

            HR_MAKLUMAT_PERIBADI item = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();

            if (item == null )
            {
                item = new HR_MAKLUMAT_PERIBADI();
          

            }
            
            HR_MAKLUMAT_PEKERJAAN item1 = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            if (item1 == null)
            {
                item1 = new HR_MAKLUMAT_PEKERJAAN();
                
            }
            GE_JABATAN jabatan = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == item1.HR_JABATAN).SingleOrDefault();
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
          
            }
            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == item1.HR_BAHAGIAN && s.GE_KOD_JABATAN == item1.HR_JABATAN).SingleOrDefault();
            if (bahagian == null)
            {
                bahagian = new GE_BAHAGIAN();
               
            }

            model.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi(); //newobject
            model.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
            
            GE_BAHAGIAN listbahagian = new GE_BAHAGIAN();
            GE_JABATAN listjabatan = new GE_JABATAN();


            model.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
            model.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN = item1.HR_JAWATAN;
            //model.GE_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
           // model.GE_BAHAGIAN = bahagian.GE_KETERANGAN;
            model.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            model.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
           // test.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;
           // test.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = item.HR_NAMA_PEKERJA;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult EditPekerja(string id, string value)
             {
    
            if (id == null && value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR_DETAIL seminarpekerja = new HR_SEMINAR_LUAR_DETAIL();

            seminarpekerja.HR_KOD_LAWATAN = id;

            seminarpekerja.HR_NO_PEKERJA = value;
            HR_SEMINAR_LUAR_DETAIL seminardetails = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_KOD_LAWATAN == id && s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == id);

            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
            ViewBag.HR_JABATAN = mPekerjaan.HR_JABATAN;
            ViewBag.HR_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;
            
            ViewBag.seminarluar = db.HR_SEMINAR_LUAR_DETAIL.Include(s => s.HR_SEMINAR_LUAR).Where(s => s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();
            
            if (seminardetails == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> kerap = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },

            };
            ViewBag.kerap = new SelectList(kerap, "Value", "Text");



            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            return PartialView("_EditPekerja", seminardetails);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPekerja([Bind(Include = "HR_KOD_LAWATAN,HR_NO_PEKERJA,HR_KERAP_IND,HR_LAPORAN_IND")] HR_SEMINAR_LUAR_DETAIL seminardetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seminardetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HR_KOD_LAWATAN = new SelectList(db.HR_SEMINAR_LUAR, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", seminardetails.HR_KOD_LAWATAN);
            return View(seminardetails);
        }

        
        public ActionResult PadamPekerja(string id, string value)
        {

            if (id == null && value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_SEMINAR_LUAR_DETAIL seminarpekerja = new HR_SEMINAR_LUAR_DETAIL();

            seminarpekerja.HR_KOD_LAWATAN = id;
            seminarpekerja.HR_NO_PEKERJA = value;

            seminarpekerja.HR_NO_PEKERJA = value;
            HR_SEMINAR_LUAR_DETAIL seminardetails = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_KOD_LAWATAN == id && s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            HR_SEMINAR_LUAR seminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == id);

            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
            ViewBag.HR_JABATAN = mPekerjaan.HR_JABATAN;
            ViewBag.HR_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;

            ViewBag.seminarluar = db.HR_SEMINAR_LUAR_DETAIL.Include(s => s.HR_SEMINAR_LUAR).Where(s => s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA).ToList();

            if (seminardetails == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> kerap = new List<SelectListItem>
            {
                new SelectListItem { Text = "Ya", Value = "Y" },
                new SelectListItem { Text = "Tidak", Value = "T" },

            };
            ViewBag.kerap = new SelectList(kerap, "Value", "Text");
            
            ViewBag.HR_NO_PEKERJA = db.HR_MAKLUMAT_PERIBADI.ToList();
            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN, "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            return PartialView("_PadamPekerja", seminardetails);
        }



        [HttpPost, ActionName("PadamPekerja")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SEMINAR_LUAR_DETAIL seminardetails)
        {
            
            seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Include(s => s.HR_SEMINAR_LUAR).SingleOrDefault(s => s.HR_KOD_LAWATAN == seminardetails.HR_KOD_LAWATAN && s.HR_NO_PEKERJA == seminardetails.HR_NO_PEKERJA);

            db.HR_SEMINAR_LUAR_DETAIL.Remove(seminardetails);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult Seminar(string HR_KOD_LAWATAN)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext mc = new MajlisContext();
            db.Configuration.ProxyCreationEnabled = false;

            //MaklumatKakitanganModels model = new MaklumatKakitanganModels();
            // Test test = new Test();

            //HR_MAKLUMAT_PERIBADI item = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA);

            LuarNegaraModels modelluar = new LuarNegaraModels();

            HR_SEMINAR_LUAR sem = db.HR_SEMINAR_LUAR.Include(s => s.HR_SEMINAR_LUAR_DETAIL).SingleOrDefault(s => s.HR_KOD_LAWATAN == HR_KOD_LAWATAN);



            List<HR_SEMINAR_LUAR> seminar = db.HR_SEMINAR_LUAR.Where(s => s.HR_KOD_LAWATAN == HR_KOD_LAWATAN).ToList();

            return Json(seminar, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Bahagian(string HR_NO_PEKERJA)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext db2 = new MajlisContext();
            db2.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PEKERJAAN item = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == item.HR_BAHAGIAN && s.GE_KOD_JABATAN == item.HR_JABATAN).SingleOrDefault();

            return Json(bahagian, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Jabatan(string HR_NO_PEKERJA)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            MajlisContext db2 = new MajlisContext();
            db2.Configuration.ProxyCreationEnabled = false;
            HR_MAKLUMAT_PEKERJAAN item = db.HR_MAKLUMAT_PEKERJAAN.Where(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA).SingleOrDefault();
            GE_JABATAN bahagian = db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == item.HR_JABATAN).SingleOrDefault();

            return Json(bahagian, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult CariPekerja(string no,string kod)
        //{
        //    List<HR_SEMINAR_LUAR_DETAIL> luardetail = new List<HR_SEMINAR_LUAR_DETAIL>();
        //    if (no != null)
        //    {
        //        luardetail = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA  == no && s.HR_KOD_LAWATAN == kod).ToList();
        //    }

        //    string msg = null;
        //    if (luardetail.Count() > 0)
        //    {
        //        msg = "Data telah wujud";
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(msg, JsonRequestBehavior.AllowGet);
        //}


    }
}