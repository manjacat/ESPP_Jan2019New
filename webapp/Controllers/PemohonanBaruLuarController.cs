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
    public class PemohonanBaruLuarController : Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        // GET: PemohonanBaruLuar

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InfoBorang(string id, string kod, string key, string value)
        {
            HR_SEMINAR_LUAR_DETAIL luardetail = new HR_SEMINAR_LUAR_DETAIL();
            HR_SEMINAR_LUAR seminar = new HR_SEMINAR_LUAR();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            luardetail.HR_NO_PEKERJA = id;
            luardetail.HR_KOD_LAWATAN = kod;
            luardetail.HR_SEMINAR_LUAR = new HR_SEMINAR_LUAR();
            //luardetail.HR_SEMINAR_LUAR.HR_TARIKH_PERMOHONAN = DateTime.Now;

            HR_SEMINAR_LUAR_DETAIL mDetail = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_LAWATAN == kod);
            HR_SEMINAR_LUAR mSeminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == kod);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();

            }
            GE_JABATAN jabatanlist = new GE_JABATAN();
            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_NAMA_SEMINAR = mSeminar.HR_NAMA_SEMINAR;
            ViewBag.HR_TARIKH_MULA = mSeminar.HR_TARIKH_MULA;
            ViewBag.HR_TARIKH_TAMAT = mSeminar.HR_TARIKH_TAMAT;
            ViewBag.HR_TEMPAT = mSeminar.HR_TEMPAT;
            ViewBag.HR_TUJUAN = mSeminar.HR_TUJUAN;
            var tarikhpemohonan = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_PERMOHONAN );
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpemohonan;
            var tarikhmula = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_MULA);
            ViewBag.HR_TARIKH_MULA = tarikhmula;
            var tarikhtamat = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_TAMAT);
            ViewBag.HR_TARIKH_TAMAT = tarikhtamat;
            var tarikhcuti = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI);
            ViewBag.HR_TARIKH_CUTI = tarikhcuti;
            var tarikhcutiakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI_AKH);
            ViewBag.HR_TARIKH_CUTI_AKH = tarikhcutiakhir;
            var tarikhkembali = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_KEMBALI);
            ViewBag.HR_TARIKH_KEMBALI = tarikhkembali;
            var tarikhmangkumula = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_MULA);
            ViewBag.HR_TARIKH_MANGKU_MULA = tarikhmangkumula;
            var tarikhmangkuakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_AKHIR);
            ViewBag.HR_TARIKH_MANGKU_AKHIR = tarikhmangkuakhir;
            
            HR_SEMINAR_LUAR h = new HR_SEMINAR_LUAR();
            h.HR_KOD_LAWATAN = "T01";
            h.HR_NAMA_SEMINAR = "TAMBAH BARU SEMINAR";

            List<HR_SEMINAR_LUAR> sem = new List<HR_SEMINAR_LUAR>();
            sem = db.HR_SEMINAR_LUAR.ToList();
            sem.Add(h);

            string[] seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA == id).Select(s => s.HR_KOD_LAWATAN).ToArray();
            
            ViewBag.HR_KOD_LAWATAN = new SelectList(sem, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", null, null, seminardetails);
            ViewBag.key = key;
            ViewBag.value = value;
            return View("InfoBorang", mDetail);
        }

        
        public ActionResult TambahBorang(ManageMessageId? message, string id, string key, string value)
        {
            ViewBag.StatusMessage =
               message == ManageMessageId.Tambah ? "Permohonan Telah Berjaya Dihantar."
               : "";
            
            HR_SEMINAR_LUAR_DETAIL luardetail = new HR_SEMINAR_LUAR_DETAIL();
            HR_SEMINAR_LUAR seminar = new HR_SEMINAR_LUAR();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
      

            luardetail.HR_NO_PEKERJA = id;
       
            luardetail.HR_SEMINAR_LUAR = new HR_SEMINAR_LUAR();
            luardetail.HR_SEMINAR_LUAR.HR_TARIKH_PERMOHONAN = DateTime.Now;
          
          
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            
          
    
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();

            }
            GE_JABATAN jabatanlist  = new GE_JABATAN();
            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();
         
            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;

            HR_SEMINAR_LUAR h = new HR_SEMINAR_LUAR();
            h.HR_KOD_LAWATAN = "T01";
            h.HR_NAMA_SEMINAR = "TAMBAH BARU SEMINAR";

            List<HR_SEMINAR_LUAR> sem = new List<HR_SEMINAR_LUAR>();
            sem = db.HR_SEMINAR_LUAR.ToList();
            sem.Add(h);

            string[] seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Where(s=> s.HR_NO_PEKERJA == id).Select(s => s.HR_KOD_LAWATAN).ToArray();
            
            ViewBag.HR_KOD_LAWATAN = new SelectList(sem, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", null, null, seminardetails);
            ViewBag.key = key;
            ViewBag.value = value;
            return View("TambahBorang", luardetail);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahBorang([Bind(Include = "HR_KOD_LAWATAN,HR_NO_PEKERJA, HR_KERAP_IND, HR_LAPORAN_IND, HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI,HR_TARIKHMULA_MANGKU,HR_TARIKHAKHIR_MANGKU,HR_TIKET_KAPAL,HR_PENGINAPAN,HR_LAIN,HR_JUMLAH_BELANJA,HR_NAMA_PEGAWAI,HR_HUBUNGAN,HR_ALAMAT_PEGAWAI,HR_NOTEL_PEGAWAI,HR_EMAIL_PEGAWAI,HR_ALASAN")]HR_SEMINAR_LUAR_DETAIL luardetail, [Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND")] HR_SEMINAR_LUAR luar)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);
            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();
            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            
            HR_SEMINAR_LUAR h = new HR_SEMINAR_LUAR();
            h.HR_KOD_LAWATAN = "T01";
            h.HR_NAMA_SEMINAR = "TAMBAH BARU SEMINAR";
            
            List<HR_SEMINAR_LUAR> sem = new List<HR_SEMINAR_LUAR>();
            sem = db.HR_SEMINAR_LUAR.ToList();
            sem.Add(h);

            string[] seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA).Select(s => s.HR_KOD_LAWATAN).ToArray();
            
            ViewBag.HR_KOD_LAWATAN = new SelectList(sem, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", null, null, seminardetails);
            if (ModelState.IsValid)
            {
                if (luar.HR_KOD_LAWATAN == "T01")
                {
                    var SelectLastID = db.HR_SEMINAR_LUAR.OrderByDescending(s => s.HR_KOD_LAWATAN).FirstOrDefault().HR_KOD_LAWATAN;
                    var LastID = new string(SelectLastID.SkipWhile(x => x == '0').ToArray());
                    var Increment = Convert.ToSingle(LastID) + 1;
                    var KodLawatan = Convert.ToString(Increment).PadLeft(5,'0');
                    luardetail.HR_KOD_LAWATAN = KodLawatan;
                    luar.HR_KOD_LAWATAN = KodLawatan;
                    db.HR_SEMINAR_LUAR.Add(luar);
                }
                    db.HR_SEMINAR_LUAR_DETAIL.Add(luardetail);
                    db.SaveChanges();
                
                return View("TambahBorang", luardetail);
            }
            return View("TambahBorang", "PemohonanBaruLuar", new { Message = ManageMessageId.Tambah});
        }
        
        public JsonResult CariLuar( string id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            HR_SEMINAR_LUAR item  = db.HR_SEMINAR_LUAR.SingleOrDefault(s =>  s.HR_KOD_LAWATAN == id);
            if (item == null)
            {
                item = new HR_SEMINAR_LUAR();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CariPemohon(string key, string value)
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

        
        public Microsoft.Office.Interop.Word.Document wordDocument { get; set; }

        public ActionResult PermohonanPerjalanan(string id, string kod)
        {
            string path_file = Server.MapPath(Url.Content("~/Content/template/"));
            var pegawai = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_LAWATAN == kod);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            var jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);

          
            var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
            templateEngine.Process(
                source: path_file + "TEMPLATE BORANG LUAR NEGARA.docx",
                destination: path_file + "BORANG_LUAR_NEGARA.docx",
                 data: new
                 {
                     nama = mPeribadi.HR_NAMA_PEKERJA,
                     ic = mPeribadi.HR_NO_KPBARU,
                     jawatan = jawatan.HR_KOD_JAWATAN + " - " + jawatan.HR_NAMA_JAWATAN,
                     mula = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_MANGKU_MULA),
                     hingga = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_MANGKU_AKHIR),
                     jabatan = jabatan.GE_KETERANGAN_JABATAN,
                     tarikhlawatan = string.Format("{0:dd/MM/yyyy}", pegawai.HR_SEMINAR_LUAR.HR_TARIKH_MULA),
                     hinggalawatan = string.Format("{0:dd/MM/yyyy}", pegawai.HR_SEMINAR_LUAR.HR_TARIKH_TAMAT),
                     negara = pegawai.HR_SEMINAR_LUAR.HR_TEMPAT,
                     tujuan = pegawai.HR_SEMINAR_LUAR.HR_TUJUAN,
                     alamatbercuti = pegawai.HR_ALAMAT_CUTI,
                     notel = pegawai.HR_NO_TEL,
                     emel = pegawai.HR_ALAMAT_EMEL,
                     tarikhcuti = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_CUTI),
                     tarikhakhir = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_CUTI_AKH),
                     jumlah = pegawai.HR_JUMLAH_CUTI,
                     tarikhkembali = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_KEMBALI),
                     tiket = pegawai.HR_TIKET_KAPAL,
                     penginapan = pegawai.HR_PENGINAPAN,
                     lainlain = pegawai.HR_LAIN,
                     jumlahbelanja = pegawai.HR_JUMLAH_BELANJA,
                     tarikhmohon = string.Format("{0:dd/MM/yyyy}", pegawai.HR_SEMINAR_LUAR.HR_TARIKH_PERMOHONAN),
                     namapegawai = pegawai.HR_NAMA_PEGAWAI,
                     hubungan = pegawai.HR_HUBUNGAN,
                     alamatpegawai = pegawai.HR_ALAMAT_PEGAWAI,
                     notelpegawai = pegawai.HR_NOTEL_PEGAWAI,
                     emelpegawai = pegawai.HR_ALAMAT_EMEL,
                     alasanpegawai = pegawai.HR_ALASAN,
                     
                 });
            
            //Interop+  \
            Application appWord = new Application();
            wordDocument = appWord.Documents.Open(@"C:\Users\RSA-02\Documents\Visual Studio 2015\Projects\New\webapp\Content\template\BORANG_LUAR_NEGARA.docx");
            wordDocument.ExportAsFixedFormat(@"C:\Users\RSA-02\Documents\Visual Studio 2015\Projects\New\webapp\Content\template\BORANG_LUAR_NEGARA.pdf", WdExportFormat.wdExportFormatPDF);

            appWord.Quit();

            string FilePath = Server.MapPath("~/Content/template/BORANG_LUAR_NEGARA.pdf");
            WebClient User = new WebClient();
            Byte[] FileBuffer = User.DownloadData(FilePath);
            if (FileBuffer != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-length", FileBuffer.Length.ToString());
                Response.BinaryWrite(FileBuffer);
            }
            return File(FilePath, "application/pdf");
        }



        public ActionResult TambahLuar()
        {
            return PartialView("_TambahLuar");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahLuar([Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND,HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI")] HR_SEMINAR_LUAR seminar)
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
            return PartialView("_TambahLuar", seminar);
        }

        public ActionResult EditBorang(string id, string kod, string key, string value)
        {
            HR_SEMINAR_LUAR_DETAIL luardetail = new HR_SEMINAR_LUAR_DETAIL();
            HR_SEMINAR_LUAR seminar = new HR_SEMINAR_LUAR();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            luardetail.HR_NO_PEKERJA = id;
            luardetail.HR_KOD_LAWATAN = kod;
            luardetail.HR_SEMINAR_LUAR = new HR_SEMINAR_LUAR();
            //luardetail.HR_SEMINAR_LUAR.HR_TARIKH_PERMOHONAN = DateTime.Now;

            HR_SEMINAR_LUAR_DETAIL mDetail = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_LAWATAN == kod);
            HR_SEMINAR_LUAR mSeminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == kod);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();

            }
            GE_JABATAN jabatanlist = new GE_JABATAN();
            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_NAMA_SEMINAR = mSeminar.HR_NAMA_SEMINAR;
            ViewBag.HR_TARIKH_MULA = mSeminar.HR_TARIKH_MULA;
            ViewBag.HR_TARIKH_TAMAT = mSeminar.HR_TARIKH_TAMAT;
            ViewBag.HR_TEMPAT = mSeminar.HR_TEMPAT;
            ViewBag.HR_TUJUAN = mSeminar.HR_TUJUAN;
            var tarikhpemohonan = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpemohonan;
            var tarikhmula = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_MULA);
            ViewBag.HR_TARIKH_MULA = tarikhmula;
            var tarikhtamat = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_TAMAT);
            ViewBag.HR_TARIKH_TAMAT = tarikhtamat;
            var tarikhcuti = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI);
            ViewBag.HR_TARIKH_CUTI = tarikhcuti;
            var tarikhcutiakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI_AKH);
            ViewBag.HR_TARIKH_CUTI_AKH = tarikhcutiakhir;
            var tarikhkembali = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_KEMBALI);
            ViewBag.HR_TARIKH_KEMBALI = tarikhkembali;
            var tarikhmangkumula = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_MULA);
            ViewBag.HR_TARIKH_MANGKU_MULA = tarikhmangkumula;
            var tarikhmangkuakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_AKHIR);
            ViewBag.HR_TARIKH_MANGKU_AKHIR = tarikhmangkuakhir;

            HR_SEMINAR_LUAR h = new HR_SEMINAR_LUAR();
            h.HR_KOD_LAWATAN = "T01";
            h.HR_NAMA_SEMINAR = "TAMBAH BARU SEMINAR";

            List<HR_SEMINAR_LUAR> sem = new List<HR_SEMINAR_LUAR>();
            sem = db.HR_SEMINAR_LUAR.ToList();
            sem.Add(h);

            string[] seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA == id).Select(s => s.HR_KOD_LAWATAN).ToArray();

            ViewBag.HR_KOD_LAWATAN = new SelectList(sem, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", null, null, seminardetails);
            ViewBag.key = key;
            ViewBag.value = value;
            return View("EditBorang", mDetail);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBorang([Bind(Include = "HR_KOD_LAWATAN,HR_NO_PEKERJA, HR_KERAP_IND, HR_LAPORAN_IND, HR_TARIKH_CUTI,HR_TARIKH_CUTI_AKH,HR_JUMLAH_CUTI,HR_TARIKH_KEMBALI,HR_ALAMAT_CUTI,HR_TARIKHMULA_MANGKU,HR_TARIKHAKHIR_MANGKU,HR_TIKET_KAPAL,HR_PENGINAPAN,HR_LAIN,HR_JUMLAH_BELANJA,HR_NAMA_PEGAWAI,HR_HUBUNGAN,HR_ALAMAT_PEGAWAI,HR_NOTEL_PEGAWAI,HR_EMAIL_PEGAWAI,HR_ALASAN")]HR_SEMINAR_LUAR_DETAIL luardetail, [Bind(Include = "HR_KOD_LAWATAN,HR_TARIKH_PERMOHONAN,HR_TARIKH_MULA,HR_TARIKH_TAMAT,HR_NAMA_SEMINAR,HR_TUJUAN,HR_TEMPAT,HR_FAEDAH,HR_LULUS_IND,HR_PERBELANJAAN,HR_LULUS_MENTERI_IND,HR_TARIKH_LULUS_MENTERI,HR_PERBELANJAAN_LAIN,HR_SOKONG_IND,HR_TARIKH_SOKONG,HR_NP_SOKONG,HR_JENIS_IND")] HR_SEMINAR_LUAR luar)
        {

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == luardetail.HR_NO_PEKERJA);

            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();
            }
            GE_JABATAN jabatanlist = new GE_JABATAN();

            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s =>  s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;


            if (ModelState.IsValid)
            {
                db.Entry(luardetail).State = EntityState.Modified;
                db.SaveChanges();  
            }
            return View(luardetail);
        }


        public ActionResult PadamBorang(string id, string kod, string key, string value)
        {
            HR_SEMINAR_LUAR_DETAIL luardetail = new HR_SEMINAR_LUAR_DETAIL();
            HR_SEMINAR_LUAR seminar = new HR_SEMINAR_LUAR();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            luardetail.HR_NO_PEKERJA = id;
            luardetail.HR_KOD_LAWATAN = kod;
            luardetail.HR_SEMINAR_LUAR = new HR_SEMINAR_LUAR();
            //luardetail.HR_SEMINAR_LUAR.HR_TARIKH_PERMOHONAN = DateTime.Now;

            HR_SEMINAR_LUAR_DETAIL mDetail = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_LAWATAN == kod);
            HR_SEMINAR_LUAR mSeminar = db.HR_SEMINAR_LUAR.SingleOrDefault(s => s.HR_KOD_LAWATAN == kod);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            if (jabatan == null)
            {
                jabatan = new GE_JABATAN();

            }
            GE_JABATAN jabatanlist = new GE_JABATAN();
            HR_JAWATAN jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            HR_JAWATAN listjawatan = new HR_JAWATAN();

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_NAMA_SEMINAR = mSeminar.HR_NAMA_SEMINAR;
            ViewBag.HR_TARIKH_MULA = mSeminar.HR_TARIKH_MULA;
            ViewBag.HR_TARIKH_TAMAT = mSeminar.HR_TARIKH_TAMAT;
            ViewBag.HR_TEMPAT = mSeminar.HR_TEMPAT;
            ViewBag.HR_TUJUAN = mSeminar.HR_TUJUAN;
            var tarikhpemohonan = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhpemohonan;
            var tarikhmula = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_MULA);
            ViewBag.HR_TARIKH_MULA = tarikhmula;
            var tarikhtamat = string.Format("{0:dd/MM/yyyy}", mSeminar.HR_TARIKH_TAMAT);
            ViewBag.HR_TARIKH_TAMAT = tarikhtamat;
            var tarikhcuti = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI);
            ViewBag.HR_TARIKH_CUTI = tarikhcuti;
            var tarikhcutiakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_CUTI_AKH);
            ViewBag.HR_TARIKH_CUTI_AKH = tarikhcutiakhir;
            var tarikhkembali = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_KEMBALI);
            ViewBag.HR_TARIKH_KEMBALI = tarikhkembali;
            var tarikhmangkumula = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_MULA);
            ViewBag.HR_TARIKH_MANGKU_MULA = tarikhmangkumula;
            var tarikhmangkuakhir = string.Format("{0:dd/MM/yyyy}", mDetail.HR_TARIKH_MANGKU_AKHIR);
            ViewBag.HR_TARIKH_MANGKU_AKHIR = tarikhmangkuakhir;

            HR_SEMINAR_LUAR h = new HR_SEMINAR_LUAR();
            h.HR_KOD_LAWATAN = "T01";
            h.HR_NAMA_SEMINAR = "TAMBAH BARU SEMINAR";

            List<HR_SEMINAR_LUAR> sem = new List<HR_SEMINAR_LUAR>();
            sem = db.HR_SEMINAR_LUAR.ToList();
            sem.Add(h);

            string[] seminardetails = db.HR_SEMINAR_LUAR_DETAIL.Where(s => s.HR_NO_PEKERJA == id).Select(s => s.HR_KOD_LAWATAN).ToArray();

            ViewBag.HR_KOD_LAWATAN = new SelectList(sem, "HR_KOD_LAWATAN", "HR_NAMA_SEMINAR", null, null, seminardetails);
            ViewBag.key = key;
            ViewBag.value = value;
            return View("PadamBorang", mDetail);
        }


        [HttpPost, ActionName("PadamBorang")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_SEMINAR_LUAR_DETAIL mDetail, string id, string kod, string key, string value)
        {
            mDetail = db.HR_SEMINAR_LUAR_DETAIL.SingleOrDefault(s => s.HR_NO_PEKERJA == mDetail.HR_NO_PEKERJA && s.HR_KOD_LAWATAN == mDetail.HR_KOD_LAWATAN);
            db.HR_SEMINAR_LUAR_DETAIL.Remove(mDetail);
            db.SaveChanges();
            {
                return RedirectToAction("CariPemohon", new { key = key, value = value });
            }
        }


        public enum ManageMessageId
        {
            Tambah,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            ResetPassword,
            Kemaskini,
            KemaskiniTunggakan,
            BayarTunggakan,
            TambahBonus,
            Error
        }


    }
}