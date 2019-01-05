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
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using System.Data.Entity;

namespace eSPP.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private static string psErrorMessage = string.Empty;
		static string CurrentUserGroupName = string.Empty;
		public AdminController()
        {

        }
        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

		public ActionResult Search()
		{
			//Peribadi("", "");
			List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();
			return View(mPeribadi);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Search(string value, string key)
		{
			MaklumatKakitanganModels mKakitangan = new MaklumatKakitanganModels();
			ViewBag.key = "";
			//Peribadi("", "");
			List<HR_MAKLUMAT_PERIBADI> mPeribadi = new List<HR_MAKLUMAT_PERIBADI>();

			if (key == "1" || key == "4")
			{
				mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == value).ToList();

			}
			else if (key == "2")
			{
				mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NAMA_PEKERJA.Contains(value)).ToList();
			}
			else if (key == "3")
			{
				mPeribadi = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU.Contains(value)).ToList();
			}

			if (mPeribadi == null)
			{
				return HttpNotFound();
			}

			return View(mPeribadi);
		}

		// GET: UserInfo
		public ActionResult Admin()
        {
            if (psErrorMessage != null && psErrorMessage != "") { ViewBag.ErrorMessage = psErrorMessage; psErrorMessage = string.Empty; }
            List<UserInfo> UserInfoList = db.UserInfoes.ToList<UserInfo>();
            List<UserInfoView> UserInfoViewList = new List<UserInfoView>();
            foreach (UserInfo userInfo in UserInfoList)
            {
                IdentityUser identityUser = db.Users.Find(userInfo.UserID.ToString());
                if (identityUser != null)
                {
                    UserInfoView userInfoView = new UserInfoView();
                    userInfoView.UserInfoID = userInfo.UserInfoID;
                    userInfoView.UserID = userInfo.UserID;
                    userInfoView.username = userInfo.username;
                    userInfoView.Email = identityUser.Email;
                    userInfoView.Position = userInfo.Position;
                    userInfoView.PhoneNo = userInfo.PhoneNo;
                    userInfoView.CreateDateTime = userInfo.CreateDateTime;
                    userInfoView.FaxNo = userInfo.FaxNo;
                    userInfoView.Addresss = userInfo.Addresss;
                    userInfoView.City = userInfo.City;
                    userInfoView.PostCode = userInfo.PostCode;
                    userInfoView.State = userInfo.State;
                    userInfoView.Status = userInfo.Status;
                    UserInfoViewList.Add(userInfoView);
                }
            }
            return View(UserInfoViewList);
        }
        public ActionResult AddAdmin()
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
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RandomString1(int length)
        {
            const string chars = "zxcvbnmasdfghjklqwertyuiop1234567890";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // GET: UserInfo/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAdmin(AccountRegistrationModel model, AgreementModels am)
        {
            //model.Password = am.USER_PASSWORD;
            var user = new ApplicationUser { UserName = model.Email = am.HR_EMAIL, Email = model.Email = am.HR_EMAIL };
            var result = await UserManager.CreateAsync(user, model.Password = am.USER_PASSWORD);

            if (result.Succeeded)
            {
                string UserID = user.Id;
                GE_PENGGUNA pengguna = new GE_PENGGUNA();
                HR_MAKLUMAT_PERIBADI peribadi = new HR_MAKLUMAT_PERIBADI();
                pengguna.NAMA_PENUH_PENGGUNA = am.NAMA_PENUH_PENGGUNA;
                pengguna.USER_ID = RandomString1(5);
                pengguna.USER_GROUP = "1";
                pengguna.USER_LEVEL = 1;
                pengguna.SARING_SENARAI_TUGAS = 1;
                pengguna.BANTUAN_BUTANG = 1;
                peribadi.HR_NO_PEKERJA = RandomString(5);
                peribadi.HR_NAMA_PEKERJA = am.NAMA_PENUH_PENGGUNA;
                pengguna.USER_PASSWORD = am.USER_PASSWORD;
                peribadi.HR_EMAIL = am.HR_EMAIL;
                peribadi.HR_NO_KPBARU = am.HR_NO_KPBARU;
                pengguna.ROWGUID = UserID;
                db.HR_MAKLUMAT_PERIBADI.Add(peribadi);
                db.GE_PENGGUNA.Add(pengguna);
                db.SaveChanges();

                /*string AdminUserID = HttpContext.Session["UserLoginID"].ToString();
                UserInfo userInfo = new UserInfo();
                userInfo.UserInfoID = Guid.NewGuid();
                userInfo.UserID = new Guid(user.Id);
                userInfo.username = model.Username ?? model.Email;
                userInfo.Email = model.Email;
                userInfo.NoKP = model.NoKP;
                userInfo.rowguid = user.Id;
                userInfo.DOB = model.DOB;
                userInfo.Age = model.Age;
                userInfo.Religion = model.Religion;
                userInfo.Sex = model.Sex;
                userInfo.Race = model.Race;
                userInfo.MStatus = model.MStatus;
                userInfo.PhoneNo = model.PhoneNo;
                userInfo.FaxNo = model.FaxNo;
                userInfo.Address2 = model.Address2;
                userInfo.Address3 = model.Address3;
                userInfo.MailAddress = model.MailAddress;
                userInfo.MailAddress2 = model.MailAddress2;
                userInfo.MailAddress3 = model.MailAddress3;
                userInfo.MailPostCode = model.MailPostCode;
                userInfo.MailCity = model.MailCity;
                userInfo.MailPostCode = model.MailPostCode;
                userInfo.MailState = model.MailState;
                userInfo.CarCC = model.CarCC;
                userInfo.CarModel = model.CarModel;
                userInfo.PlatNo = model.PlatNo;
                userInfo.License = model.License;
                userInfo.Position = model.Position;
                //userInfo.Status = "1";
                userInfo.Addresss = model.Addresss;
                userInfo.City = model.City;
                userInfo.State = model.State;
                userInfo.PostCode = model.PostCode;
                userInfo.CreateUserID = new Guid(AdminUserID);
                userInfo.CreateDateTime = DateTime.Now;
                userInfo.PasswordUpdate = DateTime.Now;
                userInfo.CreateUserInfo();

                var pokemon = User.Identity.Name.ToLower();
                var user1 = db.Users.Where(s => s.Email == pokemon).SingleOrDefault();
                var emel = db.UserInfoes.Where(s => s.Email == user1.Email).SingleOrDefault();
                var role1 = db.UserRoles.Where(d => d.UserId == user1.Id).SingleOrDefault();
                var role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
                new AuditTrailModels().Log(emel.Email, emel.username, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.username + " Telah Mendaftar Pengguna Baru " + userInfo.username, System.Net.Dns.GetHostName(), emel.PhoneNo, Request.RawUrl, "Register");*/

                return RedirectToAction("UserGroup", "UserGroup");
            }
            else
            {
                psErrorMessage = "Invalid Password";
                ViewBag.error = psErrorMessage;
                return View(model);
            }
        }
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
            items2.Add(new SelectListItem { Text = "Melayu", Value = "1" });
            items2.Add(new SelectListItem { Text = "Cina", Value = "2" });
            items2.Add(new SelectListItem { Text = "India", Value = "3" });
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

            List<SelectListItem> StateList = new List<SelectListItem>();
            StateList.Add(new SelectListItem { Text = "W.P. Kuala Lumpur", Value = "W.P. Kuala Lumpur" });
            StateList.Add(new SelectListItem { Text = "W.P. Labuan", Value = "W.P. Labuan" });
            StateList.Add(new SelectListItem { Text = "W.P. Putrajaya", Value = "W.P. Putrajaya" });
            StateList.Add(new SelectListItem { Text = "Johor", Value = "Johor" });
            StateList.Add(new SelectListItem { Text = "Kedah", Value = "Kedah" });
            StateList.Add(new SelectListItem { Text = "Kelantan", Value = "Kelantan" });
            StateList.Add(new SelectListItem { Text = "Melaka", Value = "Melaka" });
            StateList.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "Negeri Sembilan" });
            StateList.Add(new SelectListItem { Text = "Pahang", Value = "Pahang" });
            StateList.Add(new SelectListItem { Text = "Perak", Value = "Perak" });
            StateList.Add(new SelectListItem { Text = "Perlis", Value = "Perlis" });
            StateList.Add(new SelectListItem { Text = "Pulau Pinang", Value = "Pulau Pinang" });
            StateList.Add(new SelectListItem { Text = "Sabah", Value = "Sabah" });
            StateList.Add(new SelectListItem { Text = "Serawak", Value = "Serawak" });
            StateList.Add(new SelectListItem { Text = "Selangor", Value = "Selangor" });
            StateList.Add(new SelectListItem { Text = "Terengganu", Value = " Terengganu" });
            ViewBag.StateList = StateList;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {

                if (model.Username == null) { psErrorMessage = "Please enter User Name"; return RedirectToAction("Register", "Admin"); }
                if (model.Email == null) { psErrorMessage = "Please enter Email"; return RedirectToAction("Register", "Admin"); }
                //if (model.PhoneNo == null) { psErrorMessage = "Please enter Phone Number"; return RedirectToAction("Register", "Admin"); }
                if (model.Password == null) { psErrorMessage = "Please enter Password"; return RedirectToAction("Register", "Admin"); }
                if (db.Users.SingleOrDefault(i => i.Email == model.Email) != null) { psErrorMessage = "Email already Exist"; return RedirectToAction("Register", "Admin"); }
            }

            

            var user = new ApplicationUser { UserName = model.NoKP, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                HR_MAKLUMAT_PERIBADI Peribadi = new HR_MAKLUMAT_PERIBADI();
                Peribadi.HR_NO_PEKERJA = model.NoStaff;
                Peribadi.HR_NAMA_PEKERJA = model.Username;
                Peribadi.HR_NO_KPBARU = model.NoKP;
                Peribadi.HR_EMAIL = model.Email;
                /*
                
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
                Peribadi.HR_CC_KENDERAAN = model.CarCC;*/
                //Peribadi.CreateUserInfo();
                db.HR_MAKLUMAT_PERIBADI.Add(Peribadi);
                db.SaveChanges();

                

                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("Pengguna", "Admin");
            }

            // If we got this far, something failed, redisplay form
            else
            {
                psErrorMessage = "Invalid Password";
                return RedirectToAction("Register", "Admin");
            }
        }

        public ActionResult ViewRegister(AccountViewRegistrationModel model1, string id, ManageMessageId? message, string getuserid, string UserGroupID)
        {
			var result = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == id).SingleOrDefault();
			IdentityUser users = db.Users.Where(s => s.UserName == result.HR_NO_KPBARU).SingleOrDefault();
			List<IdentityRole> UserRoleList = db.Roles.ToList<IdentityRole>();
			IdentityUserRole iur = db.UserRoles.Where(s => s.UserId == users.Id).SingleOrDefault();
			
			//IdentityRole ir = db.Roles.Where(p => p.Id == iur.UserId).SingleOrDefault();
			ViewBag.emel = users.Email;
			getuserid = users.Id;
			ViewBag.id = getuserid;
			ViewBag.groupid = UserGroupID;

			ViewBag.StatusMessage =
			  message == ManageMessageId.ResetPassword ? "Katalaluan Telah Berjaya Ditukar. Katalaluan Baru Telah Ditukar Dan Dihantar Ke Emel"
			  : message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			  : message == ManageMessageId.Kemaskini ? "Profil Anda Telah Berjaya Dikemaskini."
			  : "";

			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


			if (users == null)
            {
                return HttpNotFound();
            }

            HR_MAKLUMAT_PERIBADI namaPekerja = db.HR_MAKLUMAT_PERIBADI.First(s => s.HR_NO_KPBARU == users.UserName);
            string nama = "";
            string noStaff = "";
            if (namaPekerja != null)
            {
                nama = namaPekerja.HR_NAMA_PEKERJA;
                noStaff = namaPekerja.HR_NO_PEKERJA;
            }
            AccountViewRegistrationModel model = new AccountViewRegistrationModel();

			var userRoleList = db.UserRoles.FirstOrDefault(s => s.UserId == users.Id);
			if (userRoleList == null)
			{
				userRoleList = new IdentityUserRole();
			}
			var roleList = db.Roles.FirstOrDefault(s => s.Id == userRoleList.RoleId);
			if (roleList == null)
			{
				roleList = new IdentityRole();
			}
            model.Username = nama;
            model.Email = users.Email;
            model.NoKP = users.UserName;
            //model.PhoneNo = users.PhoneNumber;
            model.NoStaff = noStaff;
            model.UserID = users.Id;
			//model1.Role = iur.RoleId;
			ViewBag.staffno = model.NoStaff;
			var ur = new SelectList(UserRoleList, "Id", "Name");
			ViewBag.role = ur;

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

            List<SelectListItem> items5 = new List<SelectListItem>();
            items5.Add(new SelectListItem { Text = "W.P. Kuala Lumpur", Value = "W.P. Kuala Lumpur" });
            items5.Add(new SelectListItem { Text = "W.P. Labuan", Value = "W.P. Labuan" });
            items5.Add(new SelectListItem { Text = "W.P. Putrajaya", Value = "W.P. Putrajaya" });
            items5.Add(new SelectListItem { Text = "Johor", Value = "Johor" });
            items5.Add(new SelectListItem { Text = "Kedah", Value = "Kedah" });
            items5.Add(new SelectListItem { Text = "Kelantan", Value = "Kelantan" });
            items5.Add(new SelectListItem { Text = "Melaka", Value = "Melaka" });
            items5.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "Negeri Sembilan" });
            items5.Add(new SelectListItem { Text = "Pahang", Value = "Pahang" });
            items5.Add(new SelectListItem { Text = "Perak", Value = "Perak" });
            items5.Add(new SelectListItem { Text = "Perlis", Value = "Perlis" });
            items5.Add(new SelectListItem { Text = "Pulau Pinang", Value = "Pulau Pinang" });
            items5.Add(new SelectListItem { Text = "Sabah", Value = "Sabah" });
            items5.Add(new SelectListItem { Text = "Serawak", Value = "Serawak" });
            items5.Add(new SelectListItem { Text = "Selangor", Value = "Selangor" });
            items5.Add(new SelectListItem { Text = "Terengganu", Value = " Terengganu" });
            ViewBag.state = items5;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> ViewRegister(AccountViewRegistrationModel model, ChangePasswordViewModel model1, ManageMessageId? message, string Command, string getuserid, string staffno)
        {

			ViewBag.staffno = staffno;
			var user = await UserManager.FindByEmailAsync(model.Email);
			var ui = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == model.NoKP).SingleOrDefault();

			if (Command == "Reset")
			{
				string resetToken = await UserManager.GeneratePasswordResetTokenAsync(getuserid);
				IdentityResult result = await UserManager.ResetPasswordAsync(getuserid, resetToken, model1.NewPassword = "P@ssw0rd123");
				//var result = await UserManager.ChangePasswordAsync(getuserid, model1.OldPassword, model1.NewPassword = "P@ssw0rd123");
				if (result.Succeeded)
				{
					var pokemon = User.Identity.Name;
					var user2 = db.Users.Where(s => s.UserName == pokemon).SingleOrDefault();
					var emel = db.Users.Where(s => s.Email == user2.Email).SingleOrDefault();
					var role1 = db.UserRoles.Where(d => d.UserId == user2.Id).SingleOrDefault();
					IdentityRole role = new IdentityRole();
					if (role1 != null)
					{
						role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					}

					ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == getuserid);
					auser.PasswordUpdate = DateTime.Now;
					db.Entry(auser).State = EntityState.Modified;
					db.SaveChanges();

					new AuditTrailModels().Log(emel.Email, emel.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.UserName + " Telah Menukar Katalaluan", System.Net.Dns.GetHostName(), emel.PhoneNumber, Request.RawUrl, "ChangePassword");



					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link

					string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
					var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					var html = "<html><head><title></title></head><body><table>Hi " + ui.HR_NAMA_PEKERJA + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP).<br />Sila tekan <a href=" + callbackUrl + ">pautan</a> ini untuk reset kata laluan anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
					html += "</table></body></html> ";
					await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

					string UserID = user.Id;
					var userInfo = db.Users.Where(s => s.Email == model.Email).SingleOrDefault();
					userInfo.PasswordUpdate = DateTime.Now;

					db.Entry(userInfo).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("ViewRegister", "Admin", new { Message = ManageMessageId.ResetPassword });
				}
			}
			if (Command == "Kemaskini")
			{
				RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
				ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == model.UserID);
				IdentityUserRole userrole = db.UserRoles.Where(s => s.UserId == auser.Id).SingleOrDefault();

				db.UserRoles.RemoveRange(db.UserRoles.Where(s => s.UserId == model.UserID));

				IdentityUserRole Peribadi = new IdentityUserRole();
				Peribadi.UserId = model.UserID;
				Peribadi.RoleId = model.Role;

				db.UserRoles.Add(Peribadi);
				db.SaveChanges();

				/*IdentityUserRole iurole = new IdentityUserRole();
				iurole = db.UserRoles.Where(s => s.UserId == model.UserID).SingleOrDefault();

				iurole.RoleId = model.Role;
				db.Entry(iurole).State = EntityState.Modified;
				db.SaveChanges();*/

				return RedirectToAction("ViewRegister", "Admin", new { Message = ManageMessageId.Kemaskini });
			}
			List<IdentityRole> UserRoleList = db.Roles.ToList<IdentityRole>();
			var ur = new SelectList(UserRoleList, "Id", "Name");
			ViewBag.role = ur;

			return View(model);
			/*ApplicationUser user = UserManager.FindById(model.UserID);
            user.UserName = model.NoKP; user.Email = model.Email;

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                IdentityUser users2 = db.Users.SingleOrDefault(s => s.Id == model.UserID);
                HR_MAKLUMAT_PERIBADI Peribadi2 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == users2.UserName);
                List<HR_MAKLUMAT_PERIBADI> Peribadi3 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA && s.HR_NO_KPBARU != users2.UserName).ToList();
                if(Peribadi3.Count() <= 0)
                {
                    HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA);
                    Peribadi.HR_NO_PEKERJA = model.NoStaff;
                    Peribadi.HR_NAMA_PEKERJA = model.Username;
                    Peribadi.HR_NO_KPBARU = model.NoKP;
                    Peribadi.HR_EMAIL = model.Email;
                    db.Entry(Peribadi).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Pengguna", "Admin");
                }
                else
                {
                    psErrorMessage = "Maaf!! pengguna telah Wujud. Sila isi no KP yang lain";
                    ViewBag.error = psErrorMessage;
                    return View(model);
                }

                

                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }

            // If we got this far, something failed, redisplay form
            else
            {
                psErrorMessage = "Invalid Password";
                return RedirectToAction("ViewRegister", "Admin");
            }*/
		}

		public ActionResult UserProfile(AccountViewRegistrationModel model1, string id, ManageMessageId? message, string getuserid, string UserGroupID)
		{
			var result = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == id).SingleOrDefault();
			IdentityUser users = db.Users.Where(s => s.UserName == result.HR_NO_KPBARU).SingleOrDefault();
			List<IdentityRole> UserRoleList = db.Roles.ToList<IdentityRole>();
			IdentityUserRole iur = db.UserRoles.Where(s => s.UserId == users.Id).SingleOrDefault();

			//IdentityRole ir = db.Roles.Where(p => p.Id == iur.UserId).SingleOrDefault();
			ViewBag.emel = users.Email;
			getuserid = users.Id;
			ViewBag.id = getuserid;
			ViewBag.groupid = UserGroupID;

			ViewBag.StatusMessage =
			  message == ManageMessageId.ResetPassword ? "Katalaluan Telah Berjaya Ditukar. Katalaluan Baru Telah Ditukar Dan Dihantar Ke Emel"
			  : message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
			  : message == ManageMessageId.Kemaskini ? "Profil Anda Telah Berjaya Dikemaskini."
			  : "";

			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}


			if (users == null)
			{
				return HttpNotFound();
			}

			HR_MAKLUMAT_PERIBADI namaPekerja = db.HR_MAKLUMAT_PERIBADI.First(s => s.HR_NO_KPBARU == users.UserName);
			string nama = "";
			string noStaff = "";
			if (namaPekerja != null)
			{
				nama = namaPekerja.HR_NAMA_PEKERJA;
				noStaff = namaPekerja.HR_NO_PEKERJA;
			}
			AccountViewRegistrationModel model = new AccountViewRegistrationModel();

			var userRoleList = db.UserRoles.FirstOrDefault(s => s.UserId == users.Id);
			if (userRoleList == null)
			{
				userRoleList = new IdentityUserRole();
			}
			var roleList = db.Roles.FirstOrDefault(s => s.Id == userRoleList.RoleId);
			if (roleList == null)
			{
				roleList = new IdentityRole();
			}
			model.Username = nama;
			model.Email = users.Email;
			model.NoKP = users.UserName;
			//model.PhoneNo = users.PhoneNumber;
			model.NoStaff = noStaff;
			model.UserID = users.Id;
			/*if (iur.RoleId != null || iur.RoleId != "")
			{
				model1.Role = iur.RoleId;
			}*/
			ViewBag.staffno = model.NoStaff;
			var ur = new SelectList(UserRoleList, "Id", "Name");
			ViewBag.role = ur;

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

			List<SelectListItem> items5 = new List<SelectListItem>();
			items5.Add(new SelectListItem { Text = "W.P. Kuala Lumpur", Value = "W.P. Kuala Lumpur" });
			items5.Add(new SelectListItem { Text = "W.P. Labuan", Value = "W.P. Labuan" });
			items5.Add(new SelectListItem { Text = "W.P. Putrajaya", Value = "W.P. Putrajaya" });
			items5.Add(new SelectListItem { Text = "Johor", Value = "Johor" });
			items5.Add(new SelectListItem { Text = "Kedah", Value = "Kedah" });
			items5.Add(new SelectListItem { Text = "Kelantan", Value = "Kelantan" });
			items5.Add(new SelectListItem { Text = "Melaka", Value = "Melaka" });
			items5.Add(new SelectListItem { Text = "Negeri Sembilan", Value = "Negeri Sembilan" });
			items5.Add(new SelectListItem { Text = "Pahang", Value = "Pahang" });
			items5.Add(new SelectListItem { Text = "Perak", Value = "Perak" });
			items5.Add(new SelectListItem { Text = "Perlis", Value = "Perlis" });
			items5.Add(new SelectListItem { Text = "Pulau Pinang", Value = "Pulau Pinang" });
			items5.Add(new SelectListItem { Text = "Sabah", Value = "Sabah" });
			items5.Add(new SelectListItem { Text = "Serawak", Value = "Serawak" });
			items5.Add(new SelectListItem { Text = "Selangor", Value = "Selangor" });
			items5.Add(new SelectListItem { Text = "Terengganu", Value = " Terengganu" });
			ViewBag.state = items5;

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserProfile(AccountViewRegistrationModel model, ChangePasswordViewModel model1, ManageMessageId? message, string Command, string getuserid, string staffno)
		{

			ViewBag.staffno = staffno;
			var user = await UserManager.FindByEmailAsync(model.Email);
			var ui = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_KPBARU == model.NoKP).SingleOrDefault();

			if (Command == "Reset")
			{
				string resetToken = await UserManager.GeneratePasswordResetTokenAsync(getuserid);
				IdentityResult result = await UserManager.ResetPasswordAsync(getuserid, resetToken, model1.NewPassword = "P@ssw0rd123");
				//var result = await UserManager.ChangePasswordAsync(getuserid, model1.OldPassword, model1.NewPassword = "P@ssw0rd123");
				if (result.Succeeded)
				{
					var user1 = await UserManager.FindByIdAsync(getuserid);
					if (user1 != null)
					{
						await SignInManager.SignInAsync(user1, isPersistent: false, rememberBrowser: false);
					}
					var pokemon = User.Identity.Name;
					var user2 = db.Users.Where(s => s.UserName == pokemon).SingleOrDefault();
					var emel = db.Users.Where(s => s.Email == user2.Email).SingleOrDefault();
					var role1 = db.UserRoles.Where(d => d.UserId == user2.Id).SingleOrDefault();
					IdentityRole role = new IdentityRole();
					if (role1 != null)
					{
						role = db.Roles.Where(e => e.Id == role1.RoleId).SingleOrDefault();
					}

					ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == getuserid);
					auser.PasswordUpdate = DateTime.Now;
					db.Entry(auser).State = EntityState.Modified;
					db.SaveChanges();

					new AuditTrailModels().Log(emel.Email, emel.UserName, System.Web.HttpContext.Current.Request.UserHostAddress, role.Name, emel.UserName + " Telah Menukar Katalaluan", System.Net.Dns.GetHostName(), emel.PhoneNumber, Request.RawUrl, "ChangePassword");



					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link

					string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
					var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
					var html = "<html><head><title></title></head><body><table>Hi " + ui.HR_NAMA_PEKERJA + ",<br /><br /><tr><td>Anda telah meminta untuk reset katalaluan yang lama untuk Sistem Pengurusan Personel (eSPP).<br />Sila tekan <a href=" + callbackUrl + ">pautan</a> ini untuk reset kata laluan anda.<br /><br />Jika anda tidak meminta untuk reset katalaluan anda, sila balas emel ini dan beritahu kami.<br /><br />Sekian,<br /><br />eSPP Support Team.</td></tr>";
					html += "</table></body></html> ";
					await UserManager.SendEmailAsync(user.Id, "Reset Katalaluan", html);

					string UserID = user.Id;
					UserInfo userInfo = db.UserInfoes.SingleOrDefault(s => s.Email == model.Email);
					userInfo.PasswordUpdate = DateTime.Now;

					db.Entry(userInfo).State = EntityState.Modified;
					db.SaveChanges();

					return RedirectToAction("ViewRegister", "Admin", new { Message = ManageMessageId.ResetPassword });
				}
			}
			if (Command == "Kemaskini")
			{
				RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
				ApplicationUser auser = db.Users.SingleOrDefault(s => s.Id == model.UserID);
				IdentityUserRole userrole = db.UserRoles.Where(s => s.UserId == auser.Id).SingleOrDefault();

				db.UserRoles.RemoveRange(db.UserRoles.Where(s => s.UserId == model.UserID));

				IdentityUserRole Peribadi = new IdentityUserRole();
				Peribadi.UserId = model.UserID;
				Peribadi.RoleId = model.Role;

				db.UserRoles.Add(Peribadi);
				db.SaveChanges();

				/*IdentityUserRole iurole = new IdentityUserRole();
				iurole = db.UserRoles.Where(s => s.UserId == model.UserID).SingleOrDefault();

				iurole.RoleId = model.Role;
				db.Entry(iurole).State = EntityState.Modified;
				db.SaveChanges();*/

				return RedirectToAction("ViewRegister", "Admin", new { Message = ManageMessageId.Kemaskini });
			}
			List<IdentityRole> UserRoleList = db.Roles.ToList<IdentityRole>();
			var ur = new SelectList(UserRoleList, "Id", "Name");
			ViewBag.role = ur;

			return View(model);
			/*ApplicationUser user = UserManager.FindById(model.UserID);
            user.UserName = model.NoKP; user.Email = model.Email;

            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                IdentityUser users2 = db.Users.SingleOrDefault(s => s.Id == model.UserID);
                HR_MAKLUMAT_PERIBADI Peribadi2 = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == users2.UserName);
                List<HR_MAKLUMAT_PERIBADI> Peribadi3 = db.HR_MAKLUMAT_PERIBADI.Where(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA && s.HR_NO_KPBARU != users2.UserName).ToList();
                if(Peribadi3.Count() <= 0)
                {
                    HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.SingleOrDefault(s => s.HR_NO_PEKERJA == Peribadi2.HR_NO_PEKERJA);
                    Peribadi.HR_NO_PEKERJA = model.NoStaff;
                    Peribadi.HR_NAMA_PEKERJA = model.Username;
                    Peribadi.HR_NO_KPBARU = model.NoKP;
                    Peribadi.HR_EMAIL = model.Email;
                    db.Entry(Peribadi).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Pengguna", "Admin");
                }
                else
                {
                    psErrorMessage = "Maaf!! pengguna telah Wujud. Sila isi no KP yang lain";
                    ViewBag.error = psErrorMessage;
                    return View(model);
                }

                

                //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }

            // If we got this far, something failed, redisplay form
            else
            {
                psErrorMessage = "Invalid Password";
                return RedirectToAction("ViewRegister", "Admin");
            }*/
		}

		public ActionResult Pengguna(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
            : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
            : message == ManageMessageId.Error ? "An error has occurred."
            : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
            : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
            : "";

            //List<GE_PENGGUNA> peribadi = db.GE_PENGGUNA.ToList();
            if (psErrorMessage != null && psErrorMessage != "") { ViewBag.ErrorMessage = psErrorMessage; psErrorMessage = string.Empty; }
            List<ApplicationUser> UserInfoList = db.Users.ToList<ApplicationUser>();
            List<UserInfoView> UserInfoViewList = new List<UserInfoView>();
            foreach (ApplicationUser userInfo in UserInfoList)
            {
                IdentityUser identityUser = db.Users.Find(userInfo.Id.ToString());
                HR_MAKLUMAT_PERIBADI Peribadi = db.HR_MAKLUMAT_PERIBADI.FirstOrDefault(s => s.HR_NO_KPBARU == userInfo.UserName);
                if (identityUser != null)
                {
                    var NamaPekerja = "";
                    if(Peribadi != null)
                    {
                        NamaPekerja = Peribadi.HR_NAMA_PEKERJA.ToLower();
                    }
                    UserInfoView userInfoView = new UserInfoView();
                    //userInfoView.UserInfoID = userInfo.UserInfoID;
                    userInfoView.UserID = new Guid(userInfo.Id);
                    userInfoView.NoKP = userInfo.UserName;
                    userInfoView.Email = identityUser.Email;
                    userInfoView.Username = NamaPekerja;
                    userInfoView.PhoneNo = userInfo.PhoneNumber;
                    //userInfoView.CreateDateTime = userInfo.CreateDateTime;
                    UserInfoViewList.Add(userInfoView);
                }
            }
            return View(UserInfoViewList);
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
			Kemaskini,
            Error
        }

    }
}