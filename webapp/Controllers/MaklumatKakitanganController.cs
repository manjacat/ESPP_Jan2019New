using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using eSPP.Models;
using Microsoft.AspNet.Identity;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace eSPP.Controllers
{
    public class MaklumatKakitanganController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();

        // GET: MaklumatKakitangan
        public ActionResult Index(string key, string value, string status, int? i)
        {
            ViewBag.photo = "";
            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            ViewBag.key = "";

            var username = User.Identity.Name;
            string pekerjaLogin = null;
            HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == username);
            if(pekerja != null)
            {
                pekerjaLogin = pekerja.HR_NO_PEKERJA;
            }
            ViewBag.pekerjaLogin = pekerjaLogin;


            //Peribadi("", "");
            List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
            
            if (key == "1" || key == "4")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == value).ToList();
                
            }
            else if (key == "2")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.StartsWith(value)).ToList();
            }
            else if (key == "3")
            {
                mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU.StartsWith(value)).ToList();
            }

            if(status != null && status != "")
            {
                mPeribadi = mPeribadi.Where(s => s.HR_AKTIF_IND == status).ToList();
            }

            mKakitangan.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
            mKakitangan.HR_GAMBAR_PENGGUNA = new HR_GAMBAR_PENGGUNA();
            mKakitangan.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
            mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY = new List<HR_MAKLUMAT_PEKERJAAN_HISTORY>();
            mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA = new List<MaklumatPengalamanKerja>();
            mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA = new List<MaklumatKemahiranBahasa>();
            mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL = new List<MaklumatKemahiranTeknikal>();
            mKakitangan.HR_MAKLUMAT_KELAYAKAN = new List<MaklumatKelayakan>();
            mKakitangan.HR_MAKLUMAT_SIJIL = new List<MaklumatSijil>();
            mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN = new List<MaklumatKursusLatihan>();
            mKakitangan.HR_MAKLUMAT_AKTIVITI = new List<MaklumatAktiviti>();
            mKakitangan.HR_MAKLUMAT_PEWARIS = new List<MaklumatPewaris>();
            mKakitangan.HR_MAKLUMAT_TANGGUNGAN = new List<MaklumatTanggungan>();
            mKakitangan.HR_MAKLUMAT_KUARTERS = new MaklumatKuarters();
            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G = new List<MaklumatElaunPotongan>();
            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E = new List<MaklumatElaunPotongan>();
            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C = new List<MaklumatElaunPotongan>();
            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P = new List<MaklumatElaunPotongan>();
            mKakitangan.HR_MAKLUMAT_KURNIAAN = new List<MaklumatKurniaan>();
            mKakitangan.HR_ANUGERAH_CEMERLANG = new List<MaklumatAnugerahCemerlang>();
            mKakitangan.HR_ANUGERAH_HAJI = new MaklumatAnugerahHaji();
            mKakitangan.HR_PERSARAAN = new MaklumatPersaraan();
            mKakitangan.HR_TINDAKAN_DISIPLIN = new List<MaklumatTindakanDisiplin>();
            mKakitangan.HR_MAKLUMAT_KEMATIAN = new MaklumatKematian();
            mKakitangan.HR_PENILAIAN_PRESTASI = new MaklumatPenilaianPrestasi();
            mKakitangan.HR_MAKLUMAT_CUTI = new MaklumatCuti();

            if (mPeribadi.Count() > 0)
            {
                mKakitangan.HR_SENARAI_PERIBADI = new List<MaklumatPeribadi>();
                foreach (HR_MAKLUMAT_PERIBADI peribadi in mPeribadi)
                {
                    mKakitangan.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = peribadi.HR_NO_PEKERJA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU = peribadi.HR_NO_KPBARU;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = peribadi.HR_NAMA_PEKERJA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_AKTIF_IND = peribadi.HR_AKTIF_IND;
                    mKakitangan.HR_SENARAI_PERIBADI.Add(mKakitangan.HR_MAKLUMAT_PERIBADI);
                    HR_GAMBAR_PENGGUNA gambar = db.HR_GAMBAR_PENGGUNA.SingleOrDefault(s => s.HR_NO_PEKERJA == peribadi.HR_NO_PEKERJA);
                    if (gambar == null)
                    {
                        gambar = new HR_GAMBAR_PENGGUNA();
                    }
                    mKakitangan.HR_GAMBAR_PENGGUNA = gambar;
                    var photoName = gambar.HR_PHOTO + gambar.HR_FORMAT_TYPE;
                    string fullPath = Server.MapPath("~/Content/uploads/" + photoName);

                    if (!System.IO.File.Exists(fullPath))
                    {
                        ViewBag.photo = "1";
                    }
                    //STRAT PERIBADI
                    ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
                }
                if (key == "4" && mPeribadi.Count() > 0)
                {
                    ViewBag.key = key;
                    //HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
                    HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == value);

                    List<HR_MAKLUMAT_PEKERJAAN_HISTORY> pKerjaMPPJ = db.HR_MAKLUMAT_PEKERJAAN_HISTORY.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_PEKERJAAN_HISTORY>();
                    List<HR_MAKLUMAT_PEKERJAAN_HISTORY> sKerjaMPPJ = new List<HR_MAKLUMAT_PEKERJAAN_HISTORY>();
                    var perubahanTarikh = DateTime.Now.ToShortDateString();
                    var perubahanTarikh2 = Convert.ToDateTime(perubahanTarikh);

                    List<HR_MAKLUMAT_PEKERJAAN_HISTORY> ExistingDate = db.HR_MAKLUMAT_PEKERJAAN_HISTORY.Where(s => s.HR_NO_PEKERJA == value && s.HR_TARIKH_PERUBAHAN == perubahanTarikh2).ToList<HR_MAKLUMAT_PEKERJAAN_HISTORY>();
                    ViewBag.ExistingDate = "T";
                    if (ExistingDate.Count() > 0)
                    {
                        ViewBag.ExistingDate = "Y";
                    }
                    

                    List<HR_MAKLUMAT_PENGALAMAN_KERJA> pKerja = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
                    List<MaklumatPengalamanKerja> sKerja = new List<MaklumatPengalamanKerja>();

                    List<HR_MAKLUMAT_PEWARIS> lPewaris = db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NO_PEKERJA == value).OrderBy(s => s.HR_PEWARIS_IND).ToList<HR_MAKLUMAT_PEWARIS>();
                    List<MaklumatPewaris> sPewaris = new List<MaklumatPewaris>();

                    List<HR_MAKLUMAT_TANGGUNGAN> lTanggungan = db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_TANGGUNGAN>();
                    List<MaklumatTanggungan> sTanggungan = new List<MaklumatTanggungan>();

                    List<HR_MAKLUMAT_KEMAHIRAN_BAHASA> lKemahiranBahasa = db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_KEMAHIRAN_BAHASA>();
                    List<MaklumatKemahiranBahasa> sKemahiranBahasa = new List<MaklumatKemahiranBahasa>();

                    List<HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL> lKemahiranTeknikal = db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL>();
                    List<MaklumatKemahiranTeknikal> sKemahiranTeknikal = new List<MaklumatKemahiranTeknikal>();

                    List<HR_MAKLUMAT_KELAYAKAN> lKelayakan = db.HR_MAKLUMAT_KELAYAKAN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_KELAYAKAN>();
                    List<MaklumatKelayakan> sKelayakan = new List<MaklumatKelayakan>();

                    List<HR_MAKLUMAT_SIJIL> lSijil = db.HR_MAKLUMAT_SIJIL.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_SIJIL>();
                    List<MaklumatSijil> sSijil = new List<MaklumatSijil>();

                    List<HR_MAKLUMAT_KURSUS_LATIHAN> lKursusLatihan = db.HR_MAKLUMAT_KURSUS_LATIHAN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_KURSUS_LATIHAN>();
                    List<MaklumatKursusLatihan> sKursusLatihan = new List<MaklumatKursusLatihan>();

                    List<HR_MAKLUMAT_AKTIVITI> lAktiviti = db.HR_MAKLUMAT_AKTIVITI.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_AKTIVITI>();
                    List<MaklumatAktiviti> sAktiviti = new List<MaklumatAktiviti>();

                    HR_MAKLUMAT_KUARTERS mKuarters = db.HR_MAKLUMAT_KUARTERS.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
                    MaklumatKuarters sKuarters = new MaklumatKuarters();
                    HR_KUARTERS mKuarters2 = new HR_KUARTERS();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> lElaunPotonganG = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value && s.HR_ELAUN_POTONGAN_IND == "G").ToList<HR_MAKLUMAT_ELAUN_POTONGAN>();
                    List<MaklumatElaunPotongan> sElaunPotonganG = new List<MaklumatElaunPotongan>();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> lElaunPotonganE = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value && s.HR_ELAUN_POTONGAN_IND == "E").ToList<HR_MAKLUMAT_ELAUN_POTONGAN>();
                    List<MaklumatElaunPotongan> sElaunPotonganE = new List<MaklumatElaunPotongan>();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> lElaunPotonganP = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value && s.HR_ELAUN_POTONGAN_IND == "P").ToList<HR_MAKLUMAT_ELAUN_POTONGAN>();
                    List<MaklumatElaunPotongan> sElaunPotonganP = new List<MaklumatElaunPotongan>();

                    List<HR_MAKLUMAT_ELAUN_POTONGAN> lElaunPotonganC = db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == value && s.HR_ELAUN_POTONGAN_IND == "C").ToList<HR_MAKLUMAT_ELAUN_POTONGAN>();
                    List<MaklumatElaunPotongan> sElaunPotonganC = new List<MaklumatElaunPotongan>();

                    List<HR_MAKLUMAT_KURNIAAN> lKurniaan = db.HR_MAKLUMAT_KURNIAAN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_KURNIAAN>();
                    List<MaklumatKurniaan> sKurniaan = new List<MaklumatKurniaan>();
                    List<HR_PENCALONAN_KURNIAAN> lPKurniaan = db.HR_PENCALONAN_KURNIAAN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_PENCALONAN_KURNIAAN>();

                    List<HR_ANUGERAH_CEMERLANG> lAnugerahCemerlang = db.HR_ANUGERAH_CEMERLANG.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_ANUGERAH_CEMERLANG>();
                    List<MaklumatAnugerahCemerlang> sAnugerahCemerlang = new List<MaklumatAnugerahCemerlang>();

                    HR_ANUGERAH_HAJI mAnugerahHaji = db.HR_ANUGERAH_HAJI.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
                    MaklumatAnugerahHaji sAnugeragHaji = new MaklumatAnugerahHaji();

                    HR_PERSARAAN mPersaraan = db.HR_PERSARAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == value);
                    MaklumatAnugerahHaji sPersaraan = new MaklumatAnugerahHaji();

                    List<HR_TINDAKAN_DISIPLIN> lTindakanDisiplin = db.HR_TINDAKAN_DISIPLIN.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_TINDAKAN_DISIPLIN>();
                    List<MaklumatTindakanDisiplin> sTindakanDisiplin = new List<MaklumatTindakanDisiplin>();
                    List<HR_TINDAKAN_DISIPLIN_DETAIL> lTindakanDisiplinDetail = db.HR_TINDAKAN_DISIPLIN_DETAIL.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_TINDAKAN_DISIPLIN_DETAIL>();

                    HR_MAKLUMAT_KEMATIAN mKematian = db.HR_MAKLUMAT_KEMATIAN.Find(value);
                    MaklumatKematian sKematian = new MaklumatKematian();

                    HR_PENILAIAN_PRESTASI mPrestasi = db.HR_PENILAIAN_PRESTASI.SingleOrDefault(s => s.HR_NO_PEKERJA == value && s.HR_TAHUN_PRESTASI == DateTime.Now.Year);
                    MaklumatPenilaianPrestasi sPrestasi = new MaklumatPenilaianPrestasi();

                    List<HR_MAKLUMAT_CUTI> MaklumatCuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_NO_PEKERJA == value && s.HR_TAHUN == DateTime.Now.Year).OrderByDescending(s => s.HR_TARIKH_PERMOHONAN).ToList();
                    List<MaklumatCuti> SenaraiCuti = new List<MaklumatCuti>();
                    MaklumatCuti sCuti = new MaklumatCuti();


                    if (mKuarters != null)
                    {
                        mKuarters2 = db.HR_KUARTERS.SingleOrDefault(s => s.HR_KOD_KUARTERS == mKuarters.HR_KOD_KUARTERS);
                    }

                    /*if(lTindakanDisiplin.Count() > 0)
                    {
                        foreach(var item in lTindakanDisiplin)
                        {
                            lTindakanDisiplinDetail = db.HR_TINDAKAN_DISIPLIN_DETAIL.Where(s => s.HR_NO_PEKERJA == item.HR_NO_PEKERJA && s.HR_TARIKH_KESALAHAN == item.HR_TARIKH_KESALAHAN).ToList<HR_TINDAKAN_DISIPLIN_DETAIL>();
                            foreach(var item2 in lTindakanDisiplinDetail)
                            {
                                TindakanDisiplinDetailModels mTindakanDisiplinDetail = new TindakanDisiplinDetailModels();
                                mTindakanDisiplinDetail.HR_NO_PEKERJA = item2.HR_NO_PEKERJA;
                                mTindakanDisiplinDetail.HR_TARIKH_KESALAHAN = item2.HR_TARIKH_KESALAHAN;
                                mTindakanDisiplinDetail.HR_KOD_TINDAKAN = item2.HR_KOD_TINDAKAN;
                                mTindakanDisiplinDetail.HR_TARIKH_MULA = item2.HR_TARIKH_MULA;
                                mTindakanDisiplinDetail.HR_TARIKH_AKHIR = item2.HR_TARIKH_AKHIR;
                                sTindakanDisiplinDetail.Add(mTindakanDisiplinDetail);
                            }
                        }

                    }*/

                    if (mPekerjaan == null)
                    {
                        mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                    }

                    if (mPeribadi == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.Umur = "";
                    if (mPeribadi.ElementAt(0).HR_TARIKH_LAHIR != null)
                    {
                        ViewBag.Umur = (DateTime.Now.Year - Convert.ToDateTime(mPeribadi.ElementAt(0).HR_TARIKH_LAHIR).Year);
                    }
                    mKakitangan.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = mPeribadi.ElementAt(0).HR_NO_PEKERJA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU = mPeribadi.ElementAt(0).HR_NO_KPBARU;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA = mPeribadi.ElementAt(0).HR_NAMA_PEKERJA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPLAMA = mPeribadi.ElementAt(0).HR_NO_KPLAMA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_LAHIR = mPeribadi.ElementAt(0).HR_TARIKH_LAHIR;
                    if (mPeribadi.ElementAt(0).HR_TEMPAT_LAHIR != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TEMPAT_LAHIR = mPeribadi.ElementAt(0).HR_TEMPAT_LAHIR.Trim();
                    }
                    if (mPeribadi.ElementAt(0).HR_WARGANEGARA != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PERIBADI.HR_WARGANEGARA = new string(mPeribadi.ElementAt(0).HR_WARGANEGARA.TakeWhile(x => char.IsDigit(x)).ToArray());
                    }
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_KETURUNAN = mPeribadi.ElementAt(0).HR_KETURUNAN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_AGAMA = mPeribadi.ElementAt(0).HR_AGAMA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_JANTINA = mPeribadi.ElementAt(0).HR_JANTINA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARAF_KAHWIN = mPeribadi.ElementAt(0).HR_TARAF_KAHWIN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_LESEN = mPeribadi.ElementAt(0).HR_LESEN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_KELAS_LESEN = mPeribadi.ElementAt(0).HR_KELAS_LESEN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT1 = mPeribadi.ElementAt(0).HR_TALAMAT1;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT2 = mPeribadi.ElementAt(0).HR_TALAMAT2;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT3 = mPeribadi.ElementAt(0).HR_TALAMAT3;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TBANDAR = mPeribadi.ElementAt(0).HR_TBANDAR;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TPOSKOD = mPeribadi.ElementAt(0).HR_TPOSKOD;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TNEGERI = mPeribadi.ElementAt(0).HR_TNEGERI;
                    if (mPeribadi.ElementAt(0).HR_TNEGERI != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TNEGERI = mPeribadi.ElementAt(0).HR_TNEGERI.Trim();
                    }

                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT1 = mPeribadi.ElementAt(0).HR_SALAMAT1;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT2 = mPeribadi.ElementAt(0).HR_SALAMAT2;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT3 = mPeribadi.ElementAt(0).HR_SALAMAT3;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SBANDAR = mPeribadi.ElementAt(0).HR_SBANDAR;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SPOSKOD = mPeribadi.ElementAt(0).HR_SPOSKOD;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SNEGERI = mPeribadi.ElementAt(0).HR_SNEGERI;
                    if (mPeribadi.ElementAt(0).HR_SNEGERI != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SNEGERI = mPeribadi.ElementAt(0).HR_SNEGERI.Trim();
                    }
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TAHUN_SPM = mPeribadi.ElementAt(0).HR_TAHUN_SPM;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_GRED_BM = mPeribadi.ElementAt(0).HR_GRED_BM;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELRUMAH = mPeribadi.ElementAt(0).HR_TELRUMAH;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELPEJABAT = mPeribadi.ElementAt(0).HR_TELPEJABAT;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELBIMBIT = mPeribadi.ElementAt(0).HR_TELBIMBIT;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_EMAIL = mPeribadi.ElementAt(0).HR_EMAIL;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_AKTIF_IND = mPeribadi.ElementAt(0).HR_AKTIF_IND;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_CC_KENDERAAN = mPeribadi.ElementAt(0).HR_CC_KENDERAAN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KENDERAAN = mPeribadi.ElementAt(0).HR_NO_KENDERAAN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_JENIS_KENDERAAN = mPeribadi.ElementAt(0).HR_JENIS_KENDERAAN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_ALASAN = mPeribadi.ElementAt(0).HR_ALASAN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_IDPEKERJA = mPeribadi.ElementAt(0).HR_IDPEKERJA;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_KEYIN = mPeribadi.ElementAt(0).HR_TARIKH_KEYIN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_KEYIN = mPeribadi.ElementAt(0).HR_NP_KEYIN;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_UBAH = mPeribadi.ElementAt(0).HR_TARIKH_UBAH;
                    mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_UBAH = mPeribadi.ElementAt(0).HR_NP_UBAH;

                    mKakitangan.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PEKERJA = mPekerjaan.HR_NO_PEKERJA;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN = mPekerjaan.HR_GELARAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN = mPekerjaan.HR_JABATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GRED = null;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MIN = null;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MAX = null;

                    if (mPekerjaan.HR_GRED != null)
                    {
                        if(mPekerjaan.HR_GRED.Trim() != null)
                        {
                            GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(mPekerjaan.HR_GRED.Trim()));
                            if(sGred != null)
                            {
                                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GRED = sGred.SHORT_DESCRIPTION;

                                HR_MATRIKS_GAJI matriks = db.HR_MATRIKS_GAJI.FirstOrDefault(s => s.HR_GRED_GAJI == sGred.SHORT_DESCRIPTION && s.HR_GAJI_POKOK == mPekerjaan.HR_GAJI_POKOK);

                                if(matriks == null)
                                {
                                    matriks = new HR_MATRIKS_GAJI();
                                    matriks.HR_GAJI_MIN = 0;
                                    matriks.HR_GAJI_MAX = 0;
                                }

                                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MIN = matriks.HR_GAJI_MIN;
                                mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MAX = matriks.HR_GAJI_MAX;
                            }
                        }
                    }

                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN = mPekerjaan.HR_KUMP_PERKHIDMATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN = mPekerjaan.HR_TARAF_JAWATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK = mPekerjaan.HR_GAJI_POKOK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_AKAUN_BANK = mPekerjaan.HR_NO_AKAUN_BANK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI = mPekerjaan.HR_BULAN_KENAIKAN_GAJI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK = mPekerjaan.HR_TARIKH_MASUK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_SAH_JAWATAN = mPekerjaan.HR_TARIKH_SAH_JAWATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT_KONTRAK = mPekerjaan.HR_TARIKH_TAMAT_KONTRAK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT = mPekerjaan.HR_TARIKH_TAMAT;
                    if (mPekerjaan.HR_SISTEM != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM = mPekerjaan.HR_SISTEM.Trim();
                    }
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENYELIA = mPekerjaan.HR_NO_PENYELIA;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP = mPekerjaan.HR_STATUS_KWSP;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_SOCSO = mPekerjaan.HR_STATUS_SOCSO;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PCB = mPekerjaan.HR_STATUS_PCB;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN = mPekerjaan.HR_STATUS_PENCEN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_KWSP = mPekerjaan.HR_NILAI_KWSP;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_SOCSO = mPekerjaan.HR_NILAI_SOCSO;
                    if (mPekerjaan.HR_KOD_PCB != null)
                    {
                        mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_PCB = new string(mPekerjaan.HR_KOD_PCB.SkipWhile(x => char.IsDigit(x)).TakeWhile(x => (!char.IsDigit(x) && !char.IsWhiteSpace(x)) || char.IsDigit(x)).ToArray());
                        mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI_PCB = new string(mPekerjaan.HR_KOD_PCB.SkipWhile(x => !char.IsDigit(x)).TakeWhile(x => char.IsDigit(x)).ToArray());
                    }

                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_PRORATA = mPekerjaan.HR_GAJI_PRORATA;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_MATRIKS_GAJI = mPekerjaan.HR_MATRIKS_GAJI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_UNIT = mPekerjaan.HR_UNIT;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN = mPekerjaan.HR_KUMPULAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK = mPekerjaan.HR_KOD_BANK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN = mPekerjaan.HR_TINGKATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND = mPekerjaan.HR_KAKITANGAN_IND;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_FAIL_PERKHIDMATAN = mPekerjaan.HR_FAIL_PERKHIDMATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_SIRI = mPekerjaan.HR_NO_SIRI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAYARAN_CEK = mPekerjaan.HR_BAYARAN_CEK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KE_JABATAN = mPekerjaan.HR_TARIKH_KE_JABATAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI = mPekerjaan.HR_KOD_GAJI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KELAS_PERJALANAN = mPekerjaan.HR_KELAS_PERJALANAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_LANTIKAN = mPekerjaan.HR_TARIKH_LANTIKAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TIDAK_AKTIF = mPekerjaan.HR_TARIKH_TIDAK_AKTIF;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND = mPekerjaan.HR_GAJI_IND;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_GAJI = mPekerjaan.HR_TARIKH_GAJI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_MULA = mPekerjaan.HR_PCB_TARIKH_MULA;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_AKHIR = mPekerjaan.HR_PCB_TARIKH_AKHIR;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_PCB = mPekerjaan.HR_NILAI_PCB;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J = mPekerjaan.HR_KOD_GELARAN_J;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TANGGUH_GERAKGAJI_IND = mPekerjaan.HR_TANGGUH_GERAKGAJI_IND;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KEYIN = mPekerjaan.HR_TARIKH_KEYIN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_KEYIN = mPekerjaan.HR_NP_KEYIN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_UBAH = mPekerjaan.HR_TARIKH_UBAH;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_UBAH = mPekerjaan.HR_NP_UBAH;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SKIM = mPekerjaan.HR_SKIM;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PERGERAKAN_GAJI = mPekerjaan.HR_PERGERAKAN_GAJI;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_KWSP = mPekerjaan.HR_NO_KWSP;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENCEN = mPekerjaan.HR_NO_PENCEN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_SOCSO = mPekerjaan.HR_NO_SOCSO;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PCB = mPekerjaan.HR_NO_PCB;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_INITIAL = mPekerjaan.HR_INITIAL;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_AM_YDP = mPekerjaan.HR_AM_YDP;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK_KERAJAAN = mPekerjaan.HR_TARIKH_MASUK_KERAJAAN;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_SAH_J_TUKAR_LANTIK = mPekerjaan.HR_TARIKH_SAH_J_TUKAR_LANTIK;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_UNIFORM = mPekerjaan.HR_UNIFORM;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TEKNIKAL = mPekerjaan.HR_TEKNIKAL;
                    mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KELUAR_MBPJ = mPekerjaan.HR_TARIKH_KELUAR_MBPJ;
                    
                    if (pKerjaMPPJ.Count > 0)
                    {
                        foreach (var item in pKerjaMPPJ)
                        {
                            HR_MAKLUMAT_PEKERJAAN_HISTORY mKerja = new HR_MAKLUMAT_PEKERJAAN_HISTORY();
                            mKerja.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKerja.HR_TARIKH_PERUBAHAN = item.HR_TARIKH_PERUBAHAN;
                            mKerja.HR_JABATAN = item.HR_JABATAN;
                            mKerja.HR_BAHAGIAN = item.HR_BAHAGIAN;
                            mKerja.HR_UNIT = item.HR_UNIT;
                            mKerja.HR_GRED = null;
                            if (item.HR_GRED != null)
                            {
                                if (item.HR_GRED.Trim() != null)
                                {
                                    GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(item.HR_GRED.Trim()));
                                    if (sGred != null)
                                    {
                                        mKerja.HR_GRED = sGred.SHORT_DESCRIPTION;
                                    }
                                }
                            }
                            mKerja.HR_KATEGORI = item.HR_KATEGORI;
                            mKerja.HR_KUMP_PERKHIDMATAN = item.HR_KUMP_PERKHIDMATAN;
                            mKerja.HR_JAWATAN = item.HR_JAWATAN;
                            mKerja.HR_TARAF_JAWATAN = item.HR_TARAF_JAWATAN;
                            mKerja.HR_TARIKH_SAH_JAWATAN = item.HR_TARIKH_SAH_JAWATAN;
                            mKerja.HR_TARIKH_TAMAT_KONTRAK = item.HR_TARIKH_TAMAT_KONTRAK;
                            mKerja.HR_NO_PENYELIA = item.HR_NO_PENYELIA;
                            mKerja.HR_MATRIKS_GAJI = item.HR_MATRIKS_GAJI;
                            mKerja.HR_KUMPULAN = item.HR_KUMPULAN;
                            mKerja.HR_TINGKATAN = item.HR_TINGKATAN;
                            mKerja.HR_KOD_GAJI = item.HR_KOD_GAJI;
                            mKerja.HR_NP_UBAH = item.HR_NP_UBAH;
                            mKerja.HR_TARIKH_MASUK = item.HR_TARIKH_MASUK;
                            mKerja.HR_GAJI = item.HR_GAJI;
                            sKerjaMPPJ.Add(mKerja);
                            mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY = sKerjaMPPJ;
                        }
                    }
                    else
                    {
                        HR_MAKLUMAT_PEKERJAAN_HISTORY mKerja = new HR_MAKLUMAT_PEKERJAAN_HISTORY();
                        mKerja.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                        mKerja.HR_TARIKH_PERUBAHAN = DateTime.Now;

                        mKerja.HR_JABATAN = mPekerjaan.HR_JABATAN;
                        mKerja.HR_BAHAGIAN = mPekerjaan.HR_BAHAGIAN;
                        mKerja.HR_UNIT = mPekerjaan.HR_UNIT;
                        mKerja.HR_GRED = null;
                        if (mPekerjaan.HR_GRED != null)
                        {
                            if (mPekerjaan.HR_GRED.Trim() != null)
                            {
                                GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(mPekerjaan.HR_GRED.Trim()));
                                if (sGred != null)
                                {
                                    mKerja.HR_GRED = sGred.SHORT_DESCRIPTION;
                                }
                            }
                        }
                        mKerja.HR_KATEGORI = mPekerjaan.HR_KATEGORI;
                        mKerja.HR_KUMP_PERKHIDMATAN = mPekerjaan.HR_KUMP_PERKHIDMATAN;
                        mKerja.HR_JAWATAN = mPekerjaan.HR_JAWATAN;
                        mKerja.HR_TARAF_JAWATAN = mPekerjaan.HR_TARAF_JAWATAN;
                        mKerja.HR_TARIKH_SAH_JAWATAN = mPekerjaan.HR_TARIKH_SAH_JAWATAN;
                        mKerja.HR_TARIKH_TAMAT_KONTRAK = mPekerjaan.HR_TARIKH_TAMAT_KONTRAK;
                        mKerja.HR_NO_PENYELIA = mPekerjaan.HR_NO_PENYELIA;
                        mKerja.HR_MATRIKS_GAJI = mPekerjaan.HR_MATRIKS_GAJI;
                        mKerja.HR_KUMPULAN = mPekerjaan.HR_KUMPULAN;
                        mKerja.HR_TINGKATAN = mPekerjaan.HR_TINGKATAN;
                        mKerja.HR_KOD_GAJI = mPekerjaan.HR_KOD_GAJI;
                        mKerja.HR_NP_UBAH = mPekerjaan.HR_NP_UBAH;
                        mKerja.HR_TARIKH_MASUK = mPekerjaan.HR_TARIKH_MASUK;
                        mKerja.HR_GAJI = mPekerjaan.HR_GAJI_POKOK;

                        sKerjaMPPJ.Add(mKerja);
                        mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY = sKerjaMPPJ;
                    }
                    /*else
                    {
                        MaklumatPengalamanKerja mKerja = new MaklumatPengalamanKerja();
                        mKerja.HR_NO_PEKERJA = null;
                        mKerja.HR_NAMA_SYARIKAT = null;
                        mKerja.HR_JAWATAN = null;
                        mKerja.HR_TARIKH_MULA = null;
                        mKerja.HR_TARIKH_TAMAT = null;
                        mKerja.HR_ALASAN_BERHENTI = null;
                        sKerjaMPPJ.Add(mKerja);
                        mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA_MPPJ = sKerjaMPPJ;
                    }*/

                    if (pKerja.Count > 0)
                    {
                        foreach (var item in pKerja)
                        {
                            MaklumatPengalamanKerja mKerja = new MaklumatPengalamanKerja();
                            mKerja.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKerja.HR_NAMA_SYARIKAT = item.HR_NAMA_SYARIKAT;
                            mKerja.HR_JAWATAN = item.HR_JAWATAN;
                            mKerja.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mKerja.HR_TARIKH_TAMAT = item.HR_TARIKH_TAMAT;
                            mKerja.HR_ALASAN_BERHENTI = item.HR_ALASAN_BERHENTI;
                            sKerja.Add(mKerja);
                            mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA = sKerja;
                        }
                    }
                    

                    if (lKemahiranBahasa.Count() > 0)
                    {
                        foreach (var item in lKemahiranBahasa)
                        {
                            MaklumatKemahiranBahasa mKemahiranBahasa = new MaklumatKemahiranBahasa();
                            mKemahiranBahasa.HR_BAHASA = item.HR_BAHASA;
                            mKemahiranBahasa.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKemahiranBahasa.HR_PEMBACAAN = item.HR_PEMBACAAN;
                            mKemahiranBahasa.HR_PENULISAN = item.HR_PENULISAN;
                            mKemahiranBahasa.HR_PERTUTURAN = item.HR_PERTUTURAN;
                            sKemahiranBahasa.Add(mKemahiranBahasa);
                            mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA = sKemahiranBahasa;
                        }
                    }
                    //else
                    //{
                    //    MaklumatKemahiranBahasa mKemahiranBahasa = new MaklumatKemahiranBahasa();
                    //    mKemahiranBahasa.HR_BAHASA = null;
                    //    mKemahiranBahasa.HR_NO_PEKERJA = null;
                    //    mKemahiranBahasa.HR_PEMBACAAN = null;
                    //    mKemahiranBahasa.HR_PENULISAN = null;
                    //    mKemahiranBahasa.HR_PERTUTURAN = null;
                    //    sKemahiranBahasa.Add(mKemahiranBahasa);
                    //    mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA = sKemahiranBahasa;
                    //}

                    if (lKemahiranTeknikal.Count() > 0)
                    {
                        foreach (var item in lKemahiranTeknikal)
                        {
                            MaklumatKemahiranTeknikal mKemahiranTeknikal = new MaklumatKemahiranTeknikal();
                            mKemahiranTeknikal.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKemahiranTeknikal.HR_KEMAHIRAN = item.HR_KEMAHIRAN;
                            mKemahiranTeknikal.HR_TAHAP = item.HR_TAHAP;
                            mKemahiranTeknikal.HR_SEQ_NO = item.HR_SEQ_NO;
                            sKemahiranTeknikal.Add(mKemahiranTeknikal);
                            mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL = sKemahiranTeknikal;
                        }
                    }
                    //else
                    //{
                    //    MaklumatKemahiranTeknikal mKemahiranTeknikal = new MaklumatKemahiranTeknikal();
                    //    mKemahiranTeknikal.HR_NO_PEKERJA = null;
                    //    mKemahiranTeknikal.HR_KEMAHIRAN = null;
                    //    mKemahiranTeknikal.HR_TAHAP = null;
                    //    mKemahiranTeknikal.HR_SEQ_NO = 1;
                    //    sKemahiranTeknikal.Add(mKemahiranTeknikal);
                    //    mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL = sKemahiranTeknikal;
                    //}

                    if (lKelayakan.Count() > 0)
                    {
                        foreach (var item in lKelayakan)
                        {
                            MaklumatKelayakan mKelayakan = new MaklumatKelayakan();
                            mKelayakan.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKelayakan.HR_SEQ_NO = item.HR_SEQ_NO;
                            mKelayakan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                            mKelayakan.HR_PANGKAT = item.HR_PANGKAT;
                            mKelayakan.HR_TAHUN_MULA = item.HR_TAHUN_MULA;
                            mKelayakan.HR_TAHUN_TAMAT = item.HR_TAHUN_TAMAT;
                            mKelayakan.HR_SEKOLAH_INSTITUSI = item.HR_SEKOLAH_INSTITUSI;
                            sKelayakan.Add(mKelayakan);
                            mKakitangan.HR_MAKLUMAT_KELAYAKAN = sKelayakan;
                        }
                    }
                    //else
                    //{
                    //    MaklumatKelayakan mKelayakan = new MaklumatKelayakan();
                    //    mKelayakan.HR_NO_PEKERJA = null;
                    //    mKelayakan.HR_SEQ_NO = 1;
                    //    mKelayakan.HR_KEPUTUSAN = null;
                    //    mKelayakan.HR_PANGKAT = null;
                    //    mKelayakan.HR_TAHUN_MULA = null;
                    //    mKelayakan.HR_TAHUN_TAMAT = null;
                    //    mKelayakan.HR_SEKOLAH_INSTITUSI = null;
                    //    sKelayakan.Add(mKelayakan);
                    //    mKakitangan.HR_MAKLUMAT_KELAYAKAN = sKelayakan;
                    //}

                    if (lSijil.Count() > 0)
                    {
                        foreach (var item in lSijil)
                        {
                            MaklumatSijil mSijil = new MaklumatSijil();
                            mSijil.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mSijil.HR_TARIKH_DIPEROLEHI = item.HR_TARIKH_DIPEROLEHI;
                            mSijil.HR_NAMA_SIJIL_PEPERIKSAAN = item.HR_NAMA_SIJIL_PEPERIKSAAN;
                            mSijil.HR_ANJURAN = item.HR_ANJURAN;
                            mSijil.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                            sSijil.Add(mSijil);
                            mKakitangan.HR_MAKLUMAT_SIJIL = sSijil;
                        }
                    }
                    //else
                    //{
                    //    MaklumatSijil mSijil = new MaklumatSijil();
                    //    mSijil.HR_NO_PEKERJA = null;
                    //    mSijil.HR_TARIKH_DIPEROLEHI = null;
                    //    mSijil.HR_NAMA_SIJIL_PEPERIKSAAN = null;
                    //    mSijil.HR_ANJURAN = null;
                    //    mSijil.HR_KEPUTUSAN = null;
                    //    sSijil.Add(mSijil);
                    //    mKakitangan.HR_MAKLUMAT_SIJIL = sSijil;
                    //}

                    if (lKursusLatihan.Count() > 0)
                    {
                        foreach (var item in lKursusLatihan)
                        {
                            MaklumatKursusLatihan mKursusLatihan = new MaklumatKursusLatihan();
                            mKursusLatihan.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKursusLatihan.HR_KOD_KURSUS = item.HR_KOD_KURSUS;
                            mKursusLatihan.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mKursusLatihan.HR_TARIKH_TAMAT = item.HR_TARIKH_TAMAT;
                            mKursusLatihan.HR_ANJURAN = item.HR_ANJURAN;
                            mKursusLatihan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                            sKursusLatihan.Add(mKursusLatihan);
                            mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN = sKursusLatihan;
                        }
                    }
                    //else
                    //{
                    //    MaklumatKursusLatihan mKursusLatihan = new MaklumatKursusLatihan();
                    //    mKursusLatihan.HR_NO_PEKERJA = null;
                    //    mKursusLatihan.HR_KOD_KURSUS = null;
                    //    mKursusLatihan.HR_TARIKH_MULA = null;
                    //    mKursusLatihan.HR_TARIKH_TAMAT = null;
                    //    mKursusLatihan.HR_ANJURAN = null;
                    //    mKursusLatihan.HR_KEPUTUSAN = null;
                    //    sKursusLatihan.Add(mKursusLatihan);
                    //    mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN = sKursusLatihan;
                    //}

                    if (lAktiviti.Count() > 0)
                    {
                        foreach (var item in lAktiviti)
                        {
                            MaklumatAktiviti mAktiviti = new MaklumatAktiviti();
                            mAktiviti.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mAktiviti.HR_TARIKH_AKTIVITI = item.HR_TARIKH_AKTIVITI;
                            mAktiviti.HR_PERINGKAT = item.HR_PERINGKAT;
                            mAktiviti.HR_NAMA_AKTIVITI = item.HR_NAMA_AKTIVITI;
                            mAktiviti.HR_ANJURAN = item.HR_ANJURAN;
                            sAktiviti.Add(mAktiviti);
                            mKakitangan.HR_MAKLUMAT_AKTIVITI = sAktiviti;
                        }
                    }
                    //else
                    //{
                    //    MaklumatAktiviti mAktiviti = new MaklumatAktiviti();
                    //    mAktiviti.HR_NO_PEKERJA = null;
                    //    mAktiviti.HR_TARIKH_AKTIVITI = null;
                    //    mAktiviti.HR_PERINGKAT = null;
                    //    mAktiviti.HR_NAMA_AKTIVITI = null;
                    //    mAktiviti.HR_ANJURAN = null;
                    //    sAktiviti.Add(mAktiviti);
                    //    mKakitangan.HR_MAKLUMAT_AKTIVITI = sAktiviti;
                    //}

                    if (lPewaris.Count() > 0)
                    {
                        foreach (var item in lPewaris)
                        {
                            MaklumatPewaris mPewaris = new MaklumatPewaris();
                            mPewaris.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mPewaris.HR_NAMA_PEWARIS = item.HR_NAMA_PEWARIS;
                            mPewaris.HR_TARIKH_LAHIR = item.HR_TARIKH_LAHIR;
                            mPewaris.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR;
                            if (item.HR_TEMPAT_LAHIR != null)
                            {
                                mPewaris.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR.Trim();
                            }
                            
                            mPewaris.HR_JANTINA = item.HR_JANTINA;
                            mPewaris.HR_PALAMAT1 = item.HR_PALAMAT1;
                            mPewaris.HR_PALAMAT2 = item.HR_PALAMAT2;
                            mPewaris.HR_PALAMAT3 = item.HR_PALAMAT3;
                            mPewaris.HR_PBANDAR = item.HR_PBANDAR;
                            mPewaris.HR_PPOSKOD = item.HR_PPOSKOD;
                            mPewaris.HR_PNEGERI = item.HR_PNEGERI;
                            if (item.HR_PNEGERI != null)
                            {
                                mPewaris.HR_PNEGERI = item.HR_PNEGERI.Trim();
                            }
                            mPewaris.HR_HUBUNGAN = null;
                            if (item.HR_HUBUNGAN != null)
                            {
                                mPewaris.HR_HUBUNGAN = new string(item.HR_HUBUNGAN.TakeWhile(x => char.IsDigit(x)).ToArray());
                            }
                            mPewaris.HR_TELRUMAH = item.HR_TELRUMAH;
                            mPewaris.HR_TELPEJABAT = item.HR_TELPEJABAT;
                            mPewaris.HR_TELBIMBIT = item.HR_TELBIMBIT;
                            mPewaris.HR_NO_KP = item.HR_NO_KP;
                            mPewaris.HR_NO_KP_LAMA = item.HR_NO_KP_LAMA;
                            mPewaris.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mPewaris.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mPewaris.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mPewaris.HR_NP_UBAH = item.HR_NP_UBAH;
                            mPewaris.HR_PEWARIS_IND = item.HR_PEWARIS_IND;
                            sPewaris.Add(mPewaris);
                            mKakitangan.HR_MAKLUMAT_PEWARIS = sPewaris;

                        }
                    }
                    else
                    {
                        MaklumatPewaris mPewaris = new MaklumatPewaris();
                        mPewaris.HR_NO_PEKERJA = null;
                        mPewaris.HR_NAMA_PEWARIS = null;
                        mPewaris.HR_TARIKH_LAHIR = null;
                        mPewaris.HR_TEMPAT_LAHIR = null;
                        mPewaris.HR_JANTINA = null;
                        mPewaris.HR_PALAMAT1 = null;
                        mPewaris.HR_PALAMAT2 = null;
                        mPewaris.HR_PALAMAT3 = null;
                        mPewaris.HR_PBANDAR = null;
                        mPewaris.HR_PPOSKOD = null;
                        mPewaris.HR_PNEGERI = null;
                        mPewaris.HR_HUBUNGAN = null;

                        mPewaris.HR_TELRUMAH = null;
                        mPewaris.HR_TELPEJABAT = null;
                        mPewaris.HR_TELBIMBIT = null;
                        mPewaris.HR_NO_KP = null;
                        mPewaris.HR_NO_KP_LAMA = null;
                        mPewaris.HR_TARIKH_KEYIN = null;
                        mPewaris.HR_NP_KEYIN = null;
                        mPewaris.HR_TARIKH_UBAH = null;
                        mPewaris.HR_NP_UBAH = null;
                        mPewaris.HR_PEWARIS_IND = null;
                        sPewaris.Add(mPewaris);
                        mKakitangan.HR_MAKLUMAT_PEWARIS = sPewaris;

                    }

                    if (lTanggungan.Count() > 0)
                    {
                        foreach (var item in lTanggungan)
                        {
                            MaklumatTanggungan mTanggungan = new MaklumatTanggungan();
                            mTanggungan.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mTanggungan.HR_NAMA_TANGGUNGAN = item.HR_NAMA_TANGGUNGAN;
                            mTanggungan.HR_TARIKH_LAHIR = item.HR_TARIKH_LAHIR;
                            mTanggungan.HR_NO_KP = item.HR_NO_KP;
                            mTanggungan.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR;
                            mTanggungan.HR_SEK_IPT = item.HR_SEK_IPT;
                            mTanggungan.HR_HUBUNGAN = item.HR_HUBUNGAN;
                            if (item.HR_HUBUNGAN != null)
                            {
                                mTanggungan.HR_HUBUNGAN = new string(item.HR_HUBUNGAN.TakeWhile(x => char.IsDigit(x)).ToArray());
                            }
                            mTanggungan.HR_JANTINA = item.HR_JANTINA;
                            mTanggungan.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mTanggungan.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mTanggungan.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mTanggungan.HR_NP_UBAH = item.HR_NP_UBAH;
                            sTanggungan.Add(mTanggungan);
                            mKakitangan.HR_MAKLUMAT_TANGGUNGAN = sTanggungan;
                        }
                    }
                    //else
                    //{
                    //    MaklumatTanggungan mTanggungan = new MaklumatTanggungan();
                    //    mTanggungan.HR_NO_PEKERJA = null;
                    //    mTanggungan.HR_NAMA_TANGGUNGAN = null;
                    //    mTanggungan.HR_TARIKH_LAHIR = null;
                    //    mTanggungan.HR_NO_KP = null;
                    //    mTanggungan.HR_TEMPAT_LAHIR = null;
                    //    mTanggungan.HR_SEK_IPT = null;
                    //    mTanggungan.HR_HUBUNGAN = null;
                    //    mTanggungan.HR_JANTINA = null;
                    //    mTanggungan.HR_TARIKH_KEYIN = null;
                    //    mTanggungan.HR_NP_KEYIN = null;
                    //    mTanggungan.HR_TARIKH_UBAH = null;
                    //    mTanggungan.HR_NP_UBAH = null;
                    //    sTanggungan.Add(mTanggungan);
                    //    mKakitangan.HR_MAKLUMAT_TANGGUNGAN = sTanggungan;
                    //}

                    if (mKuarters == null)
                    {
                        mKakitangan.HR_MAKLUMAT_KUARTERS = new MaklumatKuarters();
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS = new HR_KUARTERS();
                    }
                    else
                    {
                        mKakitangan.HR_MAKLUMAT_KUARTERS = new MaklumatKuarters();
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_NO_PEKERJA = mKuarters.HR_NO_PEKERJA;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS = mKuarters.HR_KOD_KUARTERS;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_MASUK = mKuarters.HR_TARIKH_MASUK;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS2 = mKuarters.HR_KOD_KUARTERS;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_MASUK2 = mKuarters.HR_TARIKH_MASUK;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_KELUAR = mKuarters.HR_TARIKH_KELUAR;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_NO_UNIT = mKuarters.HR_NO_UNIT;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN2X = mKuarters.HR_GANDAAN2X;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GERAI = mKuarters.HR_GERAI;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_CATATAN = mKuarters.HR_CATATAN;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_IDP = mKuarters.HR_IDP;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_AKTIF_IND = mKuarters.HR_AKTIF_IND;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN5X = mKuarters.HR_GANDAAN5X;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_JUMLAH_POTONGAN = mKuarters.HR_JUMLAH_POTONGAN;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS = new HR_KUARTERS();
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_KOD_KUARTERS = mKuarters2.HR_KOD_KUARTERS;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_AKTIF_IND = mKuarters2.HR_AKTIF_IND;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT1 = mKuarters2.HR_ALAMAT1;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT2 = mKuarters2.HR_ALAMAT2;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT3 = mKuarters2.HR_ALAMAT3;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BANDAR = mKuarters2.HR_BANDAR;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BLOK_KUARTERS = mKuarters2.HR_BLOK_KUARTERS;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_MAKLUMAT_KUARTERS = mKuarters2.HR_MAKLUMAT_KUARTERS;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_NEGERI = mKuarters2.HR_NEGERI;
                        mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_POSKOD = mKuarters2.HR_POSKOD;
                    }

                    if (lElaunPotonganG.Count() > 0)
                    {
                        foreach (var item in lElaunPotonganG)
                        {
                            MaklumatElaunPotongan mElaunPotonganG = new MaklumatElaunPotongan();
                            mElaunPotonganG.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mElaunPotonganG.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                            mElaunPotonganG.HR_PENERANGAN = item.HR_PENERANGAN;
                            mElaunPotonganG.HR_NO_FAIL = item.HR_NO_FAIL;
                            mElaunPotonganG.HR_JUMLAH = item.HR_JUMLAH;
                            mElaunPotonganG.HR_ELAUN_POTONGAN_IND = "G";
                            mElaunPotonganG.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                            mElaunPotonganG.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mElaunPotonganG.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                            mElaunPotonganG.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                            mElaunPotonganG.HR_BAKI = item.HR_BAKI;
                            mElaunPotonganG.HR_AKTIF_IND = item.HR_AKTIF_IND;
                            mElaunPotonganG.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                            mElaunPotonganG.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                            mElaunPotonganG.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mElaunPotonganG.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mElaunPotonganG.HR_UBAH_IND = item.HR_UBAH_IND;
                            mElaunPotonganG.HR_GRED_PT = item.HR_GRED_PT;
                            mElaunPotonganG.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                            mElaunPotonganG.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mElaunPotonganG.HR_NP_UBAH = item.HR_NP_UBAH;
                            mElaunPotonganG.HR_AUTO_IND = item.HR_AUTO_IND;
                            sElaunPotonganG.Add(mElaunPotonganG);
                            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G = sElaunPotonganG;
                        }
                    }
                    //else
                    //{
                    //    MaklumatElaunPotongan mElaunPotonganG = new MaklumatElaunPotongan();
                    //    mElaunPotonganG.HR_NO_PEKERJA = null;
                    //    mElaunPotonganG.HR_KOD_ELAUN_POTONGAN = null;
                    //    mElaunPotonganG.HR_PENERANGAN = null;
                    //    mElaunPotonganG.HR_NO_FAIL = null;
                    //    mElaunPotonganG.HR_JUMLAH = null;
                    //    mElaunPotonganG.HR_ELAUN_POTONGAN_IND = "G";
                    //    mElaunPotonganG.HR_MOD_BAYARAN = null;
                    //    mElaunPotonganG.HR_TARIKH_MULA = null;
                    //    mElaunPotonganG.HR_TARIKH_AKHIR = null;
                    //    mElaunPotonganG.HR_TUNTUTAN_MAKSIMA = null;
                    //    mElaunPotonganG.HR_BAKI = null;
                    //    mElaunPotonganG.HR_AKTIF_IND = "T";
                    //    mElaunPotonganG.HR_HARI_BEKERJA = null;
                    //    mElaunPotonganG.HR_NO_PEKERJA_PT = null;
                    //    mElaunPotonganG.HR_TARIKH_KEYIN = null;
                    //    mElaunPotonganG.HR_TARIKH_UBAH = null;
                    //    mElaunPotonganG.HR_UBAH_IND = null;
                    //    mElaunPotonganG.HR_GRED_PT = null;
                    //    mElaunPotonganG.HR_MATRIKS_GAJI_PT = null;
                    //    mElaunPotonganG.HR_NP_KEYIN = null;
                    //    mElaunPotonganG.HR_NP_UBAH = null;
                    //    mElaunPotonganG.HR_AUTO_IND = "Y";
                    //    sElaunPotonganG.Add(mElaunPotonganG);
                    //    mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G = sElaunPotonganG;
                    //}

                    if (lElaunPotonganE.Count() > 0)
                    {
                        foreach (var item in lElaunPotonganE)
                        {
                            MaklumatElaunPotongan mElaunPotonganE = new MaklumatElaunPotongan();
                            mElaunPotonganE.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mElaunPotonganE.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                            mElaunPotonganE.HR_PENERANGAN = item.HR_PENERANGAN;
                            mElaunPotonganE.HR_NO_FAIL = item.HR_NO_FAIL;
                            mElaunPotonganE.HR_JUMLAH = item.HR_JUMLAH;
                            mElaunPotonganE.HR_ELAUN_POTONGAN_IND = "E";
                            mElaunPotonganE.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                            mElaunPotonganE.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mElaunPotonganE.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                            mElaunPotonganE.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                            mElaunPotonganE.HR_BAKI = item.HR_BAKI;
                            mElaunPotonganE.HR_AKTIF_IND = item.HR_AKTIF_IND;
                            mElaunPotonganE.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                            mElaunPotonganE.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                            mElaunPotonganE.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mElaunPotonganE.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mElaunPotonganE.HR_UBAH_IND = item.HR_UBAH_IND;
                            mElaunPotonganE.HR_GRED_PT = item.HR_GRED_PT;
                            mElaunPotonganE.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;

                            mElaunPotonganE.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mElaunPotonganE.HR_NP_UBAH = item.HR_NP_UBAH;
                            mElaunPotonganE.HR_AUTO_IND = item.HR_AUTO_IND;
                            sElaunPotonganE.Add(mElaunPotonganE);
                            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E = sElaunPotonganE;
                        }
                    }
                    //else
                    //{
                    //    MaklumatElaunPotongan mElaunPotonganE = new MaklumatElaunPotongan();
                    //    mElaunPotonganE.HR_NO_PEKERJA = null;
                    //    mElaunPotonganE.HR_KOD_ELAUN_POTONGAN = null;
                    //    mElaunPotonganE.HR_PENERANGAN = null;
                    //    mElaunPotonganE.HR_NO_FAIL = null;
                    //    mElaunPotonganE.HR_JUMLAH = null;
                    //    mElaunPotonganE.HR_ELAUN_POTONGAN_IND = "E";
                    //    mElaunPotonganE.HR_MOD_BAYARAN = null;
                    //    mElaunPotonganE.HR_TARIKH_MULA = null;
                    //    mElaunPotonganE.HR_TARIKH_AKHIR = null;
                    //    mElaunPotonganE.HR_TUNTUTAN_MAKSIMA = null;
                    //    mElaunPotonganE.HR_BAKI = null;
                    //    mElaunPotonganE.HR_AKTIF_IND = "T";
                    //    mElaunPotonganE.HR_HARI_BEKERJA = null;
                    //    mElaunPotonganE.HR_NO_PEKERJA_PT = null;
                    //    mElaunPotonganE.HR_TARIKH_KEYIN = null;
                    //    mElaunPotonganE.HR_TARIKH_UBAH = null;
                    //    mElaunPotonganE.HR_UBAH_IND = null;
                    //    mElaunPotonganE.HR_GRED_PT = null;
                    //    mElaunPotonganE.HR_MATRIKS_GAJI_PT = null;
                    //    mElaunPotonganE.HR_NP_KEYIN = null;
                    //    mElaunPotonganE.HR_NP_UBAH = null;
                    //    mElaunPotonganE.HR_AUTO_IND = "Y";
                    //    sElaunPotonganE.Add(mElaunPotonganE);
                    //    mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E = sElaunPotonganE;
                    //}

                    if (lElaunPotonganP.Count() > 0)
                    {

                        foreach (var item in lElaunPotonganP)
                        {
                            MaklumatElaunPotongan mElaunPotonganP = new MaklumatElaunPotongan();
                            mElaunPotonganP.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mElaunPotonganP.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                            mElaunPotonganP.HR_PENERANGAN = item.HR_PENERANGAN;
                            mElaunPotonganP.HR_NO_FAIL = item.HR_NO_FAIL;
                            mElaunPotonganP.HR_JUMLAH = item.HR_JUMLAH;
                            mElaunPotonganP.HR_ELAUN_POTONGAN_IND = "P";
                            mElaunPotonganP.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                            mElaunPotonganP.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mElaunPotonganP.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                            mElaunPotonganP.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                            mElaunPotonganP.HR_BAKI = item.HR_BAKI;
                            mElaunPotonganP.HR_AKTIF_IND = item.HR_AKTIF_IND;
                            mElaunPotonganP.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                            mElaunPotonganP.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                            mElaunPotonganP.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mElaunPotonganP.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mElaunPotonganP.HR_UBAH_IND = item.HR_UBAH_IND;
                            mElaunPotonganP.HR_GRED_PT = item.HR_GRED_PT;
                            mElaunPotonganP.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                            mElaunPotonganP.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mElaunPotonganP.HR_NP_UBAH = item.HR_NP_UBAH;
                            mElaunPotonganP.HR_AUTO_IND = item.HR_AUTO_IND;
                            sElaunPotonganP.Add(mElaunPotonganP);
                            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P = sElaunPotonganP;

                        }
                    }
                    //else
                    //{
                    //    MaklumatElaunPotongan mElaunPotonganP = new MaklumatElaunPotongan();
                    //    mElaunPotonganP.HR_NO_PEKERJA = null;
                    //    mElaunPotonganP.HR_KOD_ELAUN_POTONGAN = null;
                    //    mElaunPotonganP.HR_PENERANGAN = null;
                    //    mElaunPotonganP.HR_NO_FAIL = null;
                    //    mElaunPotonganP.HR_JUMLAH = null;
                    //    mElaunPotonganP.HR_ELAUN_POTONGAN_IND = "P";
                    //    mElaunPotonganP.HR_MOD_BAYARAN = null;
                    //    mElaunPotonganP.HR_TARIKH_MULA = null;
                    //    mElaunPotonganP.HR_TARIKH_AKHIR = null;
                    //    mElaunPotonganP.HR_TUNTUTAN_MAKSIMA = null;
                    //    mElaunPotonganP.HR_BAKI = null;
                    //    mElaunPotonganP.HR_AKTIF_IND = "T";
                    //    mElaunPotonganP.HR_HARI_BEKERJA = null;
                    //    mElaunPotonganP.HR_NO_PEKERJA_PT = null;
                    //    mElaunPotonganP.HR_TARIKH_KEYIN = null;
                    //    mElaunPotonganP.HR_TARIKH_UBAH = null;
                    //    mElaunPotonganP.HR_UBAH_IND = null;
                    //    mElaunPotonganP.HR_GRED_PT = null;
                    //    mElaunPotonganP.HR_MATRIKS_GAJI_PT = null;

                    //    mElaunPotonganP.HR_NP_KEYIN = null;
                    //    mElaunPotonganP.HR_NP_UBAH = null;
                    //    mElaunPotonganP.HR_AUTO_IND = "Y";
                    //    sElaunPotonganP.Add(mElaunPotonganP);
                    //    mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P = sElaunPotonganP;
                    //}

                    if (lElaunPotonganC.Count() > 0)
                    {
                        foreach (var item in lElaunPotonganC)
                        {
                            MaklumatElaunPotongan mElaunPotonganC = new MaklumatElaunPotongan();
                            mElaunPotonganC.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mElaunPotonganC.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                            mElaunPotonganC.HR_PENERANGAN = item.HR_PENERANGAN;
                            mElaunPotonganC.HR_NO_FAIL = item.HR_NO_FAIL;
                            mElaunPotonganC.HR_JUMLAH = item.HR_JUMLAH;
                            mElaunPotonganC.HR_ELAUN_POTONGAN_IND = "C";
                            mElaunPotonganC.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                            mElaunPotonganC.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                            mElaunPotonganC.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                            mElaunPotonganC.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                            mElaunPotonganC.HR_BAKI = item.HR_BAKI;
                            mElaunPotonganC.HR_AKTIF_IND = item.HR_AKTIF_IND;
                            mElaunPotonganC.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                            mElaunPotonganC.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                            mElaunPotonganC.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                            mElaunPotonganC.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                            mElaunPotonganC.HR_UBAH_IND = item.HR_UBAH_IND;
                            mElaunPotonganC.HR_GRED_PT = item.HR_GRED_PT;
                            mElaunPotonganC.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                            mElaunPotonganC.HR_NP_KEYIN = item.HR_NP_KEYIN;
                            mElaunPotonganC.HR_NP_UBAH = item.HR_NP_UBAH;
                            mElaunPotonganC.HR_AUTO_IND = item.HR_AUTO_IND;
                            sElaunPotonganC.Add(mElaunPotonganC);
                            mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C = sElaunPotonganC;
                        }
                    }
                    //else
                    //{
                    //    MaklumatElaunPotongan mElaunPotonganC = new MaklumatElaunPotongan();
                    //    mElaunPotonganC.HR_NO_PEKERJA = null;
                    //    mElaunPotonganC.HR_KOD_ELAUN_POTONGAN = null;
                    //    mElaunPotonganC.HR_PENERANGAN = null;
                    //    mElaunPotonganC.HR_NO_FAIL = null;
                    //    mElaunPotonganC.HR_JUMLAH = null;
                    //    mElaunPotonganC.HR_ELAUN_POTONGAN_IND = "C";
                    //    mElaunPotonganC.HR_MOD_BAYARAN = null;
                    //    mElaunPotonganC.HR_TARIKH_MULA = null;
                    //    mElaunPotonganC.HR_TARIKH_AKHIR = null;
                    //    mElaunPotonganC.HR_TUNTUTAN_MAKSIMA = null;
                    //    mElaunPotonganC.HR_BAKI = null;
                    //    mElaunPotonganC.HR_AKTIF_IND = "T";
                    //    mElaunPotonganC.HR_HARI_BEKERJA = null;
                    //    mElaunPotonganC.HR_NO_PEKERJA_PT = null;
                    //    mElaunPotonganC.HR_TARIKH_KEYIN = null;
                    //    mElaunPotonganC.HR_TARIKH_UBAH = null;
                    //    mElaunPotonganC.HR_UBAH_IND = null;
                    //    mElaunPotonganC.HR_GRED_PT = null;
                    //    mElaunPotonganC.HR_MATRIKS_GAJI_PT = null;
                    //    mElaunPotonganC.HR_NP_KEYIN = null;
                    //    mElaunPotonganC.HR_NP_UBAH = null;
                    //    mElaunPotonganC.HR_AUTO_IND = "Y";
                    //    sElaunPotonganC.Add(mElaunPotonganC);
                    //    mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C = sElaunPotonganC;
                    //}

                    if (lPKurniaan.Count() > 0)
                    {
                        var noKurniaan = 0;
                        foreach (var item in lPKurniaan)
                        {
                            MaklumatKurniaan mKurniaan = new MaklumatKurniaan();
                            mKurniaan.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mKurniaan.HR_TARIKH_KURNIAAN = null;
                            if (lKurniaan.Count() > 0)
                            {
                                mKurniaan.HR_TARIKH_KURNIAAN = lKurniaan.ElementAt(noKurniaan).HR_TARIKH_KURNIAAN;
                            }

                            mKurniaan.HR_KOD_KURNIAAN = item.HR_KOD_KURNIAAN;
                            mKurniaan.HR_KURNIAAN_IND = item.HR_KURNIAAN_IND;
                            mKurniaan.HR_PERINGKAT = null;
                            if (mKurniaan.HR_KURNIAAN_IND != null)
                            {
                                mKurniaan.HR_PERINGKAT = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 130 && s.STRING_PARAM == mKurniaan.HR_KURNIAAN_IND).SHORT_DESCRIPTION;
                            }

                            mKurniaan.HR_NEGERI = item.HR_KURNIAAN_IND;
                            mKurniaan.HR_STATUS = item.HR_STATUS;

                            mKurniaan.HR_TARIKH_PENCALONAN = item.HR_TARIKH_PENCALONAN;
                            mKurniaan.HR_NP_PENCALON = item.HR_NP_PENCALON;
                            sKurniaan.Add(mKurniaan);
                            mKakitangan.HR_MAKLUMAT_KURNIAAN = sKurniaan;
                            noKurniaan++;
                        }
                    }
                    //else
                    //{
                    //    MaklumatKurniaan mKurniaan = new MaklumatKurniaan();
                    //    mKurniaan.HR_NO_PEKERJA = null;
                    //    mKurniaan.HR_TARIKH_KURNIAAN = null;
                    //    mKurniaan.HR_KOD_KURNIAAN = null;
                    //    mKurniaan.HR_KURNIAAN_IND = null;
                    //    mKurniaan.HR_NEGERI = null;
                    //    mKurniaan.HR_STATUS = null;

                    //    mKurniaan.HR_TARIKH_PENCALONAN = null;
                    //    mKurniaan.HR_NP_PENCALON = null;
                    //    sKurniaan.Add(mKurniaan);
                    //    mKakitangan.HR_MAKLUMAT_KURNIAAN = sKurniaan;
                    //}

                    if (lAnugerahCemerlang.Count() > 0)
                    {
                        foreach (var item in lAnugerahCemerlang)
                        {
                            MaklumatAnugerahCemerlang mAnugerahCemerlang = new MaklumatAnugerahCemerlang();
                            mAnugerahCemerlang.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mAnugerahCemerlang.HR_NAMA_ANUGERAH = item.HR_NAMA_ANUGERAH;
                            mAnugerahCemerlang.HR_TARIKH_PENERIMAAN = item.HR_TARIKH_PENERIMAAN;
                            mAnugerahCemerlang.HR_TAHUN = item.HR_TAHUN;
                            sAnugerahCemerlang.Add(mAnugerahCemerlang);
                            mKakitangan.HR_ANUGERAH_CEMERLANG = sAnugerahCemerlang;
                        }
                    }
                    //else
                    //{
                    //    MaklumatAnugerahCemerlang mAnugerahCemerlang = new MaklumatAnugerahCemerlang();
                    //    mAnugerahCemerlang.HR_NO_PEKERJA = null;
                    //    mAnugerahCemerlang.HR_NAMA_ANUGERAH = null;
                    //    mAnugerahCemerlang.HR_TARIKH_PENERIMAAN = null;
                    //    sAnugerahCemerlang.Add(mAnugerahCemerlang);
                    //    mKakitangan.HR_ANUGERAH_CEMERLANG = sAnugerahCemerlang;
                    //}

                    if (mAnugerahHaji != null)
                    {
                        mKakitangan.HR_ANUGERAH_HAJI = new MaklumatAnugerahHaji();
                        mKakitangan.HR_ANUGERAH_HAJI.HR_NO_PEKERJA = mAnugerahHaji.HR_NO_PEKERJA;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI = mAnugerahHaji.HR_TAHUN_PERGI;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI = mAnugerahHaji.HR_STATUS_HAJI;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP = mAnugerahHaji.HR_NP_YDP;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND = mAnugerahHaji.HR_LULUS_IND;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP = mAnugerahHaji.HR_NP_UP;
                        mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG = mAnugerahHaji.HR_NP_PEG;
                    }
                    //else
                    //{
                    //    mKakitangan.HR_ANUGERAH_HAJI = new MaklumatAnugerahHaji();
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_NO_PEKERJA = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP = null;
                    //    mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG = null;
                    //}

                    if (mPersaraan != null)
                    {
                        mKakitangan.HR_PERSARAAN = new MaklumatPersaraan();
                        mKakitangan.HR_PERSARAAN.HR_NO_PEKERJA = mPersaraan.HR_NO_PEKERJA;
                        mKakitangan.HR_PERSARAAN.HR_TARIKH_BERSARA = mPersaraan.HR_TARIKH_BERSARA;
                        mKakitangan.HR_PERSARAAN.HR_ALASAN = mPersaraan.HR_ALASAN;
                        mKakitangan.HR_PERSARAAN.HR_BERSARA_IND = mPersaraan.HR_BERSARA_IND;
                        mKakitangan.HR_PERSARAAN.HR_BAYARAN_IND = mPersaraan.HR_BAYARAN_IND;
                        mKakitangan.HR_PERSARAAN.HR_JUMLAH_BAYARAN = mPersaraan.HR_JUMLAH_BAYARAN;
                        mKakitangan.HR_PERSARAAN.HR_JUMLAH_CUTI = mPersaraan.HR_JUMLAH_CUTI;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT1 = mPersaraan.HR_PALAMAT1;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT2 = mPersaraan.HR_PALAMAT2;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT3 = mPersaraan.HR_PALAMAT3;
                        mKakitangan.HR_PERSARAAN.HR_PBANDAR = mPersaraan.HR_PBANDAR;
                        mKakitangan.HR_PERSARAAN.HR_PPOSKOD = mPersaraan.HR_PPOSKOD;
                        mKakitangan.HR_PERSARAAN.HR_PNEGERI = mPersaraan.HR_PNEGERI;
                        mKakitangan.HR_PERSARAAN.HR_EKA = mPersaraan.HR_EKA;
                        mKakitangan.HR_PERSARAAN.HR_ITP = mPersaraan.HR_ITP;
                        mKakitangan.HR_PERSARAAN.HR_GAJI_POKOK = mPersaraan.HR_GAJI_POKOK;
                        mKakitangan.HR_PERSARAAN.HR_TERIMA_BAYARAN_IND = mPersaraan.HR_TERIMA_BAYARAN_IND;
                        mKakitangan.HR_PERSARAAN.HR_NP_PEGAWAI = mPersaraan.HR_NP_PEGAWAI;
                        mKakitangan.HR_PERSARAAN.HR_JAWATAN_PEGAWAI = mPersaraan.HR_JAWATAN_PEGAWAI;
                    }
                    else
                    {
                        mKakitangan.HR_PERSARAAN = new MaklumatPersaraan();
                        mKakitangan.HR_PERSARAAN.HR_NO_PEKERJA = null;
                        mKakitangan.HR_PERSARAAN.HR_TARIKH_BERSARA = null;
                        mKakitangan.HR_PERSARAAN.HR_ALASAN = null;
                        mKakitangan.HR_PERSARAAN.HR_BERSARA_IND = null;
                        mKakitangan.HR_PERSARAAN.HR_BAYARAN_IND = null;
                        mKakitangan.HR_PERSARAAN.HR_JUMLAH_BAYARAN = null;
                        mKakitangan.HR_PERSARAAN.HR_JUMLAH_CUTI = null;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT1 = null;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT2 = null;
                        mKakitangan.HR_PERSARAAN.HR_PALAMAT3 = null;
                        mKakitangan.HR_PERSARAAN.HR_PBANDAR = null;
                        mKakitangan.HR_PERSARAAN.HR_PPOSKOD = null;
                        mKakitangan.HR_PERSARAAN.HR_PNEGERI = null;
                        mKakitangan.HR_PERSARAAN.HR_EKA = null;
                        mKakitangan.HR_PERSARAAN.HR_ITP = null;
                        mKakitangan.HR_PERSARAAN.HR_GAJI_POKOK = null;
                        mKakitangan.HR_PERSARAAN.HR_TERIMA_BAYARAN_IND = null;
                        mKakitangan.HR_PERSARAAN.HR_NP_PEGAWAI = null;
                        mKakitangan.HR_PERSARAAN.HR_JAWATAN_PEGAWAI = null;
                    }

                    if (lTindakanDisiplin.Count() > 0)
                    {
                        foreach (var item in lTindakanDisiplin)
                        {
                            MaklumatTindakanDisiplin mTindakanDisiplin = new MaklumatTindakanDisiplin();
                            mTindakanDisiplin.HR_NO_PEKERJA = item.HR_NO_PEKERJA;
                            mTindakanDisiplin.HR_TARIKH_KESALAHAN = item.HR_TARIKH_KESALAHAN;
                            mTindakanDisiplin.HR_KESALAHAN = item.HR_KESALAHAN;
                            foreach (var item2 in lTindakanDisiplinDetail.Where(s => s.HR_TARIKH_KESALAHAN == item.HR_TARIKH_KESALAHAN))
                            {
                                mTindakanDisiplin.HR_KOD_TINDAKAN = item2.HR_KOD_TINDAKAN;
                                mTindakanDisiplin.HR_TARIKH_MULA = item2.HR_TARIKH_MULA;
                                mTindakanDisiplin.HR_TARIKH_AKHIR = item2.HR_TARIKH_AKHIR;
                                sTindakanDisiplin.Add(mTindakanDisiplin);
                                mKakitangan.HR_TINDAKAN_DISIPLIN = sTindakanDisiplin;
                            }

                        }

                    }
                    //else
                    //{
                    //    MaklumatTindakanDisiplin mTindakanDisiplin = new MaklumatTindakanDisiplin();
                    //    mTindakanDisiplin.HR_NO_PEKERJA = null;
                    //    mTindakanDisiplin.HR_TARIKH_KESALAHAN = null;
                    //    mTindakanDisiplin.HR_KESALAHAN = null;
                    //    mTindakanDisiplin.HR_KOD_TINDAKAN = null;
                    //    mTindakanDisiplin.HR_TARIKH_MULA = null;
                    //    mTindakanDisiplin.HR_TARIKH_AKHIR = null;
                    //    sTindakanDisiplin.Add(mTindakanDisiplin);
                    //    mKakitangan.HR_TINDAKAN_DISIPLIN = sTindakanDisiplin;

                    //}

                    if (mKematian != null)
                    {
                        mKakitangan.HR_MAKLUMAT_KEMATIAN = new MaklumatKematian();
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_PEKERJA = mKematian.HR_NO_PEKERJA;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_KEMATIAN = mKematian.HR_TARIKH_KEMATIAN;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_KP_PEWARIS = mKematian.HR_NO_KP_PEWARIS;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT1 = mKematian.HR_ALAMAT1;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT2 = mKematian.HR_ALAMAT2;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT3 = mKematian.HR_ALAMAT3;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_BANDAR = mKematian.HR_BANDAR;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELRUMAH = mKematian.HR_NO_TELRUMAH;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_HUBUNGAN = mKematian.HR_HUBUNGAN;
                        if (mKematian.HR_HUBUNGAN != null)
                        {
                            mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_HUBUNGAN = new string(mKematian.HR_HUBUNGAN.TakeWhile(x => char.IsDigit(x)).ToArray());
                        }
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_POSKOD = mKematian.HR_POSKOD;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NEGERI = mKematian.HR_NEGERI;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEWARIS = mKematian.HR_NAMA_PEWARIS;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELPEJABAT = mKematian.HR_NO_TELPEJABAT;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELBIMBIT = mKematian.HR_NO_TELBIMBIT;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_VOUCHER = mKematian.HR_NO_VOUCHER;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEGAWAI = mKematian.HR_NAMA_PEGAWAI;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JAWATAN_PEGAWAI = mKematian.HR_JAWATAN_PEGAWAI;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_BAYAR = mKematian.HR_TARIKH_BAYAR;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_MAKLUMAT_KHIDMAT = mKematian.HR_MAKLUMAT_KHIDMAT;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JUMLAH_WANG = mKematian.HR_JUMLAH_WANG;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_VOT = mKematian.HR_VOT;
                    }
                    else
                    {
                        mKakitangan.HR_MAKLUMAT_KEMATIAN = new MaklumatKematian();
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_PEKERJA = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_KEMATIAN = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_KP_PEWARIS = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT1 = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT2 = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT3 = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_BANDAR = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELRUMAH = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_HUBUNGAN = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_POSKOD = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NEGERI = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEWARIS = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELPEJABAT = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELBIMBIT = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_VOUCHER = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEGAWAI = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JAWATAN_PEGAWAI = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_BAYAR = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_MAKLUMAT_KHIDMAT = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JUMLAH_WANG = null;
                        mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_VOT = null;
                    }

                    if (mPrestasi != null)
                    {
                        mKakitangan.HR_PENILAIAN_PRESTASI = new MaklumatPenilaianPrestasi();
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_NO_PEKERJA = mPrestasi.HR_NO_PEKERJA;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI = mPrestasi.HR_TAHUN_PRESTASI;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI2 = mPrestasi.HR_TAHUN_PRESTASI;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPP = mPrestasi.HR_PENGHASILAN_PPP;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPK = mPrestasi.HR_PENGHASILAN_PPK;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPP = mPrestasi.HR_PENGETAHUAN_PPP;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPK = mPrestasi.HR_PENGETAHUAN_PPK;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPP = mPrestasi.HR_KUALITI_PPP;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPK = mPrestasi.HR_KUALITI_PPK;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPP = mPrestasi.HR_SUMBANGAN_PPP;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPK = mPrestasi.HR_SUMBANGAN_PPK;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGHASILAN = mPrestasi.HR_PURATA_PENGHASILAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGETAHUAN = mPrestasi.HR_PURATA_PENGETAHUAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_KUALITI = mPrestasi.HR_PURATA_KUALITI;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_SUMBANGAN = mPrestasi.HR_PURATA_SUMBANGAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGHASILAN = mPrestasi.HR_PERATUS_PENGHASILAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGETAHUAN = mPrestasi.HR_PERATUS_PENGETAHUAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_KUALITI = mPrestasi.HR_PERATUS_KUALITI;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_SUMBANGAN = mPrestasi.HR_PERATUS_SUMBANGAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_JUMLAH_BESAR = mPrestasi.HR_JUMLAH_BESAR;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_CEMERLANG_IND = mPrestasi.HR_CEMERLANG_IND;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_JENIS_IND = mPrestasi.HR_JENIS_IND;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_KERAJAAN = mPrestasi.HR_CUTI_KERAJAAN;
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_SWASTA = mPrestasi.HR_CUTI_SWASTA;
                    }
                    else
                    {
                        mKakitangan.HR_PENILAIAN_PRESTASI = new MaklumatPenilaianPrestasi();
                        mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI = Convert.ToInt16(DateTime.Now.Year);
                    }

                    mKakitangan.HR_MAKLUMAT_TANGGUNG_KERJA = db.HR_MAKLUMAT_TANGGUNG_KERJA.Where(s => s.HR_NO_PEKERJA == value && s.HR_FINALISED_IND_HR == "Y").ToList();

                    if (MaklumatCuti.Count() > 0)
                    {
                        foreach (HR_MAKLUMAT_CUTI mCuti in MaklumatCuti)
                        {
                            HR_MAKLUMAT_CUTI_DETAIL mCutiDetail = db.HR_MAKLUMAT_CUTI_DETAIL.SingleOrDefault(s => s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI && s.HR_NO_PEKERJA == mCuti.HR_NO_PEKERJA && s.HR_TARIKH_PERMOHONAN == mCuti.HR_TARIKH_PERMOHONAN);
                            HR_CUTI KategoriCuti = db.HR_CUTI.SingleOrDefault(s => s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI);
                            MaklumatCuti data = new MaklumatCuti();
                            data.HR_NO_PEKERJA = mCuti.HR_NO_PEKERJA;
                            data.HR_TARIKH_PERMOHONAN = mCuti.HR_TARIKH_PERMOHONAN;
                            data.HR_TAHUN = mCuti.HR_TAHUN;
                            data.HR_TARIKH_MULA_CUTI = mCutiDetail.HR_TARIKH_MULA_CUTI;
                            data.HR_TARIKH_TAMAT_CUTI = mCutiDetail.HR_TARIKH_TAMAT_CUTI;
                            data.HR_KOD_CUTI = mCuti.HR_KOD_CUTI;

                            //data.HR_SOKONG_IND = mCutiDetail.HR_SOKONG_IND;
                            data.HR_LULUS_IND = "T";
                            //data.HR_LULUS_YDP_IND = mCutiDetail.HR_LULUS_YDP_IND;
                            var kategori = KategoriCuti.HR_KATEGORI;
                            if (KategoriCuti.HR_KATEGORI != null)
                            {
                                kategori = KategoriCuti.HR_KATEGORI.Trim();
                            }
                            data.HR_KATEGORI_CUTI = kategori;
                            if (kategori == "1")
                            {
                                if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_LULUS_IND == "Y")
                                {
                                    data.HR_LULUS_IND = "Y";
                                }
                            }
                            if (kategori == "2")
                            {
                                if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_HR_LULUS_IND == "Y" && mCutiDetail.HR_LULUS_YDP_IND == "Y")
                                {
                                    data.HR_LULUS_IND = "Y";
                                }
                            }

                            if (kategori == "3")
                            {
                                if (mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_HR_LULUS_IND == "Y")
                                {
                                    data.HR_LULUS_IND = "Y";
                                }
                            }
                            if (kategori == "4" && mCuti.HR_KOD_CUTI == "CU018")
                            {
                                if (mCutiDetail.HR_LULUS_IND == "Y" && mCutiDetail.HR_HR_LULUS_IND == "Y")
                                {
                                    data.HR_LULUS_IND = "Y";
                                }
                            }
                            SenaraiCuti.Add(data);
                            ViewBag.SenaraiCutiCount = SenaraiCuti.Count();
                            ViewBag.SenaraiCuti = SenaraiCuti;
                        }
                    }

                    List<HR_CUTI> KodCuti = db.HR_CUTI.ToList();
                    if (KodCuti.Count() < 0)
                    {
                        KodCuti = new List<HR_CUTI>();
                    }
                    ViewBag.KodCuti = KodCuti;

                    mKakitangan.HR_MAKLUMAT_CUTI = new MaklumatCuti();
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN = DateTime.Now;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_CUTI_REHAT = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_JUMLAH_MAKSIMUM = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TAHUN_LEPAS = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TAHUN = Convert.ToInt16(DateTime.Now.Year);
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_TEMP = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_PENCEN_TERKUMPUL = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_KELAYAKAN_BULANAN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_CUTI_DIAMBIL = null;

                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_MULA_CUTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_TAMAT_CUTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT1 = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT2 = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CALAMAT3 = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CBANDAR = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CPOSKOD = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_CNEGERI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TEL = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_PENGGANTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_PROGRAM = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TEMPAT_PROGRAM = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_ANJURAN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_ISTERI_BERSALIN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_ALASAN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_HUBUNGAN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_SIRI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_BIL_HARI_CUTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_SOKONG_IND = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_SOKONG = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_IND = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NP_KJ = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_HR_LULUS_IND = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_LULUS_HR = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_LULUS = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_ULASAN = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_LULUS_YDP_IND = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_YDP = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA_YDP = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_HARI_CUTI = null;
                    mKakitangan.HR_MAKLUMAT_CUTI.HR_NAMA_KLINIK = null;


                    //STRAT PERIBADI
                    ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
                    ViewBag.Agama = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 106), "STRING_PARAM", "SHORT_DESCRIPTION");
                    ViewBag.HR_WARGANEGARA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 2), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.TempatLahir = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION", mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TEMPAT_LAHIR);
                    ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
                    ViewBag.TarafKahwin = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_ALASAN", "HR_PENERANGAN");
                    ViewBag.Keturunan = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 1), "ORDINAL", "SHORT_DESCRIPTION");
                    //END PERIBADI

                    //START PEKERJAAN
                    ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA), "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
                    ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_GAJI_PRORATA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 116), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN.OrderBy(s => s.GE_KETERANGAN_JABATAN), "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
                    ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
                    ViewBag.HR_UNIT = new SelectList(db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_UNIT", "GE_KETERANGAN");
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
                    ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
                    Nullable<int> HR_UMUR_SARA = null;
                    if (mPekerjaan.HR_TARIKH_TAMAT != null && mPeribadi.ElementAt(0).HR_TARIKH_LAHIR != null)
                    {
                        HR_UMUR_SARA = (Convert.ToDateTime(mPekerjaan.HR_TARIKH_TAMAT).Year - Convert.ToDateTime(mPeribadi.ElementAt(0).HR_TARIKH_LAHIR).Year);
                    }
                    ViewBag.HR_UMUR_SARA = HR_UMUR_SARA;

                    ViewBag.HR_MATRIKS_GAJI = new SelectList(db.HR_MAKLUMAT_PEKERJAAN, "HR_MATRIKS_GAJI", "HR_MATRIKS_GAJI");
                    ViewBag.HR_KOD_GELARAN_J = new SelectList(db.HR_GELARAN_JAWATAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_GELARAN", "HR_PENERANGAN");
                    ViewBag.HR_TINGKATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 113), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_KOD_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "SHORT_DESCRIPTION");
                    ViewBag.HR_KATEGORI_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");

                    ViewBag.HR_JUM_TAHUN = null;
                    if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
                    {
                        ViewBag.HR_JUM_TAHUN = Convert.ToDateTime(mPekerjaan.HR_TARIKH_TAMAT_KONTRAK).Year - Convert.ToDateTime(mPekerjaan.HR_TARIKH_MASUK).Year;
                    }

                    List<HR_MAKLUMAT_PENGALAMAN_KERJA> sPengalaman = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == value).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
                    ViewBag.sPengalaman = sPengalaman;
                    //END PEKERJAAN

                    //START KEMAHIRAN
                    ViewBag.HR_BAHASA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 107), "STRING_PARAM", "SHORT_DESCRIPTION");
                    ViewBag.P_TAHAP_KEMAHIRAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 105), "STRING_PARAM", "SHORT_DESCRIPTION");
                    //END KEMAHIRAN

                    //START AKADEMIK
                    ViewBag.HR_KURSUS = db.HR_KURSUS.ToList();
                    List<SelectListItem> HR_PERINGKAT = new List<SelectListItem>();
                    HR_PERINGKAT.Add(new SelectListItem { Text = "KEBANGSAAN", Value = "KEBANGSAAN" });
                    HR_PERINGKAT.Add(new SelectListItem { Text = "NEGERI", Value = "NEGERI" });
                    HR_PERINGKAT.Add(new SelectListItem { Text = "DAERAH", Value = "DAERAH" });
                    HR_PERINGKAT.Add(new SelectListItem { Text = "JABATAN", Value = "JABATAN" });
                    ViewBag.HR_PERINGKAT = HR_PERINGKAT;

                    List<SelectListItem> HR_KEPUTUSAN = new List<SelectListItem>();
                    HR_KEPUTUSAN.Add(new SelectListItem { Text = "Lulus", Value = "Y" });
                    HR_KEPUTUSAN.Add(new SelectListItem { Text = "Tidak Lulus", Value = "T" });
                    ViewBag.HR_KEPUTUSAN = HR_KEPUTUSAN;
                    //END AKADEMIK

                    //START PEWARIS
                    ViewBag.HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125), "ORDINAL", "SHORT_DESCRIPTION");
                    //END PEWARIS

                    //TANGGUNGAN
                    ViewBag.HR_TEMPAT_LAHIR = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");

                    List<SelectListItem> HR_JANTINA = new List<SelectListItem>();
                    HR_JANTINA.Add(new SelectListItem { Text = "Lelaki", Value = "L" });
                    HR_JANTINA.Add(new SelectListItem { Text = "Perempuan", Value = "P" });
                    ViewBag.HR_JANTINA = HR_JANTINA;
                    //

                    //KUARTERS
                    ViewBag.HR_MAKLUMAT_KUARTERS_HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_KUARTERS = new SelectList(db.HR_KUARTERS, "HR_KOD_KUARTERS", "HR_BLOK_KUARTERS");
                    //

                    //ELAUN POTONGAN
                    var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
                    if(jawatan == null)
                    {
                        jawatan = new HR_JAWATAN();
                    }
                    ViewBag.jawatan = null;
                    if (jawatan.HR_NAMA_JAWATAN != null)
                    {
                        ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;
                    }
                    var gredEP = Convert.ToInt32(mPekerjaan.HR_GRED);
                    var KumpulanGred = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 109 && s.ORDINAL == gredEP);
                    ViewBag.KumpulanGred = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN + " / ";
                    if (KumpulanGred != null)
                    {
                        ViewBag.KumpulanGred = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN + " / " + KumpulanGred.SHORT_DESCRIPTION;
                    }
                    var jabatan = db2.GE_JABATAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN);
                    ViewBag.jabatan = null;
                    if (jabatan != null)
                    {
                        ViewBag.jabatan = jabatan.GE_KETERANGAN_JABATAN;
                    }
                    var bahagian = db2.GE_BAHAGIAN.SingleOrDefault(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN);
                    ViewBag.bahagian = null;
                    if (bahagian != null)
                    {
                        ViewBag.bahagian = bahagian.GE_KETERANGAN;
                    }
                    //

                    //GAJI
                    ViewBag.Gaji = db.HR_GAJI_UPAHAN.Where(s => s.HR_KETERANGAN_SLIP == "GAJI").OrderBy(s => s.HR_PENERANGAN_UPAH).ToList<HR_GAJI_UPAHAN>();
                    
                    //
                    //ELAUN
                    ViewBag.Elaun = db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList<HR_ELAUN>();
                    //

                    //CARUMAN
                    ViewBag.Potongan = db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList<HR_POTONGAN>();
                    ViewBag.Caruman = db.HR_CARUMAN.OrderBy(s => s.HR_PENERANGAN_CARUMAN).ToList<HR_CARUMAN>();
                    //

                    //ANUGERAH
                    ViewBag.Peringkat = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).GroupBy(s => s.SHORT_DESCRIPTION).Select(s => s.FirstOrDefault()), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
                    ViewBag.HR_KURNIAAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130), "STRING_PARAM", "LONG_DESCRIPTION");
                    ViewBag.Kurniaan = db.HR_KURNIAAN.ToList();
                    ViewBag.HR_NP_PENCALON = db.HR_MAKLUMAT_PERIBADI.ToList<HR_MAKLUMAT_PERIBADI>();
                    //

                    //GAJI, ELAUN, CARUMAN, ANUGERAH
                    ViewBag.HR_MOD_BAYARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 117), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_AKTIF_IND_PEKERJAAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")), "STRING_PARAM", "SHORT_DESCRIPTION");
                    List<SelectListItem> HR_AUTO_IND = new List<SelectListItem>();
                    HR_AUTO_IND.Add(new SelectListItem { Text = "YA", Value = "Y" });
                    HR_AUTO_IND.Add(new SelectListItem { Text = "TIDAK", Value = "T" });
                    ViewBag.HR_AUTO_IND = HR_AUTO_IND;
                    //


                    List<SelectListItem> HR_STATUS = new List<SelectListItem>();
                    HR_STATUS.Add(new SelectListItem { Text = "BERJAYA", Value = "Y" });
                    HR_STATUS.Add(new SelectListItem { Text = "TIDAK BERJAYA", Value = "T" });
                    HR_STATUS.Add(new SelectListItem { Text = "DICALONKAN", Value = "P" });
                    ViewBag.HR_STATUS = HR_STATUS;

                    List<SelectListItem> HR_STATUS_HAJI = new List<SelectListItem>();
                    HR_STATUS_HAJI.Add(new SelectListItem { Text = "TERIMA", Value = "T" });
                    HR_STATUS_HAJI.Add(new SelectListItem { Text = "SEDANG DIPROSES", Value = "S" });
                    HR_STATUS_HAJI.Add(new SelectListItem { Text = "DICALONKAN", Value = "P" });
                    HR_STATUS_HAJI.Add(new SelectListItem { Text = "TOLAK", Value = "K" });
                    ViewBag.HR_STATUS_HAJI = HR_STATUS_HAJI;

                    ViewBag.HR_TINDAKAN = new SelectList(db.HR_TINDAKAN, "HR_KOD_TINDAKAN", "HR_PENERANGAN");
                    ViewBag.HR_PEWARIS = db.HR_MAKLUMAT_PEWARIS.ToList<HR_MAKLUMAT_PEWARIS>();

                    //PRESTASI
                    var kump = Convert.ToInt32(mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN);
                    var kumpulan = db2.GE_PARAMTABLE.SingleOrDefault(s => s.GROUPID == 103 && s.ORDINAL == kump);
                    ViewBag.kumpulan = null;
                    if (kumpulan != null)
                    {
                        ViewBag.kumpulan = kumpulan.LONG_DESCRIPTION;
                    }
                    //
                    //CUTI

                    ViewBag.HR_KATEGORI_CUTI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142), "ORDINAL", "SHORT_DESCRIPTION");
                    ViewBag.HR_KOD_CUTI = new SelectList(db.HR_CUTI.Select(s => new { HR_KOD_CUTI = s.HR_KOD_CUTI, HR_KATEGORI = s.HR_KATEGORI.Trim(), HR_KETERANGAN = s.HR_KETERANGAN }).Where(s => s.HR_KATEGORI == "1"), "HR_KOD_CUTI", "HR_KETERANGAN");
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
                }
            }
            ViewBag.HR_AKTIF_IND3 = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144), "STRING_PARAM", "SHORT_DESCRIPTION");
            return View(mKakitangan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MaklumatKakitanganModels mKakitangan, List<string> kemaskini, HttpPostedFileBase file, IEnumerable<HR_MAKLUMAT_PEKERJAAN_HISTORY> HR_MAKLUMAT_HISTORY)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_GAMBAR_PENGGUNA gambar = db.HR_GAMBAR_PENGGUNA.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            List<HR_MAKLUMAT_PENGALAMAN_KERJA> pKerja = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
            List<HR_MAKLUMAT_PEWARIS> Pewaris = db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_PEWARIS>();
            HR_MAKLUMAT_KUARTERS mKuarters = db.HR_MAKLUMAT_KUARTERS.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_ANUGERAH_HAJI mAnugerahHaji = db.HR_ANUGERAH_HAJI.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            //HR_KUARTERS mKuarters2 = new HR_KUARTERS();
            HR_KUARTERS mKuarters2 = db.HR_KUARTERS.SingleOrDefault(s => s.HR_KOD_KUARTERS == mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS);
            //List<HR_MAKLUMAT_TANGGUNGAN> mTanggungan = db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_TANGGUNGAN>();

            HR_PERSARAAN mPersaraan = db.HR_PERSARAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);

            HR_MAKLUMAT_KEMATIAN mKematian = db.HR_MAKLUMAT_KEMATIAN.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_PENILAIAN_PRESTASI mPrestasi = db.HR_PENILAIAN_PRESTASI.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_TAHUN_PRESTASI == mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI);

            if (mKakitangan.HR_SENARAI_PERIBADI == null)
            {
                mKakitangan.HR_SENARAI_PERIBADI = new List<MaklumatPeribadi>();
            }

            if (mKakitangan.HR_MAKLUMAT_PERIBADI == null)
            {
                mKakitangan.HR_MAKLUMAT_PERIBADI = new MaklumatPeribadi();
            }

            if (mKakitangan.HR_GAMBAR_PENGGUNA == null)
            {
                mKakitangan.HR_GAMBAR_PENGGUNA = new HR_GAMBAR_PENGGUNA();
            }

            if (mKakitangan.HR_MAKLUMAT_PEKERJAAN == null)
            {
                mKakitangan.HR_MAKLUMAT_PEKERJAAN = new MaklumatPekerjaan();
            }

            if (mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY == null)
            {
                mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY = new List<HR_MAKLUMAT_PEKERJAAN_HISTORY>();
            }

            if (mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA == null)
            {
                mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA = new List<MaklumatPengalamanKerja>();
            }

            if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA == null)
            {
                mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA = new List<MaklumatKemahiranBahasa>();
            }

            if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL == null)
            {
                mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL = new List<MaklumatKemahiranTeknikal>();
            }

            if (mKakitangan.HR_MAKLUMAT_KELAYAKAN == null)
            {
                mKakitangan.HR_MAKLUMAT_KELAYAKAN = new List<MaklumatKelayakan>();
            }

            if (mKakitangan.HR_MAKLUMAT_SIJIL == null)
            {
                mKakitangan.HR_MAKLUMAT_SIJIL = new List<MaklumatSijil>();
            }

            if (mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN == null)
            {
                mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN = new List<MaklumatKursusLatihan>();
            }

            if (mKakitangan.HR_MAKLUMAT_AKTIVITI == null)
            {
                mKakitangan.HR_MAKLUMAT_AKTIVITI = new List<MaklumatAktiviti>();
            }

            if (mKakitangan.HR_MAKLUMAT_PEWARIS == null)
            {
                mKakitangan.HR_MAKLUMAT_PEWARIS = new List<MaklumatPewaris> { new MaklumatPewaris() };
            }

            if (mKakitangan.HR_MAKLUMAT_TANGGUNGAN == null)
            {
                mKakitangan.HR_MAKLUMAT_TANGGUNGAN = new List<MaklumatTanggungan>();
            }

            if (mKakitangan.HR_MAKLUMAT_KUARTERS == null)
            {
                mKakitangan.HR_MAKLUMAT_KUARTERS = new MaklumatKuarters();
            }

            if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G == null)
            {
                mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G = new List<MaklumatElaunPotongan>();
            }

            if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E == null)
            {
                mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E = new List<MaklumatElaunPotongan>();
            }

            if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P == null)
            {
                mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P = new List<MaklumatElaunPotongan>();
            }

            if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C == null)
            {
                mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C = new List<MaklumatElaunPotongan>();
            }

            if (mKakitangan.HR_MAKLUMAT_KURNIAAN == null)
            {
                mKakitangan.HR_MAKLUMAT_KURNIAAN = new List<MaklumatKurniaan>();
            }

            if (mKakitangan.HR_ANUGERAH_CEMERLANG == null)
            {
                mKakitangan.HR_ANUGERAH_CEMERLANG = new List<MaklumatAnugerahCemerlang>();
            }

            if (mKakitangan.HR_ANUGERAH_HAJI == null)
            {
                mKakitangan.HR_ANUGERAH_HAJI = new MaklumatAnugerahHaji();
            }

            if (mKakitangan.HR_PERSARAAN == null)
            {
                mKakitangan.HR_PERSARAAN = new MaklumatPersaraan();
            }

            if (mKakitangan.HR_TINDAKAN_DISIPLIN == null)
            {
                mKakitangan.HR_TINDAKAN_DISIPLIN = new List<MaklumatTindakanDisiplin>();
            }

            if (mKakitangan.HR_MAKLUMAT_KEMATIAN == null)
            {
                mKakitangan.HR_MAKLUMAT_KEMATIAN = new MaklumatKematian();
            }

            if (mKakitangan.HR_PENILAIAN_PRESTASI == null)
            {
                mKakitangan.HR_PENILAIAN_PRESTASI = new MaklumatPenilaianPrestasi();
            }

            if (mKakitangan.HR_MAKLUMAT_CUTI == null)
            {
                mKakitangan.HR_MAKLUMAT_CUTI = new MaklumatCuti();
            }

            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                HR_MAKLUMAT_PERIBADI pekerja = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == username);
                foreach (var Kemaskini in kemaskini)
                {
                    if (Kemaskini == "Info")
                    {
                        mPeribadi.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                        mPeribadi.HR_NO_KPBARU = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU;
                        mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA;
                        db.Entry(mPeribadi).State = EntityState.Modified;

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

                            gambar.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
                            var guid = Guid.NewGuid();
                            var format = System.IO.Path.GetExtension(file.FileName);
                            var fileName = guid + System.IO.Path.GetExtension(file.FileName);
                            string physicalPath = Server.MapPath("~/Content/uploads/");
                            var checkImage = Array.FindAll(allowedExtensions, s => s.Equals(format));
                            if(allowedExtensions.Contains(format))
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

                                    var cariGambar = db.HR_GAMBAR_PENGGUNA.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);

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
                        ViewBag.msg = "Data berjaya dikemaskini";
                    }

                    if (Kemaskini == "Peribadi")
                    {
                        mPeribadi.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                        mPeribadi.HR_NO_KPBARU = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU;
                        mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA;
                        mPeribadi.HR_NO_KPLAMA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPLAMA;
                        mPeribadi.HR_TARIKH_LAHIR = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_LAHIR;
                        mPeribadi.HR_TEMPAT_LAHIR = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TEMPAT_LAHIR;
                        mPeribadi.HR_WARGANEGARA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_WARGANEGARA;
                        mPeribadi.HR_KETURUNAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_KETURUNAN;
                        mPeribadi.HR_AGAMA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_AGAMA;
                        mPeribadi.HR_JANTINA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_JANTINA;
                        mPeribadi.HR_TARAF_KAHWIN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARAF_KAHWIN;
                        mPeribadi.HR_LESEN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_LESEN;
                        mPeribadi.HR_KELAS_LESEN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_KELAS_LESEN;
                        mPeribadi.HR_TALAMAT1 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT1;
                        mPeribadi.HR_TALAMAT2 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT2;
                        mPeribadi.HR_TALAMAT3 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TALAMAT3;
                        mPeribadi.HR_TBANDAR = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TBANDAR;
                        mPeribadi.HR_TPOSKOD = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TPOSKOD;
                        mPeribadi.HR_TNEGERI = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TNEGERI;
                        mPeribadi.HR_SALAMAT1 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT1;
                        mPeribadi.HR_SALAMAT2 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT2;
                        mPeribadi.HR_SALAMAT3 = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SALAMAT3;
                        mPeribadi.HR_SBANDAR = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SBANDAR;
                        mPeribadi.HR_SPOSKOD = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SPOSKOD;
                        mPeribadi.HR_SNEGERI = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_SNEGERI;
                        //mPeribadi.HR_TAHUN_SPM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TAHUN_SPM;
                        //mPeribadi.HR_GRED_BM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_GRED_BM;
                        mPeribadi.HR_TELRUMAH = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELRUMAH;
                        mPeribadi.HR_TELPEJABAT = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELPEJABAT;
                        mPeribadi.HR_TELBIMBIT = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TELBIMBIT;
                        mPeribadi.HR_EMAIL = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_EMAIL;
                        mPeribadi.HR_AKTIF_IND = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_AKTIF_IND;
                        mPeribadi.HR_CC_KENDERAAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_CC_KENDERAAN;
                        mPeribadi.HR_NO_KENDERAAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KENDERAAN;
                        mPeribadi.HR_JENIS_KENDERAAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_JENIS_KENDERAAN;
                        mPeribadi.HR_ALASAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_ALASAN;
                        mPeribadi.HR_IDPEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_IDPEKERJA;
                        //mPeribadi.HR_TARIKH_KEYIN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_KEYIN;
                        //mPeribadi.HR_NP_KEYIN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_KEYIN;
                        mPeribadi.HR_TARIKH_UBAH = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_UBAH;
                        mPeribadi.HR_NP_UBAH = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_UBAH;

                        db.Entry(mPeribadi).State = EntityState.Modified;
                        db.SaveChanges();

                        if (mPekerjaan == null)
                        {
                            mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                            mPekerjaan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                            mPekerjaan.HR_NP_KEYIN = pekerja.HR_NO_PEKERJA;
                            mPekerjaan.HR_TARIKH_KEYIN = DateTime.Now;
                            db.HR_MAKLUMAT_PEKERJAAN.Add(mPekerjaan);
                            db.SaveChanges();
                        }

                        mPekerjaan.HR_GELARAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN;

                        db.Entry(mPekerjaan).State = EntityState.Modified;
                        db.SaveChanges();

                        ViewBag.msg = "Data berjaya dikemaskini";
                    }
                    if (Kemaskini == "Pekerjaan")
                    {
                        if (mPekerjaan == null)
                        {
                            mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                            mPekerjaan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                            mPekerjaan.HR_NP_KEYIN = pekerja.HR_NO_PEKERJA;
                            mPekerjaan.HR_TARIKH_KEYIN = DateTime.Now;
                            db.HR_MAKLUMAT_PEKERJAAN.Add(mPekerjaan);
                            db.SaveChanges();
                        }

                        mPekerjaan.HR_JABATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN;
                        mPekerjaan.HR_BAHAGIAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN;
                        mPekerjaan.HR_JAWATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;
                        mPekerjaan.HR_GRED = null;

                        GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                        if(sGred != null)
                        {
                            mPekerjaan.HR_GRED = sGred.ORDINAL.ToString();
                        }

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI != null)
                        {
                            mPekerjaan.HR_KATEGORI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI;
                        }
                       
                        mPekerjaan.HR_KUMP_PERKHIDMATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN;
                        mPekerjaan.HR_TARAF_JAWATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN;
                        mPekerjaan.HR_GAJI_POKOK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_POKOK;
                        mPekerjaan.HR_NO_AKAUN_BANK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_AKAUN_BANK;

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI != null)
                        {
                            mPekerjaan.HR_BULAN_KENAIKAN_GAJI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BULAN_KENAIKAN_GAJI;
                        }
                       
                        mPekerjaan.HR_TARIKH_MASUK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK;
                        mPekerjaan.HR_TARIKH_SAH_JAWATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_SAH_JAWATAN;
                        mPekerjaan.HR_TARIKH_TAMAT_KONTRAK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT_KONTRAK;
                        mPekerjaan.HR_TARIKH_TAMAT = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT;
                        mPekerjaan.HR_SISTEM = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM;
                        mPekerjaan.HR_NO_PENYELIA = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENYELIA;

                        mPekerjaan.HR_STATUS_PENCEN = "T";
                        mPekerjaan.HR_STATUS_KWSP = "T";

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP == "Y")
                        {
                            mPekerjaan.HR_STATUS_KWSP = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP;
                        }
                        else if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN == "Y")
                        {
                            mPekerjaan.HR_STATUS_PENCEN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN;
                        }
                       
                        mPekerjaan.HR_STATUS_SOCSO = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_SOCSO;
                        mPekerjaan.HR_STATUS_PCB = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PCB;

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_KWSP != null)
                        {
                            mPekerjaan.HR_NILAI_KWSP = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_KWSP;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_SOCSO != null)
                        {
                            mPekerjaan.HR_NILAI_SOCSO = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_SOCSO;
                        }
                      
                        mPekerjaan.HR_KOD_PCB = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI_PCB + mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_PCB;
                        mPekerjaan.HR_GAJI_PRORATA = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_PRORATA;
                        //mPekerjaan.HR_MATRIKS_GAJI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MIN + mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_MAX;
                        mPekerjaan.HR_UNIT = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_UNIT;
                        mPekerjaan.HR_KUMPULAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN;
                        mPekerjaan.HR_KOD_BANK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN != null)
                        {
                            mPekerjaan.HR_TINGKATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN;
                        }
                        mPekerjaan.HR_KAKITANGAN_IND = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND;
                        mPekerjaan.HR_FAIL_PERKHIDMATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_FAIL_PERKHIDMATAN;
                        mPekerjaan.HR_NO_SIRI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_SIRI;
                        mPekerjaan.HR_BAYARAN_CEK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAYARAN_CEK;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KE_JABATAN != null)
                        {
                            mPekerjaan.HR_TARIKH_KE_JABATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KE_JABATAN;
                        }
                     
                        mPekerjaan.HR_KOD_GAJI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GAJI;
                        mPekerjaan.HR_KELAS_PERJALANAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KELAS_PERJALANAN;
                        mPekerjaan.HR_TARIKH_LANTIKAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_LANTIKAN;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TIDAK_AKTIF != null)
                        {
                            mPekerjaan.HR_TARIKH_TIDAK_AKTIF = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TIDAK_AKTIF;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND != null)
                        {
                            mPekerjaan.HR_GAJI_IND = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GAJI_IND;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_GAJI != null)
                        {
                            mPekerjaan.HR_TARIKH_GAJI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_GAJI;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_MULA != null)
                        {
                            mPekerjaan.HR_PCB_TARIKH_MULA = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_MULA;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_AKHIR != null)
                        {
                            mPekerjaan.HR_PCB_TARIKH_AKHIR = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PCB_TARIKH_AKHIR;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_PCB != null)
                        {
                            mPekerjaan.HR_NILAI_PCB = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NILAI_PCB;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J != null)
                        {
                            mPekerjaan.HR_KOD_GELARAN_J = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TANGGUH_GERAKGAJI_IND != null)
                        {
                            mPekerjaan.HR_TANGGUH_GERAKGAJI_IND = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TANGGUH_GERAKGAJI_IND;
                        }

                        mPekerjaan.HR_TARIKH_UBAH = DateTime.Now;
                        mPekerjaan.HR_NP_UBAH = pekerja.HR_NO_PEKERJA;

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SKIM != null)
                        {
                            mPekerjaan.HR_SKIM = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SKIM;
                        }

                        mPekerjaan.HR_PERGERAKAN_GAJI = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_PERGERAKAN_GAJI;
                        mPekerjaan.HR_NO_KWSP = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_KWSP;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENCEN != null)
                        {
                            mPekerjaan.HR_NO_PENCEN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PENCEN;
                        }
                        
                        mPekerjaan.HR_NO_SOCSO = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_SOCSO;
                        mPekerjaan.HR_NO_PCB = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NO_PCB;
                        mPekerjaan.HR_INITIAL = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_INITIAL;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_AM_YDP != null)
                        {
                            mPekerjaan.HR_AM_YDP = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_AM_YDP;
                        }
                       
                        mPekerjaan.HR_TARIKH_MASUK_KERAJAAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK_KERAJAAN;
                        mPekerjaan.HR_TARIKH_SAH_J_TUKAR_LANTIK = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_SAH_J_TUKAR_LANTIK;
                        mPekerjaan.HR_UNIFORM = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_UNIFORM;
                        mPekerjaan.HR_TEKNIKAL = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TEKNIKAL;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KELUAR_MBPJ != null)
                        {
                            mPekerjaan.HR_TARIKH_KELUAR_MBPJ = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KELUAR_MBPJ;
                        }
                        
                        mPeribadi.HR_CC_KENDERAAN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_CC_KENDERAAN;
                        db.Entry(mPeribadi).State = EntityState.Modified;
                        db.Entry(mPekerjaan).State = EntityState.Modified;
                        db.SaveChanges();
                        ViewBag.msg = "Data berjaya dikemaskini";

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY.Count() > 0)
                        {
                            db.HR_MAKLUMAT_PEKERJAAN_HISTORY.RemoveRange(db.HR_MAKLUMAT_PEKERJAAN_HISTORY.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            db.SaveChanges();
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PEKERJAAN_HISTORY)
                            {
                                HR_MAKLUMAT_PEKERJAAN_HISTORY mKerja = new HR_MAKLUMAT_PEKERJAAN_HISTORY();
                                mKerja.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                mKerja.HR_TARIKH_PERUBAHAN = item.HR_TARIKH_PERUBAHAN;
                                mKerja.HR_JABATAN = item.HR_JABATAN;
                                mKerja.HR_BAHAGIAN = item.HR_BAHAGIAN;
                                mKerja.HR_UNIT = item.HR_UNIT;
                                mKerja.HR_GRED = null;
                                GE_PARAMTABLE sGred2 = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == item.HR_GRED);
                                if (sGred != null)
                                {
                                    mKerja.HR_GRED = sGred2.ORDINAL.ToString();
                                }
                                mKerja.HR_KATEGORI = item.HR_KATEGORI;
                                mKerja.HR_KUMP_PERKHIDMATAN = item.HR_KUMP_PERKHIDMATAN;
                                mKerja.HR_JAWATAN = item.HR_JAWATAN;
                                mKerja.HR_TARAF_JAWATAN = item.HR_TARAF_JAWATAN;
                                mKerja.HR_TARIKH_SAH_JAWATAN = item.HR_TARIKH_SAH_JAWATAN;
                                mKerja.HR_TARIKH_TAMAT_KONTRAK = item.HR_TARIKH_TAMAT_KONTRAK;
                                mKerja.HR_NO_PENYELIA = item.HR_NO_PENYELIA;
                                mKerja.HR_MATRIKS_GAJI = item.HR_MATRIKS_GAJI;
                                mKerja.HR_KUMPULAN = item.HR_KUMPULAN;
                                mKerja.HR_TINGKATAN = item.HR_TINGKATAN;
                                mKerja.HR_KOD_GAJI = item.HR_KOD_GAJI;
                                mKerja.HR_NP_UBAH = item.HR_NP_UBAH;
                                mKerja.HR_TARIKH_MASUK = item.HR_TARIKH_MASUK;
                                mKerja.HR_GAJI = item.HR_GAJI;
                                db.HR_MAKLUMAT_PEKERJAAN_HISTORY.Add(mKerja);
                                db.SaveChanges();
                            }
                        }
                    }
                    /*if (Kemaskini == "Pekerjaan" || Kemaskini == "Kemahiran")
                    {
                        if (mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA_MPPJ != null)
                        {
                            db.HR_MAKLUMAT_PENGALAMAN_KERJA.RemoveRange(db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_NAMA_SYARIKAT == "MAJLIS PERBANDARAN PETALING JAYA"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA_MPPJ)
                            {
                                if (item.HR_TARIKH_MULA != null)
                                {

                                    HR_MAKLUMAT_PENGALAMAN_KERJA mKerja = new HR_MAKLUMAT_PENGALAMAN_KERJA();
                                    mKerja.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mKerja.HR_NAMA_SYARIKAT = "MAJLIS PERBANDARAN PETALING JAYA";
                                    mKerja.HR_JAWATAN = item.HR_JAWATAN;
                                    mKerja.HR_TARIKH_MULA = Convert.ToDateTime(item.HR_TARIKH_MULA);
                                    mKerja.HR_TARIKH_TAMAT = Convert.ToDateTime(item.HR_TARIKH_TAMAT);
                                    mKerja.HR_ALASAN_BERHENTI = item.HR_ALASAN_BERHENTI;

                                    db.HR_MAKLUMAT_PENGALAMAN_KERJA.Add(mKerja);
                                    db.SaveChanges();
                                }
                            }
                        }

                        
                        if (mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA != null)
                        {
                            db.HR_MAKLUMAT_PENGALAMAN_KERJA.RemoveRange(db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_NAMA_SYARIKAT != "MAJLIS PERBANDARAN PETALING JAYA"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA)
                            {
                                if (item.HR_TARIKH_MULA != null)
                                {
                                    HR_MAKLUMAT_PENGALAMAN_KERJA mKerja = new HR_MAKLUMAT_PENGALAMAN_KERJA();
                                    mKerja.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mKerja.HR_NAMA_SYARIKAT = item.HR_NAMA_SYARIKAT;
                                    mKerja.HR_JAWATAN = item.HR_JAWATAN;
                                    mKerja.HR_TARIKH_MULA = Convert.ToDateTime(item.HR_TARIKH_MULA);
                                    mKerja.HR_TARIKH_TAMAT = Convert.ToDateTime(item.HR_TARIKH_TAMAT);
                                    mKerja.HR_ALASAN_BERHENTI = item.HR_ALASAN_BERHENTI;

                                    db.HR_MAKLUMAT_PENGALAMAN_KERJA.Add(mKerja);
                                    db.SaveChanges();
                                }
                            }
                        }
                    }*/

                    if (Kemaskini == "Kemahiran")
                    {
                        db.HR_MAKLUMAT_PENGALAMAN_KERJA.RemoveRange(db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_NAMA_SYARIKAT != "MAJLIS PERBANDARAN PETALING JAYA"));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA)
                            {
                                if(item.HR_TARIKH_MULA.HasValue)
                                {
                                    HR_MAKLUMAT_PENGALAMAN_KERJA mKerja = new HR_MAKLUMAT_PENGALAMAN_KERJA();
                                    mKerja.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mKerja.HR_NAMA_SYARIKAT = item.HR_NAMA_SYARIKAT;
                                    mKerja.HR_JAWATAN = item.HR_JAWATAN;
                                    mKerja.HR_TARIKH_MULA = Convert.ToDateTime(item.HR_TARIKH_MULA);
                                    mKerja.HR_TARIKH_TAMAT = Convert.ToDateTime(item.HR_TARIKH_TAMAT);
                                    mKerja.HR_ALASAN_BERHENTI = item.HR_ALASAN_BERHENTI;

                                    db.HR_MAKLUMAT_PENGALAMAN_KERJA.Add(mKerja);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Kemahiran2")
                    {
                        db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.RemoveRange(db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA)
                            {
                                if(item.HR_BAHASA != null)
                                {
                                    HR_MAKLUMAT_KEMAHIRAN_BAHASA mKemahiranBahasa = new HR_MAKLUMAT_KEMAHIRAN_BAHASA();
                                    mKemahiranBahasa.HR_BAHASA = item.HR_BAHASA;
                                    mKemahiranBahasa.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mKemahiranBahasa.HR_PEMBACAAN = item.HR_PEMBACAAN;
                                    mKemahiranBahasa.HR_PENULISAN = item.HR_PENULISAN;
                                    mKemahiranBahasa.HR_PERTUTURAN = item.HR_PERTUTURAN;

                                    db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.Add(mKemahiranBahasa);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Kemahiran3")
                    {
                        db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.RemoveRange(db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL != null)
                        {
                            short value = 0;
                            var last_HR_SEQ_NO = db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.OrderByDescending(s => s.HR_SEQ_NO).FirstOrDefault();
                            if (last_HR_SEQ_NO != null)
                            {
                                value = last_HR_SEQ_NO.HR_SEQ_NO;
                            }
                            var no_inc = 0;
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL)
                            {
                                no_inc++;
                                var digit = (value + no_inc);

                                HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL mKemahiranTeknikal = new HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL();

                                mKemahiranTeknikal.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;

                                mKemahiranTeknikal.HR_SEQ_NO = Convert.ToInt16(digit);
                                mKemahiranTeknikal.HR_KEMAHIRAN = item.HR_KEMAHIRAN;
                                mKemahiranTeknikal.HR_TAHAP = item.HR_TAHAP;

                                db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.Add(mKemahiranTeknikal);
                                db.SaveChanges();
                            }
                        }

                    }

                    if (Kemaskini == "Akademik1")
                    {
                        mPeribadi.HR_TAHUN_SPM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TAHUN_SPM;
                        mPeribadi.HR_GRED_BM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_GRED_BM;
                        db.Entry(mPekerjaan).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (Kemaskini == "Akademik2")
                    {
                        db.HR_MAKLUMAT_KELAYAKAN.RemoveRange(db.HR_MAKLUMAT_KELAYAKAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_KELAYAKAN != null)
                        {
                            short value = 0;
                            var last_HR_SEQ_NO = db.HR_MAKLUMAT_KELAYAKAN.OrderByDescending(s => s.HR_SEQ_NO).FirstOrDefault();
                            if (last_HR_SEQ_NO != null)
                            {
                                value = last_HR_SEQ_NO.HR_SEQ_NO;
                            }
                            var no_inc = 0;
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KELAYAKAN)
                            {
                                no_inc++;
                                var digit = (value + no_inc);

                                HR_MAKLUMAT_KELAYAKAN mKelayakan = new HR_MAKLUMAT_KELAYAKAN();
                                mKelayakan.HR_PANGKAT = item.HR_PANGKAT;
                                mKelayakan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mKelayakan.HR_SEQ_NO = Convert.ToInt16(digit);
                                mKelayakan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                mKelayakan.HR_TAHUN_MULA = item.HR_TAHUN_MULA;
                                mKelayakan.HR_TAHUN_TAMAT = item.HR_TAHUN_TAMAT;
                                mKelayakan.HR_SEKOLAH_INSTITUSI = item.HR_SEKOLAH_INSTITUSI;
                                db.HR_MAKLUMAT_KELAYAKAN.Add(mKelayakan);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (Kemaskini == "Akademik3")
                    {
                        db.HR_MAKLUMAT_SIJIL.RemoveRange(db.HR_MAKLUMAT_SIJIL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_SIJIL != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_SIJIL)
                            {
                                if(item.HR_TARIKH_DIPEROLEHI.HasValue)
                                {
                                    HR_MAKLUMAT_SIJIL mSijil = new HR_MAKLUMAT_SIJIL();
                                    mSijil.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mSijil.HR_TARIKH_DIPEROLEHI = Convert.ToDateTime(item.HR_TARIKH_DIPEROLEHI);
                                    mSijil.HR_NAMA_SIJIL_PEPERIKSAAN = item.HR_NAMA_SIJIL_PEPERIKSAAN;
                                    mSijil.HR_ANJURAN = item.HR_ANJURAN;
                                    mSijil.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                    db.HR_MAKLUMAT_SIJIL.Add(mSijil);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Akademik4")
                    {
                        db.HR_MAKLUMAT_KURSUS_LATIHAN.RemoveRange(db.HR_MAKLUMAT_KURSUS_LATIHAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN)
                            {
                                if(item.HR_KOD_KURSUS != null)
                                {
                                    HR_MAKLUMAT_KURSUS_LATIHAN mKursusLatihan = new HR_MAKLUMAT_KURSUS_LATIHAN();
                                    mKursusLatihan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mKursusLatihan.HR_KOD_KURSUS = item.HR_KOD_KURSUS;
                                    mKursusLatihan.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                    mKursusLatihan.HR_TARIKH_TAMAT = item.HR_TARIKH_TAMAT;
                                    mKursusLatihan.HR_ANJURAN = item.HR_ANJURAN;
                                    mKursusLatihan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                    db.HR_MAKLUMAT_KURSUS_LATIHAN.Add(mKursusLatihan);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Akademik5")
                    {
                        db.HR_MAKLUMAT_AKTIVITI.RemoveRange(db.HR_MAKLUMAT_AKTIVITI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_AKTIVITI != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_AKTIVITI)
                            {
                                if(item.HR_TARIKH_AKTIVITI.HasValue)
                                {
                                    HR_MAKLUMAT_AKTIVITI mAktiviti = new HR_MAKLUMAT_AKTIVITI();
                                    mAktiviti.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mAktiviti.HR_TARIKH_AKTIVITI = Convert.ToDateTime(item.HR_TARIKH_AKTIVITI);
                                    mAktiviti.HR_PERINGKAT = item.HR_PERINGKAT;
                                    mAktiviti.HR_NAMA_AKTIVITI = item.HR_NAMA_AKTIVITI;
                                    mAktiviti.HR_ANJURAN = item.HR_ANJURAN;
                                    db.HR_MAKLUMAT_AKTIVITI.Add(mAktiviti);
                                    db.SaveChanges();
                                }
                                
                            }
                        }

                    }

                    if (Kemaskini == "Pewaris")
                    {
                        db.HR_MAKLUMAT_PEWARIS.RemoveRange(db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_PEWARIS != null)
                        {
                            var no = 1;
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PEWARIS)
                            {
                                if(item.HR_NO_KP != null && item.HR_NAMA_PEWARIS != null && item.HR_TARIKH_LAHIR != null)
                                {
                                    HR_MAKLUMAT_PEWARIS mPewaris = new HR_MAKLUMAT_PEWARIS();
                                    mPewaris.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mPewaris.HR_NAMA_PEWARIS = item.HR_NAMA_PEWARIS;
                                    mPewaris.HR_TARIKH_LAHIR = item.HR_TARIKH_LAHIR;
                                    mPewaris.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR;
                                    mPewaris.HR_JANTINA = item.HR_JANTINA;
                                    mPewaris.HR_PALAMAT1 = item.HR_PALAMAT1;
                                    mPewaris.HR_PALAMAT2 = item.HR_PALAMAT2;
                                    mPewaris.HR_PALAMAT3 = item.HR_PALAMAT3;
                                    mPewaris.HR_PBANDAR = item.HR_PBANDAR;
                                    mPewaris.HR_PPOSKOD = item.HR_PPOSKOD;
                                    mPewaris.HR_PNEGERI = item.HR_PNEGERI;
                                    mPewaris.HR_HUBUNGAN = null;
                                    if (item.HR_HUBUNGAN != null)
                                    {
                                        mPewaris.HR_HUBUNGAN = new string(item.HR_HUBUNGAN.TakeWhile(x => char.IsDigit(x)).ToArray());
                                    }
                                    mPewaris.HR_TELRUMAH = item.HR_TELRUMAH;
                                    mPewaris.HR_TELPEJABAT = item.HR_TELPEJABAT;
                                    mPewaris.HR_TELBIMBIT = item.HR_TELBIMBIT;
                                    mPewaris.HR_NO_KP = item.HR_NO_KP;
                                    mPewaris.HR_NO_KP_LAMA = item.HR_NO_KP_LAMA;
                                    mPewaris.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mPewaris.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mPewaris.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mPewaris.HR_NP_UBAH = item.HR_NP_UBAH;
                                    mPewaris.HR_PEWARIS_IND = Convert.ToString(no);
                                    db.HR_MAKLUMAT_PEWARIS.Add(mPewaris);
                                    db.SaveChanges();
                                    no++;
                                }
                                
                            }
                        }

                    }

                    if (Kemaskini == "Tanggungan")
                    {
                        db.HR_MAKLUMAT_TANGGUNGAN.RemoveRange(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_TANGGUNGAN != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_TANGGUNGAN)
                            {
                                if(item.HR_NO_KP != null)
                                {
                                    HR_MAKLUMAT_TANGGUNGAN mTanggungan = new HR_MAKLUMAT_TANGGUNGAN();
                                    mTanggungan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mTanggungan.HR_NAMA_TANGGUNGAN = item.HR_NAMA_TANGGUNGAN;
                                    mTanggungan.HR_TARIKH_LAHIR = item.HR_TARIKH_LAHIR;
                                    mTanggungan.HR_NO_KP = item.HR_NO_KP;
                                    mTanggungan.HR_TEMPAT_LAHIR = item.HR_TEMPAT_LAHIR;
                                    mTanggungan.HR_SEK_IPT = item.HR_SEK_IPT;
                                    mTanggungan.HR_HUBUNGAN = item.HR_HUBUNGAN;
                                    mTanggungan.HR_JANTINA = item.HR_JANTINA;
                                    mTanggungan.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mTanggungan.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mTanggungan.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mTanggungan.HR_NP_UBAH = item.HR_NP_UBAH;
                                    db.HR_MAKLUMAT_TANGGUNGAN.Add(mTanggungan);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Kuarters")
                    {
                        if(mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_AKTIF_IND != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_AKTIF_IND != null)
                        {
                            if(mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT1 != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BLOK_KUARTERS != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_NEGERI != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_POSKOD != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BANDAR != null)
                            {
                                if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN2X == "false")
                                {
                                    mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN2X = null;
                                }
                                if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN5X == "false")
                                {
                                    mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN5X = null;
                                }
                                if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GERAI == "false")
                                {
                                    mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GERAI = null;
                                }
                                if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_IDP == "false")
                                {
                                    mKakitangan.HR_MAKLUMAT_KUARTERS.HR_IDP = null;
                                }

                                if (mKuarters2 == null)
                                {
                                    mKuarters2 = new HR_KUARTERS();
                                    mKuarters2.HR_KOD_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS;
                                    mKuarters2.HR_AKTIF_IND = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_AKTIF_IND;
                                    mKuarters2.HR_ALAMAT1 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT1;
                                    mKuarters2.HR_ALAMAT2 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT2;
                                    mKuarters2.HR_ALAMAT3 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT3;
                                    mKuarters2.HR_BANDAR = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BANDAR;
                                    mKuarters2.HR_BLOK_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BLOK_KUARTERS;
                                    mKuarters2.HR_NEGERI = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_NEGERI;
                                    mKuarters2.HR_POSKOD = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_POSKOD;
                                    db.HR_KUARTERS.Add(mKuarters2);
                                    db.SaveChanges();
                                }

                                mKuarters2.HR_KOD_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS;
                                mKuarters2.HR_AKTIF_IND = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_AKTIF_IND;
                                mKuarters2.HR_ALAMAT1 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT1;
                                mKuarters2.HR_ALAMAT2 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT2;
                                mKuarters2.HR_ALAMAT3 = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT3;
                                mKuarters2.HR_BANDAR = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BANDAR;
                                mKuarters2.HR_BLOK_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BLOK_KUARTERS;
                                mKuarters2.HR_NEGERI = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_NEGERI;
                                mKuarters2.HR_POSKOD = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_POSKOD;
                                db.Entry(mKuarters2).State = EntityState.Modified;
                                db.SaveChanges();

                                if (mKuarters == null)
                                {
                                    mKuarters = new HR_MAKLUMAT_KUARTERS();
                                    mKuarters.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mKuarters.HR_KOD_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS;
                                    mKuarters.HR_TARIKH_MASUK = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_MASUK);
                                    mKuarters.HR_TARIKH_KELUAR = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_KELUAR;
                                    mKuarters.HR_NO_UNIT = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_NO_UNIT;
                                    mKuarters.HR_GANDAAN2X = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN2X;
                                    mKuarters.HR_GERAI = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GERAI;
                                    mKuarters.HR_CATATAN = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_CATATAN;
                                    mKuarters.HR_IDP = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_IDP;
                                    mKuarters.HR_AKTIF_IND = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_AKTIF_IND;
                                    mKuarters.HR_GANDAAN5X = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN5X;
                                    mKuarters.HR_JUMLAH_POTONGAN = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_JUMLAH_POTONGAN;
                                    db.HR_MAKLUMAT_KUARTERS.Add(mKuarters);
                                    db.SaveChanges();
                                }

                                mKuarters.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mKuarters.HR_KOD_KUARTERS = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS;
                                mKuarters.HR_TARIKH_MASUK = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_MASUK);
                                mKuarters.HR_TARIKH_KELUAR = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_KELUAR;
                                mKuarters.HR_NO_UNIT = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_NO_UNIT;
                                mKuarters.HR_GANDAAN2X = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN2X;
                                mKuarters.HR_GERAI = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GERAI;
                                mKuarters.HR_CATATAN = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_CATATAN;
                                mKuarters.HR_IDP = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_IDP;
                                mKuarters.HR_AKTIF_IND = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_AKTIF_IND;
                                mKuarters.HR_GANDAAN5X = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_GANDAAN5X;
                                mKuarters.HR_JUMLAH_POTONGAN = mKakitangan.HR_MAKLUMAT_KUARTERS.HR_JUMLAH_POTONGAN;
                                db.Entry(mKuarters).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                HR_MAKLUMAT_KUARTERS pKuarters = db.HR_MAKLUMAT_KUARTERS.FirstOrDefault(s => s.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA && s.HR_KOD_KUARTERS == mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS2 && s.HR_TARIKH_MASUK == mKakitangan.HR_MAKLUMAT_KUARTERS.HR_TARIKH_MASUK2);
                                if(pKuarters != null)
                                {
                                    db.HR_MAKLUMAT_KUARTERS.Remove(pKuarters);
                                    db.SaveChanges();
                                }
                            }
                            
                        }
                    }

                    if (Kemaskini == "Gaji")
                    {
                        db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G"));
                        db.SaveChanges();
                        //db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        //db.HR_SEJARAH_ELAUN_POTONGAN.RemoveRange(db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganG = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganG.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mElaunPotonganG.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                                    mElaunPotonganG.HR_PENERANGAN = item.HR_PENERANGAN;
                                    mElaunPotonganG.HR_NO_FAIL = item.HR_NO_FAIL;
                                    mElaunPotonganG.HR_JUMLAH = item.HR_JUMLAH;
                                    mElaunPotonganG.HR_ELAUN_POTONGAN_IND = "G";
                                    mElaunPotonganG.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                                    mElaunPotonganG.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                    mElaunPotonganG.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                                    mElaunPotonganG.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                                    mElaunPotonganG.HR_BAKI = item.HR_BAKI;
                                    mElaunPotonganG.HR_AKTIF_IND = item.HR_AKTIF_IND;
                                    mElaunPotonganG.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                                    mElaunPotonganG.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                                    mElaunPotonganG.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mElaunPotonganG.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mElaunPotonganG.HR_UBAH_IND = item.HR_UBAH_IND;
                                    mElaunPotonganG.HR_GRED_PT = item.HR_GRED_PT;
                                    mElaunPotonganG.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                                    mElaunPotonganG.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mElaunPotonganG.HR_NP_UBAH = item.HR_NP_UBAH;
                                    mElaunPotonganG.HR_AUTO_IND = item.HR_AUTO_IND;
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(mElaunPotonganG);
                                    db.SaveChanges();

                                    //SejarahElaunPotongan(mElaunPotonganG, mPeribadi);
                                }
                                
                            }
                        }
                    }

                    if (Kemaskini == "Gaji2")
                    {
                        db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E"));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganE = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganE.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mElaunPotonganE.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                                    mElaunPotonganE.HR_PENERANGAN = item.HR_PENERANGAN;
                                    mElaunPotonganE.HR_NO_FAIL = item.HR_NO_FAIL;
                                    mElaunPotonganE.HR_JUMLAH = item.HR_JUMLAH;
                                    mElaunPotonganE.HR_ELAUN_POTONGAN_IND = "E";
                                    mElaunPotonganE.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                                    mElaunPotonganE.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                    mElaunPotonganE.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                                    mElaunPotonganE.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                                    mElaunPotonganE.HR_BAKI = item.HR_BAKI;
                                    mElaunPotonganE.HR_AKTIF_IND = item.HR_AKTIF_IND;
                                    mElaunPotonganE.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                                    mElaunPotonganE.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                                    mElaunPotonganE.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mElaunPotonganE.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mElaunPotonganE.HR_UBAH_IND = item.HR_UBAH_IND;
                                    mElaunPotonganE.HR_GRED_PT = item.HR_GRED_PT;
                                    mElaunPotonganE.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                                    mElaunPotonganE.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mElaunPotonganE.HR_NP_UBAH = item.HR_NP_UBAH;
                                    mElaunPotonganE.HR_AUTO_IND = item.HR_AUTO_IND;
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(mElaunPotonganE);
                                    db.SaveChanges();

                                    //SejarahElaunPotongan(mElaunPotonganE, mPeribadi);
                                }
                            }
                        }
                    }

                    if (Kemaskini == "Gaji3")
                    {
                        db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "P"));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganP = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganP.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mElaunPotonganP.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                                    mElaunPotonganP.HR_PENERANGAN = item.HR_PENERANGAN;
                                    mElaunPotonganP.HR_NO_FAIL = item.HR_NO_FAIL;
                                    mElaunPotonganP.HR_JUMLAH = item.HR_JUMLAH;
                                    mElaunPotonganP.HR_ELAUN_POTONGAN_IND = "P";
                                    mElaunPotonganP.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                                    mElaunPotonganP.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                    mElaunPotonganP.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                                    mElaunPotonganP.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                                    mElaunPotonganP.HR_BAKI = item.HR_BAKI;
                                    mElaunPotonganP.HR_AKTIF_IND = item.HR_AKTIF_IND;
                                    mElaunPotonganP.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                                    mElaunPotonganP.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                                    mElaunPotonganP.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mElaunPotonganP.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mElaunPotonganP.HR_UBAH_IND = item.HR_UBAH_IND;
                                    mElaunPotonganP.HR_GRED_PT = item.HR_GRED_PT;
                                    mElaunPotonganP.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                                    mElaunPotonganP.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mElaunPotonganP.HR_NP_UBAH = item.HR_NP_UBAH;
                                    mElaunPotonganP.HR_AUTO_IND = item.HR_AUTO_IND;
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(mElaunPotonganP);
                                    db.SaveChanges();

                                    //SejarahElaunPotongan(mElaunPotonganP, mPeribadi);
                                }
                            }
                        }
                    }

                    if (Kemaskini == "Gaji4")
                    {
                        db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "C"));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C != null)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganC = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganC.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mElaunPotonganC.HR_KOD_ELAUN_POTONGAN = item.HR_KOD_ELAUN_POTONGAN;
                                    mElaunPotonganC.HR_PENERANGAN = item.HR_PENERANGAN;
                                    mElaunPotonganC.HR_NO_FAIL = item.HR_NO_FAIL;
                                    mElaunPotonganC.HR_JUMLAH = item.HR_JUMLAH;
                                    mElaunPotonganC.HR_ELAUN_POTONGAN_IND = "C";
                                    mElaunPotonganC.HR_MOD_BAYARAN = item.HR_MOD_BAYARAN;
                                    mElaunPotonganC.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                    mElaunPotonganC.HR_TARIKH_AKHIR = item.HR_TARIKH_AKHIR;
                                    mElaunPotonganC.HR_TUNTUTAN_MAKSIMA = item.HR_TUNTUTAN_MAKSIMA;
                                    mElaunPotonganC.HR_BAKI = item.HR_BAKI;
                                    mElaunPotonganC.HR_AKTIF_IND = item.HR_AKTIF_IND;
                                    mElaunPotonganC.HR_HARI_BEKERJA = item.HR_HARI_BEKERJA;
                                    mElaunPotonganC.HR_NO_PEKERJA_PT = item.HR_NO_PEKERJA_PT;
                                    mElaunPotonganC.HR_TARIKH_KEYIN = item.HR_TARIKH_KEYIN;
                                    mElaunPotonganC.HR_TARIKH_UBAH = item.HR_TARIKH_UBAH;
                                    mElaunPotonganC.HR_UBAH_IND = item.HR_UBAH_IND;
                                    mElaunPotonganC.HR_GRED_PT = item.HR_GRED_PT;
                                    mElaunPotonganC.HR_MATRIKS_GAJI_PT = item.HR_MATRIKS_GAJI_PT;
                                    mElaunPotonganC.HR_NP_KEYIN = item.HR_NP_KEYIN;
                                    mElaunPotonganC.HR_NP_UBAH = item.HR_NP_UBAH;
                                    mElaunPotonganC.HR_AUTO_IND = item.HR_AUTO_IND;
                                    db.HR_MAKLUMAT_ELAUN_POTONGAN.Add(mElaunPotonganC);
                                    db.SaveChanges();

                                    //SejarahElaunPotongan(mElaunPotonganC, mPeribadi);
                                }
                            }
                        }
                    }

                    if (Kemaskini == "Anugerah")
                    {
                        db.HR_MAKLUMAT_KURNIAAN.RemoveRange(db.HR_MAKLUMAT_KURNIAAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.HR_PENCALONAN_KURNIAAN.RemoveRange(db.HR_PENCALONAN_KURNIAAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_MAKLUMAT_KURNIAAN.Count() > 0)
                        {
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KURNIAAN)
                            {
                                if (item.HR_STATUS == "Y")
                                {
                                    HR_MAKLUMAT_KURNIAAN mKurniaan = new HR_MAKLUMAT_KURNIAAN();
                                    mKurniaan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mKurniaan.HR_TARIKH_KURNIAAN = Convert.ToDateTime(item.HR_TARIKH_KURNIAAN);
                                    mKurniaan.HR_KOD_KURNIAAN = item.HR_KOD_KURNIAAN;
                                    mKurniaan.HR_KURNIAAN_IND = item.HR_KURNIAAN_IND;
                                    mKurniaan.HR_NEGERI = item.HR_NEGERI;
                                    mKurniaan.HR_STATUS = item.HR_STATUS;

                                    db.HR_MAKLUMAT_KURNIAAN.Add(mKurniaan);
                                    db.SaveChanges();
                                }

                                HR_PENCALONAN_KURNIAAN mPKurniaan = new HR_PENCALONAN_KURNIAAN();
                                mPKurniaan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mPKurniaan.HR_KOD_KURNIAAN = item.HR_KOD_KURNIAAN;
                                mPKurniaan.HR_KURNIAAN_IND = item.HR_KURNIAAN_IND;
                                mPKurniaan.HR_NEGERI = item.HR_KURNIAAN_IND;
                                mPKurniaan.HR_STATUS = item.HR_STATUS;
                                mPKurniaan.HR_TARIKH_PENCALONAN = Convert.ToDateTime(item.HR_TARIKH_PENCALONAN);
                                mPKurniaan.HR_NP_PENCALON = item.HR_NP_PENCALON;
                                db.HR_PENCALONAN_KURNIAAN.Add(mPKurniaan);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (Kemaskini == "Anugerah2")
                    {
                        db.HR_ANUGERAH_CEMERLANG.RemoveRange(db.HR_ANUGERAH_CEMERLANG.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_ANUGERAH_CEMERLANG.Count() > 0)
                        {
                            foreach (var item in mKakitangan.HR_ANUGERAH_CEMERLANG)
                            {
                                if (item.HR_NAMA_ANUGERAH != null)
                                {
                                    HR_ANUGERAH_CEMERLANG mAnugerahCemerlang = new HR_ANUGERAH_CEMERLANG();
                                    mAnugerahCemerlang.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                    mAnugerahCemerlang.HR_NAMA_ANUGERAH = item.HR_NAMA_ANUGERAH;
                                    mAnugerahCemerlang.HR_TARIKH_PENERIMAAN = item.HR_TARIKH_PENERIMAAN;
                                    mAnugerahCemerlang.HR_TAHUN = item.HR_TAHUN;
                                    db.HR_ANUGERAH_CEMERLANG.Add(mAnugerahCemerlang);
                                    db.SaveChanges();
                                }

                            }
                        }
                    }

                    if (Kemaskini == "Anugerah3")
                    {
                        if (mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI == null && mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI == null && mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP == null && mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND == null && mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP == null)
                        {
                            HR_ANUGERAH_HAJI pAnugerahHaji = db.HR_ANUGERAH_HAJI.FirstOrDefault(s => s.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA);
                            if(pAnugerahHaji != null)
                            {
                                db.HR_ANUGERAH_HAJI.Remove(pAnugerahHaji);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (mAnugerahHaji == null)
                            {
                                mAnugerahHaji = new HR_ANUGERAH_HAJI();
                                mAnugerahHaji.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mAnugerahHaji.HR_TAHUN_PERGI = mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI;
                                mAnugerahHaji.HR_STATUS_HAJI = mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI;
                                mAnugerahHaji.HR_NP_YDP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP;
                                mAnugerahHaji.HR_LULUS_IND = mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND;
                                mAnugerahHaji.HR_NP_UP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP;
                                mAnugerahHaji.HR_NP_PEG = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG;
                                db.HR_ANUGERAH_HAJI.Add(mAnugerahHaji);
                                db.SaveChanges();
                            }
                            else
                            {
                                mAnugerahHaji.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mAnugerahHaji.HR_TAHUN_PERGI = mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI;
                                mAnugerahHaji.HR_STATUS_HAJI = mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI;
                                mAnugerahHaji.HR_NP_YDP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP;
                                mAnugerahHaji.HR_LULUS_IND = mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND;
                                mAnugerahHaji.HR_NP_UP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP;
                                mAnugerahHaji.HR_NP_PEG = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG;
                                db.Entry(mAnugerahHaji).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        
                    }

                    if (Kemaskini == "Persaraan")
                    {
                        if(mKakitangan.HR_PERSARAAN.HR_TARIKH_BERSARA == null)
                        {
                            HR_PERSARAAN pPersaraan = db.HR_PERSARAAN.FirstOrDefault(s => s.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA);
                            if(pPersaraan != null)
                            {
                                db.HR_PERSARAAN.Remove(pPersaraan);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (mPersaraan == null)
                            {
                                mPersaraan = new HR_PERSARAAN();
                                mPersaraan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mPersaraan.HR_TARIKH_BERSARA = mKakitangan.HR_PERSARAAN.HR_TARIKH_BERSARA;
                                mPersaraan.HR_ALASAN = mKakitangan.HR_PERSARAAN.HR_ALASAN;
                                mPersaraan.HR_BERSARA_IND = mKakitangan.HR_PERSARAAN.HR_BERSARA_IND;
                                mPersaraan.HR_BAYARAN_IND = mKakitangan.HR_PERSARAAN.HR_BAYARAN_IND;
                                mPersaraan.HR_JUMLAH_BAYARAN = mKakitangan.HR_PERSARAAN.HR_JUMLAH_BAYARAN;
                                mPersaraan.HR_JUMLAH_CUTI = mKakitangan.HR_PERSARAAN.HR_JUMLAH_CUTI;
                                mPersaraan.HR_PALAMAT1 = mKakitangan.HR_PERSARAAN.HR_PALAMAT1;
                                mPersaraan.HR_PALAMAT2 = mKakitangan.HR_PERSARAAN.HR_PALAMAT2;
                                mPersaraan.HR_PALAMAT3 = mKakitangan.HR_PERSARAAN.HR_PALAMAT3;
                                mPersaraan.HR_PBANDAR = mKakitangan.HR_PERSARAAN.HR_PBANDAR;
                                mPersaraan.HR_PPOSKOD = mKakitangan.HR_PERSARAAN.HR_PPOSKOD;
                                mPersaraan.HR_PNEGERI = mKakitangan.HR_PERSARAAN.HR_PNEGERI;
                                mPersaraan.HR_EKA = mKakitangan.HR_PERSARAAN.HR_EKA;
                                mPersaraan.HR_ITP = mKakitangan.HR_PERSARAAN.HR_ITP;
                                mPersaraan.HR_GAJI_POKOK = mKakitangan.HR_PERSARAAN.HR_GAJI_POKOK;
                                mPersaraan.HR_TERIMA_BAYARAN_IND = mKakitangan.HR_PERSARAAN.HR_TERIMA_BAYARAN_IND;
                                mPersaraan.HR_NP_PEGAWAI = mKakitangan.HR_PERSARAAN.HR_NP_PEGAWAI;
                                mPersaraan.HR_JAWATAN_PEGAWAI = mKakitangan.HR_PERSARAAN.HR_JAWATAN_PEGAWAI;
                                db.HR_PERSARAAN.Add(mPersaraan);
                                db.SaveChanges();
                            }
                            mPersaraan.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                            mPersaraan.HR_TARIKH_BERSARA = mKakitangan.HR_PERSARAAN.HR_TARIKH_BERSARA;
                            mPersaraan.HR_ALASAN = mKakitangan.HR_PERSARAAN.HR_ALASAN;
                            mPersaraan.HR_BERSARA_IND = mKakitangan.HR_PERSARAAN.HR_BERSARA_IND;
                            mPersaraan.HR_BAYARAN_IND = mKakitangan.HR_PERSARAAN.HR_BAYARAN_IND;
                            mPersaraan.HR_JUMLAH_BAYARAN = mKakitangan.HR_PERSARAAN.HR_JUMLAH_BAYARAN;
                            mPersaraan.HR_JUMLAH_CUTI = mKakitangan.HR_PERSARAAN.HR_JUMLAH_CUTI;
                            mPersaraan.HR_PALAMAT1 = mKakitangan.HR_PERSARAAN.HR_PALAMAT1;
                            mPersaraan.HR_PALAMAT2 = mKakitangan.HR_PERSARAAN.HR_PALAMAT2;
                            mPersaraan.HR_PALAMAT3 = mKakitangan.HR_PERSARAAN.HR_PALAMAT3;
                            mPersaraan.HR_PBANDAR = mKakitangan.HR_PERSARAAN.HR_PBANDAR;
                            mPersaraan.HR_PPOSKOD = mKakitangan.HR_PERSARAAN.HR_PPOSKOD;
                            mPersaraan.HR_PNEGERI = mKakitangan.HR_PERSARAAN.HR_PNEGERI;
                            mPersaraan.HR_EKA = mKakitangan.HR_PERSARAAN.HR_EKA;
                            mPersaraan.HR_ITP = mKakitangan.HR_PERSARAAN.HR_ITP;
                            mPersaraan.HR_GAJI_POKOK = mKakitangan.HR_PERSARAAN.HR_GAJI_POKOK;
                            mPersaraan.HR_TERIMA_BAYARAN_IND = mKakitangan.HR_PERSARAAN.HR_TERIMA_BAYARAN_IND;
                            mPersaraan.HR_NP_PEGAWAI = mKakitangan.HR_PERSARAAN.HR_NP_PEGAWAI;
                            mPersaraan.HR_JAWATAN_PEGAWAI = mKakitangan.HR_PERSARAAN.HR_JAWATAN_PEGAWAI;
                            db.Entry(mPersaraan).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    if (Kemaskini == "Tatatertib")
                    {
                        db.HR_TINDAKAN_DISIPLIN.RemoveRange(db.HR_TINDAKAN_DISIPLIN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        db.SaveChanges();
                        if (mKakitangan.HR_TINDAKAN_DISIPLIN.Count() > 0)
                        {
                            foreach (var item in mKakitangan.HR_TINDAKAN_DISIPLIN)
                            {
                                if(item.HR_TARIKH_KESALAHAN.HasValue)
                                {
                                    var check = db.HR_TINDAKAN_DISIPLIN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_TARIKH_KESALAHAN == item.HR_TARIKH_KESALAHAN).ToList<HR_TINDAKAN_DISIPLIN>();
                                    if (check.Count() <= 0)
                                    {
                                        HR_TINDAKAN_DISIPLIN mTindakanDisiplin = new HR_TINDAKAN_DISIPLIN();
                                        mTindakanDisiplin.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                        mTindakanDisiplin.HR_TARIKH_KESALAHAN = Convert.ToDateTime(item.HR_TARIKH_KESALAHAN);
                                        mTindakanDisiplin.HR_KESALAHAN = item.HR_KESALAHAN;
                                        db.HR_TINDAKAN_DISIPLIN.Add(mTindakanDisiplin);
                                        db.SaveChanges();

                                    }
                                    db.HR_TINDAKAN_DISIPLIN_DETAIL.RemoveRange(db.HR_TINDAKAN_DISIPLIN_DETAIL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_TARIKH_KESALAHAN == item.HR_TARIKH_KESALAHAN));
                                    var check2 = db.HR_TINDAKAN_DISIPLIN_DETAIL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_TARIKH_KESALAHAN == item.HR_TARIKH_KESALAHAN && s.HR_KOD_TINDAKAN == item.HR_KOD_TINDAKAN).ToList<HR_TINDAKAN_DISIPLIN_DETAIL>();
                                    if (check2.Count() <= 0)
                                    {
                                        HR_TINDAKAN_DISIPLIN_DETAIL mTindakanDisiplinDetail = new HR_TINDAKAN_DISIPLIN_DETAIL();
                                        mTindakanDisiplinDetail.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                        mTindakanDisiplinDetail.HR_TARIKH_KESALAHAN = Convert.ToDateTime(item.HR_TARIKH_KESALAHAN);
                                        mTindakanDisiplinDetail.HR_KOD_TINDAKAN = item.HR_KOD_TINDAKAN;
                                        mTindakanDisiplinDetail.HR_TARIKH_MULA = item.HR_TARIKH_MULA;
                                        mTindakanDisiplinDetail.HR_TARIKH_AKHIR = item.HR_TARIKH_KESALAHAN;
                                        db.HR_TINDAKAN_DISIPLIN_DETAIL.Add(mTindakanDisiplinDetail);
                                        db.SaveChanges();
                                    }
                                }
                                
                            }

                        }

                    }

                    if (Kemaskini == "Kematian")
                    {
                        if (mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_KEMATIAN == null)
                        {
                            HR_MAKLUMAT_KEMATIAN pKematian = db.HR_MAKLUMAT_KEMATIAN.FirstOrDefault(s => s.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA);
                            if(pKematian != null)
                            {
                                db.HR_MAKLUMAT_KEMATIAN.Remove(pKematian);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (mKematian == null)
                            {
                                mKematian = new HR_MAKLUMAT_KEMATIAN();
                                mKematian.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mKematian.HR_TARIKH_KEMATIAN = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_KEMATIAN);
                                mKematian.HR_NO_KP_PEWARIS = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_KP_PEWARIS;
                                mKematian.HR_ALAMAT1 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT1;
                                mKematian.HR_ALAMAT2 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT2;
                                mKematian.HR_ALAMAT3 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT3;
                                mKematian.HR_BANDAR = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_BANDAR;
                                mKematian.HR_NO_TELRUMAH = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELRUMAH;
                                mKematian.HR_HUBUNGAN = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_HUBUNGAN;
                                mKematian.HR_POSKOD = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_POSKOD;
                                mKematian.HR_NEGERI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NEGERI;
                                mKematian.HR_NAMA_PEWARIS = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEWARIS;
                                mKematian.HR_NO_TELPEJABAT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELPEJABAT;
                                mKematian.HR_NO_TELBIMBIT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELBIMBIT;
                                mKematian.HR_NO_VOUCHER = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_VOUCHER;
                                mKematian.HR_NAMA_PEGAWAI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEGAWAI;
                                mKematian.HR_JAWATAN_PEGAWAI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JAWATAN_PEGAWAI;
                                mKematian.HR_TARIKH_BAYAR = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_BAYAR;
                                mKematian.HR_MAKLUMAT_KHIDMAT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_MAKLUMAT_KHIDMAT;
                                mKematian.HR_JUMLAH_WANG = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JUMLAH_WANG;
                                mKematian.HR_VOT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_VOT;
                                db.HR_MAKLUMAT_KEMATIAN.Add(mKematian);
                                db.SaveChanges();
                            }
                            mKematian.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                            mKematian.HR_TARIKH_KEMATIAN = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_KEMATIAN);
                            mKematian.HR_NO_KP_PEWARIS = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_KP_PEWARIS;
                            mKematian.HR_ALAMAT1 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT1;
                            mKematian.HR_ALAMAT2 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT2;
                            mKematian.HR_ALAMAT3 = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_ALAMAT3;
                            mKematian.HR_BANDAR = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_BANDAR;
                            mKematian.HR_NO_TELRUMAH = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELRUMAH;
                            mKematian.HR_HUBUNGAN = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_HUBUNGAN;
                            mKematian.HR_POSKOD = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_POSKOD;
                            mKematian.HR_NEGERI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NEGERI;
                            mKematian.HR_NAMA_PEWARIS = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEWARIS;
                            mKematian.HR_NO_TELPEJABAT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELPEJABAT;
                            mKematian.HR_NO_TELBIMBIT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_TELBIMBIT;
                            mKematian.HR_NO_VOUCHER = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NO_VOUCHER;
                            mKematian.HR_NAMA_PEGAWAI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_NAMA_PEGAWAI;
                            mKematian.HR_JAWATAN_PEGAWAI = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JAWATAN_PEGAWAI;
                            mKematian.HR_TARIKH_BAYAR = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_TARIKH_BAYAR;
                            mKematian.HR_MAKLUMAT_KHIDMAT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_MAKLUMAT_KHIDMAT;
                            mKematian.HR_JUMLAH_WANG = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_JUMLAH_WANG;
                            mKematian.HR_VOT = mKakitangan.HR_MAKLUMAT_KEMATIAN.HR_VOT;
                            db.Entry(mKematian).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    if (Kemaskini == "Prestasi")
                    {
                        if(mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI == null)
                        {
                            HR_PENILAIAN_PRESTASI pPrestasi = db.HR_PENILAIAN_PRESTASI.FirstOrDefault(s => s.HR_TAHUN_PRESTASI == mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI2 && s.HR_NO_PEKERJA == mPeribadi.HR_NO_PEKERJA);
                            if(pPrestasi != null)
                            {
                                db.HR_PENILAIAN_PRESTASI.Remove(pPrestasi);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (mPrestasi == null)
                            {
                                mPrestasi = new HR_PENILAIAN_PRESTASI();
                                mPrestasi.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                                mPrestasi.HR_TAHUN_PRESTASI = Convert.ToInt16(mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI);
                                mPrestasi.HR_PENGHASILAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPP;
                                mPrestasi.HR_PENGHASILAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPK;
                                mPrestasi.HR_PENGETAHUAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPP;
                                mPrestasi.HR_PENGETAHUAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPK;
                                mPrestasi.HR_KUALITI_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPP;
                                mPrestasi.HR_KUALITI_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPK;
                                mPrestasi.HR_SUMBANGAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPP;
                                mPrestasi.HR_SUMBANGAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPK;
                                mPrestasi.HR_PURATA_PENGHASILAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGHASILAN;
                                mPrestasi.HR_PURATA_PENGETAHUAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGETAHUAN;
                                mPrestasi.HR_PURATA_KUALITI = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_KUALITI;
                                mPrestasi.HR_PURATA_SUMBANGAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_SUMBANGAN;
                                mPrestasi.HR_PERATUS_PENGHASILAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGHASILAN;
                                mPrestasi.HR_PERATUS_PENGETAHUAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGETAHUAN;
                                mPrestasi.HR_PERATUS_KUALITI = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_KUALITI;
                                mPrestasi.HR_PERATUS_SUMBANGAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_SUMBANGAN;
                                mPrestasi.HR_JUMLAH_BESAR = mKakitangan.HR_PENILAIAN_PRESTASI.HR_JUMLAH_BESAR;
                                mPrestasi.HR_CEMERLANG_IND = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CEMERLANG_IND;
                                mPrestasi.HR_JENIS_IND = mKakitangan.HR_PENILAIAN_PRESTASI.HR_JENIS_IND;
                                mPrestasi.HR_CUTI_KERAJAAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_KERAJAAN;
                                mPrestasi.HR_CUTI_SWASTA = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_SWASTA;
                                db.HR_PENILAIAN_PRESTASI.Add(mPrestasi);
                                db.SaveChanges();
                            }
                            mPrestasi.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
                            mPrestasi.HR_TAHUN_PRESTASI = Convert.ToInt16(mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI);
                            mPrestasi.HR_PENGHASILAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPP;
                            mPrestasi.HR_PENGHASILAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGHASILAN_PPK;
                            mPrestasi.HR_PENGETAHUAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPP;
                            mPrestasi.HR_PENGETAHUAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PENGETAHUAN_PPK;
                            mPrestasi.HR_KUALITI_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPP;
                            mPrestasi.HR_KUALITI_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_KUALITI_PPK;
                            mPrestasi.HR_SUMBANGAN_PPP = mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPP;
                            mPrestasi.HR_SUMBANGAN_PPK = mKakitangan.HR_PENILAIAN_PRESTASI.HR_SUMBANGAN_PPK;
                            mPrestasi.HR_PURATA_PENGHASILAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGHASILAN;
                            mPrestasi.HR_PURATA_PENGETAHUAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_PENGETAHUAN;
                            mPrestasi.HR_PURATA_KUALITI = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_KUALITI;
                            mPrestasi.HR_PURATA_SUMBANGAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PURATA_SUMBANGAN;
                            mPrestasi.HR_PERATUS_PENGHASILAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGHASILAN;
                            mPrestasi.HR_PERATUS_PENGETAHUAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_PENGETAHUAN;
                            mPrestasi.HR_PERATUS_KUALITI = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_KUALITI;
                            mPrestasi.HR_PERATUS_SUMBANGAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_PERATUS_SUMBANGAN;
                            mPrestasi.HR_JUMLAH_BESAR = mKakitangan.HR_PENILAIAN_PRESTASI.HR_JUMLAH_BESAR;
                            mPrestasi.HR_CEMERLANG_IND = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CEMERLANG_IND;
                            mPrestasi.HR_JENIS_IND = mKakitangan.HR_PENILAIAN_PRESTASI.HR_JENIS_IND;
                            mPrestasi.HR_CUTI_KERAJAAN = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_KERAJAAN;
                            mPrestasi.HR_CUTI_SWASTA = mKakitangan.HR_PENILAIAN_PRESTASI.HR_CUTI_SWASTA;
                            db.Entry(mPrestasi).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    if (Kemaskini == "Cuti")
                    {

                    }
                }


                return Redirect(Url.Action("Index", new { key = '4' ,value = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA })+ "#"+ mKakitangan.ACTIVE_ITEM);
            }


            if (mPekerjaan.HR_MATRIKS_GAJI != null)
            {
                var MatriksGaji = mPekerjaan.HR_MATRIKS_GAJI.Split('T');
                decimal tahap = Convert.ToDecimal(MatriksGaji[1]);
                var peringkat = Convert.ToInt32(MatriksGaji[0].Substring(1));

                ViewBag.HR_GAJI_MIN = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI), "HR_GAJI_MIN", "HR_GAJI_MIN");
                ViewBag.HR_GAJI_MAX = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI), "HR_GAJI_MAX", "HR_GAJI_MAX");
            }
            else
            {
                ViewBag.HR_GAJI_MIN = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI), "HR_GAJI_MIN", "HR_GAJI_MIN");
                ViewBag.HR_GAJI_MAX = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI), "HR_GAJI_MAX", "HR_GAJI_MAX");
            }

            //STRAT PERIBADI
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.Agama = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 106), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_WARGANEGARA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 2), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.TempatLahir = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.TarafKahwin = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_ALASAN", "HR_PENERANGAN");
            ViewBag.Keturunan = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 1), "ORDINAL", "SHORT_DESCRIPTION");
            //END PERIBADI

            //START PEKERJAAN
            ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA), "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_GAJI_PRORATA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 116), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN.OrderBy(s => s.GE_KETERANGAN_JABATAN), "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.HR_UNIT = new SelectList(db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_UNIT", "GE_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 115), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUMP_PERKHIDMATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 103), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TARAF_JAWATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_JAWATAN = new SelectList(db.HR_JAWATAN.OrderBy(s => s.HR_NAMA_JAWATAN), "HR_KOD_JAWATAN", "HR_NAMA_JAWATAN");


            List<SelectListItem> HR_KUMPULAN = new List<SelectListItem>();
            HR_KUMPULAN.Add(new SelectListItem { Text = "A", Value = "A" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "B", Value = "B" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "C", Value = "C" });
            HR_KUMPULAN.Add(new SelectListItem { Text = "D", Value = "D" });
            ViewBag.HR_KUMPULAN = HR_KUMPULAN;

            ViewBag.HR_KOD_BANK = new SelectList(db.HR_AGENSI.Where(s => s.HR_PERBANKAN == "Y"), "HR_KOD_AGENSI", "HR_NAMA_AGENSI");
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM), "ORDINAL", "SHORT_DESCRIPTION");
            Nullable<int> HR_UMUR_SARA = null;
            if (mPekerjaan.HR_TARIKH_TAMAT != null && mPeribadi.HR_TARIKH_LAHIR != null)
            {
                HR_UMUR_SARA = (Convert.ToDateTime(mPekerjaan.HR_TARIKH_TAMAT).Year - Convert.ToDateTime(mPeribadi.HR_TARIKH_LAHIR).Year);
            }
            ViewBag.HR_UMUR_SARA = HR_UMUR_SARA;

            ViewBag.HR_MATRIKS_GAJI = new SelectList(db.HR_MAKLUMAT_PEKERJAAN, "HR_MATRIKS_GAJI", "HR_MATRIKS_GAJI");
            ViewBag.HR_KOD_GELARAN_J = new SelectList(db.HR_GELARAN_JAWATAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_GELARAN", "HR_PENERANGAN");
            ViewBag.HR_TINGKATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 113), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102), "STRING_PARAM", "STRING_PARAM");
            ViewBag.HR_KATEGORI_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101), "STRING_PARAM", "SHORT_DESCRIPTION");

            ViewBag.HR_JUM_TAHUN = null;
            if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
            {
                ViewBag.HR_JUM_TAHUN = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT_KONTRAK).Year - Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK).Year;
            }

            List<HR_MAKLUMAT_PENGALAMAN_KERJA> sPengalaman = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
            ViewBag.sPengalaman = sPengalaman;
            //END PEKERJAAN
            //START KEMAHIRAN
            ViewBag.HR_BAHASA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 107), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.P_TAHAP_KEMAHIRAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 105), "STRING_PARAM", "SHORT_DESCRIPTION");
            //END KEMAHIRAN

            //START AKADEMIK
            ViewBag.HR_KURSUS = db.HR_KURSUS;
            List<SelectListItem> HR_PERINGKAT = new List<SelectListItem>();
            HR_PERINGKAT.Add(new SelectListItem { Text = "KEBANGSAAN", Value = "KEBANGSAAN" });
            HR_PERINGKAT.Add(new SelectListItem { Text = "NEGERI", Value = "NEGERI" });
            HR_PERINGKAT.Add(new SelectListItem { Text = "DAERAH", Value = "DAERAH" });
            HR_PERINGKAT.Add(new SelectListItem { Text = "JABATAN", Value = "JABATAN" });
            ViewBag.HR_PERINGKAT = HR_PERINGKAT;

            List<SelectListItem> HR_KEPUTUSAN = new List<SelectListItem>();
            HR_KEPUTUSAN.Add(new SelectListItem { Text = "Lulus", Value = "Y" });
            HR_KEPUTUSAN.Add(new SelectListItem { Text = "Tidak Lulus", Value = "T" });
            ViewBag.HR_KEPUTUSAN = HR_KEPUTUSAN;
            //END AKADEMIK
            //START PEWARIS
            ViewBag.HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125), "ORDINAL", "SHORT_DESCRIPTION");
            //END PEWARIS

            //TANGGUNGAN
            ViewBag.HR_TEMPAT_LAHIR = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            //

            //KUARTERS
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUARTERS = new SelectList(db.HR_KUARTERS, "HR_KOD_KUARTERS", "HR_BLOK_KUARTERS");
            //

            //GAJI
            ViewBag.Gaji = db.HR_GAJI_UPAHAN.Where(s => s.HR_KETERANGAN_SLIP == "GAJI").OrderBy(s => s.HR_PENERANGAN_UPAH).ToList<HR_GAJI_UPAHAN>();
            //
            //ELAUN
            ViewBag.Elaun = db.HR_ELAUN.OrderBy(s => s.HR_PENERANGAN_ELAUN).ToList<HR_ELAUN>();
            //

            //CARUMAN
            ViewBag.Potongan = db.HR_POTONGAN.OrderBy(s => s.HR_PENERANGAN_POTONGAN).ToList<HR_POTONGAN>();
            ViewBag.Caruman = db.HR_CARUMAN.OrderBy(s => s.HR_PENERANGAN_CARUMAN).ToList<HR_CARUMAN>();
            //

            //GAJI, ELAUN, CARUMAN, ANUGERAH
            ViewBag.HR_MOD_BAYARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 117), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_AKTIF_IND_PEKERJAAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")), "STRING_PARAM", "SHORT_DESCRIPTION");
            List<SelectListItem> HR_AUTO_IND = new List<SelectListItem>();
            HR_AUTO_IND.Add(new SelectListItem { Text = "YA", Value = "Y" });
            HR_AUTO_IND.Add(new SelectListItem { Text = "TIDAK", Value = "T" });
            ViewBag.HR_AUTO_IND = HR_AUTO_IND;
            //

            //ANUGERAH
            ViewBag.Peringkat = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).GroupBy(s => s.SHORT_DESCRIPTION).Select(s => s.FirstOrDefault()), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            ViewBag.HR_KURNIAAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130), "STRING_PARAM", "LONG_DESCRIPTION");
            ViewBag.Kurniaan = db.HR_KURNIAAN;
            ViewBag.HR_NP_PENCALON = db.HR_MAKLUMAT_PERIBADI.ToList<HR_MAKLUMAT_PERIBADI>();
            //


            List<SelectListItem> HR_STATUS = new List<SelectListItem>();
            HR_STATUS.Add(new SelectListItem { Text = "BERJAYA", Value = "Y" });
            HR_STATUS.Add(new SelectListItem { Text = "TIDAK BERJAYA", Value = "T" });
            HR_STATUS.Add(new SelectListItem { Text = "DICALONKAN", Value = "P" });
            ViewBag.HR_STATUS = HR_STATUS;

            List<SelectListItem> HR_STATUS_HAJI = new List<SelectListItem>();
            HR_STATUS_HAJI.Add(new SelectListItem { Text = "TERIMA", Value = "T" });
            HR_STATUS_HAJI.Add(new SelectListItem { Text = "SEDANG DIPROSES", Value = "S" });
            HR_STATUS_HAJI.Add(new SelectListItem { Text = "DICALONKAN", Value = "P" });
            HR_STATUS_HAJI.Add(new SelectListItem { Text = "TOLAK", Value = "K" });
            ViewBag.HR_STATUS_HAJI = HR_STATUS_HAJI;

            ViewBag.HR_TINDAKAN = new SelectList(db.HR_TINDAKAN, "HR_KOD_TINDAKAN", "HR_PENERANGAN");

            return View(mKakitangan);
        }

        //public ActionResult SejarahElaunPotongan(HR_MAKLUMAT_ELAUN_POTONGAN model, HR_MAKLUMAT_PERIBADI mPeribadi)
        //{
        //    HR_SEJARAH_ELAUN_POTONGAN sejarah = new HR_SEJARAH_ELAUN_POTONGAN();
        //    sejarah.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
        //    sejarah.HR_KOD_ELAUN_POTONGAN = model.HR_KOD_ELAUN_POTONGAN;
        //    sejarah.HR_PENERANGAN = model.HR_PENERANGAN;
        //    sejarah.HR_NO_FAIL = model.HR_NO_FAIL;
        //    sejarah.HR_JUMLAH = model.HR_JUMLAH;
        //    sejarah.HR_ELAUN_POTONGAN_IND = model.HR_ELAUN_POTONGAN_IND;
        //    sejarah.HR_MOD_BAYARAN = model.HR_MOD_BAYARAN;
        //    sejarah.HR_TARIKH_MULA = model.HR_TARIKH_MULA;
        //    sejarah.HR_TARIKH_AKHIR = model.HR_TARIKH_AKHIR;
        //    sejarah.HR_TUNTUTAN_MAKSIMA = model.HR_TUNTUTAN_MAKSIMA;
        //    sejarah.HR_BAKI = model.HR_BAKI;
        //    sejarah.HR_AKTIF_IND = model.HR_AKTIF_IND;
        //    sejarah.HR_HARI_BEKERJA = model.HR_HARI_BEKERJA;
        //    sejarah.HR_NO_PEKERJA_PT = model.HR_NO_PEKERJA_PT;
        //    sejarah.HR_TARIKH_KEYIN = model.HR_TARIKH_KEYIN;
        //    sejarah.HR_TARIKH_UBAH = model.HR_TARIKH_UBAH;
        //    sejarah.HR_UBAH_IND = model.HR_UBAH_IND;
        //    sejarah.HR_GRED_PT = model.HR_GRED_PT;
        //    sejarah.HR_MATRIKS_GAJI_PT = model.HR_MATRIKS_GAJI_PT;
        //    sejarah.HR_NP_KEYIN = model.HR_NP_KEYIN;
        //    sejarah.HR_NP_UBAH = model.HR_NP_UBAH;
        //    sejarah.HR_AUTO_IND = model.HR_AUTO_IND;
        //    db.HR_SEJARAH_ELAUN_POTONGAN.Add(sejarah);
        //    db.SaveChanges();

        //    return null;
        //}

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MaklumatPeribadi(MaklumatKakitanganModels mKakitangan, string Command)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Find(mKakitangan.HR_NO_PEKERJA);
            
            if (ModelState.IsValid)
            {

                //mPeribadi.HR_NO_PEKERJA = mKakitangan.HR_NO_PEKERJA;
                mPeribadi.HR_NO_KPBARU = mKakitangan.HR_NO_KPBARU;
                mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_NAMA_PEKERJA;
                mPeribadi.HR_NO_KPLAMA = mKakitangan.HR_NO_KPLAMA;
                mPeribadi.HR_TARIKH_LAHIR = mKakitangan.HR_TARIKH_LAHIR;
                mPeribadi.HR_TEMPAT_LAHIR = mKakitangan.HR_TEMPAT_LAHIR;
                mPeribadi.HR_WARGANEGARA = mKakitangan.HR_WARGANEGARA;
                mPeribadi.HR_KETURUNAN = mKakitangan.HR_KETURUNAN;
                mPeribadi.HR_AGAMA = mKakitangan.HR_AGAMA;
                mPeribadi.HR_JANTINA = mKakitangan.HR_JANTINA;
                mPeribadi.HR_TARAF_KAHWIN = mKakitangan.HR_TARAF_KAHWIN;
                mPeribadi.HR_LESEN = mKakitangan.HR_LESEN;
                mPeribadi.HR_KELAS_LESEN = mKakitangan.HR_KELAS_LESEN;
                mPeribadi.HR_TALAMAT1 = mKakitangan.HR_TALAMAT1;
                mPeribadi.HR_TALAMAT2 = mKakitangan.HR_TALAMAT2;
                mPeribadi.HR_TALAMAT3 = mKakitangan.HR_TALAMAT3;
                mPeribadi.HR_TBANDAR = mKakitangan.HR_TBANDAR;
                mPeribadi.HR_TPOSKOD = mKakitangan.HR_TPOSKOD;
                mPeribadi.HR_TNEGERI = mKakitangan.HR_TNEGERI;
                mPeribadi.HR_SALAMAT1 = mKakitangan.HR_SALAMAT1;
                mPeribadi.HR_SALAMAT2 = mKakitangan.HR_SALAMAT2;
                mPeribadi.HR_SALAMAT3 = mKakitangan.HR_SALAMAT3;
                mPeribadi.HR_SBANDAR = mKakitangan.HR_SBANDAR;
                mPeribadi.HR_SPOSKOD = mKakitangan.HR_SPOSKOD;
                mPeribadi.HR_SNEGERI = mKakitangan.HR_SNEGERI;
                mPeribadi.HR_TAHUN_SPM = mKakitangan.HR_TAHUN_SPM;
                mPeribadi.HR_GRED_BM = mKakitangan.HR_GRED_BM;
                mPeribadi.HR_TELRUMAH = mKakitangan.HR_TELRUMAH;
                mPeribadi.HR_TELPEJABAT = mKakitangan.HR_TELPEJABAT;
                mPeribadi.HR_TELBIMBIT = mKakitangan.HR_TELBIMBIT;
                mPeribadi.HR_EMAIL = mKakitangan.HR_EMAIL;
                mPeribadi.HR_AKTIF_IND = mKakitangan.HR_AKTIF_IND;
                mPeribadi.HR_CC_KENDERAAN = mKakitangan.HR_CC_KENDERAAN;
                mPeribadi.HR_NO_KENDERAAN = mKakitangan.HR_NO_KENDERAAN;
                mPeribadi.HR_JENIS_KENDERAAN = mKakitangan.HR_JENIS_KENDERAAN;
                mPeribadi.HR_ALASAN = mKakitangan.HR_ALASAN;
                mPeribadi.HR_IDPEKERJA = mKakitangan.HR_IDPEKERJA;
                mPeribadi.HR_TARIKH_KEYIN = mKakitangan.HR_TARIKH_KEYIN;
                mPeribadi.HR_NP_KEYIN = mKakitangan.HR_NP_KEYIN;
                mPeribadi.HR_TARIKH_UBAH = mKakitangan.HR_TARIKH_UBAH;
                mPeribadi.HR_NP_UBAH = mKakitangan.HR_NP_UBAH;

                db.Entry(mPeribadi).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.msg = "Data berjaya dikemaskini";
                return RedirectToAction("ProfilKakitangan", new { id = mKakitangan.HR_NO_PEKERJA });
            }

            ViewBag.status = new SelectList(db.P_STATUS, "P_STATUS_ID", "P_NAMA_STATUS");


            ViewBag.Agama = new SelectList(db.P_AGAMA, "P_AGAMA_ID", "P_NAMA_AGAMA");

            List<SelectListItem> Keturunan = new List<SelectListItem>();
            Keturunan.Add(new SelectListItem { Text = "MELAYU", Value = "1" });
            Keturunan.Add(new SelectListItem { Text = "CINA", Value = "2" });
            Keturunan.Add(new SelectListItem { Text = "INDIA", Value = "3" });
            Keturunan.Add(new SelectListItem { Text = "LAIN-LAIN", Value = "4" });
            ViewBag.Keturunan = Keturunan;

            List<SelectListItem> Warganegara = new List<SelectListItem>();
            Warganegara.Add(new SelectListItem { Text = "MALAYSIA", Value = "1  " });
            Warganegara.Add(new SelectListItem { Text = "LAIN-LAIN", Value = "0  " });

            ViewBag.Warganegara = Warganegara;

            List<SelectListItem> TempatLahir = new List<SelectListItem>();
            foreach (var item in db.P_NEGERI)
            {
                TempatLahir.Add(new SelectListItem { Text = item.P_PENERANGAN_S, Value = item.P_NEGERI_ID });
            }

            ViewBag.TempatLahir = TempatLahir;
            ViewBag.Negeri = new SelectList(db.P_NEGERI, "P_NEGERI_ID", "P_NAMA_NEGERI");
            ViewBag.TarafKahwin = new SelectList(db.P_TARAF_KAHWIN, "P_TARAF_KAHWIN_ID", "P_NAMA_TARAF_KAHWIN");
            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN, "HR_KOD_ALASAN", "HR_PENERANGAN");

            return View(mKakitangan);
        }*/

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
            if(mCuti.HR_KOD_CUTI != null && mCuti.HR_TAHUN != null)
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
            if(mKakitangan.HR_MAKLUMAT_CUTI.HR_BAKI_TERKINI == null)
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
            if(cuti.HR_KATEGORI != null)
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
            KodCuti = KodCuti.PadRight(5,' ');
            List<MaklumatCuti> item = new List<MaklumatCuti>();
            List<HR_CUTI> cuti = db.HR_CUTI.Where(s => s.HR_KATEGORI == Kategori).ToList();
            foreach(HR_CUTI c in cuti)
            {
                List<HR_MAKLUMAT_CUTI> MaklumatCuti = db.HR_MAKLUMAT_CUTI.Where(s => s.HR_KOD_CUTI == c.HR_KOD_CUTI && s.HR_NO_PEKERJA == NoPekerja).ToList();
                foreach(HR_MAKLUMAT_CUTI mCuti in MaklumatCuti)
                {
                    HR_MAKLUMAT_CUTI_DETAIL mCutiDetail = db.HR_MAKLUMAT_CUTI_DETAIL.SingleOrDefault(s => s.HR_KOD_CUTI == mCuti.HR_KOD_CUTI && s.HR_NO_PEKERJA == mCuti.HR_NO_PEKERJA && s.HR_TARIKH_PERMOHONAN == mCuti.HR_TARIKH_PERMOHONAN);
                    MaklumatCuti data = new MaklumatCuti();
                    data.HR_NO_PEKERJA = mCuti.HR_NO_PEKERJA;
                    data.HR_KOD_CUTI = mCuti.HR_KOD_CUTI;
                    //data.HR_SOKONG_IND = mCutiDetail.HR_SOKONG_IND;
                    data.HR_LULUS_IND = "T";
                    //data.HR_LULUS_YDP_IND = mCutiDetail.HR_LULUS_YDP_IND;

                    if(Kategori == "1" || (Kategori == "4" && mCuti.HR_KOD_CUTI == KodCuti))
                    {
                        if(mCutiDetail.HR_SOKONG_IND == "Y" && mCutiDetail.HR_LULUS_IND == "Y")
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

                foreach(GE_PARAMTABLE t in sTaraf)
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
                
            if(model.ACTIVE_ITEM == "Pekerjaan")
            {
                string jabatan = null;
                if(db2.GE_JABATAN.Where(s => s.GE_KOD_JABATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).Count() > 0)
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
                if(db.HR_GELARAN_JAWATAN.Where(s => s.HR_KOD_GELARAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J).Count() > 0)
                {
                    gelaran = db.HR_GELARAN_JAWATAN.FirstOrDefault(s => s.HR_KOD_GELARAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_GELARAN_J).HR_PENERANGAN;
                }

                string jawatan = null;
                if (db.HR_JAWATAN.Where(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN).Count() > 0)
                {
                    jawatan = db.HR_JAWATAN.FirstOrDefault(s => s.HR_KOD_JAWATAN == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN).HR_NAMA_JAWATAN;
                }

                string gred = null;
                if(db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED)).Count() > 0)
                {
                    gred = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 109 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GRED)).SHORT_DESCRIPTION;
                }

                string kumpulanperkhidmatan = null;
                if(db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 103 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN)).Count() > 0)
                {
                    kumpulanperkhidmatan = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 103 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KUMP_PERKHIDMATAN)).SHORT_DESCRIPTION;
                }

                string tarafjawatan = null;
                if(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN).Count() > 0)
                {
                    tarafjawatan = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 104 && s.STRING_PARAM == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TARAF_JAWATAN).SHORT_DESCRIPTION;
                }

                string gelaran2 = null;
                if(db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 114 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN)).Count() > 0)
                {
                    gelaran2 = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 114 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_GELARAN)).SHORT_DESCRIPTION;
                }

                string kategori = null;
                if(db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 115 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI)).Count() > 0)
                {
                    kategori = db2.GE_PARAMTABLE.AsEnumerable().FirstOrDefault(s => s.GROUPID == 115 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KATEGORI)).SHORT_DESCRIPTION;
                }

                string bank = null;
                if(db.HR_AGENSI.Where(s => s.HR_KOD_AGENSI == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK).Count() > 0)
                {
                    bank = db.HR_AGENSI.FirstOrDefault(s => s.HR_KOD_AGENSI == peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KOD_BANK).HR_NAMA_AGENSI;
                }

                string taraf = null;
                if(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "Y")
                {
                    taraf = "KAKITANGAN";
                }
                else if (peribadi.HR_MAKLUMAT_PEKERJAAN.HR_KAKITANGAN_IND == "T")
                {
                    taraf = "PEKERJA";
                }

                string tingkatan = null;
                if(db2.GE_PARAMTABLE.AsEnumerable().Where(s => s.GROUPID == 113 && s.ORDINAL == Convert.ToInt32(peribadi.HR_MAKLUMAT_PEKERJAAN.HR_TINGKATAN)).Count() > 0)
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
            if(HR_SISTEM != null)
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
            if(jawatan == null)
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
            if (CheckDate(HR_TARIKH_LAHIR)){
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
            if(item == null)
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
            foreach(var data in item)
            {
                bil = bil + Convert.ToInt32(data.HR_BIL_CUTI_TEMP);
            }
            return Json(bil, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BilCutiTahunLepas(string no_pekerja, string kod_cuti, int? tahun)
        {

            HR_MAKLUMAT_PEKERJAAN pekerja = db.HR_MAKLUMAT_PEKERJAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == no_pekerja);
            int bil = 0;

            if (pekerja != null && pekerja.HR_TARIKH_MASUK.Value.Year < tahun )
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

            if(item.HR_JUMLAH_CUTI == null)
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
            if(tMula == "")
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
                if(auto == "Y")
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
