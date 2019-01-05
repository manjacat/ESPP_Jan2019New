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
    public class PengesahanPerbatuanPinkController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();
        // GET: PengesahanPerbatuan
        public ActionResult SenaraiPerbatuanKJ()
        {
            return View(db.HR_PERBATUAN_PINK.ToList());
        }

        public ActionResult SenaraiPerbatuanKB()
        {
            return View(db.HR_PERBATUAN_PINK.ToList());
        }


        public ActionResult SenaraiPerbatuanHR()
        {
            List<HR_PERBATUAN_PUTIH> mPutih = new List<HR_PERBATUAN_PUTIH>();
            List<HR_PERBATUAN_PINK> mPink = new List<HR_PERBATUAN_PINK>();

            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            List<HR_PERBATUAN_PINK> pink = db.HR_PERBATUAN_PINK.ToList();

            PerbatuanModels Pengesahan = new PerbatuanModels();

            Pengesahan.HR_PERBATUAN_PUTIH = putih;
            Pengesahan.HR_PERBATUAN_PINK = pink;

            return View(Pengesahan);
        }

        public ActionResult SenaraiPerbatuanBelanja()
        {
            return View(db.HR_PERBATUAN_PINK.ToList());
        }

        public ActionResult SenaraiPerbatuanSemakan()
        {
            return View(db.HR_PERBATUAN_PINK.ToList());
        }

        public ActionResult SenaraiPengesahanHR()
        {
            return View(db.HR_PERBATUAN_PINK.ToList());
        }


        public ActionResult PerbatuanListInfoHR(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUJUAN).Include(s => s.HR_PERBATUAN_TUNTUTAN).SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);

            if (mPink == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();

            if (mPink.HR_PERBATUAN_TUJUAN.Count() <= 0)
            {
                mPink.HR_PERBATUAN_TUJUAN.Add(new HR_PERBATUAN_TUJUAN());
            }

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_NAMA_PEGAWAI_JABATAN = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_JABATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_KENDERAAN_KELAS = new SelectList(db.HR_KADAR_PERBATUAN.GroupBy(s => s.HR_KELAS).Select(s => s.FirstOrDefault()).OrderBy(s => s.HR_KELAS), "HR_KELAS", "HR_KELAS");

            ViewBag.selectGredElaun = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == "SM" && s.HR_JENIS == "M").ToList();
            ViewBag.selectGredHotel = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == "SM" && s.HR_JENIS == "H").ToList();

            List<SelectListItem> HR_LOKASI = new List<SelectListItem>();
            HR_LOKASI.Add(new SelectListItem { Text = "Semenanjung Malaysia", Value = "S" });
            HR_LOKASI.Add(new SelectListItem { Text = "Sabah, Sarawak dan Labuan", Value = "SM" });
            ViewBag.HR_LOKASI = HR_LOKASI;

            //pink.HR_NO_PEKERJA = id;
            //pink.HR_KOD_PERBATUAN = kod;



            int? JUMLAHKESELURUHAN = 0;
            int? JUMLAHKM = 0;
            //foreach (HR_PERBATUAN_TUNTUTAN perbatuan in Maklumat)
            //{
            //    if (perbatuan.HR_NILAI != null && perbatuan.HR_KILOMETER != null)
            //    {
            //        perbatuan.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
            //        perbatuan.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
            //        JUMLAHKESELURUHAN += perbatuan.HR_JUMLAH;
            //        JUMLAHKM += perbatuan.HR_KILOMETER;
            //        db.HR_PERBATUAN_TUNTUTAN.Add(perbatuan);
            //    }
            //}

            ViewBag.JUMLAHKMSELURUH = JUMLAHKESELURUHAN;
            ViewBag.TOTALPERJALANAN = JUMLAHKESELURUHAN;
            ViewBag.JUMLAHKM = JUMLAHKM;

            ViewBag.HR_RM_MAKAN_P = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            ViewBag.HR_RM_MAKAN_T = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            ViewBag.HR_RM_MAKAN_M = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;


            return View("PerbatuanListInfoHR", mPink);
        }


        public ActionResult PerbatuanInfo(string id, string kod, string jenis)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUJUAN).Include(s => s.HR_PERBATUAN_TUNTUTAN).SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            if (mPink == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();

            if (mPink.HR_PERBATUAN_TUJUAN.Count() <= 0)
            {
                mPink.HR_PERBATUAN_TUJUAN.Add(new HR_PERBATUAN_TUJUAN());
            }

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_NAMA_PEGAWAI_JABATAN = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_JABATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_KENDERAAN_KELAS = new SelectList(db.HR_KADAR_PERBATUAN.GroupBy(s => s.HR_KELAS).Select(s => s.FirstOrDefault()).OrderBy(s => s.HR_KELAS), "HR_KELAS", "HR_KELAS");

            ViewBag.selectGredElaun = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == "SM" && s.HR_JENIS == "M").ToList();
            ViewBag.selectGredHotel = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == "SM" && s.HR_JENIS == "H").ToList();

            List<SelectListItem> HR_LOKASI = new List<SelectListItem>();
            HR_LOKASI.Add(new SelectListItem { Text = "Semenanjung Malaysia", Value = "S" });
            HR_LOKASI.Add(new SelectListItem { Text = "Sabah, Sarawak dan Labuan", Value = "SM" });
            ViewBag.HR_LOKASI = HR_LOKASI;

            //pink.HR_NO_PEKERJA = id;
            //pink.HR_KOD_PERBATUAN = kod;



            int? JUMLAHKESELURUHAN = 0;
            int? JUMLAHKM = 0;
            //foreach (HR_PERBATUAN_TUNTUTAN perbatuan in Maklumat)
            //{
            //    if (perbatuan.HR_NILAI != null && perbatuan.HR_KILOMETER != null)
            //    {
            //        perbatuan.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
            //        perbatuan.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
            //        JUMLAHKESELURUHAN += perbatuan.HR_JUMLAH;
            //        JUMLAHKM += perbatuan.HR_KILOMETER;
            //        db.HR_PERBATUAN_TUNTUTAN.Add(perbatuan);
            //    }
            //}

            ViewBag.JUMLAHKMSELURUH = JUMLAHKESELURUHAN;
            ViewBag.TOTALPERJALANAN = JUMLAHKESELURUHAN;
            ViewBag.JUMLAHKM = JUMLAHKM;

            ViewBag.HR_RM_MAKAN_P = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            ViewBag.HR_RM_MAKAN_T = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            ViewBag.HR_RM_MAKAN_M = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;


            return View("PerbatuanList" + jenis, mPink);
        }


        public ActionResult PerbatuanListInfo(string id, string kod)
        {
            return PerbatuanInfo(id, kod, "Info");
        }


        public ActionResult PerbatuanListInfoKB(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);


            mPink.HR_NO_PEKERJA = no_pekerja;
            mPink.HR_KOD_PERBATUAN = kod;


            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;


            if (mPink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_KB = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");

            ViewBag.HR_JAWATAN_PEGAWAI_KB = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            ViewBag.HR_NAMA_KJ = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");

            ViewBag.HR_JAWATAN_PEGAWAI_KJ = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPink.HR_NAMA_KJ);


            return View("PerbatuanListInfoKB", mPink);
        }


        public ActionResult PerbatuanListInfoKJ(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK Pinky = new HR_PERBATUAN_PINK();


            Pinky.HR_NO_PEKERJA = no_pekerja;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_NAMA_PEGAWAI_JABATAN = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_JABATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            pink.HR_NO_PEKERJA = no_pekerja;
            pink.HR_KOD_PERBATUAN = kod;

            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);
            if (mPink == null)
            {
                return HttpNotFound();
            }
            return View("PerbatuanListInfoKJ", mPink);
        }


        public ActionResult PerbatuanListInfoBelanja(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK Pinky = new HR_PERBATUAN_PINK();


            Pinky.HR_NO_PEKERJA = no_pekerja;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_NAMA_PEGAWAI_JABATAN = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_JABATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            pink.HR_NO_PEKERJA = no_pekerja;
            pink.HR_KOD_PERBATUAN = kod;

            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);
            if (mPink == null)
            {
                return HttpNotFound();
            }
            return View("PerbatuanListInfoBelanja", mPink);
        }

        public ActionResult PerbatuanListInfoSemakan(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK Pinky = new HR_PERBATUAN_PINK();


            Pinky.HR_NO_PEKERJA = no_pekerja;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_NAMA_PEGAWAI_JABATAN = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_JABATAN = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            pink.HR_NO_PEKERJA = no_pekerja;
            pink.HR_KOD_PERBATUAN = kod;

            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);
            if (mPink == null)
            {
                return HttpNotFound();
            }
            return View("PerbatuanListInfoSemakan", mPink);
        }


        public ActionResult AddLulusPinkHR(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            mPink.HR_NO_PEKERJA = id;
            mPink.HR_KOD_PERBATUAN = kod;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;

            mPink.HR_TARIKH_HR = DateTime.Now;
            var tarikhhr = string.Format("{0:dd/MM/yyyy}", mPink.HR_TARIKH_HR);
            ViewBag.HR_TARIKH_HR = tarikhhr;

            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_AddLulusPinkHR", mPink);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddLulusPinkHR(HR_PERBATUAN_PINK mPink)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NAMA_HR);
            
            if (ModelState.IsValid)
            {
                HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN);
                if (pink != null)
                {
                    pink.HR_NAMA_HR = mPink.HR_NAMA_HR;
                    pink.HR_TARIKH_HR = mPink.HR_TARIKH_HR;
                    pink.HR_IND_HR = mPink.HR_IND_HR;
                    pink.HR_CATATAN = mPink.HR_CATATAN;

                    db.Entry(pink).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
                return RedirectToAction("senaraipengesahanHR");
            }

            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPink.HR_NAMA_HR);
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi2.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return PartialView("_AddLulusPinkHR", mPink);
        }



        public ActionResult EditLulusPinkHR(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            mPink.HR_NO_PEKERJA = id;
            mPink.HR_KOD_PERBATUAN = kod;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;

            mPink.HR_TARIKH_HR = DateTime.Now;
            var tarikhhr = string.Format("{0:dd/MM/yyyy}", mPink.HR_TARIKH_HR);
            ViewBag.HR_TARIKH_HR = tarikhhr;

            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_EditLulusPinkHR", mPink);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLulusPinkHR(HR_PERBATUAN_PINK mPink)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi2 = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NAMA_HR);

            if (ModelState.IsValid)
            {
                HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN);
                if (pink != null)
                {
                    pink.HR_NAMA_HR = mPink.HR_NAMA_HR;
                    pink.HR_TARIKH_HR = mPink.HR_TARIKH_HR;
                    pink.HR_IND_HR = mPink.HR_IND_HR;

                    db.Entry(pink).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("senaraipengesahanHR");
            }

            ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA", mPink.HR_NAMA_HR);
            ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN", mPeribadi2.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);

            return PartialView("_EditLulusPinkHR", mPink);
        }



        public ActionResult EditLulusPinkKB(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK mPink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            mPink.HR_NO_PEKERJA = id;
            mPink.HR_KOD_PERBATUAN = kod;


            HR_PERBATUAN_PINK pink = new HR_PERBATUAN_PINK();
            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();
            GE_JABATAN jabatan = new GE_JABATAN();
            HR_JAWATAN jawatan = new HR_JAWATAN();
            GE_BAHAGIAN bahagian = new GE_BAHAGIAN();

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            GE_JABATAN mjabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
            HR_JAWATAN mjawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
            GE_BAHAGIAN mbahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

            ViewBag.HR_JABATAN = mjabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = mbahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = mjawatan.HR_NAMA_JAWATAN;
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;
            ViewBag.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
            ViewBag.HR_ALAMAT = mPeribadi.HR_SALAMAT1;

            ViewBag.HR_NAMA_KB = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_NAMA_KJ = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_KB = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            ViewBag.HR_JAWATAN_PEGAWAI_KJ = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");


            mPink.HR_TARIKH_KB = DateTime.Now;
            var tarikhkb = string.Format("{0:dd/MM/yyyy}", mPink.HR_TARIKH_KB);
            ViewBag.HR_TARIKH_KB = tarikhkb;

            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_KB = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_KB = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_EditLulusPinkKB", mPink);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLulusPinkKB([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERBATUAN,HR_KENDERAAN_NOMBOR,HR_KENDERAAN_KUASA,HR_KENDERAAN_KELAS,HR_MAKSIMA_TUNTUTAN,HR_TARIKH,HR_WAKTU_BERTOLAK,HR_WAKTU_SAMPAI,HR_TUJUAN,HR_JARAK,HR_JUMLAH,HR_TARIKH_PERMOHONAN,HR_IND_PEMOHON,HR_TANDATANGAN_PEMOHON,HR_TANDATANGAN_KB,HR_NAMA_KB,HR_TARIKH_KB,HR_TANDATANGAN_KJ,HR_NAMA_KJ,HR_TARIKH_KJ,HR_IND_KB,HR_IND_KJ,HR_TARIKH_KERANIP,HR_IND_KERANIP,HR_TARIKH_KERANIS,HR_IND_KERANIS,DOCUMENT_ID,DESC_FILE")] HR_PERBATUAN_PINK mPink)
        {

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA);

            if (ModelState.IsValid)
            {
                db.Entry(mPink).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("senaraiperbatuanKB");
            }

            ViewBag.HR_NAMA_KBI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI_KB = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_EditLulusPinkKB", mPink);
        }


        public ActionResult EditLulusPinkKJ(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");
            return PartialView("_EditLulusPinkKJ", pink);
        }

        public ActionResult EditLulusPinkBelanja(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            return PartialView("_EditLulusPinkBelanja", pink);
        }

        public ActionResult EditLulusPinkSemakan(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (pink == null)
            {
                return HttpNotFound();
            }
            ViewBag.HR_NAMA_PEGAWAI = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_JAWATAN_PEGAWAI = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");


            return PartialView("_EditLulusPinkSemakan", pink);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLulusPinkSemakan([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERBATUAN,HR_KENDERAAN_NOMBOR,HR_KENDERAAN_KUASA,HR_KENDERAAN_KELAS,HR_MAKSIMA_TUNTUTAN,HR_TARIKH,HR_WAKTU_BERTOLAK,HR_WAKTU_SAMPAI,HR_TUJUAN,HR_JARAK,HR_JUMLAH,HR_TARIKH_PERMOHONAN,HR_IND_PEMOHON,HR_TANDATANGAN_PEMOHON,HR_TANDATANGAN_KB,HR_NAMA_KB,HR_TARIKH_KB,HR_TANDATANGAN_KJ,HR_NAMA_KJ,HR_TARIKH_KJ,HR_IND_KB,HR_IND_KJ,HR_TARIKH_KERANIP,HR_IND_KERANIP,HR_TARIKH_KERANIS,HR_IND_KERANIS,DOCUMENT_ID,DESC_FILE")] HR_PERBATUAN_PINK pink)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pink).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("senaraiperbatuansemakan");
            }
            return View(pink);
        }


        public ActionResult InfoPerbatuanPutih(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            HR_PERBATUAN_PUTIH putih1 = putih.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();

            putih1.HR_TARIKH_SAH_PTB = DateTime.Now;

            var tarikhkj = string.Format("{0:dd/MM/yyyy}", putih1.HR_TARIKH_SAH_PTB);
            ViewBag.HR_TARIKH_SAH_PTB = tarikhkj;

            if (putih == null)
            {
                return HttpNotFound();
            }
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            ViewBag.HR_NO_PEKERJA = putih1.HR_NO_PEKERJA;
            ViewBag.HR_KOD_PERBATUAN = putih1.HR_KOD_PERBATUAN;


            return View("InfoPerbatuanPutih", putih1);
        }



        public ActionResult EditPerbatuanPutih(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            HR_PERBATUAN_PUTIH putih1 = putih.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();

            putih1.HR_TARIKH_SAH_PTB = DateTime.Now;

            var tarikhkj = string.Format("{0:dd/MM/yyyy}", putih1.HR_TARIKH_SAH_PTB);
            ViewBag.HR_TARIKH_SAH_PTB = tarikhkj;

            if (putih == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            ViewBag.HR_NO_PEKERJA = putih1.HR_NO_PEKERJA;
            ViewBag.HR_KOD_PERBATUAN = putih1.HR_KOD_PERBATUAN;



            return View("EditPerbatuanPutih", putih1);
        }

        public ActionResult InfoPerbatuanPutihHR(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            HR_PERBATUAN_PUTIH putih1 = putih.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();

            putih1.HR_TARIKH_SAH_PTB = DateTime.Now;

            var tarikhkj = string.Format("{0:dd/MM/yyyy}", putih1.HR_TARIKH_SAH_PTB);
            ViewBag.HR_TARIKH_SAH_PTB = tarikhkj;

            if (putih == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            ViewBag.HR_NO_PEKERJA = putih1.HR_NO_PEKERJA;
            ViewBag.HR_KOD_PERBATUAN = putih1.HR_KOD_PERBATUAN;



            return View("InfoPerbatuanPutihHR", putih1);
        }



        public ActionResult EditPerbatuanPutihHR(string no_pekerja, string kod)
        {
            if (no_pekerja == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            HR_PERBATUAN_PUTIH putih1 = putih.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_PERBATUAN == kod);

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();

            putih1.HR_TARIKH_SAH_PTB = DateTime.Now;

            var tarikhkj = string.Format("{0:dd/MM/yyyy}", putih1.HR_TARIKH_SAH_PTB);
            ViewBag.HR_TARIKH_SAH_PTB = tarikhkj;

            if (putih == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);

            ViewBag.HR_NO_PEKERJA = putih1.HR_NO_PEKERJA;
            ViewBag.HR_KOD_PERBATUAN = putih1.HR_KOD_PERBATUAN;



            return View("EditPerbatuanPutihHR", putih1);
        }


        public ActionResult EditPerbatuan(string id, string kod)
        {
            if (id == null && kod == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            List<HR_PERBATUAN_PUTIH> mPutih = new List<HR_PERBATUAN_PUTIH>();
            List<HR_PERBATUAN_PINK> mPink = new List<HR_PERBATUAN_PINK>();

            List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
            List<HR_PERBATUAN_PINK> pink = db.HR_PERBATUAN_PINK.ToList();
            PerbatuanModels Pengesahan = new PerbatuanModels();

            Pengesahan.HR_PERBATUAN_PUTIH = putih;
            Pengesahan.HR_PERBATUAN_PINK = pink;


            HR_PERBATUAN_PUTIH putih1 = putih.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.ToList();

            if (putih == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            ViewBag.noPekerja = peribadi.HR_NO_PEKERJA;
            ViewBag.kodPerbatuan = putih1.HR_KOD_PERBATUAN;

            return View("EditPerbatuan", Pengesahan);
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

        public JsonResult CariJawatanPegawaiKB(string id)
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

        public JsonResult CariJawatanPegawaiKJ(string id)
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