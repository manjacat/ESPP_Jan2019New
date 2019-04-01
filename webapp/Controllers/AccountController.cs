using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using eSPP.Models;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Cryptography;

namespace eSPP.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		private ApplicationDbContext db = new ApplicationDbContext();
		private static string psErrorMessage = string.Empty;
		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl, ManageMessageId? message)
		{
			ViewBag.StatusMessage =
			message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			: message == ManageMessageId.PasswordSalah ? "Katalaluan Tidak Betul."
			: message == ManageMessageId.UserNull ? "Sila Daftar ID Terlebih Dahulu."
			: message == ManageMessageId.Error ? "An error has occurred."
			: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
			: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
			: message == ManageMessageId.ResetPassword ? "Katalaluan Baru Telah Dihantar Ke Emel"
			: "";

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
		public ActionResult Lock()
		{
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(AccountLoginModel model, string returnUrl, ManageMessageId? message)
		{
			ViewBag.StatusMessage =
			 message == ManageMessageId.ResetPassword ? "Katalaluan Baru Telah Dihantar Ke Emel"
			: "";

			eSPP.Models.GroupFeature groupFeature = new eSPP.Models.GroupFeature();
			List<Guid> userGroupIDList = new List<Guid>();
			List<eSPP.Models.GroupFeaturesDisplay> groupFeatureDisplayList = new List<eSPP.Models.GroupFeaturesDisplay>();
			if (User.Identity.IsAuthenticated)
			{
				eSPP.Models.UserGroup userGroup = new eSPP.Models.UserGroup();
				userGroupIDList = userGroup.GetGroupByLoginUser();
				groupFeatureDisplayList = groupFeature.GetFeatureListByGroup(userGroupIDList);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			MajlisContext mc = new MajlisContext();
			ApplicationDbContext db = new ApplicationDbContext();
			var user = await UserManager.FindByNameAsync(model.UserName);
			UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(60);
			// This doesn't count login failures towards account lockout
			//new EmailSMTP().SendEmail();
			// To enable password failures to trigger account lockout, change to shouldLockout: true

			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
			//var attempt = UserManager.AccessFailed(user.Id);

			switch (result)
			{
				case SignInStatus.Success:
				var role1 = db.UserRoles.Where(d => d.UserId == user.Id).SingleOrDefault();
				var role = new IdentityRole();
				if (role1 != null)
				{
					role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
                    //khairil: temp store role dlm cookie sbb tak tau macam mana nak call role
                    WriteCookie(role1.RoleId);
				}

				new AuditTrailModels().Log(user.Email, user.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, user.UserName + " Telah Log Masuk Ke Dalam Sistem", System.Net.Dns.GetHostName(), user.PhoneNumber, Request.RawUrl, "Login");
				HttpContext.Session["UserLoginId"] = user.Id;
				await UserManager.ResetAccessFailedCountAsync(user.Id);
				if (model.Password == "P@ssw0rd123")
				{
					return RedirectToAction("ChangePassword", "Manage", new { getuserid = user.Id, Message = ManageMessageId.ChangePassword });
				}
				return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
				return View("Lock");
				case SignInStatus.RequiresVerification:
				return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
				ViewBag.error = "Alamat Emel Atau Kata Laluan Tidak Betul";
				if (user != null)
				{
					var attempt = user.AccessFailedCount;
					if (attempt == 4)
					{
						ViewBag.error1 = "Anda Mempunyai 1 Kali Percubaan";
					}
					if (attempt == 3)
					{
						ViewBag.error1 = "Anda Mempunyai 2 Kali Percubaan";
					}
					if (attempt == 2)
					{
						ViewBag.error1 = "Anda Mempunyai 3 Kali Percubaan";
					}
					if (attempt == 1)
					{
						ViewBag.error1 = "Anda Mempunyai 4 Kali Percubaan";
					}
				}
				return View();
			}
		}


        private void WriteCookie(string roleId)
        {
            HttpCookie RoleCookie = new HttpCookie("RoleCookie");
            RoleCookie.Value = roleId;
            RoleCookie.Expires = DateTime.Now.AddDays(1);
            Response.SetCookie(RoleCookie);
            Response.Flush();
        }

        public void RemoveCookie()
        {
            //remove cookie
            HttpCookie RoleCookie = new HttpCookie("RoleCookie");
            RoleCookie.Value = "";
            RoleCookie.Expires = DateTime.Now.AddDays(-1);
            try
            {
                Response.SetCookie(RoleCookie);
                Response.Flush();
            }
            catch
            {

            }
            
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return View("Error");
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		public ActionResult Admin()
		{
			return View();
		}

		public ActionResult AddAdmin()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult UserProfile(Guid? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			IdentityUser users = db.Users.Find(id.ToString());
			if (users == null)
			{
				return HttpNotFound();
			}

			HR_MAKLUMAT_PERIBADI namaPekerja = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == users.UserName);
			string nama = "";
			string noStaff = "";
			Nullable<System.DateTime> DOB = new Nullable<System.DateTime>();
			string Sex = "";
			string Religion = "";
			string Race = "";
			string MStatus = "";
			string Addresss = "";
			string Address2 = "";
			string Address3 = "";
			string PostCode = "";

			string City = "";
			string State = "";
			string MailAddress = "";
			string MailAddress2 = "";
			string MailAddress3 = "";
			string MailPostCode = "";

			string MailCity = "";
			string MailState = "";

			string License = "";
			string CarModel = "";
			string PlatNo = "";
			Nullable<decimal> CarCC = new Nullable<decimal>();
			if (namaPekerja != null)
			{
				nama = namaPekerja.HR_NAMA_PEKERJA;
				noStaff = namaPekerja.HR_NO_PEKERJA;

				DOB = namaPekerja.HR_TARIKH_LAHIR;
				Sex = namaPekerja.HR_JANTINA;
				Religion = namaPekerja.HR_AGAMA;
				Race = namaPekerja.HR_KETURUNAN;
				MStatus = namaPekerja.HR_TARAF_KAHWIN;
				Addresss = namaPekerja.HR_TALAMAT1;
				Address2 = namaPekerja.HR_TALAMAT2;
				Address3 = namaPekerja.HR_TALAMAT3;
				PostCode = namaPekerja.HR_TPOSKOD;

				City = namaPekerja.HR_TBANDAR;
				State = namaPekerja.HR_TNEGERI;
				MailAddress = namaPekerja.HR_SALAMAT1;
				MailAddress2 = namaPekerja.HR_SALAMAT2;
				MailAddress3 = namaPekerja.HR_SALAMAT3;
				MailPostCode = namaPekerja.HR_SPOSKOD;

				MailCity = namaPekerja.HR_SBANDAR;
				MailState = namaPekerja.HR_SNEGERI;

				License = namaPekerja.HR_LESEN;
				CarModel = namaPekerja.HR_JENIS_KENDERAAN;
				PlatNo = namaPekerja.HR_NO_KENDERAAN;
				CarCC = namaPekerja.HR_CC_KENDERAAN;
			}
			AccountViewRegistrationModel model = new AccountViewRegistrationModel();
			model.Username = nama;
			model.Email = users.Email;
			model.NoKP = users.UserName;
			model.PhoneNo = users.PhoneNumber;
			model.NoStaff = noStaff;
			model.UserID = users.Id;

			model.DOB = DOB;
			model.Sex = Sex;
			model.Religion = Religion;
			model.Race = Race;
			model.MStatus = MStatus;
			model.Addresss = Addresss;
			model.Address2 = Address2;
			model.Address3 = Address3;
			model.PostCode = PostCode;
			model.Address2 = Address2;
			model.City = City;
			model.State = State;
			model.MailAddress = MailAddress;
			model.MailAddress2 = MailAddress2;
			model.MailAddress3 = MailAddress3;
			model.MailPostCode = MailPostCode;
			model.MailAddress2 = MailAddress2;
			model.MailCity = MailCity;
			model.MailState = MailState;

			model.License = License;
			model.CarModel = CarModel;
			model.PlatNo = PlatNo;
			model.CarCC = CarCC;


			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "Lelaki", Value = "Lelaki" });
			items.Add(new SelectListItem { Text = "Perempuan", Value = "Perempuan" });
			ViewBag.jantina = items;

			List<SelectListItem> items1 = new List<SelectListItem>();
			items1.Add(new SelectListItem { Text = "Islam", Value = "Islam" });
			items1.Add(new SelectListItem { Text = "Buddha", Value = "Buddha" });
			items1.Add(new SelectListItem { Text = "Hindu", Value = "Hindu" });
			items1.Add(new SelectListItem { Text = "Kristian", Value = "Kristian" });
			ViewBag.religion = items1;

			List<SelectListItem> items2 = new List<SelectListItem>();
			items2.Add(new SelectListItem { Text = "Melayu", Value = "Melayu" });
			items2.Add(new SelectListItem { Text = "Cina", Value = "Cina" });
			items2.Add(new SelectListItem { Text = "India", Value = "India" });
			items2.Add(new SelectListItem { Text = "Lain-Lain", Value = "Lain-Lain" });
			ViewBag.race = items2;

			List<SelectListItem> items3 = new List<SelectListItem>();
			items3.Add(new SelectListItem { Text = "Bujang", Value = "Bujang" });
			items3.Add(new SelectListItem { Text = "Berkahwin", Value = "Berkahwin" });
			items3.Add(new SelectListItem { Text = "Duda", Value = "Duda" });
			items3.Add(new SelectListItem { Text = "Janda", Value = "Janda" });
			ViewBag.mstatus = items3;

			List<SelectListItem> items4 = new List<SelectListItem>();
			items4.Add(new SelectListItem { Text = "Ada", Value = "Ada" });
			items4.Add(new SelectListItem { Text = "Tiada", Value = "Tiada" });
			ViewBag.license = items4;

			List<SelectListItem> stateList = new List<SelectListItem>();
			stateList.Add(new SelectListItem { Text = "W.P. Kuala Lumpur", Value = "W.P. Kuala Lumpur" });
			stateList.Add(new SelectListItem { Text = "W.P. Labuan", Value = "W.P. Labuan" });
			stateList.Add(new SelectListItem { Text = "W.P. Putrajaya", Value = "W.P. Putrajaya" });
			stateList.Add(new SelectListItem { Text = "Johor", Value = "01" });
			stateList.Add(new SelectListItem { Text = "Kedah", Value = "Kedah" });
			stateList.Add(new SelectListItem { Text = "Kelantan", Value = "Kelantan" });
			stateList.Add(new SelectListItem { Text = "Melaka", Value = "Melaka" });
			stateList.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "Negeri Sembilan" });
			stateList.Add(new SelectListItem { Text = "Pahang", Value = "Pahang" });
			stateList.Add(new SelectListItem { Text = "Perak", Value = "Perak" });
			stateList.Add(new SelectListItem { Text = "Perlis", Value = "Perlis" });
			stateList.Add(new SelectListItem { Text = "Pulau Pinang", Value = "Pulau Pinang" });
			stateList.Add(new SelectListItem { Text = "Sabah", Value = "Sabah" });
			stateList.Add(new SelectListItem { Text = "Serawak", Value = "Serawak" });
			stateList.Add(new SelectListItem { Text = "Selangor", Value = "Selangor" });
			stateList.Add(new SelectListItem { Text = "Terengganu", Value = " Terengganu" });
			ViewBag.stateList = stateList;

			return View(model);

		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserProfile(AccountViewRegistrationModel model)
		{
			if (!ModelState.IsValid)
			{
				if (model.Username == null) { psErrorMessage = "Please enter User Name"; return RedirectToAction("ViewRegister", "Admin"); }
				if (model.Email == null) { psErrorMessage = "Please enter Email"; return RedirectToAction("ViewRegister", "Admin"); }
				//if (model.PhoneNo == null) { psErrorMessage = "Please enter Phone Number"; return RedirectToAction("ViewRegister", "Admin"); }
				//if (model.Password == null) { psErrorMessage = "Please enter Password"; return RedirectToAction("ViewRegister", "Admin"); }
				if (db.Users.SingleOrDefault(i => i.Email == model.Email) != null) { psErrorMessage = "Email already Exist"; return RedirectToAction("ViewRegister", "Admin"); }
			}
			ApplicationUser user = UserManager.FindById(model.UserID);
			user.Email = model.Email;
			user.UserName = model.NoKP;
			user.PhoneNumber = model.PhoneNo;
			var result = await UserManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				IdentityUser users2 = db.Users.SingleOrDefault(s => s.Id == model.UserID);
				HR_MAKLUMAT_PERIBADI Peribadi2 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == users2.UserName);
				if (Peribadi2 != null)
				{

					List<HR_MAKLUMAT_PERIBADI> Peribadi3 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA && s.HR_NO_KPBARU != users2.UserName).ToList();
					if (Peribadi3.Count() <= 0)
					{
						HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA);

						Peribadi.HR_NO_PEKERJA = model.NoStaff;
						Peribadi.HR_NAMA_PEKERJA = model.Username;
						Peribadi.HR_NO_KPBARU = model.NoKP;
						Peribadi.HR_EMAIL = model.Email;

						Peribadi.HR_TARIKH_LAHIR = model.DOB;
						Peribadi.HR_JANTINA = model.Sex;
						Peribadi.HR_AGAMA = model.Religion;
						Peribadi.HR_KETURUNAN = model.Race;
						Peribadi.HR_TARAF_KAHWIN = model.MStatus;
						Peribadi.HR_TALAMAT1 = model.Addresss;
						Peribadi.HR_TALAMAT2 = model.Address2;
						Peribadi.HR_TALAMAT3 = model.Address3;
						Peribadi.HR_TPOSKOD = model.PostCode;
						Peribadi.HR_TALAMAT2 = model.Address2;
						Peribadi.HR_TBANDAR = model.City;
						Peribadi.HR_TNEGERI = model.State;
						Peribadi.HR_SALAMAT1 = model.MailAddress;
						Peribadi.HR_SALAMAT2 = model.MailAddress2;
						Peribadi.HR_SALAMAT3 = model.MailAddress3;
						Peribadi.HR_SPOSKOD = model.MailPostCode;
						Peribadi.HR_SALAMAT2 = model.MailAddress2;
						Peribadi.HR_SBANDAR = model.MailCity;
						Peribadi.HR_SNEGERI = model.MailState;

						Peribadi.HR_LESEN = model.License;
						Peribadi.HR_JENIS_KENDERAAN = model.CarModel;
						Peribadi.HR_NO_KENDERAAN = model.PlatNo;
						Peribadi.HR_CC_KENDERAAN = model.CarCC;
						db.Entry(Peribadi).State = EntityState.Modified;
						db.SaveChanges();
						return RedirectToAction("Index", "Home");


					}
					else
					{
						psErrorMessage = "Maaf!! pengguna telah Wujud. Sila isi no KP yang lain";
						ViewBag.error = psErrorMessage;
						return View(model);
					}
				}
				else
				{
					List<HR_MAKLUMAT_PERIBADI> Peribadi3 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == users2.UserName).ToList();
					if (Peribadi3.Count() <= 0)
					{
						HR_MAKLUMAT_PERIBADI Peribadi = new HR_MAKLUMAT_PERIBADI();
						Peribadi.HR_NO_PEKERJA = model.NoStaff;
						Peribadi.HR_NAMA_PEKERJA = model.Username;
						Peribadi.HR_NO_KPBARU = model.NoKP;
						Peribadi.HR_EMAIL = model.Email;

						Peribadi.HR_TARIKH_LAHIR = model.DOB;
						Peribadi.HR_JANTINA = model.Sex;
						Peribadi.HR_AGAMA = model.Religion;
						Peribadi.HR_KETURUNAN = model.Race;
						Peribadi.HR_TARAF_KAHWIN = model.MStatus;
						Peribadi.HR_TALAMAT1 = model.Addresss;
						Peribadi.HR_TALAMAT2 = model.Address2;
						Peribadi.HR_TALAMAT3 = model.Address3;
						Peribadi.HR_TPOSKOD = model.PostCode;
						Peribadi.HR_TALAMAT2 = model.Address2;
						Peribadi.HR_TBANDAR = model.City;
						Peribadi.HR_TNEGERI = model.State;
						Peribadi.HR_SALAMAT1 = model.MailAddress;
						Peribadi.HR_SALAMAT2 = model.MailAddress2;
						Peribadi.HR_SALAMAT3 = model.MailAddress3;
						Peribadi.HR_SPOSKOD = model.MailPostCode;
						Peribadi.HR_SALAMAT2 = model.MailAddress2;
						Peribadi.HR_SBANDAR = model.MailCity;
						Peribadi.HR_SNEGERI = model.MailState;

						Peribadi.HR_LESEN = model.License;
						Peribadi.HR_JENIS_KENDERAAN = model.CarModel;
						Peribadi.HR_NO_KENDERAAN = model.PlatNo;
						Peribadi.HR_CC_KENDERAAN = model.CarCC;
						db.HR_MAKLUMAT_PERIBADI.Add(Peribadi);
						db.SaveChanges();
						return RedirectToAction("Index", "Home");
					}
					else
					{
						psErrorMessage = "Maaf!! pengguna telah Wujud. Sila isi no KP yang lain";
						ViewBag.error = psErrorMessage;
						return View(model);
					}
				}
			}


			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "Lelaki", Value = "Lelaki" });
			items.Add(new SelectListItem { Text = "Perempuan", Value = "Perempuan" });
			ViewBag.jantina = items;

			List<SelectListItem> items1 = new List<SelectListItem>();
			items1.Add(new SelectListItem { Text = "Islam", Value = "Islam" });
			items1.Add(new SelectListItem { Text = "Buddha", Value = "Buddha" });
			items1.Add(new SelectListItem { Text = "Hindu", Value = "Hindu" });
			items1.Add(new SelectListItem { Text = "Kristian", Value = "Kristian" });
			ViewBag.religion = items1;

			List<SelectListItem> items2 = new List<SelectListItem>();
			items2.Add(new SelectListItem { Text = "Melayu", Value = "Melayu" });
			items2.Add(new SelectListItem { Text = "Cina", Value = "Cina" });
			items2.Add(new SelectListItem { Text = "India", Value = "India" });
			items2.Add(new SelectListItem { Text = "Lain-Lain", Value = "Lain-Lain" });
			ViewBag.race = items2;

			List<SelectListItem> items3 = new List<SelectListItem>();
			items3.Add(new SelectListItem { Text = "Bujang", Value = "Bujang" });
			items3.Add(new SelectListItem { Text = "Berkahwin", Value = "Berkahwin" });
			items3.Add(new SelectListItem { Text = "Duda", Value = "Duda" });
			items3.Add(new SelectListItem { Text = "Janda", Value = "Janda" });
			ViewBag.mstatus = items3;

			List<SelectListItem> items4 = new List<SelectListItem>();
			items4.Add(new SelectListItem { Text = "Ada", Value = "Ada" });
			items4.Add(new SelectListItem { Text = "Tiada", Value = "Tiada" });
			ViewBag.license = items4;

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "Lelaki", Value = "Lelaki" });
			items.Add(new SelectListItem { Text = "Perempuan", Value = "Perempuan" });
			ViewBag.jantina = items;

			List<SelectListItem> items1 = new List<SelectListItem>();
			items1.Add(new SelectListItem { Text = "Islam", Value = "Islam" });
			items1.Add(new SelectListItem { Text = "Buddha", Value = "Buddha" });
			items1.Add(new SelectListItem { Text = "Hindu", Value = "Hindu" });
			items1.Add(new SelectListItem { Text = "Kristian", Value = "Kristian" });
			ViewBag.religion = items1;

			List<SelectListItem> items2 = new List<SelectListItem>();
			items2.Add(new SelectListItem { Text = "Melayu", Value = "Melayu" });
			items2.Add(new SelectListItem { Text = "Cina", Value = "Cina" });
			items2.Add(new SelectListItem { Text = "India", Value = "India" });
			items2.Add(new SelectListItem { Text = "Lain-Lain", Value = "Lain-Lain" });
			ViewBag.race = items2;

			List<SelectListItem> items3 = new List<SelectListItem>();
			items3.Add(new SelectListItem { Text = "Bujang", Value = "Bujang" });
			items3.Add(new SelectListItem { Text = "Berkahwin", Value = "Berkahwin" });
			items3.Add(new SelectListItem { Text = "Duda", Value = "Duda" });
			items3.Add(new SelectListItem { Text = "Janda", Value = "Janda" });
			ViewBag.mstatus = items3;

			List<SelectListItem> items4 = new List<SelectListItem>();
			items4.Add(new SelectListItem { Text = "Ada", Value = "Ada" });
			items4.Add(new SelectListItem { Text = "Tiada", Value = "Tiada" });
			ViewBag.license = items4;

			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(AccountRegistrationModel model, AgreementModels am)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.NoKP, Email = model.Email, PasswordUpdate = DateTime.Now };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					string UserID = user.Id;
					GE_PENGGUNA pengguna = new GE_PENGGUNA();
					HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
					pengguna.NAMA_PENUH_PENGGUNA = am.NAMA_PENUH_PENGGUNA;
					peribadi.HR_NAMA_PEKERJA = am.NAMA_PENUH_PENGGUNA;
					pengguna.USER_PASSWORD = am.USER_PASSWORD;
					peribadi.HR_EMAIL = am.HR_EMAIL;
					peribadi.HR_NO_KPBARU = am.HR_NO_KPBARU;
					pengguna.ROWGUID = UserID;

					peribadi.CreateUserInfo();
					pengguna.CreateUserInfo();
					await UserManager.AddToRoleAsync(UserID, "Staff");

					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					var pokemon = User.Identity.Name.ToLower();
					var user1 = db.Users.Where(s => s.Email == pokemon).SingleOrDefault();
					var emel = db.UserInfoes.Where(s => s.Email == user1.Email).SingleOrDefault();
					var role1 = db.UserRoles.Where(d => d.UserId == user1.Id).SingleOrDefault();
					var role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					new AuditTrailModels().Log(emel.Email, emel.username, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.username + " Telah Mendaftar Pengguna Baru " + am.NAMA_PENUH_PENGGUNA, System.Net.Dns.GetHostName(), emel.PhoneNo, Request.RawUrl, "Register");

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					// await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					return RedirectToAction("Index", "Home");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}
			var result = await UserManager.ConfirmEmailAsync(userId, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		public string ComputeHash(string input, HashAlgorithm algorithm)
		{
			Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

			Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		async Task<ActionResult> ForgotPassword(AccountViewRegistrationModel model, ForgotPasswordViewModel model1, ChangePasswordViewModel model2, string Command)
		{
			ApplicationDbContext db = new ApplicationDbContext();
			MajlisContext mc = new MajlisContext();

			if (Command == "Reset")
			{
				PRUSER pruser = mc.PRUSER.SingleOrDefault(s => s.USERNAME == model.Username);
				if (pruser == null)
				{
					return RedirectToAction("ForgotPassword", "Account");
				}

				string password = "P@ssw0rd123";
				string hPassword = ComputeHash(password, new MD5CryptoServiceProvider());

				pruser.USERPASSWORD = hPassword;
				mc.Entry(pruser).State = EntityState.Modified;
				mc.SaveChanges();

				var kpfp = db.Users.Where(s => s.UserName == model.Username).SingleOrDefault();
				var user = await UserManager.FindByEmailAsync(kpfp.Email);
				var kp = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
				var ui = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
				if (user == null)
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}
				string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model2.NewPassword = "P@ssw0rd123");
				//var result = await UserManager.ChangePasswordAsync(getuserid, model1.OldPassword, model1.NewPassword = "P@ssw0rd123");
				if (result.Succeeded)
				{
					var user1 = await UserManager.FindByIdAsync(user.Id);
					var role1 = db.UserRoles.Where(d => d.UserId == user.Id).SingleOrDefault();
					IdentityRole role = new IdentityRole();
					if (role1 != null)
					{
						role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					}

					ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == user.Id);
					auser.PasswordUpdate = DateTime.Now;
					db.Entry(auser).State = EntityState.Modified;
					db.SaveChanges();

					new AuditTrailModels().Log(user.Email, user.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, ui.HR_NAMA_PEKERJA + " Telah Menukar Katalaluan", System.Net.Dns.GetHostName(), ui.HR_TELBIMBIT, Request.RawUrl, "ChangePassword");

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link

					string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
					var url = Url.Action("Login", "Account", new { }, protocol: Request.Url.Scheme);
					var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					var html = "<html><head><title></title></head><body><table>Hi " + ui.HR_NAMA_PEKERJA + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP). Katalaluan anda telah ditukar kepada <font color='blue'><b>" + model2.NewPassword + "</b></font>.<br />Sila tekan <a href=" + url + ">pautan</a> ini untuk log masuk menggunakan katalaluan baru anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br /><font color='red'><b>NOTA : SILA TUKAR KATALALUAN YANG DIBERIKAN KEPADA KATALALUAN BARU ANDA</b></font><br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
					html += "</table></body></html> ";
					await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

					string UserID = user.Id;
					UserInfo userInfo = db.UserInfoes.SingleOrDefault(s => s.Email == kpfp.Email);
					userInfo.PasswordUpdate = DateTime.Now;

					db.Entry(userInfo).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("Login", "Account", new { Message = ManageMessageId.ResetPassword });
				}
			}
			return RedirectToAction("ForgotPassword", "Account");
		}

		/*[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(AccountViewRegistrationModel model, ForgotPasswordViewModel model1, ChangePasswordViewModel model2, ManageMessageId? message, string Command)
		{
			ApplicationDbContext db = new ApplicationDbContext();
			MajlisContext mc = new MajlisContext();
			var kpfp = db.Users.Where(s => s.UserName == model.Username).SingleOrDefault();
			var user = await UserManager.FindByEmailAsync(kpfp.Email);
			var kp = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
			var ui = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
			if (Command == "Reset")
			{
				if (user == null)
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}
				string password = "P@ssw0rd123";
				string hPassword = ComputeHash(password, new MD5CryptoServiceProvider());
				string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

				PRUSER pruser = mc.PRUSER.SingleOrDefault(s => s.USERNAME == model.Username);
				pruser.USERPASSWORD = hPassword;
				db.Entry(pruser).State = EntityState.Modified;
				db.SaveChanges();

				IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model2.NewPassword = "P@ssw0rd123");
				//var result = await UserManager.ChangePasswordAsync(getuserid, model1.OldPassword, model1.NewPassword = "P@ssw0rd123");
				if (result.Succeeded)
				{
					var user1 = await UserManager.FindByIdAsync(user.Id);
					var role1 = db.UserRoles.Where(d => d.UserId == user.Id).SingleOrDefault();
					IdentityRole role = new IdentityRole();
					if (role1 != null)
					{
						role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					}

					ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == user.Id);
					auser.PasswordUpdate = DateTime.Now;
					db.Entry(auser).State = EntityState.Modified;
					db.SaveChanges();

					new AuditTrailModels().Log(user.Email, user.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, ui.HR_NAMA_PEKERJA + " Telah Menukar Katalaluan", System.Net.Dns.GetHostName(), ui.HR_TELBIMBIT, Request.RawUrl, "ChangePassword");

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link

					string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
					var url = Url.Action("Login", "Account", new { }, protocol: Request.Url.Scheme);
					var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					var html = "<html><head><title></title></head><body><table>Hi " + ui.HR_NAMA_PEKERJA + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP). Katalaluan anda telah ditukar kepada <font color='blue'><b>" + model2.NewPassword + "</b></font>.<br />Sila tekan <a href=" + url + ">pautan</a> ini untuk log masuk menggunakan katalaluan baru anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br /><font color='red'><b>NOTA : SILA TUKAR KATALALUAN YANG DIBERIKAN KEPADA KATALALUAN BARU ANDA</b></font><br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
					html += "</table></body></html> ";
					await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

					string UserID = user.Id;
					UserInfo userInfo = db.UserInfoes.SingleOrDefault(s => s.Email == kpfp.Email);
					userInfo.PasswordUpdate = DateTime.Now;

					db.Entry(userInfo).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("Login", "Account", new { Message = ManageMessageId.ResetPassword });
				}
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}*/

		/*[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model, ManageMessageId? message)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                var ui = db.UserInfoes.Where(s => s.Email == model.Email).SingleOrDefault();
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                var html = "<html><head><title></title></head><body><table>Hi " + ui.username + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP).<br />Sila tekan <a href=" + callbackUrl + ">pautan</a> ini untuk reset kata laluan anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
                html += "</table></body></html> ";
                await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

                string UserID = user.Id;
                UserInfo userInfo = db.UserInfoes.SingleOrDefault(s => s.Email == model.Email);
                userInfo.PasswordUpdate = DateTime.Now;

                db.Entry(userInfo).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Login", "Account", new { Message = ManageMessageId.ResetPassword });
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }*/

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult Test()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		async Task<ActionResult> Test(AccountViewRegistrationModel model, ForgotPasswordViewModel model1, ChangePasswordViewModel model2, string Command)
		{
			ApplicationDbContext db = new ApplicationDbContext();
			MajlisContext mc = new MajlisContext();

			if (Command == "Reset")
			{
				PRUSER pruser = mc.PRUSER.SingleOrDefault(s => s.USERNAME == model.Username);
				if (pruser == null)
				{
					return RedirectToAction("ForgotPassword", "Account");
				}

				string password = "P@ssw0rd123";
				string hPassword = ComputeHash(password, new MD5CryptoServiceProvider());

				pruser.USERPASSWORD = hPassword;
				mc.Entry(pruser).State = EntityState.Modified;
				mc.SaveChanges();

				var kpfp = db.Users.Where(s => s.UserName == model.Username).SingleOrDefault();
				var user = await UserManager.FindByEmailAsync(kpfp.Email);
				var kp = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
				var ui = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == user.UserName).SingleOrDefault();
				if (user == null)
				{
					// Don't reveal that the user does not exist or is not confirmed
					return View("ForgotPasswordConfirmation");
				}
				string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, resetToken, model2.NewPassword = "P@ssw0rd123");
				//var result = await UserManager.ChangePasswordAsync(getuserid, model1.OldPassword, model1.NewPassword = "P@ssw0rd123");
				if (result.Succeeded)
				{
					var user1 = await UserManager.FindByIdAsync(user.Id);
					var role1 = db.UserRoles.Where(d => d.UserId == user.Id).SingleOrDefault();
					IdentityRole role = new IdentityRole();
					if (role1 != null)
					{
						role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					}

					ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == user.Id);
					auser.PasswordUpdate = DateTime.Now;
					db.Entry(auser).State = EntityState.Modified;
					db.SaveChanges();

					new AuditTrailModels().Log(user.Email, user.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, ui.HR_NAMA_PEKERJA + " Telah Menukar Katalaluan", System.Net.Dns.GetHostName(), ui.HR_TELBIMBIT, Request.RawUrl, "ChangePassword");

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link

					string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
					var url = Url.Action("Login", "Account", new { }, protocol: Request.Url.Scheme);
					var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					var html = "<html><head><title></title></head><body><table>Hi " + ui.HR_NAMA_PEKERJA + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP). Katalaluan anda telah ditukar kepada <font color='blue'><b>" + model2.NewPassword + "</b></font>.<br />Sila tekan <a href=" + url + ">pautan</a> ini untuk log masuk menggunakan katalaluan baru anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br /><font color='red'><b>NOTA : SILA TUKAR KATALALUAN YANG DIBERIKAN KEPADA KATALALUAN BARU ANDA</b></font><br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
					html += "</table></body></html> ";
					await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

					string UserID = user.Id;
					UserInfo userInfo = db.UserInfoes.SingleOrDefault(s => s.Email == kpfp.Email);
					userInfo.PasswordUpdate = DateTime.Now;

					db.Entry(userInfo).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("Login", "Account", new { Message = ManageMessageId.ResetPassword });
				}
			}
			return RedirectToAction("ForgotPassword", "Account");

		}


		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction("Login", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction("Login", "Account");
			}
			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return View("Error");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return View("Error");
			}
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
        //Khairil remove validate anti forgery token
		public ActionResult LogOff()
		{
            
            var pokemon = User.Identity.GetUserId();
			var user = db.Users.Where(s => s.Id == pokemon).SingleOrDefault();
			var role1 = db.UserRoles.Where(d => d.UserId == user.Id).SingleOrDefault();
			var role = new IdentityRole();
			if (role1 != null)
			{
				role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
			}

            //khairil add remove cookie
            RemoveCookie();

            new AuditTrailModels().Log(user.Email, user.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, user.UserName + " Telah Log Keluar Dari Sistem", System.Net.Dns.GetHostName(), user.PhoneNumber, Request.RawUrl, "Logout");

			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			GroupFeature groupFeature = new GroupFeature();
			List<Guid> userGroupIDList = new List<Guid>();
			List<GroupFeaturesDisplay> groupFeatureDisplayList = new List<GroupFeaturesDisplay>();
			if (User.Identity.IsAuthenticated)
			{
				UserGroup userGroup = new UserGroup();
				userGroupIDList = userGroup.GetGroupByLoginUser();
				groupFeatureDisplayList = groupFeature.GetFeatureListByGroup(userGroupIDList);
			}

			if (groupFeature.ShowFeature(userGroupIDList, groupFeatureDisplayList, "Admin", "Admin", "Admin"))
			{
				if (Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}
				return RedirectToAction("Index", "Home");
			}
			if (groupFeature.ShowFeature(userGroupIDList, groupFeatureDisplayList, "Staff", "Staff", "Staff"))
			{
				if (Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}
				return RedirectToAction("UserProfile", "UserInfoes");
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		public ActionResult blank()
		{
			return View();
		}
		public enum ManageMessageId
		{
			AddPhoneSuccess,
			ChangePasswordSuccess,
			SetTwoFactorSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			RemovePhoneSuccess,
			ResetPassword,
			ChangePassword,
			UpdatePassword,
			PasswordSalah,
			UserNull,
			Error
		}

		#endregion
	}
}