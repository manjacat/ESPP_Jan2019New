using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using iTextSharp.text.pdf;
using System.IO;
using iTextSharp.text;
using iTextSharp.tool.xml;
using ClosedXML.Excel;
using System.Web.Script.Serialization;
using static eSPP.Controllers.AccountController;
using Microsoft.Office.Interop.Word;

namespace eSPP.Controllers
{
    public class PendahuluanDiriController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: HR_PENDAHULUAN_DIRI


        public List<HR_MAKLUMAT_PERIBADI> CariPekerja(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> sPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            if (key == "1")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_PEKERJA == value && s.HR_AKTIF_IND == "Y").ToList();

            }
            else if (key == "2")
            {
                value = value.ToUpper();
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NAMA_PEKERJA.ToUpper().Contains(value.ToUpper()) && s.HR_AKTIF_IND == "Y").ToList();
            }
            else if (key == "3")
            {
                sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).Where(s => s.HR_NO_KPBARU.Contains(value) && s.HR_AKTIF_IND == "Y").ToList();
            }

            //else if (key == "4")
            //{
            //    sPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).AsEnumerable().Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null && Convert.ToDateTime(s.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI).Month == bulan && s.HR_AKTIF_IND == "Y" && db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(p => p.HR_NO_PEKERJA == s.HR_NO_PEKERJA && p.HR_AKTIF_IND == "Y").Count() > 0).ToList();
            //}

            //if (kod != "00025")
            //{
            //    sPeribadi = sPeribadi.Where(s => s.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND == "Y").ToList();
            //}
            return sPeribadi;
        }

        public ActionResult SenaraiPendahuluanDiri(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value);

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }

        public PartialViewResult PendahuluanList(string key, string value)
        {
            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.HR_NO_PEKERJA = value;

            List<HR_PENDAHULUAN_DIRI> diri = db.HR_PENDAHULUAN_DIRI.Where(s => s.HR_NO_PEKERJA == value).ToList();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }
            HR_MAKLUMAT_PERIBADI listperibadi = new HR_MAKLUMAT_PERIBADI();

            ViewBag.noPekerja = peribadi.HR_NO_PEKERJA;

            ViewBag.detail = db.HR_PENDAHULUAN_DIRI.ToList<HR_PENDAHULUAN_DIRI>();

            return PartialView("_PendahuluanList", diri);
        }





        public ActionResult TambahPendahuluan(string id, string kod)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PENDAHULUAN_DIRI mPendahuluan = new HR_PENDAHULUAN_DIRI();

            mPendahuluan.HR_NO_PEKERJA = id;

            mPendahuluan.HR_TARIKH_PERMOHONAN = DateTime.Now;
            var tarikhpendahuluan = string.Format("{0:dd/MM/yyyy}", mPendahuluan.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpendahuluan;

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            
            if (mPeribadi == null)
            {
                return HttpNotFound();
            }
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();

            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();
            if (bahagian == null)
            {
                bahagian = new GE_BAHAGIAN();
            }
            GE_BAHAGIAN bahagianlist = new GE_BAHAGIAN();

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            
            ViewBag.HR_NO_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
            ViewBag.HR_PEGAWAI = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_IC = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JUMLAH_PENUH = mPendahuluan.HR_JUMLAH_PENUH;
            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;

            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return View("TambahPendahuluan", mPendahuluan);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPendahuluan(HR_PENDAHULUAN_DIRI pendahuluan)
        {

            if (ModelState.IsValid)
            {
                HR_PENDAHULUAN_DIRI semakPendahuluan = db.HR_PENDAHULUAN_DIRI.OrderByDescending(s => s.HR_KOD_PENDAHULUAN).FirstOrDefault();
               
                if (semakPendahuluan == null)
                {
                    semakPendahuluan = new HR_PENDAHULUAN_DIRI();
                }

                int LastID2 = 0;
                if (semakPendahuluan.HR_KOD_PENDAHULUAN != null)
                {
                    var LastID = new string(semakPendahuluan.HR_KOD_PENDAHULUAN.SkipWhile(x => x == 'D' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(LastID);
                }

                var Increment = LastID2 + 1;
                var KodPendahuluan = Convert.ToString(Increment).PadLeft(4, '0');
                pendahuluan.HR_KOD_PENDAHULUAN = "DD" + KodPendahuluan;
                db.HR_PENDAHULUAN_DIRI.Add(pendahuluan);
                db.SaveChanges();

                return RedirectToAction("senaraipendahuluandiri");

            }
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pendahuluan.HR_NO_PEKERJA);

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();

            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();
            if (bahagian == null)
            {
                bahagian = new GE_BAHAGIAN();
            }
            GE_BAHAGIAN bahagianlist = new GE_BAHAGIAN();

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();


            ViewBag.HR_NO_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
            ViewBag.HR_PEGAWAI = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_IC = mPeribadi.HR_NO_KPBARU;
           
            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;

            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", pendahuluan.HR_NAMA_PEGAWAI);
            ViewBag.HR_JAWATAN_NP = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return View(pendahuluan);
        }

        public ActionResult PendahuluanInfo(string id, string kod, string jenis)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
       
            HR_PENDAHULUAN_DIRI mPendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PENDAHULUAN == kod);
          

            if (mPendahuluan == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NO_PEKERJA);

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();

            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();
            if (bahagian == null)
            {
                bahagian = new GE_BAHAGIAN();
            }
            GE_BAHAGIAN bahagianlist = new GE_BAHAGIAN();

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

           
            ViewBag.HR_NO_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
            ViewBag.HR_PEGAWAI = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_IC = mPeribadi.HR_NO_KPBARU;

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;

        

            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_PEGAWAI);
            ViewBag.HR_JAWATAN_NP = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return View("PendahuluanList" + jenis, mPendahuluan);
        }

     


        public ActionResult PendahuluanListInfo(string id, string kod)
        {
            return PendahuluanInfo(id, kod, "Info");
        }

        public ActionResult PendahuluanListEdit(string id, string kod)
        {
            return PendahuluanInfo(id, kod, "Edit");
        }

        public ActionResult PendahuluanListPadam(string id, string kod)
        {
            return PendahuluanInfo(id, kod, "Padam");
        }



        [HttpPost, ActionName("PendahuluanListPadam")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_PENDAHULUAN_DIRI mPendahuluan, string key, string value, string jenis)
        {

            db.HR_PENDAHULUAN_DIRI.RemoveRange(db.HR_PENDAHULUAN_DIRI.Where(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NO_PEKERJA && s.HR_KOD_PENDAHULUAN == mPendahuluan.HR_KOD_PENDAHULUAN));
            db.SaveChanges();
            return RedirectToAction("SenaraiPendahuluanDiri");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PendahuluanListEdit(HR_PENDAHULUAN_DIRI mPendahuluan)
        {

            if (ModelState.IsValid)
            {
                db.Entry(mPendahuluan).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("senaraipendahuluandiri");
            }
           
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NO_PEKERJA);

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();

            GE_BAHAGIAN bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();
            if (bahagian == null)
            {
                bahagian = new GE_BAHAGIAN();
            }
            GE_BAHAGIAN bahagianlist = new GE_BAHAGIAN();

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();


            ViewBag.HR_NO_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI;
            ViewBag.HR_PEGAWAI = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_IC = mPeribadi.HR_NO_KPBARU;

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;

            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_PEGAWAI);
            ViewBag.HR_JAWATAN_NP = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return View(mPendahuluan);
        }

        public Microsoft.Office.Interop.Word.Document wordDocument { get; set; }

        public ActionResult PrintBorang(string id, string kod)
        {
            string path_file = Server.MapPath(Url.Content("~/Content/template/"));
            var pendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PENDAHULUAN == kod);

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            var jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);


            var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
            templateEngine.Process(
                source: path_file + "TEMPLATE PENDAHULUAN DIRI.docx",
                destination: path_file + "BORANG_PENDAHULUAN_DIRI.docx",
                 data: new
                 {

                     tarikhpermohonan = string.Format("{0:dd/MM/yyyy}", pendahuluan.HR_TARIKH_PERMOHONAN),
                     jabatan = jabatan.GE_KETERANGAN_JABATAN,
                     kelulusan = pendahuluan.HR_KELULUSAN,
                     jumlahsekarang = pendahuluan.HR_JUMLAH_SEKARANG,
                     jumlahpenuh = pendahuluan.HR_JUMLAH_PENUH,
                     no_kp = mPeribadi.HR_NO_KPBARU,
                     no_gaji = mPekerjaan.HR_MATRIKS_GAJI,
                     jawatan = jawatan.HR_KOD_JAWATAN + " - " + jawatan.HR_NAMA_JAWATAN,
                    
                     bayarbalik = pendahuluan.HR_BAYARAN_BALIK,
                     tujuan = pendahuluan.HR_TUJUAN,
                     mula = string.Format("{0:dd/MM/yyyy}", pendahuluan.HR_TARIKH_MULA),
                     akhir = string.Format("{0:dd/MM/yyyy}", pendahuluan.HR_TARIKH_AKHIR),

                     waktu = pendahuluan.HR_TARIKH_MULA,
                     soalan = pendahuluan.HR_KELULUSAN_1,
                     senarai = pendahuluan.HR_KELULUSAN_2,
                     belanja = pendahuluan.HR_KELULUSAN_3,
                     kadar = pendahuluan.HR_KELULUSAN_4,

                     pegawai2 = pendahuluan.HR_PEGAWAI_2,
                     pegawai3 = pendahuluan.HR_PEGAWAI_3,
                     sebab = pendahuluan.HR_PEGAWAI_4,
                     amanah = mPeribadi.HR_NAMA_PEKERJA,

                     minta = pendahuluan.HR_NAMA_PEGAWAI,
                     pegawai = mPeribadi.HR_NAMA_PEKERJA,
                     tangung = pendahuluan.HR_NAMA_PEGAWAI,
                     amaun = pendahuluan.HR_JUMLAH_PENUH,
                     nopekerja = mPeribadi.HR_NO_PEKERJA,


                 });

            string path = path_file + "BORANG_PENDAHULUAN_DIRI.docx";
            System.IO.FileInfo file = new System.IO.FileInfo(path);

            Response.Clear();
            Response.AddHeader("content-length", file.Length.ToString());
            Response.AddHeader("content-disposition", "attachment; filename = BORANG_PENDAHULUAN_DIRI(" + mPeribadi.HR_NAMA_PEKERJA + ").docx");
            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.TransmitFile(path_file + "BORANG_PENDAHULUAN_DIRI.docx");
            Response.Flush();
            Response.Close();

            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", pendahuluan.HR_NAMA_PEGAWAI);
            ViewBag.HR_JAWATAN_NP = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);


            return View();
        }

        public JsonResult CariJawatanPegawai(string id)
        {
            HR_MAKLUMAT_PEKERJAAN item = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            if (item == null)
            {
                item = new HR_MAKLUMAT_PEKERJAAN();
            }
            HR_JAWATAN jawatan = db.HR_JAWATAN.Find(item.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }

            return Json(jawatan.HR_KOD_JAWATAN, JsonRequestBehavior.AllowGet);

        }

    }
}
