using eSPP.Models;
using System;
using System.Collections.Generic;
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