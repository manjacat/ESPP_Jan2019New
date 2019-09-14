using eSPP.Models;
using eSPP.Models.RoleManagement;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace eSPP.Controllers
{
    public class LantikanJawatanBaruController : Controller
    {
		ApplicationDbContext db = new ApplicationDbContext();
        private MajlisContext db2 = new MajlisContext();


        public ActionResult LantikanJawatanBaru(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
			  message == ManageMessageId.Error ? "Maklumat calon tidak wujud"
			  : message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			  : message == ManageMessageId.Kemaskini ? "Profil Anda Telah Berjaya Dikemaskini."
			  : message == ManageMessageId.Exist ? "Maklumat Calon Telah Wujud."
			  : "";

			var SelectLastID = db.HR_MAKLUMAT_PERIBADI.OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault().HR_NO_PEKERJA;
			var LastID = new string(SelectLastID.SkipWhile(x => x == '0').ToArray());
			var Increment = Convert.ToSingle(LastID) + 1;
			var KodElaun = Convert.ToString(Increment).PadLeft(5, '0');
			ViewBag.HR_NO_PEKERJA = KodElaun;

			return View();
		}

		[HttpPost]
		public ActionResult LantikanJawatanBaru(IEnumerable<HR_MAKLUMAT_KELAYAKAN> kelayakan, HR_MAKLUMAT_PERIBADI peribadi, string Command, string HR_NO_PEKERJA, string HR_NO_KPBARU)
		{
			ApplicationDbContext db = new ApplicationDbContext();
			MajlisContext mc = new MajlisContext();
			HR_MAKLUMAT_PERIBADI mperibadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == HR_NO_KPBARU).SingleOrDefault();

			if (Command == "kemaskini")
			{
				if (mperibadi == null)
				{
					foreach (var item in kelayakan)
					{
						item.HR_NO_PEKERJA = HR_NO_PEKERJA;
					}
					string[] alamat = peribadi.HR_TALAMAT1.Split(',');
					peribadi.HR_TALAMAT1 = alamat[0].ToString();
					peribadi.HR_TALAMAT2 = alamat[1].ToString();
					peribadi.HR_TALAMAT3 = alamat[2].ToString();
					db.HR_MAKLUMAT_PERIBADI.Add(peribadi);
					db.HR_MAKLUMAT_KELAYAKAN.AddRange(kelayakan);
					db.SaveChanges();

					return RedirectToAction("Index", "MaklumatKakitangan", new { key = "4", value = HR_NO_PEKERJA });
				}
				if (mperibadi != null)
				{
					return RedirectToAction("LantikanJawatanBaru", "LantikanJawatanBaru", new { Message = ManageMessageId.Exist });
				}
			}
			return View();
		}

		public string ComputeHash(string input, HashAlgorithm algorithm)
		{
			Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

			Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}

		public ActionResult Search(string value)
		{
			var peribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == value).SingleOrDefault();

			string secret = "mbpjperjawatan2018" + value;
			string hPassword = ComputeHash(secret, new MD5CryptoServiceProvider());

			ViewBag.ic = value;
			ViewBag.md5 = hPassword;
			ViewBag.link = "http://myjob.mbpj.gov.my/api/resume/" + value + "/" + hPassword;

			return Json("http://myjob.mbpj.gov.my/api/resume/" + value + "/" + hPassword, JsonRequestBehavior.AllowGet);
		}

        public ActionResult LantikanJawatanBaruManual()
        {
            //Add this ViewBag to get RoleManager
            //tak tau macam mana nk dpt roleId ?? dapat dari session/cookie?

            string roleId = "4a205be5-cadc-446c-87c2-86c7ec5d6559";
            HttpCookie cookie = HttpContext.Request.Cookies.Get("RoleCookie");
            if (cookie != null)
            {
                roleId = cookie.Value;
            }
            ViewBag.RoleManager = RoleManager.GetByRoleId(roleId, ModuleConstant.MaklumatKakiTangan);

            ViewBag.photo = "";
            MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
            mKakitangan.HR_SENARAI_PERIBADI = new List<MaklumatPeribadi>();
            mKakitangan.HR_SENARAI_PERIBADI.Add(new MaklumatPeribadi());
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
            mKakitangan.HR_MAKLUMAT_PEWARIS = new List<MaklumatPewaris>() { new MaklumatPewaris() };
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

            //STRAT PERIBADI
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.Agama = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 106).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_WARGANEGARA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 2).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.TempatLahir = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.LONG_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION", mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TEMPAT_LAHIR);
            ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.TarafKahwin = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_ALASAN", "HR_PENERANGAN");
            ViewBag.Keturunan = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 1).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            //END PERIBADI

            //START PEKERJAAN
            ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA), "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_GAJI_PRORATA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 116).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN.OrderBy(s => s.GE_KETERANGAN_JABATAN), "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.HR_UNIT = new SelectList(db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_UNIT", "GE_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 115).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUMP_PERKHIDMATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 103).OrderBy(s => s.LONG_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TARAF_JAWATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
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
            ViewBag.HR_UMUR_SARA = HR_UMUR_SARA;

            ViewBag.HR_MATRIKS_GAJI = new SelectList(db.HR_MAKLUMAT_PEKERJAAN.OrderBy(s => s.HR_MATRIKS_GAJI), "HR_MATRIKS_GAJI", "HR_MATRIKS_GAJI");
            ViewBag.HR_KOD_GELARAN_J = new SelectList(db.HR_GELARAN_JAWATAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_GELARAN", "HR_PENERANGAN");
            ViewBag.HR_TINGKATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 113).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");

            ViewBag.HR_JUM_TAHUN = null;

            //END PEKERJAAN

            //START KEMAHIRAN
            ViewBag.HR_BAHASA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 107).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.P_TAHAP_KEMAHIRAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 105).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            //END KEMAHIRAN

            //START AKADEMIK
            ViewBag.HR_KURSUS = db.HR_KURSUS.OrderBy(s => s.HR_NAMA_KURSUS);
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
            ViewBag.HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            //END PEWARIS

            //TANGGUNGAN
            ViewBag.HR_TEMPAT_LAHIR = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.SHORT_DESCRIPTION), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");

            List<SelectListItem> HR_JANTINA = new List<SelectListItem>();
            HR_JANTINA.Add(new SelectListItem { Text = "Lelaki", Value = "L" });
            HR_JANTINA.Add(new SelectListItem { Text = "Perempuan", Value = "P" });
            ViewBag.HR_JANTINA = HR_JANTINA;
            //

            //KUARTERS
            ViewBag.HR_MAKLUMAT_KUARTERS_HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUARTERS = new SelectList(db.HR_KUARTERS.OrderBy(s => s.HR_BLOK_KUARTERS), "HR_KOD_KUARTERS", "HR_BLOK_KUARTERS");
            //

            //ELAUN POTONGAN
            var jawatan = db.HR_JAWATAN.SingleOrDefault(s => s.HR_KOD_JAWATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN);
            if (jawatan == null)
            {
                jawatan = new HR_JAWATAN();
            }
            ViewBag.jawatan = null;
            if (jawatan.HR_NAMA_JAWATAN != null)
            {
                ViewBag.jawatan = jawatan.HR_NAMA_JAWATAN;
            }

            ViewBag.KumpulanGred = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_KUMPULAN + " / ";

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
            ViewBag.Peringkat = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).GroupBy(s => s.SHORT_DESCRIPTION).Select(s => s.FirstOrDefault()).OrderBy(s => s.SHORT_DESCRIPTION), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            ViewBag.HR_KURNIAAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).OrderBy(s => s.LONG_DESCRIPTION), "STRING_PARAM", "LONG_DESCRIPTION");
            ViewBag.Kurniaan = db.HR_KURNIAAN.OrderBy(s => s.HR_PENERANGAN);
            ViewBag.HR_NP_PENCALON = db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA).ToList<HR_MAKLUMAT_PERIBADI>();
            //

            //GAJI, ELAUN, CARUMAN, ANUGERAH
            ViewBag.HR_MOD_BAYARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 117).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_AKTIF_IND_PEKERJAAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
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

            ViewBag.HR_TINDAKAN = new SelectList(db.HR_TINDAKAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_TINDAKAN", "HR_PENERANGAN");
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

            ViewBag.HR_KATEGORI_CUTI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 142).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_CUTI = new SelectList(db.HR_CUTI.Select(s => new { HR_KOD_CUTI = s.HR_KOD_CUTI, HR_KATEGORI = s.HR_KATEGORI.Trim(), HR_KETERANGAN = s.HR_KETERANGAN }).Where(s => s.HR_KATEGORI == "1").OrderBy(s => s.HR_KETERANGAN), "HR_KOD_CUTI", "HR_KETERANGAN");
            var stc = "Ya";
            stc = stc.PadRight(5, ' ');
            var tc = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() }).OrderBy(s => s.HR_SENARAI_TARIKH);
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhCuti = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == stc && s.HR_LULUS_IND == "Y").OrderBy(s => s.HR_SENARAI_TARIKH).ToList();
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
            var tb = db.HR_SENARAI_TARIKH_CUTI.AsEnumerable().Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc).Select(s => new { HR_SENARAI_TARIKH = s.HR_SENARAI_TARIKH.ToShortDateString() }).OrderBy(s => s.HR_SENARAI_TARIKH);
            //pegang tarikh yg lulus
            List<HR_SENARAI_TARIKH_CUTI> mTarikhBatal = db.HR_SENARAI_TARIKH_CUTI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_CUTI.HR_NO_PEKERJA && s.HR_KOD_CUTI == mKakitangan.HR_MAKLUMAT_CUTI.HR_KOD_CUTI && s.HR_TARIKH_PERMOHONAN == mKakitangan.HR_MAKLUMAT_CUTI.HR_TARIKH_PERMOHONAN && s.HR_STATUS_TARIKH_CUTI == sbc && s.HR_LULUS_IND == null).OrderBy(s => s.HR_SENARAI_TARIKH).ToList();
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
            ViewBag.HR_MAKLUMAT_CUTI_HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125 && s.STRING_PARAM == "K" && s.ORDINAL != 17 && s.ORDINAL != 18).OrderBy(s => s.SHORT_DESCRIPTION).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
                

            return View(mKakitangan);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LantikanJawatanBaruManual(MaklumatKakitanganModels mKakitangan, List<string> kemaskini, HttpPostedFileBase file, IEnumerable<HR_MAKLUMAT_PEKERJAAN_HISTORY> HR_MAKLUMAT_HISTORY)
        {
            HR_MAKLUMAT_PERIBADI mPeribadi = db.HR_MAKLUMAT_PERIBADI.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            if (mPeribadi == null)
            {
                int incrementID = 0;
                string lastID = db.HR_MAKLUMAT_PERIBADI.OrderByDescending(s => s.HR_NO_PEKERJA).FirstOrDefault().HR_NO_PEKERJA;
                if (lastID != null)
                {
                    incrementID = Convert.ToInt32(lastID);
                }
                incrementID++;
                mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA = Convert.ToString(incrementID).PadLeft(5, '0');
            }
            HR_GAMBAR_PENGGUNA gambar = db.HR_GAMBAR_PENGGUNA.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_MAKLUMAT_PEKERJAAN mPekerjaan = db.HR_MAKLUMAT_PEKERJAAN.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            List<HR_MAKLUMAT_PENGALAMAN_KERJA> pKerja = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
            List<HR_MAKLUMAT_PEWARIS> Pewaris = db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_PEWARIS>();
            HR_MAKLUMAT_KUARTERS mKuarters = db.HR_MAKLUMAT_KUARTERS.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_ANUGERAH_HAJI mAnugerahHaji = db.HR_ANUGERAH_HAJI.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_KUARTERS mKuarters2 = db.HR_KUARTERS.SingleOrDefault(s => s.HR_KOD_KUARTERS == mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS);
            //List<HR_MAKLUMAT_TANGGUNGAN> mTanggungan = db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_NO_PEKERJA).ToList<HR_MAKLUMAT_TANGGUNGAN>();
            HR_PERSARAAN mPersaraan = db.HR_PERSARAAN.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_MAKLUMAT_KEMATIAN mKematian = db.HR_MAKLUMAT_KEMATIAN.Find(mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA);
            HR_PENILAIAN_PRESTASI mPrestasi = db.HR_PENILAIAN_PRESTASI.SingleOrDefault(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_TAHUN_PRESTASI == mKakitangan.HR_PENILAIAN_PRESTASI.HR_TAHUN_PRESTASI);

            if(mKakitangan.HR_SENARAI_PERIBADI == null)
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

            if(mKakitangan.HR_MAKLUMAT_KURNIAAN == null)
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

                if (mPeribadi == null)
                {
                    mPeribadi = new HR_MAKLUMAT_PERIBADI();
                    mPeribadi.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                    mPeribadi.HR_NO_KPBARU = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU;
                    mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA;
                    db.HR_MAKLUMAT_PERIBADI.Add(mPeribadi);
                }
                else
                {
                    mPeribadi.HR_NO_KPBARU = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU;
                    mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA;
                    db.Entry(mPeribadi).State = EntityState.Modified;
                }


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

                foreach (var Kemaskini in kemaskini)
                {
                    if (Kemaskini == "Peribadi")
                    {
                        //mPeribadi.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                        //mPeribadi.HR_NO_KPBARU = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_KPBARU;
                        //mPeribadi.HR_NAMA_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NAMA_PEKERJA;
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
                        mPeribadi.HR_TARIKH_KEYIN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_KEYIN;
                        mPeribadi.HR_NP_KEYIN = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_KEYIN;
                        mPeribadi.HR_TARIKH_UBAH = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TARIKH_UBAH;
                        mPeribadi.HR_NP_UBAH = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NP_UBAH;

                        db.Entry(mPeribadi).State = EntityState.Modified;
                        db.SaveChanges();

                        if (mPekerjaan == null)
                        {
                            mPekerjaan = new HR_MAKLUMAT_PEKERJAAN();
                            mPekerjaan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                            db.HR_MAKLUMAT_PEKERJAAN.Add(mPekerjaan);
                            db.SaveChanges();
                        }

                        mPekerjaan.HR_JABATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN;
                        mPekerjaan.HR_BAHAGIAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN;
                        mPekerjaan.HR_JAWATAN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JAWATAN;
                        mPekerjaan.HR_GRED = null;

                        GE_PARAMTABLE sGred = db2.GE_PARAMTABLE.FirstOrDefault(s => s.GROUPID == 109 && s.SHORT_DESCRIPTION == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_GRED);
                        if (sGred != null)
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

                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP != null)
                        {
                            mPekerjaan.HR_STATUS_KWSP = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_KWSP;
                        }

                        mPekerjaan.HR_STATUS_SOCSO = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_SOCSO;
                        mPekerjaan.HR_STATUS_PCB = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PCB;
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN != null)
                        {
                            mPekerjaan.HR_STATUS_PENCEN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_STATUS_PENCEN;
                        }
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
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KEYIN2 != null)
                        {
                            mPekerjaan.HR_TARIKH_KEYIN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_KEYIN2;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_KEYIN2 != null)
                        {
                            mPekerjaan.HR_NP_KEYIN = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_KEYIN2;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_UBAH2 != null)
                        {
                            mPekerjaan.HR_TARIKH_UBAH = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_UBAH2;
                        }
                        if (mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_UBAH2 != null)
                        {
                            mPekerjaan.HR_NP_UBAH = mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_NP_UBAH2;
                        }
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
                        if (mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA != null)
                        {
                            db.HR_MAKLUMAT_PENGALAMAN_KERJA.RemoveRange(db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_NAMA_SYARIKAT != "MAJLIS PERBANDARAN PETALING JAYA"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PENGALAMAN_KERJA)
                            {
                                if (item.HR_TARIKH_MULA.HasValue)
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
                        if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA != null)
                        {
                            db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.RemoveRange(db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KEMAHIRAN_BAHASA)
                            {
                                if (item.HR_BAHASA != null)
                                {
                                    HR_MAKLUMAT_KEMAHIRAN_BAHASA mKemahiranBahasa = new HR_MAKLUMAT_KEMAHIRAN_BAHASA();
                                    mKemahiranBahasa.HR_BAHASA = item.HR_BAHASA;
                                    mKemahiranBahasa.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mKemahiranBahasa.HR_PEMBACAAN = item.HR_PEMBACAAN;
                                    mKemahiranBahasa.HR_PENULISAN = item.HR_PENULISAN;
                                    mKemahiranBahasa.HR_PERTUTURAN = item.HR_PERTUTURAN;

                                    db.HR_MAKLUMAT_KEMAHIRAN_BAHASA.Add(mKemahiranBahasa);
                                    db.SaveChanges();
                                }

                            }
                        }
                    }

                    if (Kemaskini == "Kemahiran")
                    {
                        if (mKakitangan.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL != null)
                        {
                            db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.RemoveRange(db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
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

                                mKemahiranTeknikal.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;

                                mKemahiranTeknikal.HR_SEQ_NO = Convert.ToInt16(digit);
                                mKemahiranTeknikal.HR_KEMAHIRAN = item.HR_KEMAHIRAN;
                                mKemahiranTeknikal.HR_TAHAP = item.HR_TAHAP;

                                db.HR_MAKLUMAT_KEMAHIRAN_TEKNIKAL.Add(mKemahiranTeknikal);
                                db.SaveChanges();
                            }
                        }

                    }

                    if (Kemaskini == "Akademik")
                    {
                        mPeribadi.HR_TAHUN_SPM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_TAHUN_SPM;
                        mPeribadi.HR_GRED_BM = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_GRED_BM;
                        db.Entry(mPekerjaan).State = EntityState.Modified;
                        db.SaveChanges();

                        if (mKakitangan.HR_MAKLUMAT_KELAYAKAN != null)
                        {
                            db.HR_MAKLUMAT_KELAYAKAN.RemoveRange(db.HR_MAKLUMAT_KELAYAKAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
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
                                mKelayakan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                mKelayakan.HR_SEQ_NO = Convert.ToInt16(digit);
                                mKelayakan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                mKelayakan.HR_TAHUN_MULA = item.HR_TAHUN_MULA;
                                mKelayakan.HR_TAHUN_TAMAT = item.HR_TAHUN_TAMAT;
                                db.HR_MAKLUMAT_KELAYAKAN.Add(mKelayakan);
                                db.SaveChanges();
                            }
                        }
                        if (mKakitangan.HR_MAKLUMAT_SIJIL != null)
                        {
                            db.HR_MAKLUMAT_SIJIL.RemoveRange(db.HR_MAKLUMAT_SIJIL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_SIJIL)
                            {
                                if (item.HR_TARIKH_DIPEROLEHI.HasValue)
                                {
                                    HR_MAKLUMAT_SIJIL mSijil = new HR_MAKLUMAT_SIJIL();
                                    mSijil.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mSijil.HR_TARIKH_DIPEROLEHI = Convert.ToDateTime(item.HR_TARIKH_DIPEROLEHI);
                                    mSijil.HR_NAMA_SIJIL_PEPERIKSAAN = item.HR_NAMA_SIJIL_PEPERIKSAAN;
                                    mSijil.HR_ANJURAN = item.HR_ANJURAN;
                                    mSijil.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                    db.HR_MAKLUMAT_SIJIL.Add(mSijil);
                                    db.SaveChanges();
                                }

                            }
                        }

                        if (mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN != null)
                        {
                            db.HR_MAKLUMAT_KURSUS_LATIHAN.RemoveRange(db.HR_MAKLUMAT_KURSUS_LATIHAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN)
                            {
                                if (item.HR_KOD_KURSUS != null)
                                {
                                    HR_MAKLUMAT_KURSUS_LATIHAN mKursusLatihan = new HR_MAKLUMAT_KURSUS_LATIHAN();
                                    mKursusLatihan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_AKTIVITI != null)
                        {
                            db.HR_MAKLUMAT_AKTIVITI.RemoveRange(db.HR_MAKLUMAT_AKTIVITI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));

                            foreach (var item in mKakitangan.HR_MAKLUMAT_AKTIVITI)
                            {
                                if (item.HR_TARIKH_AKTIVITI.HasValue)
                                {
                                    HR_MAKLUMAT_AKTIVITI mAktiviti = new HR_MAKLUMAT_AKTIVITI();
                                    mAktiviti.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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

                    if (Kemaskini == "Akademik2")
                    {
                        if (mKakitangan.HR_MAKLUMAT_KELAYAKAN != null)
                        {
                            db.HR_MAKLUMAT_KELAYAKAN.RemoveRange(db.HR_MAKLUMAT_KELAYAKAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
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
                                mKelayakan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                mKelayakan.HR_SEQ_NO = Convert.ToInt16(digit);
                                mKelayakan.HR_KEPUTUSAN = item.HR_KEPUTUSAN;
                                mKelayakan.HR_TAHUN_MULA = item.HR_TAHUN_MULA;
                                mKelayakan.HR_TAHUN_TAMAT = item.HR_TAHUN_TAMAT;
                                db.HR_MAKLUMAT_KELAYAKAN.Add(mKelayakan);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (Kemaskini == "Akademik3")
                    {
                        if (mKakitangan.HR_MAKLUMAT_SIJIL != null)
                        {
                            db.HR_MAKLUMAT_SIJIL.RemoveRange(db.HR_MAKLUMAT_SIJIL.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_SIJIL)
                            {
                                if (item.HR_TARIKH_DIPEROLEHI.HasValue)
                                {
                                    HR_MAKLUMAT_SIJIL mSijil = new HR_MAKLUMAT_SIJIL();
                                    mSijil.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN != null)
                        {
                            db.HR_MAKLUMAT_KURSUS_LATIHAN.RemoveRange(db.HR_MAKLUMAT_KURSUS_LATIHAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KURSUS_LATIHAN)
                            {
                                if (item.HR_KOD_KURSUS != null)
                                {
                                    HR_MAKLUMAT_KURSUS_LATIHAN mKursusLatihan = new HR_MAKLUMAT_KURSUS_LATIHAN();
                                    mKursusLatihan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_AKTIVITI != null)
                        {
                            db.HR_MAKLUMAT_AKTIVITI.RemoveRange(db.HR_MAKLUMAT_AKTIVITI.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));

                            foreach (var item in mKakitangan.HR_MAKLUMAT_AKTIVITI)
                            {
                                if (item.HR_TARIKH_AKTIVITI.HasValue)
                                {
                                    HR_MAKLUMAT_AKTIVITI mAktiviti = new HR_MAKLUMAT_AKTIVITI();
                                    mAktiviti.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_PEWARIS != null)
                        {
                            db.HR_MAKLUMAT_PEWARIS.RemoveRange(db.HR_MAKLUMAT_PEWARIS.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            var no = 1;
                            foreach (var item in mKakitangan.HR_MAKLUMAT_PEWARIS)
                            {
                                if (item.HR_NO_KP != null)
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
                        if (mKakitangan.HR_MAKLUMAT_TANGGUNGAN != null)
                        {
                            db.HR_MAKLUMAT_TANGGUNGAN.RemoveRange(db.HR_MAKLUMAT_TANGGUNGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));

                            foreach (var item in mKakitangan.HR_MAKLUMAT_TANGGUNGAN)
                            {
                                if (item.HR_NO_KP != null)
                                {
                                    HR_MAKLUMAT_TANGGUNGAN mTanggungan = new HR_MAKLUMAT_TANGGUNGAN();
                                    mTanggungan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KOD_KUARTERS != null)
                        {
                            if (mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_ALAMAT1 != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BLOK_KUARTERS != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_NEGERI != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_POSKOD != null && mKakitangan.HR_MAKLUMAT_KUARTERS.HR_KUARTERS.HR_BANDAR != null)
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
                                    mKuarters.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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

                                mKuarters.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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

                        }




                    }

                    if (Kemaskini == "Gaji")
                    {
                        //db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        //db.HR_SEJARAH_ELAUN_POTONGAN.RemoveRange(db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G != null)
                        {
                            db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "G"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_G)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganG = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganG.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        //db.HR_SEJARAH_ELAUN_POTONGAN.RemoveRange(db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E != null)
                        {
                            db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "E"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_E)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganE = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganE.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        //db.HR_SEJARAH_ELAUN_POTONGAN.RemoveRange(db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P != null)
                        {
                            db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "P"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_P)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganP = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganP.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        //db.HR_SEJARAH_ELAUN_POTONGAN.RemoveRange(db.HR_SEJARAH_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C != null)
                        {
                            db.HR_MAKLUMAT_ELAUN_POTONGAN.RemoveRange(db.HR_MAKLUMAT_ELAUN_POTONGAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA && s.HR_ELAUN_POTONGAN_IND == "C"));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_ELAUN_POTONGAN_C)
                            {
                                if (item.HR_KOD_ELAUN_POTONGAN != null)
                                {
                                    HR_MAKLUMAT_ELAUN_POTONGAN mElaunPotonganC = new HR_MAKLUMAT_ELAUN_POTONGAN();
                                    mElaunPotonganC.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_MAKLUMAT_KURNIAAN.Count() > 0)
                        {
                            db.HR_MAKLUMAT_KURNIAAN.RemoveRange(db.HR_MAKLUMAT_KURNIAAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            db.HR_PENCALONAN_KURNIAAN.RemoveRange(db.HR_PENCALONAN_KURNIAAN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_MAKLUMAT_KURNIAAN)
                            {
                                if (item.HR_STATUS == "Y")
                                {
                                    HR_MAKLUMAT_KURNIAAN mKurniaan = new HR_MAKLUMAT_KURNIAAN();
                                    mKurniaan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mKurniaan.HR_TARIKH_KURNIAAN = Convert.ToDateTime(item.HR_TARIKH_KURNIAAN);
                                    mKurniaan.HR_KOD_KURNIAAN = item.HR_KOD_KURNIAAN;
                                    mKurniaan.HR_KURNIAAN_IND = item.HR_KURNIAAN_IND;
                                    mKurniaan.HR_NEGERI = item.HR_NEGERI;
                                    mKurniaan.HR_STATUS = item.HR_STATUS;

                                    db.HR_MAKLUMAT_KURNIAAN.Add(mKurniaan);
                                    db.SaveChanges();
                                }

                                HR_PENCALONAN_KURNIAAN mPKurniaan = new HR_PENCALONAN_KURNIAAN();
                                mPKurniaan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        if (mKakitangan.HR_ANUGERAH_CEMERLANG.Count() > 0)
                        {
                            db.HR_ANUGERAH_CEMERLANG.RemoveRange(db.HR_ANUGERAH_CEMERLANG.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                            foreach (var item in mKakitangan.HR_ANUGERAH_CEMERLANG)
                            {
                                if (item.HR_NAMA_ANUGERAH != null)
                                {
                                    HR_ANUGERAH_CEMERLANG mAnugerahCemerlang = new HR_ANUGERAH_CEMERLANG();
                                    mAnugerahCemerlang.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                                    mAnugerahCemerlang.HR_NAMA_ANUGERAH = item.HR_NAMA_ANUGERAH;
                                    mAnugerahCemerlang.HR_TARIKH_PENERIMAAN = item.HR_TARIKH_PENERIMAAN;
                                    db.HR_ANUGERAH_CEMERLANG.Add(mAnugerahCemerlang);
                                    db.SaveChanges();
                                }

                            }
                        }
                    }

                    if (Kemaskini == "Anugerah3")
                    {
                        if (mAnugerahHaji == null)
                        {
                            mAnugerahHaji = new HR_ANUGERAH_HAJI();
                            mAnugerahHaji.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                            mAnugerahHaji.HR_TAHUN_PERGI = mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI;
                            mAnugerahHaji.HR_STATUS_HAJI = mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI;
                            mAnugerahHaji.HR_NP_YDP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP;
                            mAnugerahHaji.HR_LULUS_IND = mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND;
                            mAnugerahHaji.HR_NP_UP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP;
                            mAnugerahHaji.HR_NP_PEG = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG;
                            db.HR_ANUGERAH_HAJI.Add(mAnugerahHaji);
                            db.SaveChanges();
                        }
                        //mAnugerahHaji.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
                        mAnugerahHaji.HR_TAHUN_PERGI = mKakitangan.HR_ANUGERAH_HAJI.HR_TAHUN_PERGI;
                        mAnugerahHaji.HR_STATUS_HAJI = mKakitangan.HR_ANUGERAH_HAJI.HR_STATUS_HAJI;
                        mAnugerahHaji.HR_NP_YDP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_YDP;
                        mAnugerahHaji.HR_LULUS_IND = mKakitangan.HR_ANUGERAH_HAJI.HR_LULUS_IND;
                        mAnugerahHaji.HR_NP_UP = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_UP;
                        mAnugerahHaji.HR_NP_PEG = mKakitangan.HR_ANUGERAH_HAJI.HR_NP_PEG;
                        db.Entry(mAnugerahHaji).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (Kemaskini == "Persaraan")
                    {
                        if (mPersaraan == null)
                        {
                            mPersaraan = new HR_PERSARAAN();
                            mPersaraan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //mPersaraan.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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

                    if (Kemaskini == "Tatatertib")
                    {
                        db.HR_TINDAKAN_DISIPLIN.RemoveRange(db.HR_TINDAKAN_DISIPLIN.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA));
                        if (mKakitangan.HR_TINDAKAN_DISIPLIN.Count() > 0)
                        {
                            foreach (var item in mKakitangan.HR_TINDAKAN_DISIPLIN)
                            {
                                if (item.HR_TARIKH_KESALAHAN.HasValue)
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
                        if (mKematian == null)
                        {
                            mKematian = new HR_MAKLUMAT_KEMATIAN();
                            mKematian.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //mKematian.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                    if (Kemaskini == "Prestasi")
                    {
                        if (mPrestasi == null)
                        {
                            mPrestasi = new HR_PENILAIAN_PRESTASI();
                            mPrestasi.HR_NO_PEKERJA = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA;
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
                        //mPrestasi.HR_NO_PEKERJA = mPeribadi.HR_NO_PEKERJA;
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
                    if (Kemaskini == "Cuti")
                    {

                    }
                }


                return Redirect(Url.Action("Index", "MaklumatKakitangan", new { key = '4', value = mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA }) + "#" + mKakitangan.ACTIVE_ITEM);
            }


            if (mPekerjaan.HR_MATRIKS_GAJI != null)
            {
                var MatriksGaji = mPekerjaan.HR_MATRIKS_GAJI.Split('T');
                decimal tahap = Convert.ToDecimal(MatriksGaji[1]);
                var peringkat = Convert.ToInt32(MatriksGaji[0].Substring(1));

                ViewBag.HR_GAJI_MIN = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI).OrderBy(s => s.HR_GAJI_MIN), "HR_GAJI_MIN", "HR_GAJI_MIN");
                ViewBag.HR_GAJI_MAX = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_PERINGKAT == peringkat && s.HR_TAHAP == tahap && s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI).OrderBy(s => s.HR_GAJI_MAX), "HR_GAJI_MAX", "HR_GAJI_MAX");
            }
            else
            {
                ViewBag.HR_GAJI_MIN = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI).OrderBy(s => s.HR_GAJI_MIN), "HR_GAJI_MIN", "HR_GAJI_MIN");
                ViewBag.HR_GAJI_MAX = new SelectList(db.HR_MATRIKS_GAJI.Where(s => s.HR_KOD_GAJI == mPekerjaan.HR_KOD_GAJI).OrderBy(s => s.HR_GAJI_MAX), "HR_GAJI_MAX", "HR_GAJI_MAX");
            }

            //STRAT PERIBADI
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.Agama = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 106).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_WARGANEGARA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 2).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.TempatLahir = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.LONG_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.Negeri = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.LONG_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.TarafKahwin = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 4).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_ALASAN = new SelectList(db.HR_ALASAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_ALASAN", "HR_PENERANGAN");
            ViewBag.Keturunan = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 1).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            //END PERIBADI

            //START PEKERJAAN
            ViewBag.HR_NO_PENYELIA = new SelectList(db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA), "HR_NO_PEKERJA", "HR_NAMA_PEKERJA");
            ViewBag.HR_GELARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 114).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_GAJI_PRORATA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 116).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_JABATAN = new SelectList(db2.GE_JABATAN.OrderBy(s => s.GE_KETERANGAN_JABATAN), "GE_KOD_JABATAN", "GE_KETERANGAN_JABATAN");
            ViewBag.HR_BAHAGIAN = new SelectList(db2.GE_BAHAGIAN.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_BAHAGIAN", "GE_KETERANGAN");
            ViewBag.HR_UNIT = new SelectList(db2.GE_UNIT.Where(s => s.GE_KOD_JABATAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_JABATAN && s.GE_KOD_BAHAGIAN == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_BAHAGIAN).OrderBy(s => s.GE_KETERANGAN), "GE_KOD_UNIT", "GE_KETERANGAN");
            ViewBag.HR_KATEGORI = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 115).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUMP_PERKHIDMATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 103).OrderBy(s => s.LONG_DESCRIPTION), "ORDINAL", "LONG_DESCRIPTION");
            ViewBag.HR_TARAF_JAWATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 104).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
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
            ViewBag.HR_KUMPULAN = HR_KUMPULAN;

            ViewBag.HR_KOD_BANK = new SelectList(db.HR_AGENSI.Where(s => s.HR_PERBANKAN == "Y").OrderBy(s => s.HR_NAMA_AGENSI), "HR_KOD_AGENSI", "HR_NAMA_AGENSI");
            ViewBag.HR_GRED = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 109 && s.STRING_PARAM == mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_SISTEM).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            Nullable<int> HR_UMUR_SARA = null;
            if (mPekerjaan.HR_TARIKH_TAMAT != null && mPeribadi.HR_TARIKH_LAHIR != null)
            {
                HR_UMUR_SARA = (Convert.ToDateTime(mPekerjaan.HR_TARIKH_TAMAT).Year - Convert.ToDateTime(mPeribadi.HR_TARIKH_LAHIR).Year);
            }
            ViewBag.HR_UMUR_SARA = HR_UMUR_SARA;

            ViewBag.HR_MATRIKS_GAJI = new SelectList(db.HR_MAKLUMAT_PEKERJAAN.OrderBy(s => s.HR_MATRIKS_GAJI), "HR_MATRIKS_GAJI", "HR_MATRIKS_GAJI");
            ViewBag.HR_KOD_GELARAN_J = new SelectList(db.HR_GELARAN_JAWATAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_GELARAN", "HR_PENERANGAN");
            ViewBag.HR_TINGKATAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 113).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KOD_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 102).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.HR_KATEGORI_PCB = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 101).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");

            ViewBag.HR_JUM_TAHUN = null;
            if (mPekerjaan.HR_TARIKH_TAMAT_KONTRAK != null)
            {
                ViewBag.HR_JUM_TAHUN = Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_TAMAT_KONTRAK).Year - Convert.ToDateTime(mKakitangan.HR_MAKLUMAT_PEKERJAAN.HR_TARIKH_MASUK).Year;
            }

            List<HR_MAKLUMAT_PENGALAMAN_KERJA> sPengalaman = db.HR_MAKLUMAT_PENGALAMAN_KERJA.Where(s => s.HR_NO_PEKERJA == mKakitangan.HR_MAKLUMAT_PERIBADI.HR_NO_PEKERJA).OrderBy(s => s.HR_TARIKH_MULA).ThenBy(s => s.HR_TARIKH_TAMAT).ToList<HR_MAKLUMAT_PENGALAMAN_KERJA>();
            ViewBag.sPengalaman = sPengalaman;
            //END PEKERJAAN
            //START KEMAHIRAN
            ViewBag.HR_BAHASA = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 107).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            ViewBag.P_TAHAP_KEMAHIRAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 105).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            //END KEMAHIRAN

            //START AKADEMIK
            ViewBag.HR_KURSUS = db.HR_KURSUS.OrderBy(s => s.HR_NAMA_KURSUS);
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
            ViewBag.HR_HUBUNGAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 125).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            //END PEWARIS

            //TANGGUNGAN
            ViewBag.HR_TEMPAT_LAHIR = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 3).OrderBy(s => s.SHORT_DESCRIPTION), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            //

            //KUARTERS
            ViewBag.HR_AKTIF_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_KUARTERS = new SelectList(db.HR_KUARTERS.OrderBy(s => s.HR_BLOK_KUARTERS), "HR_KOD_KUARTERS", "HR_BLOK_KUARTERS");
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
            ViewBag.HR_MOD_BAYARAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 117).OrderBy(s => s.SHORT_DESCRIPTION), "ORDINAL", "SHORT_DESCRIPTION");
            ViewBag.HR_AKTIF_IND_PEKERJAAN = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 144 && (s.STRING_PARAM == "Y" || s.STRING_PARAM == "T")).OrderBy(s => s.SHORT_DESCRIPTION), "STRING_PARAM", "SHORT_DESCRIPTION");
            List<SelectListItem> HR_AUTO_IND = new List<SelectListItem>();
            HR_AUTO_IND.Add(new SelectListItem { Text = "YA", Value = "Y" });
            HR_AUTO_IND.Add(new SelectListItem { Text = "TIDAK", Value = "T" });
            ViewBag.HR_AUTO_IND = HR_AUTO_IND;
            //

            //ANUGERAH
            ViewBag.Peringkat = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).GroupBy(s => s.SHORT_DESCRIPTION).Select(s => s.FirstOrDefault()).OrderBy(s => s.SHORT_DESCRIPTION), "SHORT_DESCRIPTION", "SHORT_DESCRIPTION");
            ViewBag.HR_KURNIAAN_IND = new SelectList(db2.GE_PARAMTABLE.Where(s => s.GROUPID == 130).OrderBy(s => s.LONG_DESCRIPTION), "STRING_PARAM", "LONG_DESCRIPTION");
            ViewBag.Kurniaan = db.HR_KURNIAAN.OrderBy(s => s.HR_PENERANGAN);
            ViewBag.HR_NP_PENCALON = db.HR_MAKLUMAT_PERIBADI.OrderBy(s => s.HR_NAMA_PEKERJA).ToList<HR_MAKLUMAT_PERIBADI>();
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

            ViewBag.HR_TINDAKAN = new SelectList(db.HR_TINDAKAN.OrderBy(s => s.HR_PENERANGAN), "HR_KOD_TINDAKAN", "HR_PENERANGAN");

            return View(mKakitangan);
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
			Error,
			Exist
		}
	}
}