using eSPP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class TuntutanInsuranController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        public ActionResult Index(string key, string value)
        {
            ViewBag.key = "";
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

            List<HR_TUNTUTAN_INSURAN> model = new List<HR_TUNTUTAN_INSURAN>();
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
            ViewBag.HR_SENARAI_PERIBADI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GAMBAR_PENGGUNA = new SelectList(db.HR_GAMBAR_PENGGUNA, "HR_NO_PEKERJA", "HR_PHOTO");

            if (key == "4")
            {
                ViewBag.key = key;
                model = db.HR_TUNTUTAN_INSURAN.Where(s => s.HR_NO_PEKERJA == value).ToList();

            }
            return View(model.ToList());
        }


        public ActionResult InfoTuntutan(string id, string tarikh)
        {
            if (id == null || tarikh == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime Date = Convert.ToDateTime(tarikh);
            HR_TUNTUTAN_INSURAN tuntutan = db.HR_TUNTUTAN_INSURAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_TARIKH_TUNTUTAN == Date);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_TANGGUNGAN mTanggung = db.HR_MAKLUMAT_TANGGUNGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_NO_KP == tuntutan.HR_NO_KP_PESAKIT);
            if(mTanggung == null)
            {
                mTanggung = new HR_MAKLUMAT_TANGGUNGAN();
            }
            

            ViewBag.value = "";
            if (tuntutan.HR_NAMA_PENYAKIT != null || tuntutan.HR_TARIKH_SIMPTOM != null || tuntutan.HR_TARIKH_NASIHAT != null)
            {
                ViewBag.value = "1";
            }
            else if (tuntutan.HR_KECEDERAAN != null || tuntutan.HR_TARIKH_MASA_CEDERA != null || tuntutan.HR_KERJA_IND != null)
            {
                ViewBag.value = "2";
            }

            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;

            var tarikhtanggung = string.Format("{0:dd/MM/yyyy}", mTanggung.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR = tarikhtanggung;
           
            ViewBag.HR_JANTINA = mTanggung.HR_JANTINA;

            var tarikhMasuk = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_MASUK);
            ViewBag.HR_TARIKH_MASUK = tarikhMasuk;
            var tarikhLahir = string.Format("{0:dd/MM/yyyy}", mPeribadi.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR_PERIBADI = tarikhLahir;
            ViewBag.HR_JANTINA_PERIBADI = mPeribadi.HR_JANTINA;
            ViewBag.NAMA_ORGANISASI = "MAJLIS BANDARAYA PETALING JAYA";

            List<SelectListItem> pelan = new List<SelectListItem>
            {
             new SelectListItem { Text = "Sendiri", Value = "F" },
             new SelectListItem { Text = "Suami/Isteri", Value = "S" },
             new SelectListItem { Text = "Anak", Value = "A" },
           
             };
             ViewBag.pelan = new SelectList(pelan, "Value", "Text");
        
            List<SelectListItem> kodAgensi = new List<SelectListItem>();
            kodAgensi.Add(new SelectListItem { Text = "Johor", Value = "1 " });
            kodAgensi.Add(new SelectListItem { Text = "Kedah", Value = "2 " });
            kodAgensi.Add(new SelectListItem { Text = "Kelantan", Value = "3 " });
            kodAgensi.Add(new SelectListItem { Text = "Melaka", Value = "4 " });
            kodAgensi.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "5 " });
            kodAgensi.Add(new SelectListItem { Text = "Pahang", Value = "6 " });
            kodAgensi.Add(new SelectListItem { Text = "Pulau Pinang", Value = "7 " });
            kodAgensi.Add(new SelectListItem { Text = "Perak", Value = "8 " });
            kodAgensi.Add(new SelectListItem { Text = "Perlis", Value = "9 " });
            kodAgensi.Add(new SelectListItem { Text = "Selangor", Value = "10 " });
            kodAgensi.Add(new SelectListItem { Text = "Terengganu", Value = "11 " });
            kodAgensi.Add(new SelectListItem { Text = "Sabah", Value = "12 " });
            kodAgensi.Add(new SelectListItem { Text = "Sarawak", Value = "13 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Kuala Lumpur", Value = "14 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Labuan", Value = "15 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Putrajaya", Value = "16 " });
            ViewBag.kodAgensi = kodAgensi;

            List<SelectListItem> kodRawatan = new List<SelectListItem>();
            kodRawatan.Add(new SelectListItem { Text = "Hospital Besar Kuala Lumpur", Value = "HKL" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Selayang", Value = "HS" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Serdang", Value = "HSD" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti Kebangsaan Malaysia", Value = "HKM" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Putrajaya", Value = "HP" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti", Value = "HU" });
            ViewBag.kodRawatan = kodRawatan;

            if (tuntutan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_NAMA_TANGGUNGAN = new SelectList(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == id), "HR_NO_KP", "HR_NAMA_TANGGUNGAN");

            ViewBag.NAMA_ORGANISASI = "MAJLIS BANDARAYA PETALING JAYA";
          
            return PartialView("_InfoTuntutan", tuntutan);
        }


        public ActionResult TambahTuntutan(string id, string Jenis)
        {
            if (id == null || Jenis == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_TUNTUTAN_INSURAN tuntutan = new HR_TUNTUTAN_INSURAN();

            tuntutan.HR_NO_PEKERJA = id;

            tuntutan.HR_JENIS_TUNTUTAN = Jenis;
            tuntutan.HR_TARIKH_TUNTUTAN = DateTime.Now;

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;

            var tarikhMasuk = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_MASUK);
            ViewBag.HR_TARIKH_MASUK = tarikhMasuk;
            var tarikhLahir = string.Format("{0:dd/MM/yyyy}", mPeribadi.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR_PERIBADI = tarikhLahir;
            var tarikhtuntutan = string.Format("{0:dd/MM/yyyy}", tuntutan.HR_TARIKH_TUNTUTAN);
            ViewBag.HR_TARIKH_TUNTUTAN = tarikhtuntutan;
            ViewBag.HR_JANTINA_PERIBADI = mPeribadi.HR_JANTINA;

            List<SelectListItem> pelan = new List<SelectListItem>
            {
             new SelectListItem { Text = "Sendiri", Value = "F" },
             new SelectListItem { Text = "Suami/Isteri", Value = "S" },
             new SelectListItem { Text = "Anak", Value = "A" },

             };
            ViewBag.pelan = new SelectList(pelan, "Value", "Text");

            List<SelectListItem> kodAgensi = new List<SelectListItem>();
            kodAgensi.Add(new SelectListItem { Text = "Johor", Value = "1 " });
            kodAgensi.Add(new SelectListItem { Text = "Kedah", Value = "2 " });
            kodAgensi.Add(new SelectListItem { Text = "Kelantan", Value = "3 " });
            kodAgensi.Add(new SelectListItem { Text = "Melaka", Value = "4 " });
            kodAgensi.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "5 " });
            kodAgensi.Add(new SelectListItem { Text = "Pahang", Value = "6 " });
            kodAgensi.Add(new SelectListItem { Text = "Pulau Pinang", Value = "7 " });
            kodAgensi.Add(new SelectListItem { Text = "Perak", Value = "8 " });
            kodAgensi.Add(new SelectListItem { Text = "Perlis", Value = "9 " });
            kodAgensi.Add(new SelectListItem { Text = "Selangor", Value = "10 " });
            kodAgensi.Add(new SelectListItem { Text = "Terengganu", Value = "11 " });
            kodAgensi.Add(new SelectListItem { Text = "Sabah", Value = "12 " });
            kodAgensi.Add(new SelectListItem { Text = "Sarawak", Value = "13 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Kuala Lumpur", Value = "14 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Labuan", Value = "15 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Putrajaya", Value = "16 " });
            ViewBag.kodAgensi = kodAgensi;

            List<SelectListItem> kodRawatan = new List<SelectListItem>();
            kodRawatan.Add(new SelectListItem { Text = "Hospital Besar Kuala Lumpur", Value = "HKL" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Selayang", Value = "HS" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Serdang", Value = "HSD" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti Kebangsaan Malaysia", Value = "HKM" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Putrajaya", Value = "HP" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti", Value = "HU" });
            ViewBag.kodRawatan = kodRawatan;

            ViewBag.HR_NAMA_TANGGUNGAN = new SelectList(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == id), "HR_NO_KP", "HR_NAMA_TANGGUNGAN");
            
            ViewBag.NAMA_ORGANISASI = "MAJLIS BANDARAYA PETALING JAYA";

            return PartialView("_TambahTuntutan", tuntutan);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahTuntutan([Bind(Include = "HR_NO_PEKERJA,HR_TARIKH_TUNTUTAN,HR_NO_POLISI,HR_PELAN_KATEGORI,HR_JENIS_TUNTUTAN,HR_NAMA_PENYAKIT,HR_TARIKH_SIMPTOM,HR_TARIKH_NASIHAT,HR_KECEDERAAN,HR_TARIKH_MASA_CEDERA,HR_KERJA_IND,HR_RAWATAN_IND,HR_KAITAN_RAWATAN_IND,HR_NAMA_DOKTOR,HR_TARIKH_RAWATAN,HR_KOD_AGENSI_DOKTOR,HR_KOD_AGENSI_RAWATAN,HR_TARIKH_MASUK_HOSP,HR_TARIKH_BEDAH,HR_TARIKH_KELUAR_HOSP,HR_CLAIM_IND,HR_SYKT_INSURANS,HR_NO_POLISI_LAIN,HR_TARIKH_AKHIR_TUGAS,HR_TARIKH_KEMATIAN,HR_PUNCA_KEMATIAN,HR_PESAKIT_IND,HR_NO_KP_PESAKIT,HR_MASA_CEDERA,HR_TARIKH_PEMBAYARAN,HR_JUMLAH_TUNTUTAN,HR_BAYAR_IND")] HR_TUNTUTAN_INSURAN tuntutan)
        {
            if (ModelState.IsValid)
            {
                List<HR_TUNTUTAN_INSURAN> selectTuntutan = db.HR_TUNTUTAN_INSURAN.Where(s => s.HR_NO_PEKERJA == tuntutan.HR_NO_PEKERJA && s.HR_JENIS_TUNTUTAN == "K").ToList();
                if (selectTuntutan.Count() <= 0)
                {
                    db.HR_TUNTUTAN_INSURAN.Add(tuntutan);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return PartialView("_TambahTuntutan", tuntutan);
        }

        
        public ActionResult EditTuntutan(string id, string tarikh)
        {
            if (id == null || tarikh == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DateTime Date = Convert.ToDateTime(tarikh);
            HR_TUNTUTAN_INSURAN tuntutan = db.HR_TUNTUTAN_INSURAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_TARIKH_TUNTUTAN == Date);

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_TANGGUNGAN mTanggung = db.HR_MAKLUMAT_TANGGUNGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_NO_KP == tuntutan.HR_NO_KP_PESAKIT);

            if (mTanggung == null)
            {
                mTanggung = new HR_MAKLUMAT_TANGGUNGAN();
            }

            ViewBag.value = "";
            if (tuntutan.HR_NAMA_PENYAKIT != null || tuntutan.HR_TARIKH_SIMPTOM != null || tuntutan.HR_TARIKH_NASIHAT != null)
            {
                ViewBag.value = "1";
            }
            else if (tuntutan.HR_KECEDERAAN != null || tuntutan.HR_TARIKH_MASA_CEDERA != null || tuntutan.HR_KERJA_IND != null)
            {
                ViewBag.value = "2";
            }

           
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;

           
         

            var tarikhtanggung = string.Format("{0:dd/MM/yyyy}", mTanggung.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR = tarikhtanggung;

            ViewBag.HR_JANTINA = mTanggung.HR_JANTINA;



            var tarikhMasuk = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_MASUK);
            ViewBag.HR_TARIKH_MASUK = tarikhMasuk;
            var tarikhLahir = string.Format("{0:dd/MM/yyyy}", mPeribadi.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR_PERIBADI = tarikhLahir;
            ViewBag.HR_JANTINA_PERIBADI = mPeribadi.HR_JANTINA;

            List<SelectListItem> pelan = new List<SelectListItem>
            {
             new SelectListItem { Text = "Sendiri", Value = "F" },
             new SelectListItem { Text = "Suami/Isteri", Value = "S" },
             new SelectListItem { Text = "Anak", Value = "A" },

             };
            ViewBag.pelan = new SelectList(pelan, "Value", "Text");


            List<SelectListItem> kodAgensi = new List<SelectListItem>();
            kodAgensi.Add(new SelectListItem { Text = "Johor", Value = "1 " });
            kodAgensi.Add(new SelectListItem { Text = "Kedah", Value = "2 " });
            kodAgensi.Add(new SelectListItem { Text = "Kelantan", Value = "3 " });
            kodAgensi.Add(new SelectListItem { Text = "Melaka", Value = "4 " });
            kodAgensi.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "5 " });
            kodAgensi.Add(new SelectListItem { Text = "Pahang", Value = "6 " });
            kodAgensi.Add(new SelectListItem { Text = "Pulau Pinang", Value = "7 " });
            kodAgensi.Add(new SelectListItem { Text = "Perak", Value = "8 " });
            kodAgensi.Add(new SelectListItem { Text = "Perlis", Value = "9 " });
            kodAgensi.Add(new SelectListItem { Text = "Selangor", Value = "10 " });
            kodAgensi.Add(new SelectListItem { Text = "Terengganu", Value = "11 " });
            kodAgensi.Add(new SelectListItem { Text = "Sabah", Value = "12 " });
            kodAgensi.Add(new SelectListItem { Text = "Sarawak", Value = "13 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Kuala Lumpur", Value = "14 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Labuan", Value = "15 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Putrajaya", Value = "16 " });
            ViewBag.kodAgensi = kodAgensi;

            List<SelectListItem> kodRawatan = new List<SelectListItem>();
            kodRawatan.Add(new SelectListItem { Text = "Hospital Besar Kuala Lumpur", Value = "HKL" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Selayang", Value = "HS" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Serdang", Value = "HSD" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti Kebangsaan Malaysia", Value = "HKM" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Putrajaya", Value = "HP" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti", Value = "HU" });
            ViewBag.kodRawatan = kodRawatan;

            if (tuntutan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_NAMA_TANGGUNGAN = new SelectList(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == id), "HR_NO_KP", "HR_NAMA_TANGGUNGAN");
           
            ViewBag.NAMA_ORGANISASI = "MAJLIS BANDARAYA PETALING JAYA";
            return PartialView("_EditTuntutan", tuntutan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTuntutan([Bind(Include = "HR_NO_PEKERJA,HR_TARIKH_TUNTUTAN,HR_NO_POLISI,HR_PELAN_KATEGORI,HR_JENIS_TUNTUTAN,HR_NAMA_PENYAKIT,HR_TARIKH_SIMPTOM,HR_TARIKH_NASIHAT,HR_KECEDERAAN,HR_TARIKH_MASA_CEDERA,HR_KERJA_IND,HR_RAWATAN_IND,HR_KAITAN_RAWATAN_IND,HR_NAMA_DOKTOR,HR_TARIKH_RAWATAN,HR_KOD_AGENSI_DOKTOR,HR_KOD_AGENSI_RAWATAN,HR_TARIKH_MASUK_HOSP,HR_TARIKH_BEDAH,HR_TARIKH_KELUAR_HOSP,HR_CLAIM_IND,HR_SYKT_INSURANS,HR_NO_POLISI_LAIN,HR_TARIKH_AKHIR_TUGAS,HR_TARIKH_KEMATIAN,HR_PUNCA_KEMATIAN,HR_PESAKIT_IND,HR_NO_KP_PESAKIT,HR_MASA_CEDERA,HR_TARIKH_PEMBAYARAN,HR_JUMLAH_TUNTUTAN,HR_BAYAR_IND")] HR_TUNTUTAN_INSURAN tuntutan)

        {
            if (ModelState.IsValid)
            {
                db.Entry(tuntutan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tuntutan);
        }


        public ActionResult PadamTuntutan(string id, string tarikh)
        {
            if (id == null || tarikh == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DateTime Date = Convert.ToDateTime(tarikh);
            HR_TUNTUTAN_INSURAN tuntutan = db.HR_TUNTUTAN_INSURAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_TARIKH_TUNTUTAN == Date);

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_TANGGUNGAN mTanggung = db.HR_MAKLUMAT_TANGGUNGAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_NO_KP == tuntutan.HR_NO_KP_PESAKIT);
            if (mTanggung == null)
            {
                mTanggung = new HR_MAKLUMAT_TANGGUNGAN();
            }


            ViewBag.value = "";
            if (tuntutan.HR_NAMA_PENYAKIT != null || tuntutan.HR_TARIKH_SIMPTOM != null || tuntutan.HR_TARIKH_NASIHAT != null)
            {
                ViewBag.value = "1";
            }
            else if (tuntutan.HR_KECEDERAAN != null || tuntutan.HR_TARIKH_MASA_CEDERA != null || tuntutan.HR_KERJA_IND != null)
            {
                ViewBag.value = "2";
            }


            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;

          

            var tarikhtanggung = string.Format("{0:dd/MM/yyyy}", mTanggung.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR = tarikhtanggung;

            ViewBag.HR_JANTINA = mTanggung.HR_JANTINA;


            var tarikhMasuk = string.Format("{0:dd/MM/yyyy}", mPekerjaan.HR_TARIKH_MASUK);
            ViewBag.HR_TARIKH_MASUK = tarikhMasuk;
            var tarikhLahir = string.Format("{0:dd/MM/yyyy}", mPeribadi.HR_TARIKH_LAHIR);
            ViewBag.HR_TARIKH_LAHIR_PERIBADI = tarikhLahir;

            var tarikhtuntutan = string.Format("{0:dd/MM/yyyy}", tuntutan.HR_TARIKH_TUNTUTAN);
            ViewBag.HR_TARIKH_TUNTUTAN = tarikhtuntutan;
            ViewBag.HR_JANTINA_PERIBADI = mPeribadi.HR_JANTINA;


            List<SelectListItem> pelan = new List<SelectListItem>
            {
             new SelectListItem { Text = "Sendiri", Value = "F" },
             new SelectListItem { Text = "Suami/Isteri", Value = "S" },
             new SelectListItem { Text = "Anak", Value = "A" },

             };
            ViewBag.pelan = new SelectList(pelan, "Value", "Text");


            List<SelectListItem> kodAgensi = new List<SelectListItem>();
            kodAgensi.Add(new SelectListItem { Text = "Johor", Value = "1 " });
            kodAgensi.Add(new SelectListItem { Text = "Kedah", Value = "2 " });
            kodAgensi.Add(new SelectListItem { Text = "Kelantan", Value = "3 " });
            kodAgensi.Add(new SelectListItem { Text = "Melaka", Value = "4 " });
            kodAgensi.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "5 " });
            kodAgensi.Add(new SelectListItem { Text = "Pahang", Value = "6 " });
            kodAgensi.Add(new SelectListItem { Text = "Pulau Pinang", Value = "7 " });
            kodAgensi.Add(new SelectListItem { Text = "Perak", Value = "8 " });
            kodAgensi.Add(new SelectListItem { Text = "Perlis", Value = "9 " });
            kodAgensi.Add(new SelectListItem { Text = "Selangor", Value = "10 " });
            kodAgensi.Add(new SelectListItem { Text = "Terengganu", Value = "11 " });
            kodAgensi.Add(new SelectListItem { Text = "Sabah", Value = "12 " });
            kodAgensi.Add(new SelectListItem { Text = "Sarawak", Value = "13 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Kuala Lumpur", Value = "14 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Labuan", Value = "15 " });
            kodAgensi.Add(new SelectListItem { Text = "W. P. Putrajaya", Value = "16 " });
            ViewBag.kodAgensi = kodAgensi;

            List<SelectListItem> kodRawatan = new List<SelectListItem>();
            kodRawatan.Add(new SelectListItem { Text = "Hospital Besar Kuala Lumpur", Value = "HKL" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Selayang", Value = "HS" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Serdang", Value = "HSD" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti Kebangsaan Malaysia", Value = "HKM" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Putrajaya", Value = "HP" });
            kodRawatan.Add(new SelectListItem { Text = "Hospital Universiti", Value = "HU" });
            ViewBag.kodRawatan = kodRawatan;

            if (tuntutan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_NAMA_TANGGUNGAN = new SelectList(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == id), "HR_NO_KP", "HR_NAMA_TANGGUNGAN");
           
            ViewBag.NAMA_ORGANISASI = "MAJLIS BANDARAYA PETALING JAYA";
            return PartialView("_PadamTuntutan", tuntutan);
        }



        [HttpPost, ActionName("PadamTuntutan")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_TUNTUTAN_INSURAN tuntutan)
        {

            tuntutan = db.HR_TUNTUTAN_INSURAN.SingleOrDefault(s => s.HR_NO_PEKERJA == tuntutan.HR_NO_PEKERJA && s.HR_TARIKH_TUNTUTAN == tuntutan.HR_TARIKH_TUNTUTAN);
            db.HR_TUNTUTAN_INSURAN.Remove(tuntutan);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public JsonResult CariTanggungan(string id, string HR_NO_PEKERJA)
        {
            HR_MAKLUMAT_TANGGUNGAN item = db.HR_MAKLUMAT_TANGGUNGAN.SingleOrDefault(s => s.HR_NO_KP == id && s.HR_NO_PEKERJA == HR_NO_PEKERJA);
            if (item == null)
            {
                item = new HR_MAKLUMAT_TANGGUNGAN();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CariTuntutan(string no,string kod)
        {
            List<HR_TUNTUTAN_INSURAN> tuntutaninsuran = new List<HR_TUNTUTAN_INSURAN>();
            if (no != null)
            {
                tuntutaninsuran = db.HR_TUNTUTAN_INSURAN.Where(s => s.HR_NO_POLISI == no && s.HR_NO_PEKERJA == kod).ToList();
            }

            string msg = null;
            if (tuntutaninsuran.Count() > 0)
            {
                msg = "Data telah wujud";
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


    }
}