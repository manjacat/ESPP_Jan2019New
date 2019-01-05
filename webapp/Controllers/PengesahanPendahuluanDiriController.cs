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
    public class PengesahanPendahuluanDiriController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        // GET: PengesahanPendahuluanDiri
        public ActionResult Index()
        {
            return View();
        }

       

        public ActionResult SenaraiPendahuluanHR()
        {
            return View(db.HR_PENDAHULUAN_DIRI.ToList());
        }


       


            public ActionResult PendahuluanListInfoHR(string no_pekerja, string kod)
            {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_PENDAHULUAN_DIRI mPendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PENDAHULUAN == kod);


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

            return View("PendahuluanListInfoHR", mPendahuluan);
        }


        public ActionResult AddLulusHR(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENDAHULUAN_DIRI mPendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PENDAHULUAN == kod);
          
            mPendahuluan.HR_NO_PEKERJA = id;
            mPendahuluan.HR_KOD_PENDAHULUAN = kod;

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NAMA_HR);

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }
           
            mPendahuluan.HR_TARIKH_HR = DateTime.Now;
            var tarikhhr = string.Format("{0:dd/MM/yyyy}", mPendahuluan.HR_TARIKH_HR);
            ViewBag.HR_TARIKH_HR = tarikhhr;

            if (mPendahuluan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_HR);
            ViewBag.HR_JAWATAN_PEGAWAI_HR2 = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return PartialView("_AddLulusHR", mPendahuluan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLulusHR(HR_PENDAHULUAN_DIRI mPendahuluan)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NAMA_HR);

            if (ModelState.IsValid)
            {
                HR_PENDAHULUAN_DIRI pendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NO_PEKERJA && s.HR_KOD_PENDAHULUAN == mPendahuluan.HR_KOD_PENDAHULUAN);
                if (pendahuluan != null)
                {
                    pendahuluan.HR_NAMA_HR = mPendahuluan.HR_NAMA_HR;
                    pendahuluan.HR_TARIKH_HR = mPendahuluan.HR_TARIKH_HR;
                    pendahuluan.HR_IND_HR = mPendahuluan.HR_IND_HR;
                    pendahuluan.HR_CATATAN = mPendahuluan.HR_CATATAN;

                    db.Entry(pendahuluan).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("senaraipendahuluanHR");
            }

            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_HR);
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi2.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return PartialView("_AddLulusHR", mPendahuluan);
        }



        public ActionResult EditLulusHR(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PENDAHULUAN_DIRI mPendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PENDAHULUAN == kod);

            mPendahuluan.HR_NO_PEKERJA = id;
            mPendahuluan.HR_KOD_PENDAHULUAN = kod;

            mPendahuluan.HR_TARIKH_HR = DateTime.Now;
            var tarikhhr = string.Format("{0:dd/MM/yyyy}", mPendahuluan.HR_TARIKH_HR);
            ViewBag.HR_TARIKH_HR = tarikhhr;

            if (mPendahuluan == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_HR);

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NAMA_HR);
            if (mPeribadi == null)
            {
                ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            }
            else
            {
                ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            }
            


            return PartialView("_EditLulusHR", mPendahuluan);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLulusHR(HR_PENDAHULUAN_DIRI mPendahuluan)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NAMA_HR);

            if (ModelState.IsValid)
            {
                HR_PENDAHULUAN_DIRI pendahuluan = db.HR_PENDAHULUAN_DIRI.SingleOrDefault(s => s.HR_NO_PEKERJA == mPendahuluan.HR_NO_PEKERJA && s.HR_KOD_PENDAHULUAN == mPendahuluan.HR_KOD_PENDAHULUAN);
                if (pendahuluan != null)
                {
                    pendahuluan.HR_NAMA_HR = mPendahuluan.HR_NAMA_HR;
                    pendahuluan.HR_TARIKH_HR = mPendahuluan.HR_TARIKH_HR;
                    pendahuluan.HR_IND_HR = mPendahuluan.HR_IND_HR;
                    pendahuluan.HR_CATATAN = mPendahuluan.HR_CATATAN;

                    db.Entry(pendahuluan).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("senaraipendahuluanHR");
            }

            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPendahuluan.HR_NAMA_HR);
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi2.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return PartialView("_EditLulusHR", mPendahuluan);

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