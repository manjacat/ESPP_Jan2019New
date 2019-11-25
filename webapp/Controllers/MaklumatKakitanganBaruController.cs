using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace eSPP.Controllers
{
    public class MaklumatKakitanganBaruController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: MaklumatKakitanganBaru
        public ActionResult Index(string key, string value, string status, int? i)
        {
            var username = User.Identity.Name;
            string pekerjaLogin = null;
            HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == username);
            if (pekerja != null)
            {
                pekerjaLogin = pekerja.HR_NO_PEKERJA;
            }
            ViewBag.pekerjaLogin = pekerjaLogin;
            ViewBag.key = key;
            HR_MAKLUMAT_PERIBADI Peribadi = new HR_MAKLUMAT_PERIBADI();
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = Peribadi.CariPekerja(key, value, status);

            if(mPeribadi.Count() == 1)
            {
                ViewBag.key = "4";
            }

            ViewBag.HR_AKTIF_IND3 = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_GAMBAR_PENGGUNA = db.HR_GAMBAR_PENGGUNA.ToList();
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(mPeribadi);
        }

        public HR_MAKLUMAT_PERIBADI Peribadi(string id)
        {
            HR_MAKLUMAT_PERIBADI model = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == id);
            if (model.HR_MAKLUMAT_PEKERJAAN == null)
            {
                model.HR_MAKLUMAT_PEKERJAAN = new HR_MAKLUMAT_PEKERJAAN();
            }
            return model;
        }

        public Boolean InfoPekerja(HR_MAKLUMAT_PERIBADI model, HttpPostedFileBase file, string jenis)
        {
            try
            {
                if (jenis != null)
                {
                    HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Include(s => s.HR_MAKLUMAT_PEKERJAAN).FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_NO_PEKERJA);
                    if (model.HR_NAMA_PEKERJA != mPeribadi.HR_NAMA_PEKERJA || model.HR_NO_KPBARU != mPeribadi.HR_NO_KPBARU)
                    {
                        mPeribadi.HR_NAMA_PEKERJA = model.HR_NAMA_PEKERJA;
                        mPeribadi.HR_NO_KPBARU = model.HR_NO_KPBARU;
                        db.Entry(mPeribadi).State = EntityState.Modified;
                    }
                }


                HR_GAMBAR_PENGGUNA gambar = db.HR_GAMBAR_PENGGUNA.Find(model.HR_NO_PEKERJA);

                if (file != null)
                {
                    var photoName = "";
                    if (gambar == null)
                    {
                        gambar = new HR_GAMBAR_PENGGUNA();
                    }
                    else
                    {
                        photoName = gambar.HR_PHOTO + gambar.HR_FORMAT_TYPE;
                    }

                    gambar.HR_NO_PEKERJA = model.HR_NO_PEKERJA;
                    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                    var guid = Guid.NewGuid();
                    var format = System.IO.Path.GetExtension(file.FileName);
                    var fileName = guid + System.IO.Path.GetExtension(file.FileName);
                    string physicalPath = Server.MapPath("~/Content/uploads/");
                    var checkImage = Array.FindAll(allowedExtensions, s => s.Equals(format));
                    if (allowedExtensions.Contains(format))
                    {
                        if (checkImage != null)
                        {
                            // save image in folder
                            file.SaveAs(System.IO.Path.Combine(physicalPath, fileName));

                            Bitmap bitmap = new Bitmap(physicalPath + fileName);

                            gambar.HR_PHOTO = guid;
                            gambar.HR_FORMAT_TYPE = format;
                            gambar.HR_BYTE_SIZE = file.ContentLength;
                            gambar.HR_PIX_WIDTH = Convert.ToString(bitmap.Width);
                            gambar.HR_PIX_HEIGHT = Convert.ToString(bitmap.Height);

                            var cariGambar = db.HR_GAMBAR_PENGGUNA.Find(model.HR_NO_PEKERJA);

                            if (cariGambar == null)
                            {
                                db.HR_GAMBAR_PENGGUNA.Add(gambar);
                            }
                            else
                            {
                                string fullPath = Server.MapPath("~/Content/uploads/" + photoName);

                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                                db.Entry(gambar).State = EntityState.Modified;
                            }
                        }
                    }

                }

                db.SaveChanges();
                return true;
            }   
            catch(Exception ex)
            {
                return false;
            }
            
        }

        public ActionResult TabPeribadi(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_PERIBADI model = Peribadi(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            ViewBag.Umur = "";
            if (model.HR_TARIKH_LAHIR != null)
            {
                ViewBag.Umur = (DateTime.Now.Year - Convert.ToDateTime(model.HR_TARIKH_LAHIR).Year);
            }

            if (model.HR_TEMPAT_LAHIR != null)
            {
                model.HR_TEMPAT_LAHIR = model.HR_TEMPAT_LAHIR.Trim();
            }

            if (model.HR_WARGANEGARA != null)
            {
                model.HR_WARGANEGARA = new string(model.HR_WARGANEGARA.TakeWhile(x => char.IsDigit(x)).ToArray());
            }

            if (model.HR_TNEGERI != null)
            {
                model.HR_TNEGERI = model.HR_TNEGERI.Trim();
            }

            if (model.HR_SNEGERI != null)
            {
                model.HR_SNEGERI = model.HR_SNEGERI.Trim();
            }

            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_AGAMA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 106), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_WARGANEGARA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 2), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_TEMPAT_LAHIR = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", model.HR_TEMPAT_LAHIR);
            ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TARAF_KAHWIN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_ALASAN", "HR_PENERANGAN");
            ViewBag.HR_KETURUNAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 1), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION", model.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN);
            return PartialView("TabPeribadi", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TabPeribadi(HR_MAKLUMAT_PERIBADI Peribadi, [Bind(Include = "HR_NO_PEKERJA, HR_GELARAN")]HR_MAKLUMAT_PEKERJAAN Pekerja, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                HR_MAKLUMAT_PERIBADI pengguna = Peribadi.CariPekerja("3", User.Identity.Name, null).FirstOrDefault();
                if(pengguna == null)
                {
                    pengguna = new HR_MAKLUMAT_PERIBADI();
                }

                InfoPekerja(Peribadi, file, null);

                Peribadi.HR_NP_UBAH = pengguna.HR_NO_PEKERJA;
                Peribadi.HR_TARIKH_UBAH = DateTime.Now;
                db.Entry(Peribadi).State = EntityState.Modified;
                db.SaveChanges();
                db.Entry(Pekerja).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect(Url.Action("Index", new { key = '1', value = Peribadi.HR_NO_PEKERJA }));
            }
            return Redirect(Url.Action("Index", new { key = '1', value = Peribadi.HR_NO_PEKERJA }));
        }

        public ActionResult TabPekerjaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HR_MAKLUMAT_PEKERJAAN model = db.HR_MAKLUMAT_PEKERJAAN.Find(id);
            HR_MAKLUMAT_PERIBADI mPeribadi = Peribadi(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            if (model.HR_GRED != null)
            {
                if (model.HR_GRED.Trim() != null)
                {
                    GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(model.HR_GRED.Trim()));
                    if (sGred != null)
                    {
                        model.HR_GRED = sGred.SHORT_DESCRIPTION;

                        HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.FirstOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_GAJI_POKOK == model.HR_GAJI_POKOK);

                        if (matriks == null)
                        {
                            matriks = new HR_MATRIKS_GAJI();
                            matriks.HR_GAJI_MIN = 0;
                            matriks.HR_GAJI_MAX = 0;
                        }

                        ViewBag.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                        ViewBag.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                    }
                }
            }

            if (model.HR_SISTEM != null)
            {
                model.HR_SISTEM = model.HR_SISTEM.Trim();
            }

            if (model.HR_KOD_PCB != null)
            {
                var kodpcb = model.HR_KOD_PCB;
                model.HR_KOD_PCB = new string(model.HR_KOD_PCB.SkipWhile(x => char.IsDigit(x)).TakeWhile(x => (!char.IsDigit(x) && !char.IsWhiteSpace(x)) || char.IsDigit(x)).ToArray());
                ViewBag.HR_KATEGORI_PCB = new string(kodpcb.SkipWhile(x => !char.IsDigit(x)).TakeWhile(x => char.IsDigit(x)).ToArray());
            }

            ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA), "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_GAJI_PRORATA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 116), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN.OrderBy(s => s.GE_KETERANGAN_JABATAN), "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == model.HR_JABATAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.HR_UNIT = new SelectList(db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == model.HR_JABATAN && s.GE_KOD_BAHAGIAN == model.HR_BAHAGIAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_UNIT", "GE_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 115), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUMP_PERKHIDMATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 103), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TARAF_JAWATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");

            List<SelectListItem> HR_UNIFORM = new List<SelectListItem>();
            HR_UNIFORM.Add(new SelectListItem { Text = "Y", Value = "Y" });
            HR_UNIFORM.Add(new SelectListItem { Text = "T", Value = "T" });
            ViewBag.HR_UNIFORM = HR_UNIFORM;

            List<SelectListItem> HR_KUMPULAN = new List<SelectListItem>();
            HR_KUMPULAN.Add(new SelectListItem { Text = "A", Value = "A" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "B", Value = "B" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "C", Value = "C" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "D", Value = "D" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "JUSA", Value = "JUSA" });
            ViewBag.HR_KUMPULAN = HR_KUMPULAN;

            ViewBag.HR_KOD_BANK = new SelectList(db.HR_AGENSI.Where(s => s.HR_PERBANKAN == "Y").OrderBy(s => s.HR_NAMA_AGENSI), "HR_KOD_AGENSI", "HR_NAMA_AGENSI");
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == model.HR_SISTEM).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            Nullable<int> HR_UMUR_SARA = null;
            if (model.HR_TARIKH_TAMAT != null && mPeribadi.HR_TARIKH_LAHIR != null)
            {
                HR_UMUR_SARA = (Convert.ToDateTime(model.HR_TARIKH_TAMAT).Year - Convert.ToDateTime(mPeribadi.HR_TARIKH_LAHIR).Year);
            }
            ViewBag.HR_UMUR_SARA = HR_UMUR_SARA;

            ViewBag.HR_MATRIKS_GAJI = new SelectList(db.HR_MAKLUMAT_PEKERJAAN, "HR_MATRIKS_GAJI", "HR_MATRIKS_GAJI");
            ViewBag.HR_KOD_GELARAN_J = new SelectList(db.HR_GELARAN_JAWATAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_GELARAN", "HR_PENERANGAN");
            ViewBag.HR_TINGKATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 113), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI_PCB2 = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");

            ViewBag.HR_JUM_TAHUN = null;
            if (model.HR_TARIKH_TAMAT_KONTRAK != null)
            {
                ViewBag.HR_JUM_TAHUN = Convert.ToDateTime(model.HR_TARIKH_TAMAT_KONTRAK).Year - Convert.ToDateTime(model.HR_TARIKH_MASUK).Year;
            }

            List<HR_MAKLUMAT_PENGALAMAN_KERJA> sPengalaman = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == id).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
            ViewBag.sPengalaman = sPengalaman;
            return PartialView("TabPekerjaan", model);
        }

        public ActionResult MaklumatPekerja(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            if (mPeribadi == null)
            {
                return HttpNotFound();
            }

            ViewBag.Umur = "";
            if (mPeribadi.HR_TARIKH_LAHIR != null)
            {
                ViewBag.Umur = (DateTime.Now.Year - Convert.ToDateTime(mPeribadi.HR_TARIKH_LAHIR).Year);
            }

            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem { Text = "Aktif", Value = "Y" });
            status.Add(new SelectListItem { Text = "Tidak Aktif", Value = "T" });
            status.Add(new SelectListItem { Text = "Pesara", Value = "P" });
            status.Add(new SelectListItem { Text = "Tahan Gaji", Value = "N" });
            status.Add(new SelectListItem { Text = "Gantung", Value = "G" });
            ViewBag.status = status;

            List<SelectListItem> Agama = new List<SelectListItem>();
            Agama.Add(new SelectListItem { Text = "Islam", Value = "I" });
            Agama.Add(new SelectListItem { Text = "Budha", Value = "B" });
            Agama.Add(new SelectListItem { Text = "Hindu", Value = "H" });
            Agama.Add(new SelectListItem { Text = "Kristian", Value = "K" });
            Agama.Add(new SelectListItem { Text = "Lain-Lain", Value = "L" });
            ViewBag.Agama = Agama;

            List<SelectListItem> Keturunan = new List<SelectListItem>();
            Keturunan.Add(new SelectListItem { Text = "Melayu", Value = "1" });
            Keturunan.Add(new SelectListItem { Text = "Cina", Value = "2" });
            Keturunan.Add(new SelectListItem { Text = "India", Value = "3" });
            Keturunan.Add(new SelectListItem { Text = "Lain-Lain", Value = "4" });
            ViewBag.Keturunan = Keturunan;

            List<SelectListItem> Warganegara = new List<SelectListItem>();
            Warganegara.Add(new SelectListItem { Text = "Malaysia", Value = "1  " });
            Warganegara.Add(new SelectListItem { Text = "Lain-Lain", Value = "0  " });

            ViewBag.Warganegara = Warganegara;

            List<SelectListItem> TempatLahir = new List<SelectListItem>();
            TempatLahir.Add(new SelectListItem { Text = "Johor", Value = "1 " });
            TempatLahir.Add(new SelectListItem { Text = "Kedah", Value = "2 " });
            TempatLahir.Add(new SelectListItem { Text = "Kelantan", Value = "3 " });
            TempatLahir.Add(new SelectListItem { Text = "Melaka", Value = "4 " });
            TempatLahir.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "5 " });
            TempatLahir.Add(new SelectListItem { Text = "Pahang", Value = "6 " });
            TempatLahir.Add(new SelectListItem { Text = "Pulau Pinang", Value = "7 " });
            TempatLahir.Add(new SelectListItem { Text = "Perak", Value = "8 " });
            TempatLahir.Add(new SelectListItem { Text = "Perlis", Value = "9 " });
            TempatLahir.Add(new SelectListItem { Text = "Selangor", Value = "10 " });
            TempatLahir.Add(new SelectListItem { Text = "Terengganu", Value = "11 " });
            TempatLahir.Add(new SelectListItem { Text = "Sabah", Value = "12 " });
            TempatLahir.Add(new SelectListItem { Text = "Sarawak", Value = "13 " });
            TempatLahir.Add(new SelectListItem { Text = "W. P. Kuala Lumpur", Value = "14 " });
            TempatLahir.Add(new SelectListItem { Text = "W. P. Labuan", Value = "15 " });
            TempatLahir.Add(new SelectListItem { Text = "W. P. Putrajaya", Value = "16 " });
            ViewBag.TempatLahir = TempatLahir;

            List<SelectListItem> TarafKahwin = new List<SelectListItem>();
            TarafKahwin.Add(new SelectListItem { Text = "Bujang", Value = "1" });
            TarafKahwin.Add(new SelectListItem { Text = "Berkahwin", Value = "2" });
            TarafKahwin.Add(new SelectListItem { Text = "Duda", Value = "3" });
            TarafKahwin.Add(new SelectListItem { Text = "Janda", Value = "4" });
            ViewBag.TarafKahwin = TarafKahwin;

            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN, "HR_KOD_ALASAN", "HR_PENERANGAN");

            return View(mPeribadi);
        }

        public PartialViewResult JenisCuti(MaklumatCuti mCuti, string NoPekerja, string HR_TARIKH_PERMOHONAN)
        {
            //dpt Max Cuti
            int JumMaxCuti = 0;

            HR_CUTI maxCuti = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI);
            if (maxCuti == null)
            {
                maxCuti = new HR_CUTI();
            }

            if (mCuti.HR_KOD_CUTI == "CU017")
            {
                string tahunHadapan = "01/01/" + (mCuti.HR_TAHUN);
                DateTime? TarikhMohon = Convert.ToDateTime(tahunHadapan);
                HR_MAKLUMAT_CUTI cuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == "CU018" && s.HR_NO_PEKERJA == NoPekerja && s.HR_TARIKH_PERMOHONAN == TarikhMohon);

                if (cuti == null)
                {
                    cuti = new HR_MAKLUMAT_CUTI();
                    cuti.HR_JUMLAH_MAKSIMUM = 0;
                }

                maxCuti.HR_JUMLAH_CUTI = cuti.HR_JUMLAH_MAKSIMUM;
            }
            //
            // jumlah cuti yg telah diambil
            List<HR_MAKLUMAT_CUTI> BilCuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == NoPekerja && s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI && s.HR_TAHUN == mCuti.HR_TAHUN).ToList();
            if (BilCuti.Count() <= 0)
            {
                BilCuti = new List<HR_MAKLUMAT_CUTI>();
            }
            var jumCuti = 0;
            foreach (var data in BilCuti)
            {
                jumCuti = jumCuti + Convert.ToInt32(data.HR_BIL_CUTI_TEMP);
            }
            //
            if (mCuti.HR_KOD_CUTI != null && mCuti.HR_TAHUN != null)
            {
                HR_MAKLUMAT_PEKERJAAN pekerja = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == NoPekerja);
                int bakiCutiTahunLepas = 0;
                if (pekerja != null && pekerja.HR_TARIKH_MASUK.Value.Year < mCuti.HR_TAHUN)
                {

                    JumMaxCuti = Convert.ToInt32(maxCuti.HR_JUMLAH_CUTI);
                    for (int i = pekerja.HR_TARIKH_MASUK.Value.Year; i <= mCuti.HR_TAHUN; i++)
                    {
                        if (mCuti.HR_KOD_CUTI == "CU001")
                        {
                            HR_MAKLUMAT_CUTI cuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == "CU018" && s.HR_NO_PEKERJA == NoPekerja && s.HR_TAHUN == i);

                            if (cuti == null)
                            {
                                cuti = new HR_MAKLUMAT_CUTI();
                                cuti.HR_JUMLAH_MAKSIMUM = 0;
                            }
                            maxCuti.HR_JUMLAH_CUTI = cuti.HR_JUMLAH_MAKSIMUM;
                            JumMaxCuti = Convert.ToInt32(maxCuti.HR_JUMLAH_CUTI);
                        }

                        List<HR_MAKLUMAT_CUTI> item = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == NoPekerja && s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI && s.HR_TAHUN == i).OrderBy(s => s.HR_TAHUN).ToList();
                        if (item.Count() > 0)
                        {
                            var bakiCuti = 0;
                            foreach (var data in item)
                            {
                                bakiCuti = bakiCuti + Convert.ToInt32(data.HR_BIL_CUTI_TEMP);

                            }
                            bakiCutiTahunLepas = Convert.ToInt32(JumMaxCuti) - bakiCuti;
                        }
                        else
                        {

                            bakiCutiTahunLepas = bakiCutiTahunLepas + Convert.ToInt32(JumMaxCuti);
                        }
                    }

                }
                mCuti.HR_BAKI_TAHUN_LEPAS = Convert.ToInt16(bakiCutiTahunLepas);
            }


            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            mKakitangan.HR_MAKLUMAT_CUTI = mCuti;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = Convert.ToDateTime(HR_TARIKH_PERMOHONAN);
            mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = Convert.ToInt16(maxCuti.HR_JUMLAH_CUTI);
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_DIAMBIL = jumCuti;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TERKINI = maxCuti.HR_JUMLAH_CUTI - jumCuti;
            if (mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TERKINI == null)
            {
                mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TERKINI = 0;
            }
            //mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = Convert.ToInt16(bakiCutiTahunLepas);


            if (mCuti.HR_KOD_CUTI == "CU018")
            {
                HR_MAKLUMAT_PEKERJAAN Pekerja = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == NoPekerja);
                if (Pekerja == null)
                {
                    Pekerja = new HR_MAKLUMAT_PEKERJAAN();
                }
                var tahun_depan = mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN - 1;
                //mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                var tarikh_depan = "01/01/" + tahun_depan;
                mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN = Convert.ToInt16(tahun_depan);
                mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = Convert.ToDateTime(tarikh_depan);
                mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_MULA_CUTI = Convert.ToDateTime(tarikh_depan);
                mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_TAMAT_CUTI = Convert.ToDateTime(Pekerja.HR_TARIKH_TAMAT);
                if (DateTime.Now.Year == mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN)
                {
                    tahun_depan = mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN;
                }
                var cuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == NoPekerja && s.HR_KOD_CUTI == "CU001" && s.HR_TAHUN == tahun_depan).OrderByDescending(s => s.HR_TARIKH_PERMOHONAN).FirstOrDefault();
                if (cuti == null)
                {
                    cuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == NoPekerja && s.HR_KOD_CUTI == "CU018" && s.HR_TAHUN == tahun_depan).OrderByDescending(s => s.HR_TARIKH_PERMOHONAN).FirstOrDefault();
                    if (cuti == null)
                    {
                        cuti = new HR_MAKLUMAT_CUTI();
                        cuti.HR_JUMLAH_MAKSIMUM = 0;
                        cuti.HR_BAKI_CUTI_REHAT = 0;
                        cuti.HR_BAKI_TAHUN_LEPAS = 0;
                        cuti.HR_BAKI_PENCEN = 0;
                        cuti.HR_BAKI_PENCEN_TERKUMPUL = 0;
                    }
                }

                if (DateTime.Now.Year == mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN)
                {
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = cuti.HR_JUMLAH_MAKSIMUM;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = cuti.HR_BAKI_CUTI_REHAT;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = cuti.HR_BAKI_CUTI_REHAT;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = 0;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = 0;
                }
                else if (DateTime.Now.Year > mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN)
                {
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = cuti.HR_JUMLAH_MAKSIMUM;
                    var BakiCutiRehat = cuti.HR_BAKI_TAHUN_LEPAS + cuti.HR_BAKI_PENCEN;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = Convert.ToInt16(BakiCutiRehat);
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = cuti.HR_BAKI_TAHUN_LEPAS;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = cuti.HR_BAKI_PENCEN;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = cuti.HR_BAKI_PENCEN_TERKUMPUL;
                }

                return PengumpulanCuti(mKakitangan, NoPekerja);
            }
            else
            {
                mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = DateTime.Now;
                return Cuti(mKakitangan, NoPekerja);
            }
        }

        public PartialViewResult DataCuti(MaklumatCuti mCuti, string KodCuti, string NoPekerja, string HR_TARIKH_PERMOHONAN)
        {
            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            var tarikhPermohonan = Convert.ToDateTime(HR_TARIKH_PERMOHONAN);
            HR_MAKLUMAT_CUTI sCuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == KodCuti && s.HR_NO_PEKERJA == NoPekerja && s.HR_TARIKH_PERMOHONAN == tarikhPermohonan);
            HR_MAKLUMAT_CUTI_DETAIL sCutiDetail = db.HR_MAKLUMAT_CUTI_DETAIL.SingleOrDefault(s => s.HR_KOD_CUTI == sCuti.HR_KOD_CUTI && s.HR_NO_PEKERJA == sCuti.HR_NO_PEKERJA && s.HR_TARIKH_PERMOHONAN == sCuti.HR_TARIKH_PERMOHONAN);
            HR_CUTI cuti = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == KodCuti);

            mKakitangan.HR_MAKLUMAT_CUTI = new MaklumatCuti();
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA = NoPekerja;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI = sCuti.HR_KOD_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = sCuti.HR_TARIKH_PERMOHONAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = sCuti.HR_BAKI_CUTI_REHAT;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = sCuti.HR_JUMLAH_MAKSIMUM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = sCuti.HR_BAKI_TAHUN_LEPAS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = sCuti.HR_BAKI_PENCEN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN = sCuti.HR_TAHUN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_TEMP = sCuti.HR_BIL_CUTI_TEMP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = sCuti.HR_BAKI_PENCEN_TERKUMPUL;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_KELAYAKAN_BULANAN = sCuti.HR_KELAYAKAN_BULANAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_DIAMBIL = sCuti.HR_BIL_CUTI_DIAMBIL;

            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_MULA_CUTI = sCutiDetail.HR_TARIKH_MULA_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_TAMAT_CUTI = sCutiDetail.HR_TARIKH_TAMAT_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT1 = sCutiDetail.HR_CALAMAT1;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT2 = sCutiDetail.HR_CALAMAT2;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT3 = sCutiDetail.HR_CALAMAT3;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CBANDAR = sCutiDetail.HR_CBANDAR;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CPOSKOD = sCutiDetail.HR_CPOSKOD;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CNEGERI = sCutiDetail.HR_CNEGERI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TEL = sCutiDetail.HR_TEL;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_PENGGANTI = sCutiDetail.HR_NP_PENGGANTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_PROGRAM = sCutiDetail.HR_NAMA_PROGRAM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TEMPAT_PROGRAM = sCutiDetail.HR_TEMPAT_PROGRAM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ANJURAN = sCutiDetail.HR_ANJURAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_ISTERI_BERSALIN = sCutiDetail.HR_TARIKH_ISTERI_BERSALIN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ALASAN = sCutiDetail.HR_ALASAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HUBUNGAN = sCutiDetail.HR_HUBUNGAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_SIRI = sCutiDetail.HR_NO_SIRI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_HARI_CUTI = sCutiDetail.HR_BIL_HARI_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_SOKONG_IND = sCutiDetail.HR_SOKONG_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_SOKONG = sCutiDetail.HR_TARIKH_SOKONG;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_IND = sCutiDetail.HR_LULUS_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS = sCutiDetail.HR_TARIKH_LULUS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_KJ = sCutiDetail.HR_NP_KJ;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HR_LULUS_IND = sCutiDetail.HR_HR_LULUS_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS_HR = sCutiDetail.HR_TARIKH_LULUS_HR;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_LULUS = sCutiDetail.HR_NO_PEKERJA_LULUS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ULASAN = sCutiDetail.HR_ULASAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_YDP_IND = sCutiDetail.HR_LULUS_YDP_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_YDP = sCutiDetail.HR_TARIKH_YDP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_YDP = sCutiDetail.HR_NO_PEKERJA_YDP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HARI_CUTI = sCutiDetail.HR_HARI_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_KLINIK = sCutiDetail.HR_NAMA_KLINIK;

            mKakitangan.HR_MAKLUMAT_CUTI.DISABLED = mCuti.DISABLED;

            mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI = cuti.HR_KATEGORI;
            if (cuti.HR_KATEGORI != null)
            {
                mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI = cuti.HR_KATEGORI.Trim();
            }

            if (KodCuti == "CU018")
            {
                return PengumpulanCuti(mKakitangan, NoPekerja);
            }
            else
            {
                return Cuti(mKakitangan, NoPekerja);
            }
        }
        public ActionResult KategoriPCB(int id)
        {
            db2.Configuration.ProxyCreationEnabled = false;
            List<GE_PARAMTABLE> item = new List<GE_PARAMTABLE>();
            if (id == 1)
            {
                item = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102 && s.ORDINAL == id).ToList();
            }
            else
            {
                item = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102 && s.ORDINAL != 1).ToList();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DataCuti2(MaklumatCuti mCuti, string KodCuti, string NoPekerja, string HR_TARIKH_PERMOHONAN)
        {
            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            var tarikhPermohonan = Convert.ToDateTime(HR_TARIKH_PERMOHONAN);
            HR_MAKLUMAT_CUTI sCuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == KodCuti && s.HR_NO_PEKERJA == NoPekerja && s.HR_TARIKH_PERMOHONAN == tarikhPermohonan);
            HR_MAKLUMAT_CUTI_DETAIL sCutiDetail = db.HR_MAKLUMAT_CUTI_DETAIL.SingleOrDefault(s => s.HR_KOD_CUTI == sCuti.HR_KOD_CUTI && s.HR_NO_PEKERJA == sCuti.HR_NO_PEKERJA && s.HR_TARIKH_PERMOHONAN == sCuti.HR_TARIKH_PERMOHONAN);
            HR_CUTI cuti = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == KodCuti);

            mKakitangan.HR_MAKLUMAT_CUTI = new MaklumatCuti();
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA = NoPekerja;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI = sCuti.HR_KOD_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = sCuti.HR_TARIKH_PERMOHONAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = sCuti.HR_BAKI_CUTI_REHAT;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = sCuti.HR_JUMLAH_MAKSIMUM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = sCuti.HR_BAKI_TAHUN_LEPAS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = sCuti.HR_BAKI_PENCEN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN = sCuti.HR_TAHUN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_TEMP = sCuti.HR_BIL_CUTI_TEMP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = sCuti.HR_BAKI_PENCEN_TERKUMPUL;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_KELAYAKAN_BULANAN = sCuti.HR_KELAYAKAN_BULANAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_DIAMBIL = sCuti.HR_BIL_CUTI_DIAMBIL;

            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_MULA_CUTI = sCutiDetail.HR_TARIKH_MULA_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_TAMAT_CUTI = sCutiDetail.HR_TARIKH_TAMAT_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT1 = sCutiDetail.HR_CALAMAT1;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT2 = sCutiDetail.HR_CALAMAT2;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT3 = sCutiDetail.HR_CALAMAT3;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CBANDAR = sCutiDetail.HR_CBANDAR;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CPOSKOD = sCutiDetail.HR_CPOSKOD;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_CNEGERI = sCutiDetail.HR_CNEGERI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TEL = sCutiDetail.HR_TEL;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_PENGGANTI = sCutiDetail.HR_NP_PENGGANTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_PROGRAM = sCutiDetail.HR_NAMA_PROGRAM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TEMPAT_PROGRAM = sCutiDetail.HR_TEMPAT_PROGRAM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ANJURAN = sCutiDetail.HR_ANJURAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_ISTERI_BERSALIN = sCutiDetail.HR_TARIKH_ISTERI_BERSALIN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ALASAN = sCutiDetail.HR_ALASAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HUBUNGAN = sCutiDetail.HR_HUBUNGAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_SIRI = sCutiDetail.HR_NO_SIRI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_HARI_CUTI = sCutiDetail.HR_BIL_HARI_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_SOKONG_IND = sCutiDetail.HR_SOKONG_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_SOKONG = sCutiDetail.HR_TARIKH_SOKONG;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_IND = sCutiDetail.HR_LULUS_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS = sCutiDetail.HR_TARIKH_LULUS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_KJ = sCutiDetail.HR_NP_KJ;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HR_LULUS_IND = sCutiDetail.HR_HR_LULUS_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS_HR = sCutiDetail.HR_TARIKH_LULUS_HR;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_LULUS = sCutiDetail.HR_NO_PEKERJA_LULUS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_ULASAN = sCutiDetail.HR_ULASAN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_YDP_IND = sCutiDetail.HR_LULUS_YDP_IND;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_YDP = sCutiDetail.HR_TARIKH_YDP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_YDP = sCutiDetail.HR_NO_PEKERJA_YDP;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_HARI_CUTI = sCutiDetail.HR_HARI_CUTI;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_KLINIK = sCutiDetail.HR_NAMA_KLINIK;

            //mKakitangan.HR_MAKLUMAT_CUTI.DISABLED = mCuti.DISABLED;

            mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI = cuti.HR_KATEGORI;
            if (cuti.HR_KATEGORI != null)
            {
                mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI = cuti.HR_KATEGORI.Trim();
            }

            return Json(mKakitangan, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Cuti(MaklumatKakitanganModels mKakitangan, string NoPekerja)
        {

            //CUTI
            var Kategori = Convert.ToString(mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI);
            ViewBag.HR_KATEGORI_CUTI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_CUTI = new SelectList(db.HR_CUTI.Select(s => new { HR_KOD_CUTI = s.HR_KOD_CUTI, HR_KATEGORI = s.HR_KATEGORI.Trim(), HR_KETERANGAN = s.HR_KETERANGAN }).Where(s => s.HR_KATEGORI == Kategori), "HR_KOD_CUTI", "HR_KETERANGAN");
            var stc = "Ya";
            stc = stc.PadRight(5, ' ');
            var tc = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() });
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhCuti = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc && s.HR_LULUS_IND == "Y").ToList();
            if (mTarikhCuti.Count() <= 0)
            {
                mTarikhCuti = new List<HR_SENARAI_TARIKH_CUTI>();
            }
            List<string> tarikhcuti = new List<string>();
            foreach (var tarikhCuti in mTarikhCuti)
            {
                tarikhcuti.Add(tarikhCuti.HR_SENARAI_TARIKH.ToShortDateString());
            }

            ViewBag.HR_SENARAI_TARIKH = new MultiSelectList(tc, "HR_SENARAI_TARIKH", "HR_SENARAI_TARIKH", null, tarikhcuti);

            var sbc = "c";
            sbc = sbc.PadRight(5, ' ');
            var tb = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() });
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhBatal = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc && s.HR_LULUS_IND == null).ToList();
            if (mTarikhBatal.Count() <= 0)
            {
                mTarikhBatal = new List<HR_SENARAI_TARIKH_CUTI>();
            }
            List<string> tarikhbatal = new List<string>();
            foreach (var tarikhBatal in mTarikhBatal)
            {
                tarikhbatal.Add(tarikhBatal.HR_SENARAI_TARIKH.ToShortDateString());

            }
            ViewBag.HR_TARIKH_BATAL = new MultiSelectList(tb, "HR_SENARAI_TARIKH", "HR_SENARAI_TARIKH", null, tarikhbatal);
            ViewBag.HR_MAKLUMAT_CUTI_HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125 && s.STRING_PARAM == "K" && s.ORDINAL != 17 && s.ORDINAL != 18), "ORDINAL", "SHORT_DESCRIPTION");
            //
            return PartialView("_Cuti", mKakitangan);
        }
        public PartialViewResult PengumpulanCuti(MaklumatKakitanganModels mKakitangan, string NoPekerja)
        {


            /*mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = cuti.HR_JUMLAH_MAKSIMUM;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = cuti.HR_BAKI_CUTI_REHAT;            
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = cuti.HR_BAKI_TAHUN_LEPAS;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = cuti.HR_BAKI_PENCEN;
            mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = cuti.HR_BAKI_PENCEN_TERKUMPUL;*/

            //CUTI
            var Kategori = Convert.ToString(mKakitangan.HR_MAKLUMAT_CUTI.HR_KATEGORI_CUTI);
            ViewBag.HR_KATEGORI_CUTI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_CUTI = new SelectList(db.HR_CUTI.Select(s => new { HR_KOD_CUTI = s.HR_KOD_CUTI, HR_KATEGORI = s.HR_KATEGORI.Trim(), HR_KETERANGAN = s.HR_KETERANGAN }).Where(s => s.HR_KATEGORI == Kategori), "HR_KOD_CUTI", "HR_KETERANGAN");
            var stc = "Ya";
            stc = stc.PadRight(5, ' ');
            var tc = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() });
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhCuti = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc && s.HR_LULUS_IND == "Y").ToList();
            if (mTarikhCuti.Count() <= 0)
            {
                mTarikhCuti = new List<HR_SENARAI_TARIKH_CUTI>();
            }
            List<string> tarikhcuti = new List<string>();
            foreach (var tarikhCuti in mTarikhCuti)
            {
                tarikhcuti.Add(tarikhCuti.HR_SENARAI_TARIKH.ToShortDateString());

            }

            ViewBag.HR_SENARAI_TARIKH = new MultiSelectList(tc, "HR_SENARAI_TARIKH", "HR_SENARAI_TARIKH", null, tarikhcuti);

            var sbc = "c";
            sbc = sbc.PadRight(5, ' ');
            var tb = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() });
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhBatal = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc && s.HR_LULUS_IND == null).ToList();
            if (mTarikhBatal.Count() <= 0)
            {
                mTarikhBatal = new List<HR_SENARAI_TARIKH_CUTI>();
            }
            List<string> tarikhbatal = new List<string>();
            foreach (var tarikhBatal in mTarikhBatal)
            {
                tarikhbatal.Add(tarikhBatal.HR_SENARAI_TARIKH.ToShortDateString());

            }
            ViewBag.HR_TARIKH_BATAL = new MultiSelectList(tb, "HR_SENARAI_TARIKH", "HR_SENARAI_TARIKH", null, tarikhbatal);
            ViewBag.HR_MAKLUMAT_CUTI_HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125 && s.STRING_PARAM == "K" && s.ORDINAL != 17 && s.ORDINAL != 18), "ORDINAL", "SHORT_DESCRIPTION");
            //

            return PartialView("_PengumpulanCuti", mKakitangan);
        }

        public JsonResult SenaraiCuti(string Kategori, string NoPekerja, string KodCuti)
        {
            KodCuti = KodCuti.PadRight(5, ' ');
            List<MaklumatCuti> item = new List<MaklumatCuti>();
            List<HR_CUTI> cuti = db.HR_CUTI.Where(s => s.HR_KATEGORI == Kategori).ToList();
            foreach (HR_CUTI c in cuti)
            {
                List<HR_MAKLUMAT_CUTI> MaklumatCuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_KOD_CUTI == c.HR_KOD_CUTI && s.HR_NO_PEKERJA == NoPekerja).ToList();
                foreach (HR_MAKLUMAT_CUTI mCuti in MaklumatCuti)
                {
                    HR_MAKLUMAT_CUTI_DETAIL mCutiDetail = db.HR_MAKLUMAT_CUTI_DETAIL.SingleOrDefault(s => s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI && s.HR_NO_PEKERJA == mCuti.HR_NO_PEKERJA && s.HR_TARIKH_PERMOHONAN == mCuti.HR_TARIKH_PERMOHONAN);
                    MaklumatCuti data = new MaklumatCuti();
                    data.HR_NO_PEKERJA = mCuti.HR_NO_PEKERJA;
                    data.HR_KOD_CUTI = mCuti.HR_KOD_CUTI;
                    //data.HR_SOKONG_IND = mCutiDetail.HR_SOKONG_IND;
                    data.HR_LULUS_IND = "T";
                    //data.HR_LULUS_YDP_IND = mCutiDetail.HR_LULUS_YDP_IND;

                    if (Kategori == "1" || (Kategori == "4" && mCuti.HR_KOD_CUTI == KodCuti))
                    {
                        if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_LULUS_IND == "Y")
                        {
                            data.HR_LULUS_IND = "Y";
                        }
                    }
                    if (Kategori == "2")
                    {
                        if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_LULUS_IND == "Y" && mCutiDetail.HR_LULUS_YDP_IND == "Y")
                        {
                            data.HR_LULUS_IND = "Y";
                        }
                    }

                    if (Kategori == "3")
                    {
                        if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_LULUS_YDP_IND == "Y")
                        {
                            data.HR_LULUS_IND = "Y";
                        }
                    }
                    item.Add(data);
                }
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pekerjaan(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == id);

            if (mPekerjaan == null)
            {
                return HttpNotFound();
            }

            ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI, "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");

            return View(mPekerjaan);
        }

        // GET: MaklumatKakitangan/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI = db.HR_MAKLUMAT_PERIBADI.Find(id);
            if (hR_MAKLUMAT_PERIBADI == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_PERIBADI);
        }

        // GET: MaklumatKakitangan/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MaklumatKakitangan/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HR_NO_PEKERJA,HR_NO_KPBARU,HR_NAMA_PEKERJA,HR_NO_KPLAMA,HR_TARIKH_LAHIR,HR_TEMPAT_LAHIR,HR_WARGANEGARA,HR_KETURUNAN,HR_AGAMA,HR_JANTINA,HR_TARAF_KAHWIN,HR_LESEN,HR_KELAS_LESEN,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TPOSKOD,HR_TBANDAR,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_TAHUN_SPM,HR_GRED_BM,HR_TELRUMAH,HR_TELPEJABAT,HR_TELBIMBIT,HR_EMAIL,HR_AKTIF_IND,HR_CC_KENDERAAN,HR_NO_KENDERAAN,HR_JENIS_KENDERAAN,HR_ALASAN,HR_IDPEKERJA,HR_TARIKH_KEYIN,HR_NP_KEYIN,HR_TARIKH_UBAH,HR_NP_UBAH")] HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI)
        {
            if (ModelState.IsValid)
            {
                db.HR_MAKLUMAT_PERIBADI.Add(hR_MAKLUMAT_PERIBADI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hR_MAKLUMAT_PERIBADI);
        }

        // GET: MaklumatKakitangan/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI = db.HR_MAKLUMAT_PERIBADI.Find(id);
            if (hR_MAKLUMAT_PERIBADI == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_PERIBADI);
        }

        // POST: MaklumatKakitangan/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HR_NO_PEKERJA,HR_NO_KPBARU,HR_NAMA_PEKERJA,HR_NO_KPLAMA,HR_TARIKH_LAHIR,HR_TEMPAT_LAHIR,HR_WARGANEGARA,HR_KETURUNAN,HR_AGAMA,HR_JANTINA,HR_TARAF_KAHWIN,HR_LESEN,HR_KELAS_LESEN,HR_TALAMAT1,HR_TALAMAT2,HR_TALAMAT3,HR_TPOSKOD,HR_TBANDAR,HR_TNEGERI,HR_SALAMAT1,HR_SALAMAT2,HR_SALAMAT3,HR_SBANDAR,HR_SPOSKOD,HR_SNEGERI,HR_TAHUN_SPM,HR_GRED_BM,HR_TELRUMAH,HR_TELPEJABAT,HR_TELBIMBIT,HR_EMAIL,HR_AKTIF_IND,HR_CC_KENDERAAN,HR_NO_KENDERAAN,HR_JENIS_KENDERAAN,HR_ALASAN,HR_IDPEKERJA,HR_TARIKH_KEYIN,HR_NP_KEYIN,HR_TARIKH_UBAH,HR_NP_UBAH")] HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hR_MAKLUMAT_PERIBADI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hR_MAKLUMAT_PERIBADI);
        }

        // GET: MaklumatKakitangan/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI = db.HR_MAKLUMAT_PERIBADI.Find(id);
            if (hR_MAKLUMAT_PERIBADI == null)
            {
                return HttpNotFound();
            }
            return View(hR_MAKLUMAT_PERIBADI);
        }

        // POST: MaklumatKakitangan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HR_MAKLUMAT_PERIBADI hR_MAKLUMAT_PERIBADI = db.HR_MAKLUMAT_PERIBADI.Find(id);
            db.HR_MAKLUMAT_PERIBADI.Remove(hR_MAKLUMAT_PERIBADI);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileStreamResult CetakPDF(MaklumatKakitanganModels model)
        {
            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 45, 45, 30, 30);
            var writer = PdfWriter.GetInstance(document, output);
            PageEventHelper page = new PageEventHelper();
            page.jenis = "MK";
            writer.PageEvent = page;
            writer.CloseStream = false;
            document.Open();

            iTextSharp.text.Font contentFont = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font contentFont2 = iTextSharp.text.FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL);
            HR_MAKLUMAT_PERIBADI peribadi = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_PEKERJA == model.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);

            var html = "<html><head>";

            html += "<title>Slip</title><link rel='shortcut icon' href='~/Content/img/logo-mbpj.gif' type='image/x-icon'/></head>";
            html += "<body>";
            if (model.ACTIVE_ITEM == "Peribadi")
            {
                List<GE_PARAMTABLE> negeri = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).ToList();
                List<GE_PARAMTABLE> sTaraf = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4).ToList();

                string taraf = null;

                foreach (GE_PARAMTABLE t in sTaraf)
                {
                    taraf += "&nbsp;&nbsp;&nbsp;&nbsp; " + string.Format("{0:#,0.00}", t.ORDINAL) + " " + t.SHORT_DESCRIPTION;
                }

                var font_size = "86%";
                var style = "style='font-size: 86%;'";
                var style2 = "style='font-size: 66%;'";
                var height = "45";
                html += "<h3>&nbsp;MAKLUMAT PERIBADI</h3>";
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u><strong>NO. PEKERJA &nbsp;:</strong></u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + "><strong>" + peribadi.HR_NO_PEKERJA + "</strong></td>";
                html += "<td valign='top' width='28' " + style + " align='right'><u>ID. PEKERJA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='50' " + style + ">" + peribadi.HR_IDPEKERJA + "</td>";
                html += "<td valign='top' width='23' " + style + "><u>KEAKTIFAN &nbsp;:</u></td>";
                html += "<td valign='top' width='85' " + style + ">" + peribadi.HR_AKTIF_IND + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>NAMA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='12' width='236' " + style + ">" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>NO K/P BARU &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td valign='top' width='28' " + style + " align='right'><u>NO K/P LAMA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='50' " + style + ">" + peribadi.HR_NO_KPLAMA + "</td>";
                html += "<td valign='top' width='23' " + style + "></td>";
                html += "<td valign='top' width='85' " + style + "></td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='38' " + style + "><u>TARAF PERKAHWINAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='10' width='226' " + style + ">" + peribadi.HR_TARAF_KAHWIN + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' rowspan='3' width='38' " + style + "><u>ALAMAT TETAP &nbsp;:</u></td>";
                html += "<td valign='top' colspan='11' width='226' " + style + "><u>1)</u> &nbsp;" + peribadi.HR_TALAMAT1 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='11' width='226' " + style + "><u>2)</u> &nbsp;" + peribadi.HR_TALAMAT2 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='11' width='226' " + style + "><u>3)</u> &nbsp;" + peribadi.HR_TALAMAT3 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>BANDAR &nbsp;:</u></td>";
                html += "<td valign='top' colspan='6' width='84' " + style + ">" + peribadi.HR_TBANDAR + "</td>";
                html += "<td valign='top' colspan='2' width='33' " + style + " align='right'><u>POSKOD &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='109' " + style + ">" + peribadi.HR_TPOSKOD + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>NEGERI &nbsp;:</u></td>";
                int tnegeri = Convert.ToInt32(peribadi.HR_TNEGERI);
                string tnegeri2 = null;
                if (negeri.Exists(s => s.ORDINAL == tnegeri))
                {
                    tnegeri2 = negeri.FirstOrDefault(s => s.ORDINAL == tnegeri).LONG_DESCRIPTION;
                }
                html += "<td valign='top' colspan='11' width='226' " + style + ">" + tnegeri2 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' rowspan='3' width='48' " + style + "><u>ALAMAT SEMENTARA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='10' width='216' " + style + "><u>1)</u> &nbsp;" + peribadi.HR_SALAMAT1 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='10' width='216' " + style + "><u>2)</u> &nbsp;" + peribadi.HR_SALAMAT2 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='10' width='216' " + style + "><u>3)</u> &nbsp;" + peribadi.HR_SALAMAT3 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='48' " + style + "><u>BANDAR &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='74' " + style + ">" + peribadi.HR_SBANDAR + "</td>";
                html += "<td valign='top' colspan='2' width='33' " + style + " align='right'><u>POSKOD &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='109' " + style + ">" + peribadi.HR_SPOSKOD + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='48' " + style + "><u>NEGERI &nbsp;:</u></td>";
                int snegeri = Convert.ToInt32(peribadi.HR_TNEGERI);
                string snegeri2 = null;
                if (negeri.Exists(s => s.ORDINAL == snegeri))
                {
                    snegeri2 = negeri.FirstOrDefault(s => s.ORDINAL == snegeri).LONG_DESCRIPTION;
                }
                html += "<td valign='top' colspan='10' width='216' " + style + ">" + snegeri2 + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>TELEFON RUMAH &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_TELRUMAH + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + " align='right'><u>TELEFON PEJABAT &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='142' " + style + ">" + peribadi.HR_TELPEJABAT + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>TELEFON BIMBIT &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_TELBIMBIT + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + " align='right'></td>";
                html += "<td valign='top' colspan='5' width='142' " + style + "></td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>EMAIL &nbsp;:</u></td>";
                html += "<td valign='top' colspan='11' width='226' " + style + ">" + peribadi.HR_EMAIL + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>KELAS LESEN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_KELAS_LESEN + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + " align='right'></td>";
                html += "<td valign='top' colspan='5' width='142' " + style + "></td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>CC KENDERAAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + string.Format("{0:#,0.00}", peribadi.HR_CC_KENDERAAN) + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + " align='right'></td>";
                html += "<td valign='top' colspan='5' width='142' " + style + "></td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>NO KENDERAAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_NO_KENDERAAN + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + " ><u>JENIS KENDERAAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='142' " + style + ">" + peribadi.HR_JENIS_KENDERAAN + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='13' width='264' " + style2 + ">RUJUKAN &nbsp;: &nbsp;&nbsp;&nbsp; TARAF PERKAHWINAN - &nbsp;&nbsp;&nbsp;&nbsp;" + taraf + "</td>";
                html += "</tr>";
                html += "</table>";
            }

            if (model.ACTIVE_ITEM == "Pekerjaan")
            {
                string jabatan = null;
                if (db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).Count() > 0)
                {
                    jabatan = db2.GE_JABATAN.FirstOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).GE_KETERANGAN_JABATAN;
                }

                string bahagian = null;
                if (db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).Count() > 0)
                {
                    bahagian = db2.GE_BAHAGIAN.FirstOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).GE_KETERANGAN;
                }

                string unit = null;
                if (db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_UNIT == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT).Count() > 0)
                {
                    unit = db2.GE_UNIT.FirstOrDefault(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN && s.GE_KOD_UNIT == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_UNIT).GE_KETERANGAN;
                }

                string gelaran = null;
                if (db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J).Count() > 0)
                {
                    gelaran = db.HR_GELARAN_JAWATAN.FirstOrDefault(s => s.HR_KOD_GELARAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J).HR_PENERANGAN;
                }

                string jawatan = null;
                if (db.HR_JAWATAN.Where(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN).Count() > 0)
                {
                    jawatan = db.HR_JAWATAN.FirstOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN).HR_NAMA_JAWATAN;
                }

                string gred = null;
                if (db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED)).Count() > 0)
                {
                    gred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED)).SHORT_DESCRIPTION;
                }

                string kumpulanperkhidmatan = null;
                if (db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 103 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN)).Count() > 0)
                {
                    kumpulanperkhidmatan = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 103 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN)).SHORT_DESCRIPTION;
                }

                string tarafjawatan = null;
                if (db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN).Count() > 0)
                {
                    tarafjawatan = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN).SHORT_DESCRIPTION;
                }

                string gelaran2 = null;
                if (db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 114 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN)).Count() > 0)
                {
                    gelaran2 = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 114 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN)).SHORT_DESCRIPTION;
                }

                string kategori = null;
                if (db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 115 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI)).Count() > 0)
                {
                    kategori = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 115 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI)).SHORT_DESCRIPTION;
                }

                string bank = null;
                if (db.HR_AGENSI.Where(s => s.HR_KOD_AGENSI == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK).Count() > 0)
                {
                    bank = db.HR_AGENSI.FirstOrDefault(s => s.HR_KOD_AGENSI == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK).HR_NAMA_AGENSI;
                }

                string taraf = null;
                if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    taraf = "KAKITANGAN";
                }
                else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    taraf = "PEKERJA";
                }

                string tingkatan = null;
                if (db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 113 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN)).Count() > 0)
                {
                    tingkatan = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 113 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN)).SHORT_DESCRIPTION;
                }

                var font_size = "86%";
                var style = "style='font-size: 86%;'";
                var style2 = "style='font-size: 66%;'";
                var height = "45";
                html += "<h3>&nbsp;MAKLUMAT PEKERJAAN</h3>";
                html += "<table width='100%' cellpadding='5' cellspacing='0' style='border: 0;'>";

                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u><strong>NO. PEKERJA &nbsp;:</strong></u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + "><strong>" + peribadi.HR_NO_PEKERJA + "</strong></td>";
                html += "<td valign='top' width='30' " + style + "><u>NO. SIRI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='50' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_NO_SIRI + "</td>";
                html += "<td valign='top' width='23' " + style + "><u>STATUS &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='83' " + style + ">" + peribadi.HR_AKTIF_IND + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>NO K/P BARU &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + peribadi.HR_NO_KPBARU + "</td>";
                html += "<td valign='top' width='30' " + style + "><u>NO K/P LAMA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='50' " + style + ">" + peribadi.HR_NO_KPLAMA + "</td>";
                html += "<td valign='top' width='23' " + style + "></td>";
                html += "<td valign='top' colspan='4' width='83' " + style + "></td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>JABATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + jabatan + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>BAHAGIAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='6' width='96' " + style + ">" + bahagian + "</td>";
                html += "<td valign='top' colspan='2' width='33' " + style + " align='right'><u>UNIT &nbsp;:</u></td>";
                html += "<td valign='top' colspan='7' width='107' " + style + ">" + unit + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>NAMA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + peribadi.HR_NAMA_PEKERJA + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>GELARAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + gelaran2 + "</td>";
                html += "<td valign='top' colspan='2' width='40' " + style + "><u>GELARAN JAWATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='8' width='146' " + style + ">" + gelaran + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>JAWATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + jawatan + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>GRED &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + gred + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>KATEGORI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + kategori + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>KUMPULAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN + "</td>";
                html += "<td valign='top' colspan='3' width='50' " + style + "><u>KUMPULAN PERKHIDMATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='7' width='140' " + style + ">" + kumpulanperkhidmatan + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>TARAF JAWATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + tarafjawatan + "</td>";
                html += "<td valign='top' colspan='2' width='40' " + style + "><u>SISTEM SARAAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='8' width='146' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>GAJI POKOK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK) + "</td>";
                html += "<td valign='top' colspan='3' width='50' " + style + "><u>BULAN KENAIKAN GAJI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='2' width='25' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI) + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>MATRIKS GAJI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>NO. AKAUN BANK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_NO_AKAUN_BANK + "</td>";
                html += "<td valign='top' width='30' " + style + "><u>JENIS BANK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='9' width='156' " + style + ">" + bank + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='48' " + style + " ><u>JENIS BAYARAN CEK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='13' width='216' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_BAYARAN_CEK + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>KOD GAJI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI + "</td>";
                html += "<td valign='top' width='30' " + style + "><u>TERIMA GAJI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='50' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>TARIKH GAJI &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_GAJI) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>TARIKH MASUK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK) + "</td>";
                html += "<td valign='top' colspan='3' width='55' " + style + "><u>TARIKH SAH JAWATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='2' width='25' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_SAH_JAWATAN) + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>TARIKH LANTIKAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_LANTIKAN) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>TARIKH TAMAT &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT) + "</td>";
                html += "<td valign='top' colspan='3' width='55' " + style + "><u>TARIKH TAMAT KONTRAK &nbsp;:</u></td>";
                html += "<td valign='top' colspan='2' width='25' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT_KONTRAK) + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>TARIKH TIDAK AKTIF &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TIDAK_AKTIF) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>NO. PENYELIA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENYELIA + "</td>";
                html += "<td valign='top' colspan='3' width='55' " + style + "><u>TARIKH KE JABATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='7' width='131' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KE_JABATAN) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='2' width='38' " + style + "><u>STATUS CARUMAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='4' width='40' " + style + "><u>1) KWSP &nbsp;:</u> &nbsp;" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP + "</td>";
                html += "<td valign='top' colspan='2' width='42' " + style + "><u>2) PERKESO &nbsp;:</u> &nbsp;" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_SOCSO + "</td>";
                html += "<td valign='top' colspan='3' width='37' " + style + "><u>3) PENCEN &nbsp;:</u> &nbsp;" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>4) PCB &nbsp;:</u> &nbsp;" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PCB + "</td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TIDAK_AKTIF) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>KOD PCB &nbsp;:</u></td>";
                html += "<td valign='top' colspan='5' width='50' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_PCB + "</td>";
                html += "<td valign='top' colspan='2' width='42' " + style + "><u>TARIKH MULA PCB &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='37' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_MULA) + "</td>";
                html += "<td valign='top' colspan='2' width='44' " + style + "><u>TARIKH AKHIR PCB &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='62' " + style + ">" + string.Format("{0:dd/MM/yyyy}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_AKHIR) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' width='28' " + style + "><u>NILAI PCB &nbsp;:</u></td>";
                html += "<td valign='top' colspan='15' width='236' " + style + ">" + string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_PCB) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='48' " + style + "><u>KAKITANGAN/PEKERJA &nbsp;:</u></td>";
                html += "<td valign='top' colspan='3' width='30' " + style + ">" + taraf + "</td>";
                html += "<td valign='top' colspan='2' width='40' " + style + "><u>JENIS TINGKATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='8' width='146' " + style + ">" + tingkatan + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='4' width='58' " + style + "><u>NO. FAIL PERKHIDMATAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='12' width='206' " + style + ">" + peribadi.HR_MAKLUMAT_PEKERJAAN.HR_FAIL_PERKHIDMATAN + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='3' width='48' " + style + "><u>KELAS PERJALANAN &nbsp;:</u></td>";
                html += "<td valign='top' colspan='13' width='216' " + style + ">" + string.Format("{0:#,0.00}", peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KELAS_PERJALANAN) + "</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='16' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='16' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='16' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='top' colspan='16' width='264' " + style + ">&nbsp;</td>";
                html += "</tr>";
                html += "<tr>";
                html += "<td valign='bottom' colspan='16' width='264' " + style2 + ">RUJUKAN &nbsp;: &nbsp;&nbsp;&nbsp;T - TIDAK, Y - YA</td>";
                html += "</tr>";
                html += "</table>";
            }

            html += "</body></html>";
            string exportData = string.Format(html);
            var bytes = System.Text.Encoding.UTF8.GetBytes(exportData);
            var input = new MemoryStream(bytes);
            var xmlWorker = XMLWorkerHelper.GetInstance();

            xmlWorker.ParseXHtml(writer, document, input, System.Text.Encoding.UTF8);

            document.Close();
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        //JSON

        [HttpPost]
        public JsonResult SenaraiPekerja(string nilai)
        {
            Object model = new { HR_NAMA_PEKERJA = "", HR_NO_KPBARU = "", HR_NO_PEKERJA = "" };

            if (nilai != null && nilai != "")
            {
                model = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA.Contains(nilai) || s.HR_NO_KPBARU.Contains(nilai) || s.HR_NAMA_PEKERJA.Contains(nilai)).Select(s => new { HR_NAMA_PEKERJA = s.HR_NAMA_PEKERJA, HR_NO_KPBARU = s.HR_NO_KPBARU, HR_NO_PEKERJA = s.HR_NO_PEKERJA }).ToList();
            }
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public JsonResult KiraUmur(string id, string date)
        {
            int? umur = null;
            if (CheckDate(date))
            {
                umur = (DateTime.Now.Year - Convert.ToDateTime(date).Year);

            }
            return Json(umur, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CariGred(string HR_SISTEM)
        {
            if (HR_SISTEM != null)
            {
                HR_SISTEM = new string(HR_SISTEM.TakeWhile(x => !char.IsWhiteSpace(x)).ToArray());
            }

            db2.Configuration.ProxyCreationEnabled = false;
            List<GE_PARAMTABLE> gred = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == HR_SISTEM).ToList();
            if (gred.Count() <= 0)
            {
                gred = new List<GE_PARAMTABLE>();
            }
            return Json(gred, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CariGred2(string HR_GRED)
        //{
        //    List<GE_PARAMTABLE> gred = db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION.Contains(HR_GRED)).ToList();
        //    if (gred.Count() <= 0)
        //    {
        //        gred = new List<GE_PARAMTABLE>();
        //    }
        //    return Json(gred.Select(s => new { SHORT_DESCRIPTION = s.SHORT_DESCRIPTION, ORDINAL = s.ORDINAL } ), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult CariKodKuarters(string HR_KOD_KUARTERS)
        {
            HR_KUARTERS mKuerters = db.HR_KUARTERS.SingleOrDefault(s => s.HR_KOD_KUARTERS == HR_KOD_KUARTERS);
            if (mKuerters == null)
            {
                mKuerters = new HR_KUARTERS();
            }
            return Json(mKuerters, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariNoPekerja(string id)
        {
            HR_MAKLUMAT_PERIBADI item = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NAMA_PEKERJA == id);
            if (item == null)
            {
                item = new HR_MAKLUMAT_PERIBADI();
            }
            return Json(item.HR_NO_PEKERJA, JsonRequestBehavior.AllowGet);
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

        public JsonResult CariPewaris(string id, string no)
        {
            List<HR_MAKLUMAT_PEWARIS> item = db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NAMA_PEWARIS == id && s.HR_NO_PEKERJA == no).ToList<HR_MAKLUMAT_PEWARIS>();
            if (item.Count() <= 0)
            {
                item = new List<HR_MAKLUMAT_PEWARIS>();
            }


            return Json(item, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Bahagian(string KodJabatan)
        {
            db2.Configuration.ProxyCreationEnabled = false;
            List<GE_BAHAGIAN> item = db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == KodJabatan).ToList<GE_BAHAGIAN>();
            if (item.Count() <= 0)
            {
                item = new List<GE_BAHAGIAN>();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Unit(string KodJabatan, string KodBahagian)
        {
            db2.Configuration.ProxyCreationEnabled = false;
            List<GE_UNIT> item = db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == KodJabatan && s.GE_KOD_BAHAGIAN == KodBahagian).ToList<GE_UNIT>();
            if (item.Count() <= 0)
            {
                item = new List<GE_UNIT>();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KiraUmurBersara(string HR_TARIKH_LAHIR, string HR_TARIKH_TAMAT)
        {
            int? umur = null;
            if (CheckDate(HR_TARIKH_LAHIR) && CheckDate(HR_TARIKH_TAMAT))
            {
                umur = (Convert.ToDateTime(HR_TARIKH_TAMAT).Year - Convert.ToDateTime(HR_TARIKH_LAHIR).Year);
            }
            return Json(umur, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KiraUmurBersara2(string HR_TARIKH_LAHIR, int HR_UMUR_SARA)
        {
            DateTime? umur = null;
            if (CheckDate(HR_TARIKH_LAHIR))
            {
                DateTime umur2 = (Convert.ToDateTime(HR_TARIKH_LAHIR));
                umur = umur2.AddYears(HR_UMUR_SARA);
            }

            return Json(umur, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MinGaji(string HR_SISTEM, string HR_GRED)
        {
            if (HR_SISTEM != null)
            {
                HR_SISTEM = new string(HR_SISTEM.TakeWhile(x => !char.IsWhiteSpace(x)).ToArray());
            }

            List<HR_JADUAL_GAJI> item = new List<HR_JADUAL_GAJI>();
            List<HR_JADUAL_GAJI> jadualGaji = db.HR_JADUAL_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_GAJI_MIN).ToList();
            foreach (HR_JADUAL_GAJI item2 in jadualGaji)
            {
                List<HR_MATRIKS_GAJI> matriks = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GAJI_MIN == item2.HR_GAJI_MIN).ToList();
                if (matriks.Count() > 0)
                {
                    item.Add(item2);
                }
            }
            return Json(item.OrderBy(s => s.HR_GAJI_MIN).GroupBy(s => s.HR_GAJI_MIN).Select(s => s.FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaxGaji(string HR_SISTEM, string HR_GRED, decimal? HR_GAJI_MIN)
        {
            if (HR_SISTEM != null)
            {
                HR_SISTEM = new string(HR_SISTEM.TakeWhile(x => !char.IsWhiteSpace(x)).ToArray());
            }

            List<HR_MATRIKS_GAJI> item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_MIN >= HR_GAJI_MIN).OrderByDescending(s => s.HR_GAJI_MAX).GroupBy(s => s.HR_GAJI_MAX).Select(s => s.FirstOrDefault()).ToList();
            if (item.Count() < 0)
            {
                item = new List<HR_MATRIKS_GAJI>();
                HR_MATRIKS_GAJI item2 = new HR_MATRIKS_GAJI();
                item2.HR_GAJI_MIN = 0;
                item2.HR_GAJI_MAX = 0;
                item2.HR_GAJI_POKOK = 0;
                item2.HR_RM_KENAIKAN = 0;
                item2.HR_PERINGKAT = 0;
                item2.HR_TAHAP = 0;

                item.Add(item2);

            }
            return Json(item.OrderBy(s => s.HR_GAJI_MAX).GroupBy(s => s.HR_GAJI_MAX).Select(s => s.FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult JadualGaji(int HR_GRED)
        {
            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            mKakitangan.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
            GE_PARAMTABLE SelectGred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == HR_GRED);
            if (SelectGred == null)
            {
                SelectGred = new GE_PARAMTABLE();
            }

            var JadualGaji = db.HR_JADUAL_GAJI.Where(s => s.HR_GRED_GAJI == SelectGred.SHORT_DESCRIPTION).OrderByDescending(s => s.HR_PERINGKAT).FirstOrDefault();
            if (JadualGaji != null)
            {
                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MIN = JadualGaji.HR_GAJI_MIN;
                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MAX = JadualGaji.HR_GAJI_MAX;
                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI = JadualGaji.HR_KOD_GAJI;
            }
            return Json(mKakitangan, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GajiPokok(string HR_SISTEM, string HR_GRED, decimal? HR_GAJI_MIN, decimal? HR_GAJI_MAX)
        {
            if (HR_SISTEM != null)
            {
                HR_SISTEM = new string(HR_SISTEM.TakeWhile(x => !char.IsWhiteSpace(x)).ToArray());
            }

            List<HR_MATRIKS_GAJI> item = new List<HR_MATRIKS_GAJI>();
            if (HR_GAJI_MIN != null && HR_GAJI_MAX != null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK >= HR_GAJI_MIN && s.HR_GAJI_POKOK <= HR_GAJI_MAX)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else if (HR_GAJI_MIN != null && HR_GAJI_MAX == null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK >= HR_GAJI_MIN)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else if (HR_GAJI_MIN == null && HR_GAJI_MAX != null)
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED && (s.HR_GAJI_POKOK <= HR_GAJI_MAX)).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            else
            {
                item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED).OrderByDescending(s => s.HR_TAHAP).ToList();
            }
            return Json(item.OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).GroupBy(s => new { s.HR_GAJI_POKOK }).Select(s => s.FirstOrDefault()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult KodMatriks(string HR_SISTEM, string HR_GRED, decimal? HR_GAJI_POKOK)
        {
            HR_MATRIKS_GAJI item = db.HR_MATRIKS_GAJI.Where(s => s.HR_SISTEM_SARAAN == HR_SISTEM && s.HR_GRED_GAJI == HR_GRED && s.HR_GAJI_POKOK == HR_GAJI_POKOK).OrderBy(s => s.HR_PERINGKAT).ThenBy(s => s.HR_TAHAP).FirstOrDefault();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JumlahTahunKontrak(int HR_JUM_TAHUN, string HR_TARIKH_MASUK)
        {
            var item = (Convert.ToDateTime(HR_TARIKH_MASUK));
            item = item.AddYears(HR_JUM_TAHUN);

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariPrestasi(int HR_TAHUN_PRESTASI, string HR_NO_PEKERJA)
        {
            HR_PENILAIAN_PRESTASI item = db.HR_PENILAIAN_PRESTASI.SingleOrDefault(s => s.HR_NO_PEKERJA == HR_NO_PEKERJA && s.HR_TAHUN_PRESTASI == HR_TAHUN_PRESTASI);
            if (item == null)
            {
                item = new HR_PENILAIAN_PRESTASI();
            }

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BilCuti(string no_pekerja, string kod_cuti, int? tahun)
        {

            List<HR_MAKLUMAT_CUTI> item = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_CUTI == kod_cuti && s.HR_TAHUN == tahun).ToList();
            if (item.Count() <= 0)
            {
                item = new List<HR_MAKLUMAT_CUTI>();
            }
            var bil = 0;
            foreach (var data in item)
            {
                bil = bil + Convert.ToInt32(data.HR_BIL_CUTI_TEMP);
            }
            return Json(bil, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BilCutiTahunLepas(string no_pekerja, string kod_cuti, int? tahun)
        {

            HR_MAKLUMAT_PEKERJAAN pekerja = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            int bil = 0;

            if (pekerja != null && pekerja.HR_TARIKH_MASUK.Value.Year < tahun)
            {
                int MaxCuti = 0;
                HR_CUTI item2 = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == kod_cuti);
                if (item2 == null)
                {
                    item2 = new HR_CUTI();
                }
                if (item2.HR_JUMLAH_CUTI == null)
                {
                    string tahunHadapan = "01/01/" + (tahun);
                    DateTime? TarikhMohon = Convert.ToDateTime(tahunHadapan);
                    HR_MAKLUMAT_CUTI cuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == "CU018" && s.HR_NO_PEKERJA == no_pekerja && s.HR_TARIKH_PERMOHONAN == TarikhMohon);

                    if (cuti == null)
                    {
                        cuti = new HR_MAKLUMAT_CUTI();
                    }

                    item2.HR_JUMLAH_CUTI = cuti.HR_JUMLAH_MAKSIMUM;
                }

                MaxCuti = Convert.ToInt32(item2.HR_JUMLAH_CUTI);
                for (int i = pekerja.HR_TARIKH_MASUK.Value.Year; i < tahun; i++)
                {


                    List<HR_MAKLUMAT_CUTI> item = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == no_pekerja && s.HR_KOD_CUTI == kod_cuti && s.HR_TAHUN == i).OrderBy(s => s.HR_TAHUN).ToList();
                    if (item.Count() > 0)
                    {
                        var bil2 = 0;
                        foreach (var data in item)
                        {
                            bil2 = bil2 + Convert.ToInt32(data.HR_BIL_CUTI_TEMP);

                        }
                        bil = Convert.ToInt32(MaxCuti) - bil2;
                    }
                    else
                    {
                        HR_CUTI item3 = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == kod_cuti);
                        if (item3 == null)
                        {
                            item3 = new HR_CUTI();
                        }
                        bil = bil + Convert.ToInt32(item3.HR_JUMLAH_CUTI);
                    }
                }

            }

            return Json(bil, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KategoriCuti(string HR_KATEGORI_CUTI)
        {
            string kategori = HR_KATEGORI_CUTI.PadRight(3, ' ');
            List<HR_CUTI> item = db.HR_CUTI.Where(s => s.HR_KATEGORI == kategori).ToList();
            if (item == null)
            {
                item = new List<HR_CUTI>();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JumlahMaxCuti(string KodCuti, string NoPekerja, int? tahun)
        {

            HR_CUTI item = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == KodCuti);
            if (item == null)
            {
                item = new HR_CUTI();
            }

            if (item.HR_JUMLAH_CUTI == null)
            {
                string tahunHadapan = "01/01/" + (tahun);
                DateTime? TarikhMohon = Convert.ToDateTime(tahunHadapan);
                HR_MAKLUMAT_CUTI cuti = db.HR_MAKLUMAT_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == "CU018" && s.HR_NO_PEKERJA == NoPekerja && s.HR_TARIKH_PERMOHONAN == TarikhMohon);

                if (cuti == null)
                {
                    cuti = new HR_MAKLUMAT_CUTI();
                }

                item.HR_JUMLAH_CUTI = cuti.HR_JUMLAH_MAKSIMUM;
            }
            return Json(item.HR_JUMLAH_CUTI, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JumElaun(string HR_KOD)
        {
            HR_ELAUN item = db.HR_ELAUN.SingleOrDefault(s => s.HR_KOD_ELAUN == HR_KOD);
            if (item == null)
            {
                item = new HR_ELAUN();
            }
            return Json(item.HR_NILAI, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JumPotongan(string HR_KOD)
        {
            HR_POTONGAN item = db.HR_POTONGAN.SingleOrDefault(s => s.HR_KOD_POTONGAN == HR_KOD);
            if (item == null)
            {
                item = new HR_POTONGAN();
            }
            return Json(item.HR_NILAI, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JumCaruman(string HR_KOD)
        {
            HR_CARUMAN item = db.HR_CARUMAN.SingleOrDefault(s => s.HR_KOD_CARUMAN == HR_KOD);
            if (item == null)
            {
                item = new HR_CARUMAN();
            }
            return Json(item.HR_NILAI, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CariKod(string HR_PENERANGAN_UPAH)
        {
            HR_GAJI_UPAHAN item = db.HR_GAJI_UPAHAN.FirstOrDefault(s => s.HR_PENERANGAN_UPAH == HR_PENERANGAN_UPAH);
            if (item == null)
            {
                item = new HR_GAJI_UPAHAN();
            }
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoAktif(string tMula, string tAkhir, string auto)
        {
            if (tMula == "")
            {
                tMula = null;
            }

            if (tAkhir == "")
            {
                tAkhir = null;
            }

            string data = null;
            if (tMula != null && tAkhir != null)
            {
                DateTime HR_TARIKH_MULA = Convert.ToDateTime(tMula);
                DateTime HR_TARIKH_AKHIR = Convert.ToDateTime(tAkhir);
                if (auto == "Y")
                {
                    if (HR_TARIKH_MULA <= DateTime.Now && HR_TARIKH_AKHIR >= DateTime.Now)
                    {
                        data = "Y";
                    }
                    else
                    {
                        data = "T";
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}