using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using Microsoft.Office.Interop.Word;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text;

namespace eSPP.Controllers
{
    public class PerbatuanPinkController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: Perbatuan

        //public ActionResult Index()
        //{
        //    List<HR_PERBATUAN_PUTIH> MPut = new List<HR_PERBATUAN_PUTIH>();
        //    List<HR_PERBATUAN_PINK> MPin = new List<HR_PERBATUAN_PINK>();

        //    List<HR_PERBATUAN_PUTIH> putih = db.HR_PERBATUAN_PUTIH.ToList();
        //    List<HR_PERBATUAN_PINK> pink = db.HR_PERBATUAN_PINK.ToList();

        //    PerbatuanModels Perbatuan = new PerbatuanModels();

        //    Perbatuan.HR_PERBATUAN_PUTIH = putih;
        //    Perbatuan.HR_PERBATUAN_PINK = pink;

        //    return View(Perbatuan);
        //}

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

        public ActionResult TambahPerbatuan(string id, string kod)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PERBATUAN_PINK mPink = new HR_PERBATUAN_PINK();
            mPink.HR_PERBATUAN_TUJUAN = new List<HR_PERBATUAN_TUJUAN>();
            mPink.HR_PERBATUAN_TUNTUTAN = new HR_PERBATUAN_TUNTUTAN();

            mPink.HR_NO_PEKERJA = id;

            mPink.HR_TARIKH_PERMOHONAN = DateTime.Now;
            var tarikhakuan = string.Format("{0:dd/MM/yyyy}", mPink.HR_TARIKH_PERMOHONAN);
            ViewBag.HR_TARIKH_PERMOHONAN = tarikhakuan;


            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == id);
           

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }


            HR_PERBATUAN_TUJUAN mTujuan = db.HR_PERBATUAN_TUJUAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (mTujuan == null)
            {
                mTujuan = new HR_PERBATUAN_TUJUAN();
            }
            HR_PERBATUAN_TUJUAN tujuanlist = new HR_PERBATUAN_TUJUAN();

            HR_PERBATUAN_TUNTUTAN mPerbatuan = db.HR_PERBATUAN_TUNTUTAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (mPerbatuan == null)
            {
                mPerbatuan = new HR_PERBATUAN_TUNTUTAN();
            }
            HR_PERBATUAN_TUNTUTAN perbatuanlist = new HR_PERBATUAN_TUNTUTAN();


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

            //List<SelectListItem> lokasi = new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "Semenanjung Malaysia", Value = "S" },
            //    new SelectListItem { Text = "Sabah, Sarawak dan Labuan", Value = "SM" },
            //};
            
            List<SelectListItem> HR_LOKASI = new List<SelectListItem>();
            HR_LOKASI.Add(new SelectListItem { Text = "SEMENANJUNG MALAYSIA", Value = "SM" });
            HR_LOKASI.Add(new SelectListItem { Text = "SABAH,SARAWAK DAN LABUAN", Value = "SS" });
            ViewBag.HR_LOKASI = HR_LOKASI;


            List<SelectListItem> bulanbekerja = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulanbekerja = new SelectList(bulanbekerja, "Value", "Text", DateTime.Now.Month - 1);

            if (DateTime.Now.Month == '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year - 1;
            }
            if (DateTime.Now.Month != '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year;
            }
            var HR_KOD_GREDELAUN = db.HR_GRED_ELAUN_PEKELILING.ToList();
            ViewBag.HR_KOD_GREDELAUN = HR_KOD_GREDELAUN;

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
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



            ViewBag.HR_RM_MAKAN_P = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            ViewBag.HR_RM_MAKAN_T = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            ViewBag.HR_RM_MAKAN_M = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;



            return View("TambahPerbatuan", mPink);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPerbatuan([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERBATUAN,HR_KENDERAAN_JENIS,HR_KENDERAAN_NOMBOR,HR_KENDERAAN_KUASA,HR_KENDERAAN_KELAS,HR_MAKSIMA_TUNTUTAN,HR_TARIKH_PERMOHONAN,HR_IND_PEMOHON,HR_TANDATANGAN_PEMOHON,HR_TANDATANGAN_KB,HR_NAMA_KB,HR_TARIKH_KB,HR_TANDATANGAN_KJ,HR_NAMA_KJ,HR_TARIKH_KJ,HR_IND_KB,HR_IND_KJ,HR_TARIKH_KERANIP,HR_IND_KERANIP,HR_TARIKH_KERANIS,HR_IND_KERANIS,DOCUMENT_ID,DESC_FILE,HR_IND_HR,HR_NAMA_HR,HR_TARIKH_HR,HR_CATATAN,HR_FINALISED_IND")] HR_PERBATUAN_PINK pink, PemohonanPinkModels mohon, HR_PERBATUAN_TUNTUTAN tuntut)
        {

            if (ModelState.IsValid)
            {
                HR_PERBATUAN_PINK semakIDPink = db.HR_PERBATUAN_PINK.OrderByDescending(s => s.HR_KOD_PERBATUAN).FirstOrDefault();
                if(semakIDPink == null)
                {
                    semakIDPink = new HR_PERBATUAN_PINK();
                }

                int LastID2 = 0;
                if(semakIDPink.HR_KOD_PERBATUAN != null)
                {
                    var LastID = new string(semakIDPink.HR_KOD_PERBATUAN.SkipWhile(x => x == 'P' || x == '0').ToArray());
                    LastID2 = Convert.ToInt32(LastID);
                }
                
                var Increment = LastID2 + 1;
                var KodPerbatuan = Convert.ToString(Increment).PadLeft(4, '0');
                pink.HR_KOD_PERBATUAN = "PP" + KodPerbatuan;
                db.HR_PERBATUAN_PINK.Add(pink);
                db.SaveChanges();


                if(tuntut != null)
                {
                    tuntut.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
                    tuntut.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
                    db.HR_PERBATUAN_TUNTUTAN.Add(tuntut);
                    db.SaveChanges();
                }
                
                List<HR_PERBATUAN_TUJUAN> semakTujuan = db.HR_PERBATUAN_TUJUAN.Where(s => s.HR_NO_PEKERJA == pink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == pink.HR_KOD_PERBATUAN).ToList();
                db.HR_PERBATUAN_TUJUAN.RemoveRange(semakTujuan);
                db.SaveChanges();

                int LastIDtujuan = 0;
                foreach (HR_PERBATUAN_TUJUAN tujuan in mohon.HR_PERBATUAN_TUJUAN)
                {
                    if(tujuan.HR_TARIKH != null)
                    {
                        LastIDtujuan++;
                        var Kodtujuan = Convert.ToString(LastIDtujuan).PadLeft(4, '0');
                        tujuan.HR_KOD_TUJUAN = "T" + Kodtujuan;
                        tujuan.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
                        tujuan.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
                        db.HR_PERBATUAN_TUJUAN.Add(tujuan);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("SenaraiPerbatuanPink");
            }

            List<SelectListItem> HR_LOKASI = new List<SelectListItem>();
            HR_LOKASI.Add(new SelectListItem { Text = "SEMENANJUNG MALAYSIA", Value = "SM" });
            HR_LOKASI.Add(new SelectListItem { Text = "SABAH,SARAWAK DAN LABUAN", Value = "SS" });
            ViewBag.HR_LOKASI = HR_LOKASI;


            List<SelectListItem> bulanbekerja = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulanbekerja = new SelectList(bulanbekerja, "Value", "Text", DateTime.Now.Month - 1);

            if (DateTime.Now.Month == '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year - 1;
            }
            if (DateTime.Now.Month != '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year;
            }

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pink.HR_NO_PEKERJA);

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


           

           
            var HR_KOD_GREDELAUN = db.HR_GRED_ELAUN_PEKELILING.ToList();
            ViewBag.HR_KOD_GREDELAUN = HR_KOD_GREDELAUN;

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
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


            ViewBag.HR_RM_MAKAN_P = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            ViewBag.HR_RM_MAKAN_T = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            ViewBag.HR_RM_MAKAN_M = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;

            return View(pink);
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
                mPink.HR_PERBATUAN_TUJUAN.Add( new HR_PERBATUAN_TUJUAN());
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
            return PerbatuanInfo(id, kod,"Info");
        }

        public ActionResult PerbatuanListEdit(string id, string kod)
        {
            return PerbatuanInfo(id, kod, "Edit");
        }

        public ActionResult PerbatuanListPadam(string id, string kod)
        {
            return PerbatuanInfo(id, kod, "Padam");
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult PerbatuanListEdit([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERBATUAN,HR_KENDERAAN_NOMBOR,HR_KENDERAAN_KUASA,HR_KENDERAAN_KELAS,HR_MAKSIMA_TUNTUTAN,HR_TARIKH,HR_WAKTU_BERTOLAK,HR_WAKTU_SAMPAI,HR_TUJUAN,HR_JARAK,HR_JUMLAH,HR_TARIKH_PERMOHONAN,HR_IND_PEMOHON,HR_TANDATANGAN_PEMOHON,HR_TANDATANGAN_KB,HR_NAMA_KB,HR_TARIKH_KB,HR_TANDATANGAN_KJ,HR_NAMA_KJ,HR_TARIKH_KJ,HR_IND_KB,HR_IND_KJ,HR_TARIKH_KERANIP,HR_IND_KERANIP,HR_TARIKH_KERANIS,HR_IND_KERANIS,DOCUMENT_ID,DESC_FILE,HR_IND_HR,HR_NAMA_HR,HR_TARIKH_HR,HR_CATATAN,HR_FINALISED_IND")] HR_PERBATUAN_PINK mPink, PemohonanPinkModels mPerbatuan)
        //{

        //    HR_PERBATUAN_TUNTUTAN batuan = db.HR_PERBATUAN_TUNTUTAN.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN);
        //    HR_PERBATUAN_TUJUAN tujuan = db.HR_PERBATUAN_TUJUAN.SingleOrDefault(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN);


        //    if (ModelState.IsValid)
        //    {
        //        HR_PERBATUAN_PINK semakIDPink = db.HR_PERBATUAN_PINK.OrderByDescending(s => s.HR_KOD_PERBATUAN).FirstOrDefault();
        //        if (semakIDPink == null)
        //        {
        //            semakIDPink = new HR_PERBATUAN_PINK();
        //        }
        //        db.Entry(semakIDPink).State = EntityState.Modified;
        //        db.SaveChanges();

        //        foreach (HR_PERBATUAN_TUNTUTAN perbatuan in mPerbatuan.HR_PERBATUAN_TUNTUTAN)
        //        {
        //            if (perbatuan.HR_NILAI != null && perbatuan.HR_KILOMETER != null)
        //            {
        //                perbatuan.HR_NO_PEKERJA = mPink.HR_NO_PEKERJA;
        //                perbatuan.HR_KOD_PERBATUAN = mPink.HR_KOD_PERBATUAN;
        //                db.HR_PERBATUAN_TUNTUTAN.Add(perbatuan);
        //            }
        //        }
        //        db.SaveChanges();

        //        List<HR_PERBATUAN_TUJUAN> semakTujuan = db.HR_PERBATUAN_TUJUAN.Where(s => s.HR_NO_PEKERJA == mPink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN).ToList();
        //        db.HR_PERBATUAN_TUJUAN.RemoveRange(semakTujuan);
        //        db.SaveChanges();

        //        int LastIDtujuan = 0;
        //        foreach (HR_PERBATUAN_TUJUAN tujuanan in mPerbatuan.HR_PERBATUAN_TUJUAN)
        //        {
        //            LastIDtujuan++;
        //            var Kodtujuan = Convert.ToString(LastIDtujuan).PadLeft(4, '0');
        //            tujuanan.HR_KOD_TUJUAN = "T" + Kodtujuan;
        //            tujuanan.HR_NO_PEKERJA = mPink.HR_NO_PEKERJA;
        //            tujuanan.HR_KOD_PERBATUAN = mPink.HR_KOD_PERBATUAN;
        //            db.HR_PERBATUAN_TUJUAN.Add(tujuan);

        //        }
        //        db.SaveChanges();
        //        return RedirectToAction("senaraiperbatuanpink");
        //    }

        //    ViewBag.HR_NAMA_HR = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
        //    ViewBag.HR_JAWATAN_PEGAWAI_HR = new SelectList(db.HR_JAWATAN, "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

        //    return View(mPink);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerbatuanListEdit([Bind(Include = "HR_NO_PEKERJA,HR_KOD_PERBATUAN,HR_KENDERAAN_JENIS,HR_KENDERAAN_NOMBOR,HR_KENDERAAN_KUASA,HR_KENDERAAN_KELAS,HR_MAKSIMA_TUNTUTAN,HR_TARIKH_PERMOHONAN,HR_IND_PEMOHON,HR_TANDATANGAN_PEMOHON,HR_TANDATANGAN_KB,HR_NAMA_KB,HR_TARIKH_KB,HR_TANDATANGAN_KJ,HR_NAMA_KJ,HR_TARIKH_KJ,HR_IND_KB,HR_IND_KJ,HR_TARIKH_KERANIP,HR_IND_KERANIP,HR_TARIKH_KERANIS,HR_IND_KERANIS,DOCUMENT_ID,DESC_FILE,HR_IND_HR,HR_NAMA_HR,HR_TARIKH_HR,HR_CATATAN,HR_FINALISED_IND")] HR_PERBATUAN_PINK pink, PemohonanPinkModels mohon, HR_PERBATUAN_TUNTUTAN tuntut)
        {
          
            if (ModelState.IsValid)
            {
                db.Entry(pink).State = EntityState.Modified;
                db.SaveChanges();

                if (tuntut != null)
                {
                    tuntut.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
                    tuntut.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
                    db.Entry(tuntut).State = EntityState.Modified;
                }

                List<HR_PERBATUAN_TUJUAN> semakTujuan = db.HR_PERBATUAN_TUJUAN.Where(s => s.HR_NO_PEKERJA == pink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == pink.HR_KOD_PERBATUAN).ToList();
                db.HR_PERBATUAN_TUJUAN.RemoveRange(semakTujuan);
                db.SaveChanges();

                int LastIDtujuan = 0;
                foreach (HR_PERBATUAN_TUJUAN tujuan in mohon.HR_PERBATUAN_TUJUAN)
                {
                    if (tujuan.HR_TARIKH != null)
                    {
                        LastIDtujuan++;
                        var Kodtujuan = Convert.ToString(LastIDtujuan).PadLeft(4, '0');
                        tujuan.HR_KOD_TUJUAN = "T" + Kodtujuan;
                        tujuan.HR_NO_PEKERJA = pink.HR_NO_PEKERJA;
                        tujuan.HR_KOD_PERBATUAN = pink.HR_KOD_PERBATUAN;
                        db.HR_PERBATUAN_TUJUAN.Add(tujuan);
                    }
                }
                db.SaveChanges();

                return RedirectToAction("senaraiperbatuanpink");
            }
            List<SelectListItem> HR_LOKASI = new List<SelectListItem>();
            HR_LOKASI.Add(new SelectListItem { Text = "SEMENANJUNG MALAYSIA", Value = "SM" });
            HR_LOKASI.Add(new SelectListItem { Text = "SABAH,SARAWAK DAN LABUAN", Value = "SS" });
            ViewBag.HR_LOKASI = HR_LOKASI;


            List<SelectListItem> bulanbekerja = new List<SelectListItem>
            {
                new SelectListItem { Text = "JANUARI", Value = "1" },
                new SelectListItem { Text = "FEBRUARI", Value = "2" },
                new SelectListItem { Text = "MAC", Value = "3" },
                new SelectListItem { Text = "APRIL", Value = "4" },
                new SelectListItem { Text = "MAY", Value = "5" },
                new SelectListItem { Text = "JUN", Value = "6" },
                new SelectListItem { Text = "JULAI", Value = "7" },
                new SelectListItem { Text = "OGOS", Value = "8" },
                new SelectListItem { Text = "SEPTEMBER", Value = "9" },
                new SelectListItem { Text = "OKTOBER", Value = "10" },
                new SelectListItem { Text = "NOVEMBER", Value = "11" },
                new SelectListItem { Text = "DISEMBER", Value = "12" }
            };
            ViewBag.bulanbekerja = new SelectList(bulanbekerja, "Value", "Text", DateTime.Now.Month - 1);

            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).SingleOrDefault(s => s.HR_NO_PEKERJA == pink.HR_NO_PEKERJA);

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



            if (DateTime.Now.Month == '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year - 1;
            }
            if (DateTime.Now.Month != '1')
            {
                ViewBag.tahunbekerja = DateTime.Now.Year;
            }
            var HR_KOD_GREDELAUN = db.HR_GRED_ELAUN_PEKELILING.ToList();
            ViewBag.HR_KOD_GREDELAUN = HR_KOD_GREDELAUN;

            ViewBag.HR_JABATAN = jabatan.GE_KETERANGAN_JABATAN;
            ViewBag.HR_BAHAGIAN = bahagian.GE_KETERANGAN;
            ViewBag.HR_NAMA_PEKERJA = mPeribadi.HR_NAMA_PEKERJA;
            ViewBag.HR_NO_KPBARU = mPeribadi.HR_NO_KPBARU;
            ViewBag.HR_JAWATAN = jawatan.HR_NAMA_JAWATAN;
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



            ViewBag.pagi = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            ViewBag.tengahari = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            ViewBag.petang = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;
            return View(pink);
        }


        //[HttpPost, ActionName("PerbatuanListPadam")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(HR_PERBATUAN_PINK pink)
        //{
        //    pink = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == pink.HR_NO_PEKERJA && s.HR_KOD_PERBATUAN == pink.HR_KOD_PERBATUAN);

        //    db.HR_PERBATUAN_PINK.Remove(pink);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}



        public ActionResult TambahPerbatuanPutih(string id)
        {
            HR_PERBATUAN_PUTIH mPutih = new HR_PERBATUAN_PUTIH();


            HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
            HR_MAKLUMAT_PEKERJAAN pekerjaan = new HR_MAKLUMAT_PEKERJAAN();


            mPutih.HR_NO_PEKERJA = id;

            mPutih.HR_TARIKH_AKUAN = DateTime.Now;
            var tarikhakuan = string.Format("{0:dd/MM/yyyy}", mPutih.HR_TARIKH_AKUAN);
            ViewBag.HR_TARIKH_AKUAN = tarikhakuan;


            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);



            GE_JABATAN jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);

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
            ViewBag.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;
            ViewBag.HR_GRED = mPekerjaan.HR_GRED;


            return View("TambahPerbatuanPutih", mPutih);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TambahPerbatuanPutih([Bind(Include = "HR_KOD_PERBATUAN,HR_KELAS,HR_KM,HR_SEBAB,HR_TARIKH_AKUAN,HR_SOKONG_KETUA,HR_KELAYAKAN,HR_TARIKH_SAH_PTB,HR_ULASAN_TP,HR_TARIKH_TP,HR_JENIS_IND,HR_NO_PEKERJA,HR_IND_KETUA,HR_IND_PTB,HR_IND_TB,HR_MOHON_IND")] HR_PERBATUAN_PUTIH putih)
        {
            if (ModelState.IsValid)
            {
                var SelectLastID = db.HR_PERBATUAN_PUTIH.OrderByDescending(s => s.HR_KOD_PERBATUAN).FirstOrDefault().HR_KOD_PERBATUAN;
                var LastID = new string(SelectLastID.SkipWhile(x => x == 'P' || x == '0').ToArray());
                var Increment = Convert.ToSingle(LastID) + 1;
                var KodPerbatuan = Convert.ToString(Increment).PadLeft(4, '0');
                putih.HR_KOD_PERBATUAN = "P" + KodPerbatuan;

                db.HR_PERBATUAN_PUTIH.Add(putih);
                db.SaveChanges();
                return RedirectToAction("SenaraiPerbatuan");
            }

            return View(putih);
        }



        public ActionResult SenaraiPerbatuanPink(string key, string value)
        {
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            mPeribadi = CariPekerja(key, value);

            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.gambar = db.HR_GAMBAR_PENGGUNA.ToList<HR_GAMBAR_PENGGUNA>();
            return View(mPeribadi);
        }




        public PartialViewResult PerbatuanList(string key, string value)
        {
            ViewBag.key = key;
            ViewBag.value = value;

            ViewBag.HR_NO_PEKERJA = value;

            List<HR_PERBATUAN_PINK> pink = db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUNTUTAN).Include(s => s.HR_PERBATUAN_TUJUAN).Where(s => s.HR_NO_PEKERJA == value).ToList();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).ToList();
            HR_MAKLUMAT_PERIBADI peribadi = mPeribadi.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
            if (peribadi == null)
            {
                peribadi = new HR_MAKLUMAT_PERIBADI();
            }
            HR_MAKLUMAT_PERIBADI listperibadi= new HR_MAKLUMAT_PERIBADI();

            ViewBag.noPekerja = peribadi.HR_NO_PEKERJA;

            ViewBag.detail = db.HR_PERBATUAN_PINK.ToList<HR_PERBATUAN_PINK>();

            return PartialView("_PerbatuanList", pink);
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

        // GET: Perbatuan/Details/5
      
        // GET: Perbatuan/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_PERBATUAN_PUTIH hR_PERBATUAN_PUTIH = db.HR_PERBATUAN_PUTIH.Find(id);
            if (hR_PERBATUAN_PUTIH == null)
            {
                return HttpNotFound();
            }
            return View(hR_PERBATUAN_PUTIH);
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


       

       

        [HttpPost, ActionName("PerbatuanListPadam")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(HR_PERBATUAN_PINK mPink, string key, string value, string jenis)
        {
            
            db.HR_PERBATUAN_PINK.RemoveRange(db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUNTUTAN).Include(s => s.HR_PERBATUAN_TUJUAN).Where(s => s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN && s.HR_KOD_PERBATUAN == mPink.HR_KOD_PERBATUAN));
            db.SaveChanges();
            return RedirectToAction("SenaraiPerbatuanPink");
        }


        public Microsoft.Office.Interop.Word.Document wordDocument { get; set; }

        //public ActionResult PrintBorang(string id, string kod,string tuju)
        //{
        //    string path_file = Server.MapPath(Url.Content("~/Content/template/"));
        //    var pegawai = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
        //    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
        //    HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);
        //    HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUNTUTAN).Include(s => s.HR_PERBATUAN_TUJUAN).FirstOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
        //    if(pink.HR_PERBATUAN_TUJUAN == null)
        //    {
        //        pink.HR_PERBATUAN_TUJUAN = new List<HR_PERBATUAN_TUJUAN>();
        //    }

        //    if(pink.HR_PERBATUAN_TUNTUTAN == null)
        //    {
        //        pink.HR_PERBATUAN_TUNTUTAN = new HR_PERBATUAN_TUNTUTAN();
        //    }

        //    var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPekerjaan.HR_JAWATAN);
        //    var jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN);
        //    var bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPekerjaan.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPekerjaan.HR_JABATAN).SingleOrDefault();

        //    //var tujuan = db.HR_PERBATUAN_TUJUAN.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
        //    //var tuntutan = db.HR_PERBATUAN_TUNTUTAN.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
        //    //var pink = db.HR_PERBATUAN_PINK.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

        //    int? jarak = pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_JARAK);

        //    List<HR_KADAR_PERBATUAN> kadar2 = new List<HR_KADAR_PERBATUAN>();
        //    kadar2 = CariJarak(pink.HR_KENDERAAN_KELAS, jarak);


        //    //HR_KADAR_PERBATUAN data3 = new HR_KADAR_PERBATUAN();
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 0, 500);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 501, 650);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 651, 800);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 801, 950);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 951, 1100);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1101, 1250);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1251, 1400);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1401, 1550);
        //    //kadar2.Add(data3);
        //    //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1551, 1551);
        //    //kadar2.Add(data3);

        //    int? prevJarak = 0;
        //    int? nextJarak = 0;
        //    decimal? percent = 0;
        //    decimal? makan = 0;
        //    decimal? elaunmakan = 0;
        //    decimal? jumTuntutanPelbagai = 0;
        //    decimal? jumBesarTuntutan = 0;


        //    PinkTuntutanDetailModels data2 = new PinkTuntutanDetailModels();
        //    List<PinkTuntutanDetailModels> kilometer = new List<PinkTuntutanDetailModels>();
        //    foreach (HR_KADAR_PERBATUAN kadar in kadar2)
        //    {
        //        if (jarak >= (kadar.HR_KM_AKHIR - prevJarak))
        //        {
        //            nextJarak = Convert.ToInt32((kadar.HR_KM_AKHIR - prevJarak));
        //            jarak = jarak - nextJarak;
        //            prevJarak = Convert.ToInt32((kadar.HR_KM_AKHIR));
        //            if(jarak <= 0)
        //            {
        //                jarak = 0;
        //            }
        //        }
        //        else
        //        {
        //            nextJarak = jarak;
        //        }

        //        data2.HR_JARAK = nextJarak;
        //        data2.HR_SEN = kadar.HR_NILAI;
        //        data2.HR_JUMLAH = jarak * kadar.HR_NILAI;
        //        kilometer.Add(data2);
        //    }

        //    for(var i = kilometer.Count(); i <=8; i++)
        //    {
        //        kilometer.Add(new PinkTuntutanDetailModels());
        //    }


        //    decimal? peratuspagi = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
        //    decimal? peratustengahari = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
        //    decimal? peratuspetang = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;

        //    foreach (HR_PERBATUAN_TUJUAN tujuan in pink.HR_PERBATUAN_TUJUAN)
        //    {
        //        if (tujuan.HR_RM_MAKAN_P != null)
        //        {
        //            percent += peratuspagi;
        //            makan += tujuan.HR_RM_MAKAN_P;
        //            elaunmakan += (peratuspagi / 100 )* tujuan.HR_RM_MAKAN_P;
        //        }

        //        if (tujuan.HR_RM_MAKAN_T != null)
        //        {
        //            percent += peratustengahari;
        //            makan += tujuan.HR_RM_MAKAN_T;
        //            elaunmakan += (peratustengahari / 100) * tujuan.HR_RM_MAKAN_T;
        //        }

        //        if(tujuan.HR_RM_MAKAN_M != null)
        //        {
        //            percent += peratuspetang;
        //            makan += tujuan.HR_RM_MAKAN_M;
        //            elaunmakan += (peratuspetang / 100) * tujuan.HR_RM_MAKAN_M;
        //        }
        //    }
        //    jumTuntutanPelbagai = pink.HR_PERBATUAN_TUNTUTAN.HR_RM_PENGANGKUTAN + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_CUKAI + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TELEFON + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_DOBI + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TIP + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_GANTIRUGI;
        //    jumBesarTuntutan = kilometer.Sum(s => s.HR_JUMLAH) + makan + pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_HOTEL) + pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_LOJING) + jumTuntutanPelbagai;

        //    var txtTujuan = pink.HR_PERBATUAN_TUJUAN.Select(s => s.HR_TUJUAN);
        //    var joinTxtTujuan = string.Join("\n", txtTujuan);

        //    var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
        //    templateEngine.Process(
        //        source: path_file + "TEMPLATE BORANG PINK.docx",
        //        destination: path_file + "BORANG_PINK.docx",
        //         data: new
        //         {
        //             nama = mPeribadi.HR_NAMA_PEKERJA,
        //             jabatan = jabatan.GE_KETERANGAN_JABATAN,
        //             nokp = mPeribadi.HR_NO_KPBARU,
        //             bahagian = bahagian.GE_KETERANGAN,
        //             jawatan = jawatan.HR_KOD_JAWATAN + " - " + jawatan.HR_NAMA_JAWATAN,
        //             jenis = pink.HR_KENDERAAN_JENIS,
        //             gredgaji = mPekerjaan.HR_GRED,
        //             nombor = pink.HR_KENDERAAN_NOMBOR,
        //             gajipokok = mPekerjaan.HR_GAJI_POKOK,
        //             kuasa = pink.HR_KENDERAAN_KUASA,
        //             alamat = mPeribadi.HR_SALAMAT1, 
        //             kelas = pink.HR_KENDERAAN_KELAS,
        //             maksima = pink.HR_MAKSIMA_TUNTUTAN,

        //             tarikhakui = string.Format("{0:dd/MM/yyyy}", pink.HR_TARIKH_PERMOHONAN),

        //             tarikh = "",
        //             tolak = "",
        //             sampai = "",
        //             tujuan = "",
        //             jarak = "",
        //             jumlah = "",

        //             sen1 = string.Format("{0:#,0.00}", kilometer.ElementAt(0).HR_SEN),
        //             sen2 = string.Format("{0:#,0.00}", kilometer.ElementAt(1).HR_SEN),
        //             sen3 = string.Format("{0:#,0.00}", kilometer.ElementAt(2).HR_SEN),
        //             sen4 = string.Format("{0:#,0.00}", kilometer.ElementAt(3).HR_SEN),
        //             sen5 = string.Format("{0:#,0.00}", kilometer.ElementAt(4).HR_SEN),
        //             sen6 = string.Format("{0:#,0.00}", kilometer.ElementAt(5).HR_SEN),
        //             sen7 = string.Format("{0:#,0.00}", kilometer.ElementAt(6).HR_SEN),
        //             sen8 = string.Format("{0:#,0.00}", kilometer.ElementAt(7).HR_SEN),

        //             km1 = kilometer.ElementAt(0).HR_JARAK,
        //             km2 = kilometer.ElementAt(1).HR_JARAK,
        //             km3 = kilometer.ElementAt(2).HR_JARAK,
        //             km4 = kilometer.ElementAt(3).HR_JARAK,
        //             km5 = kilometer.ElementAt(4).HR_JARAK,
        //             km6 = kilometer.ElementAt(5).HR_JARAK,
        //             km7 = kilometer.ElementAt(6).HR_JARAK,
        //             km8 = kilometer.ElementAt(7).HR_JARAK,

        //             jumkm = string.Format("{0:#,0.00}", kilometer.Sum(s => s.HR_JARAK)),
        //             jum1 = string.Format("{0:#,0.00}", kilometer.ElementAt(0).HR_JUMLAH),
        //             jum2 = string.Format("{0:#,0.00}", kilometer.ElementAt(1).HR_JUMLAH),
        //             jum3 = string.Format("{0:#,0.00}", kilometer.ElementAt(2).HR_JUMLAH),
        //             jum4 = string.Format("{0:#,0.00}", kilometer.ElementAt(3).HR_JUMLAH),
        //             jum5 = string.Format("{0:#,0.00}", kilometer.ElementAt(4).HR_JUMLAH),
        //             jum6 = string.Format("{0:#,0.00}", kilometer.ElementAt(5).HR_JUMLAH),
        //             jum7 = string.Format("{0:#,0.00}", kilometer.ElementAt(6).HR_JUMLAH),
        //             jum8 = string.Format("{0:#,0.00}", kilometer.ElementAt(7).HR_JUMLAH),
        //             jumrm = string.Format("{0:#,0.00}", kilometer.Sum(s => s.HR_JUMLAH)),
        //             totalrm = string.Format("{0:#,0.00}", kilometer.Sum(s => s.HR_JUMLAH)),

        //             jumlahp = percent + "%",
        //             jumlahm = string.Format("{0:#,0.00}", makan),
        //             emakan = string.Format("{0:#,0.00}", makan),

        //             kalihot = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_NILAI_HOTEL)),
        //             rmhotel = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_HOTEL)),
        //             rmloj = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_LOJING)),
        //             ehotel = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_HOTEL)),
        //             kalicuk = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_NILAI_LOJING)),
        //             elojj = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_LOJING)),


        //             rmp = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_PENGANGKUTAN),
        //             rmc = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_CUKAI),
        //             rmtel = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TELEFON),
        //             rmd = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_DOBI),
        //             rmt = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TIP),
        //             rmg = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_GANTIRUGI),
        //             jump = string.Format("{0:#,0.00}", jumTuntutanPelbagai),
        //             jumb = string.Format("{0:#,0.00}", jumBesarTuntutan),
        //             pend = string.Format("{0:#,0.00}", pink.HR_PERBATUAN_TUNTUTAN.HR_RM_PENDAHULUAN),
        //             pendt = string.Format("{0:#,0.00}", jumBesarTuntutan - pink.HR_PERBATUAN_TUNTUTAN.HR_RM_PENDAHULUAN),





        //             //jawatan = jawatan.HR_KOD_JAWATAN + " - " + jawatan.HR_NAMA_JAWATAN,
        //             //mula = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_MANGKU_MULA),
        //             //hingga = string.Format("{0:dd/MM/yyyy}", pegawai.HR_TARIKH_MANGKU_AKHIR),

        //             //tarikhlawatan = string.Format("{0:dd/MM/yyyy}", pegawai.HR_SEMINAR_LUAR.HR_TARIKH_MULA),
        //             //hinggalawatan = string.Format("{0:dd/MM/yyyy}", pegawai.HR_SEMINAR_LUAR.HR_TARIKH_TAMAT),


        //         });
        //    //Interop+  \
        //    Application appWord = new Application();
        //    wordDocument = appWord.Documents.Open(@"C:\inetpub\wwwroot\espp\webapp\Content\template\BORANG_PINK.docx");
        //    wordDocument.ExportAsFixedFormat(@"C:\inetpub\wwwroot\espp\webapp\Content\template\BORANG_PINK.pdf", WdExportFormat.wdExportFormatPDF);

        //    appWord.Quit();

        //    string FilePath = Server.MapPath("~/Content/template/BORANG_PINK.pdf");
        //    WebClient User = new WebClient();
        //    Byte[] FileBuffer = User.DownloadData(FilePath);
        //    if (FileBuffer != null)
        //    {
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-length", FileBuffer.Length.ToString());
        //        Response.BinaryWrite(FileBuffer);
        //    }
        //    return File(FilePath, "application/pdf");
        //}

        public FileStreamResult PrintBorang(string id, string kod, string tuju)
        {
            string path_file = Server.MapPath(Url.Content("~/Content/template/"));
            var pegawai = db.HR_PERBATUAN_PINK.SingleOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == id);

            HR_PERBATUAN_PINK pink = db.HR_PERBATUAN_PINK.Include(s => s.HR_PERBATUAN_TUNTUTAN).Include(s => s.HR_PERBATUAN_TUJUAN).FirstOrDefault(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            if (pink.HR_PERBATUAN_TUJUAN == null)
            {
                pink.HR_PERBATUAN_TUJUAN = new List<HR_PERBATUAN_TUJUAN>();
            }

            if (pink.HR_PERBATUAN_TUNTUTAN == null)
            {
                pink.HR_PERBATUAN_TUNTUTAN = new HR_PERBATUAN_TUNTUTAN();
            }

            var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            var jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
            var bahagian = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_BAHAGIAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_JABATAN == mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).SingleOrDefault();

            //var tujuan = db.HR_PERBATUAN_TUJUAN.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            //var tuntutan = db.HR_PERBATUAN_TUNTUTAN.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);
            //var pink = db.HR_PERBATUAN_PINK.Where(s => s.HR_NO_PEKERJA == id && s.HR_KOD_PERBATUAN == kod);

            int? jarak = pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_JARAK);

            List<HR_KADAR_PERBATUAN> kadar2 = new List<HR_KADAR_PERBATUAN>();
            kadar2 = CariJarak(pink.HR_KENDERAAN_KELAS, jarak);


            //HR_KADAR_PERBATUAN data3 = new HR_KADAR_PERBATUAN();
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 0, 500);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 501, 650);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 651, 800);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 801, 950);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 951, 1100);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1101, 1250);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1251, 1400);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1401, 1550);
            //kadar2.Add(data3);
            //data3 = SusunanJarak(pink.HR_KENDERAAN_KELAS, 1551, 1551);
            //kadar2.Add(data3);

            int? prevJarak = 0;
            int? nextJarak = 0;
            decimal? percent = 0;
            decimal? makan = 0;
            decimal? elaunmakan = 0;
            decimal? jumTuntutanPelbagai = 0;
            decimal? jumBesarTuntutan = 0;


            PinkTuntutanDetailModels data2 = new PinkTuntutanDetailModels();
            List<PinkTuntutanDetailModels> kilometer = new List<PinkTuntutanDetailModels>();
            foreach (HR_KADAR_PERBATUAN kadar in kadar2)
            {
                if (jarak >= (kadar.HR_KM_AKHIR - prevJarak))
                {
                    nextJarak = Convert.ToInt32((kadar.HR_KM_AKHIR - prevJarak));
                    jarak = jarak - nextJarak;
                    prevJarak = Convert.ToInt32((kadar.HR_KM_AKHIR));
                    if (jarak <= 0)
                    {
                        jarak = 0;
                    }
                }
                else
                {
                    nextJarak = jarak;
                }

                data2.HR_JARAK = nextJarak;
                data2.HR_SEN = kadar.HR_NILAI;
                data2.HR_JUMLAH = jarak * kadar.HR_NILAI;
                kilometer.Add(data2);
            }

            for (var i = kilometer.Count(); i <= 8; i++)
            {
                kilometer.Add(new PinkTuntutanDetailModels());
            }


            decimal? peratuspagi = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "P").HR_NILAI;
            decimal? peratustengahari = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "T").HR_NILAI;
            decimal? peratuspetang = db.HR_PERATUS_MAKAN.FirstOrDefault(s => s.HR_KOD_PERATUS == "M").HR_NILAI;

            foreach (HR_PERBATUAN_TUJUAN tujuan in pink.HR_PERBATUAN_TUJUAN)
            {
                if (tujuan.HR_RM_MAKAN_P != null)
                {
                    percent += peratuspagi;
                    makan += tujuan.HR_RM_MAKAN_P;
                    elaunmakan += (peratuspagi / 100) * tujuan.HR_RM_MAKAN_P;
                }

                if (tujuan.HR_RM_MAKAN_T != null)
                {
                    percent += peratustengahari;
                    makan += tujuan.HR_RM_MAKAN_T;
                    elaunmakan += (peratustengahari / 100) * tujuan.HR_RM_MAKAN_T;
                }

                if (tujuan.HR_RM_MAKAN_M != null)
                {
                    percent += peratuspetang;
                    makan += tujuan.HR_RM_MAKAN_M;
                    elaunmakan += (peratuspetang / 100) * tujuan.HR_RM_MAKAN_M;
                }
            }
            jumTuntutanPelbagai = pink.HR_PERBATUAN_TUNTUTAN.HR_RM_PENGANGKUTAN + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_CUKAI + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TELEFON + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_DOBI + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_TIP + pink.HR_PERBATUAN_TUNTUTAN.HR_RM_GANTIRUGI;
            jumBesarTuntutan = kilometer.Sum(s => s.HR_JUMLAH) + makan + pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_HOTEL) + pink.HR_PERBATUAN_TUJUAN.Sum(s => s.HR_RM_LOJING) + jumTuntutanPelbagai;

            var txtTujuan = pink.HR_PERBATUAN_TUJUAN.Select(s => s.HR_TUJUAN);
            var joinTxtTujuan = string.Join("\n", txtTujuan);

            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);
            var writer = PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();

            var html = "<html><head>";
            html += "<title>Slip</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";

            html += "<p align='center'><u> MAJLIS BANDARAYA PETALING JAYA </u></p>";
            html += "<br/>";
            html += "<p align='center' style='font-size: 90%'>KENYATAAN TUNTUTAN PERBATUAN PERJALANAN UNTUK BULAN _____ 20 ___</p>";
            html += "<hr></hr>";
            html += "<br/>";
            html += "<p><u>MUSTAHAK</u>: Tuntutan dalam 2 salinan hendaklah sampai ke Jabatan Perbendaharaan tidak lewat dari 10hb. bulan berikutnya.</p>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='50' valign='top'>Nama</td>";
            html += "<td width='10' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' align='left' style='border-bottom: 1px solid black'>" + mPeribadi.HR_NAMA_PEKERJA + "</td>";
            html += "<td width='85' valign='top' align='center'>Jabatan</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom'  align='left' style='border-bottom: 1px solid black'>" + jabatan.GE_KETERANGAN_JABATAN + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>No. K/P</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + mPeribadi.HR_NO_KPBARU + "</td>";
            html += "<td width='85' valign='top' align='center'>Bahagian</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + bahagian.GE_KETERANGAN + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>Jawatan</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + jawatan.HR_KOD_JAWATAN + " - " + jawatan.HR_NAMA_JAWATAN + "</td>";
            html += "<td width='85' valign='top' align='center'>Kenderaan</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='66' valign='top'>Jenis</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_KENDERAAN_JENIS + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>Gred Gaji</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED + "</td>";
            html += "<td width='85' valign='top'></td>";
            html += "<td width='19' valign='top'></td>";
            html += "<td width='66' valign='top'>Nombor</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_KENDERAAN_NOMBOR + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>Kategori</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_KENDERAAN_NOMBOR + "</td>";
            html += "<td width='85' valign='top'></td>";
            html += "<td width='19' valign='top'></td>";
            html += "<td width='66' valign='top'></td>";
            html += "<td width='66' valign='top'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>Gaji Pokok</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + mPeribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK + "</td>";
            html += "<td width='85' valign='top'></td>";
            html += "<td width='19' valign='top'></td>";
            html += "<td width='66' valign='top'>Kuasa</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_KENDERAAN_KUASA + " Cc</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'>Alamat</td>";
            html += "<td width='19' valign='top'>:</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + mPeribadi.HR_SALAMAT1 + "</td>";
            html += "<td width='85' valign='top'></td>";
            html += "<td width='19' valign='top'></td>";
            html += "<td width='66' valign='top'>Kelas</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_KENDERAAN_KELAS + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='85' valign='top'></td>";
            html += "<td width='19' valign='top'></td>";
            html += "<td width='255' valign='top'></td>";
            html += "<td width='170' colspan='3' valign='top'  align='right'>Maksima Tuntutan :</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>" + pink.HR_MAKSIMA_TUNTUTAN + " Km</td>";
            html += "</tr>";
            //html += "</tbody>";
            html += "</table>";

            html += "<br/>";

            html += "<table border='1' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='84' rowspan='2' valign='middle' align='center'>Tarikh</td>";
            html += "<td width='169' colspan='2' valign='middle' align='center'>Waktu</td>";
            html += "<td width='326' rowspan='2' valign='middle' align='center'>Tempat/Tujuan</td>";
            html += "<td width='66' rowspan='2' valign='middle' align='center'>Jarak (KM)</td>";
            html += "<td width='72' rowspan='2' valign='middle' align='center'>Jumlah</td>";
            html += "</tr>";

            html += "<tr valign='middle'>";
            html += "<td width='85' valign='middle' align='center'>Bertolak</td>";
            html += "<td width='85' valign='middle' align='center'>Sampai</td>";
            html += "</tr>";
            //html += "</tbody>";
            html += "</table>";

            html += "<br/>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%; page-break-before: always;'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 500 Km pertama sebanyak</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen1»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km1»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum1»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'> Bagi 150 Km selepas 500 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen2»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km2»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum2»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 150 Km selepas 650 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen3»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km3» </td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'> «jum3»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 150 Km selepas 800 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen4»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km4»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum4»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 150 Km selepas 950 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen5»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km5»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum5»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 150 Km selepas 1100 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen6»</td>";
            html += "<td width='135' valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km6»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum6»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Bagi 150 Km selepas 1250 Km</td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'> «sen7»</td>";
            html += "<td width='135' valign='bottom'> sen tiap-tiap 1Km x </td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«km7»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jum7»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210'valign='bottom'>Bagi 150 Km selepas 1400 Km</td> ";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«sen8»</td>";
            html += "<td width='135'valign='bottom'>sen tiap-tiap 1Km x</td>";
            html += "<td width='76' valign='bottom' style='border-bottom:1px solid black'>«km8»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM</td>";
            html += "<td width='65' valign='bottom' style='border-bottom:1px solid black'>«jum8»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='135' valign='bottom'></td>";
            html += "<td width='76' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'></td>";
            html += " </tr>";

            html += "<tr>";
            html += "<td width='210' valign='bottom'>Jumlah :</td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='135' valign='bottom'></td>";
            html += "<td width='76' valign='bottom' style='border-bottom: 1px solid black'>«jumkm»</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jumrm»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='275' colspan='2' valign='bottom'>Had maksima hitungan kilometer :</td>";
            //html += "<td width='65' valign='bottom'></td>";
            html += "<td width='135' valign='bottom'></td>";
            html += "<td width='76' valign='bottom'></td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«totalrm»</td>";
            html += "</tr>";
            //html += "</tbody>";
            html += "</table>";

            html += "<br/>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jumlahp»</td>";
            html += "<td width='185' valign='bottom'> x elaun makan sebanyak RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jumlahm»</td>";
            html += "<td width='171' valign='bottom'></td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«emakan»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'></td>";
            html += "<td width='185' valign='bottom'> x elaun harian sebanyak RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'></td>";
            html += "<td width='171' valign='bottom'></td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«kalihot»</td>";
            html += "<td width='185' valign='bottom'>x elaun hotel sebanyak RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmhotel» </td>";
            html += "<td width='171' valign='bottom'>(beresit)</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«ehotel»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' colspan='4' valign='bottom'> Bayaran perkhidmatan dan cukai perkhidmatan atas sewa hotel</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«kalicuk»</td>";
            html += "<td width='185' valign='bottom'>x elaun loging sebanyak RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmloj»</td>";
            html += "<td width='171' valign='bottom'></td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«elojj»</td>";
            html += "</tr>";

            //html += "</tbody>";
            html += "</table>";
            html += "<br/>";
            html += "<p><u>TUNTUTAN PELBAGAI</u></p>";
            html += "<br/>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='486' valign='bottom'>Tambang pengangkutan</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmp»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Cukai lapangan terbang (beresit)</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmc»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Bayaran telefon , faks dll.</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmtel»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Belanja dobi</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmd»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Tips, gratuities, porterage ( 15% daripada elaun makan)</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmt»</td>";
            html += "</tr>";

            html += "<tr>";
            html += " <td width='486' valign='bottom'>Gantirugi tukaran matawang asing ( 3% daripada jumlah tuntutan)</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«rmg»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Jumlah</td>";
            html += "<td width='38' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='124' valign='bottom' style='border-bottom: 1px solid black'>«jump»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Jumlah besar tuntutan</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«jumb»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Pendahuluan telah diberi (tunai / cek no. ………………..)</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«pend»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Tolak : tuntutan sekarang</td>";
            html += "<td width='65' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='65' valign='bottom' style='border-bottom: 1px solid black'>«pendt»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Baki dituntut / baki dibayar balik</td>";
            html += "<td width='38' valign='bottom' align='right'>=&nbsp;&nbsp;&nbsp;&nbsp;RM </td>";
            html += "<td width='124' valign='bottom' style='border-bottom: 1px solid black'>«pendt»</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'><u></u></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'><u>Catatan :</u></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='486' valign='bottom'>Sila isikan dalam lampiran jika ruangan tidak mencukupi.</td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "<td width='65' valign='bottom'></td>";
            html += "</tr>";
            //html += "</tbody>";
            html += "</table>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%' style='page-break-before:always'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>Saya mengakui bahawa :</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>1. Perjalanan pada tarikh-tarikh tersenut adalah benar dan diatas urusan rasmi.</td>";
            html += "</tr>";

            html += " <tr>";
            html += "<td width='697' colspan='2' valign='bottom'>2. Tuntutan ini dibuat mengikut Pekeliling Perbendaharaan Bil. 2/92 dan Perintah Am Bab ‘B’.</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>3. Butir-butir dalam tuntutan saya ini adalah benar dan saya bertanggugjawab terhadapnya.</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='349' valign='bottom'></td>";
            html += "<td width='349' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='349' valign='bottom'></td>";
            html += "<td width='349' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='349' valign='bottom'>Tarikh : <u>«tarikhakui»</u> </td>";
            html += "<td width='349' valign='bottom'> ……………………………………………. </td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='349' valign='bottom'></td>";
            html += "<td width='349' valign='bottom'>Tandatangan Pegawai</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='349' valign='bottom'></td>";
            html += "<td width='349' valign='bottom'></td>";
            html += "</tr>";

            //html += "</tbody>";
            html += "</table>";

            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>Disahkan bahawa tuntutan diatas adalah kerana urusan rasmi dan disokong.</td>";
            html += " </tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'></td>";
            html += "<td width='480' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Tandatangan Ketua Bahagian :</td>";
            html += "<td width='480' valign='bottom'> ……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Nama Pegawai : </td>";
            html += "<td width='480' valign='bottom'> ……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'> Jawatan :</td>";
            html += "<td width='480' valign='bottom'> ……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Tarikh :</td>";
            html += "<td width='480' valign='bottom'>………………………………………</td>";
            html += "</tr>";

            //html += "</tbody>";
            html += "</table>";
            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>Diluluskan / Tidak diluluskan.</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'></td>";
            html += "<td width='480' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Tandatangan Ketua Jabatan :</td>";
            html += "<td width='480' valign='bottom'> ……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Nama Pegawai :</td>";
            html += "<td width='480' valign='bottom'>……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Jawatan :</td>";
            html += "<td width='480' valign='bottom'> ……………………………………………………………………………………………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='217' valign='bottom'>Tarikh :</td>";
            html += "<td width='480' valign='bottom'> ………………………………………</td>";
            html += "</tr>";

            //html += "</tbody>";
            html += "</table>";
            html += "<table border='0' cellspacing='0' cellpadding='5' width='100%' style='font-size: 90%'>";
            //html += "<tbody>";
            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'><u>CATATAN</u></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='697' colspan='2' valign='bottom'>Bayaran akan dibuat berdasarakan had maksima dibenarkan kecuali kelulusan bertulis diberi olehKetua Jabatan</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'>Diperiksa : ………………………………………………</td>";
            html += "<td width='329' valign='bottom'>Disemak : ………………………………………………</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'>(Kerani Perbelanjaan)</td>";
            html += "<td width='329' valign='bottom'>(Kerani Semakan)</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'>Tarikh : …………………………….</td>";
            html += "<td width='329' valign='bottom'>Tarikh : …………………………….</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td width='368' valign='bottom'></td>";
            html += "<td width='329' valign='bottom'></td>";
            html += "</tr>";

            html += "</table>";
            html += "<br/>";

            html += "</body>";
            html += "</html>";

            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            var input = new MemoryStream(bytes);
            var xmlWorker = XMLWorkerHelper.GetInstance();

            xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");

            //var templateEngine = new swxben.docxtemplateengine.DocXTemplateEngine();
            //templateEngine.Process(
            //    source: path_file + "TEMPLATE BORANG PINK2.docx",
            //    destination: path_file + "BORANG_PINK.docx",
            //     data: new
            //     {
            //         nama = mPeribadi.HR_NAMA_PEKERJA
            //     });
            ////Interop
            //string FilePath = Server.MapPath("~/Content/template/BORANG_PINK.docx");
            //object oMissing = System.Reflection.Missing.Value;
            //Application appWord = new Application();
            //appWord.Visible = true;
            //appWord.Activate();

            //wordDocument = appWord.Documents.Open(FilePath);
            //appWord.Selection.Find.ClearFormatting();
            //appWord.Selection.Find.Replacement.ClearFormatting();

            //Microsoft.Office.Interop.Word.Table table = wordDocument.Tables[1];

            //var noRow = 2;
            //table.Rows[1].HeadingFormat = -1;
            //foreach (HR_PERBATUAN_TUJUAN vt in pink.HR_PERBATUAN_TUJUAN)
            //{
            //    table.Rows.Add();
            //    table.Rows[noRow].Cells[1].Range.Text = string.Format("{0:dd/MM/yyyy}", vt.HR_TARIKH);
            //    table.Rows[noRow].Cells[2].Range.Text = vt.HR_WAKTU_BERTOLAK;
            //    table.Rows[noRow].Cells[3].Range.Text = vt.HR_WAKTU_SAMPAI;
            //    table.Rows[noRow].Cells[4].Range.Text = vt.HR_TUJUAN;
            //    table.Rows[noRow].Cells[5].Range.Text = Convert.ToString(vt.HR_JARAK);
            //    table.Rows[noRow].Cells[6].Range.Text = string.Format("{0:#,0.00}", vt.HR_JUMLAH);

            //    noRow++;
            //}
            //wordDocument.Save();
            //wordDocument.Application.Quit();
            //appWord.Quit();
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(appWord);

            //System.IO.FileInfo file = new System.IO.FileInfo(FilePath);

            //Response.Clear();
            //Response.AddHeader("content-length", file.Length.ToString());
            //Response.AddHeader("content-disposition", "attachment; filename = BORANG_PINK(" + mPeribadi.HR_NAMA_PEKERJA + ").docx");
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            //Response.TransmitFile(path_file + "BORANG_PINK.docx");
            //Response.Flush();
            //Response.Close();


            //return View();
        }

        public HR_KADAR_PERBATUAN SusunanJarak(string kelas, decimal HR_KM_MULA, decimal HR_KM_AKHIR)
        {
            HR_KADAR_PERBATUAN data = new HR_KADAR_PERBATUAN();

            HR_KADAR_PERBATUAN kadar = db.HR_KADAR_PERBATUAN.FirstOrDefault(s => s.HR_KELAS == kelas && (HR_KM_AKHIR >= s.HR_KM_MULA && HR_KM_AKHIR <= s.HR_KM_AKHIR));
            data.HR_KELAS = kadar.HR_KELAS;
            data.HR_NILAI = kadar.HR_NILAI;
            data.HR_KM_MULA = HR_KM_MULA;
            data.HR_KM_AKHIR = HR_KM_AKHIR;
            return data;
        }

        public List<HR_KADAR_PERBATUAN> CariJarak(string kelas, decimal? jarak)
        {
            var jumJarak = jarak;
            List<HR_KADAR_PERBATUAN> kadar2 = new List<HR_KADAR_PERBATUAN>();
            //List<HR_KADAR_PERBATUAN> kadar = db.HR_KADAR_PERBATUAN.Where(s => s.HR_KELAS == kelas && s.HR_KM_MULA <= jarak).OrderBy(s => s.HR_KM_MULA).ToList();


            HR_KADAR_PERBATUAN data = new HR_KADAR_PERBATUAN();

            if (jarak > 0)
            {
                data = SusunanJarak(kelas, 0, 500);
                kadar2.Add(data);

                if (jarak >= 501)
                {
                    data = SusunanJarak(kelas, 501, 650);
                    kadar2.Add(data);

                    if (jarak >= 651)
                    {
                        data = SusunanJarak(kelas, 651, 800);
                        kadar2.Add(data);

                        if (jarak >= 801)
                        {
                            data = SusunanJarak(kelas, 801, 950);
                            kadar2.Add(data);

                            if (jarak >= 951)
                            {
                                data = SusunanJarak(kelas, 951, 1100);
                                kadar2.Add(data);

                                if (jarak >= 1101)
                                {
                                    data = SusunanJarak(kelas, 1101, 1250);
                                    kadar2.Add(data);

                                    if (jarak >= 1251)
                                    {
                                        data = SusunanJarak(kelas, 1251, 1400);
                                        kadar2.Add(data);

                                        if (jarak >= 1401)
                                        {
                                            data = SusunanJarak(kelas, 1401, 1550);
                                            kadar2.Add(data);

                                            if (jarak >= 1551)
                                            {
                                                data = SusunanJarak(kelas, 1551, 1551);
                                                kadar2.Add(data);
                                            }
                                        }
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
            return kadar2;
        }

        public JsonResult JsonGredElaun(string HR_LOKASI, string HR_JENIS)
        {
            List<HR_GRED_ELAUN_PEKELILING> elaunMakan = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == HR_LOKASI && s.HR_JENIS == HR_JENIS).ToList();
            return Json(elaunMakan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonGredLojing(string HR_LOKASI, string HR_JENIS, int? HR_NILAI)
        {
            List<HR_GRED_ELAUN_PEKELILING> gredlojing = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == HR_LOKASI && s.HR_JENIS == HR_JENIS && s.HR_NILAI == HR_NILAI).ToList();
            if (gredlojing.Count() <= 0)
            {
                gredlojing = new List<HR_GRED_ELAUN_PEKELILING>();
                gredlojing = db.HR_GRED_ELAUN_PEKELILING.Where(s => s.HR_KATEGORI == HR_LOKASI && s.HR_JENIS == HR_JENIS && s.HR_NILAI == null).ToList();
            }


            return Json(gredlojing, JsonRequestBehavior.AllowGet);
        }
    }
}
