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

namespace eSPP.Controllers
{
    public class MaklumatController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private static string psErrorMessage = string.Empty;
        public MaklumatController()
        {

        }
        public MaklumatController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: UserInfo
        public ActionResult Peribadi()
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

        // GET: UserInfo/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAdmin([Bind(Include = "Username,rowguid,NoKP,DOB,Email,Age,Sex,Religion,Race,MStatus,Email,Password,Position,Addresss,Address2,Address3,City,State,PhoneNo,FaxNo,MailAddress,MailAddress2,MailAddress3,MailPostCode,MailCity,MailState,CarCC,CarModel,PlatNo,License,Status,PostCode")] AccountRegistrationModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            //string LoginPasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //string AdminUserID = HttpContext.Session["UserLoginID"].ToString();
                UserInfo userInfo = new UserInfo();
                userInfo.UserInfoID = Guid.NewGuid();
                userInfo.UserID = new Guid(user.Id);
                userInfo.username = model.Username ?? model.Email;
                userInfo.Email = model.Email;
                userInfo.NoKP = model.NoKP;
                userInfo.rowguid = user.Id;
                //userInfo.DOB = model.DOB;
                //userInfo.Age = model.Age;
                //userInfo.Religion = model.Religion;
                //userInfo.Sex = model.Sex;
                //userInfo.Race = model.Race;
                //userInfo.MStatus = model.MStatus;
                //userInfo.PhoneNo = model.PhoneNo;
                //userInfo.FaxNo = model.FaxNo;
                //userInfo.Address2 = model.Address2;
                //userInfo.Address3 = model.Address3;
                //userInfo.MailAddress = model.MailAddress;
                //userInfo.MailAddress2 = model.MailAddress2;
                //userInfo.MailAddress3 = model.MailAddress3;
                //userInfo.MailPostCode = model.MailPostCode;
                //userInfo.MailCity = model.MailCity;
                //userInfo.MailPostCode = model.MailPostCode;
                //userInfo.MailState = model.MailState;
                //userInfo.CarCC = model.CarCC;
                //userInfo.CarModel = model.CarModel;
                //userInfo.PlatNo = model.PlatNo;
                //userInfo.License = model.License;
                //userInfo.Position = model.Position;
                //userInfo.Status = "1";
                //userInfo.Addresss = model.Addresss;
                //userInfo.City = model.City;
                //userInfo.State = model.State;
                //userInfo.PostCode = model.PostCode;
                userInfo.CreateUserID = new Guid(user.Id);
                userInfo.CreateDateTime = DateTime.Now;
                userInfo.PasswordUpdate = DateTime.Now;
                userInfo.CreateUserInfo();

                return RedirectToAction("Pengguna", "Admin");
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

            return View();
        }

        public ActionResult Peribadi(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
              message == ManageMessageId.ChangePasswordSuccess ? "Katalaluan Anda Telah Berjaya Ditukar."
            : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
            : message == ManageMessageId.Error ? "An error has occurred."
            : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
            : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
            : "";

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
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

    }
}