using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eSPP.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Emel")]
		public string Email { get; set; }
	}

	public class ExternalLoginListViewModel
	{
		public string ReturnUrl { get; set; }
	}

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
		public bool RememberMe { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		[Display(Name = "Kod")]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?")]
		public bool RememberBrowser { get; set; }

		public bool RememberMe { get; set; }
	}

	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Emel")]
		public string Email { get; set; }
	}

	public class AccountRegistrationModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		//[Required]
		//[EmailAddress]
		//[Compare("Email")]
		//public string EmailConfirm { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		//[Required]
		//[DataType(DataType.Password)]
		//[Compare("Password")]
		//public string PasswordConfirm { get; set; }
		[Required]
		public string NoStaff { get; set; }
		public string PhoneNo { get; set; }
		public string NoKP { get; set; }
		public string UserID { get; set; }
		public string Position { get; set; }

		public string FaxNo { get; set; }
		public string Status { get; set; }
		public string Addresss { get; set; }

		public string City { get; set; }
		public Nullable<System.DateTime> DOB { get; set; }
		public string Age { get; set; }
		public string Sex { get; set; }
		public string Religion { get; set; }
		public string Race { get; set; }
		public string MStatus { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string MailAddress { get; set; }
		public string MailAddress2 { get; set; }
		public string MailAddress3 { get; set; }
		public string MailPostCode { get; set; }
		public string MailState { get; set; }
		public string MailCity { get; set; }
		public string License { get; set; }
		public string CarModel { get; set; }
		public Nullable<decimal> CarCC { get; set; }
		public string PlatNo { get; set; }
		public string PostCode { get; set; }
		public string State { get; set; }
		public bool RememberMe { get; set; }
		public string rowguid { get; set; }
		public DateTime PasswordUpdate { get; set; }

	}

	public class AccountViewRegistrationModel
	{
		[Required]
		public string Username { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		public string Role { get; set; }
		//[Required]
		//[EmailAddress]
		//[Compare("Email")]
		//public string EmailConfirm { get; set; }
		//[Required]
		//[DataType(DataType.Password)]
		//public string Password { get; set; }

		//[Required]
		//[DataType(DataType.Password)]
		//[Compare("Password")]
		//public string PasswordConfirm { get; set; }
		public string NoStaff { get; set; }
		public string PhoneNo { get; set; }
		public string NoKP { get; set; }
		public string UserID { get; set; }
		public string Position { get; set; }

		public string FaxNo { get; set; }
		public string Status { get; set; }
		public string Addresss { get; set; }

		public string City { get; set; }
		public Nullable<System.DateTime> DOB { get; set; }
		public string Age { get; set; }
		public string Sex { get; set; }
		public string Religion { get; set; }
		public string Race { get; set; }
		public string MStatus { get; set; }
		public string Address2 { get; set; }
		public string Address3 { get; set; }
		public string MailAddress { get; set; }
		public string MailAddress2 { get; set; }
		public string MailAddress3 { get; set; }
		public string MailPostCode { get; set; }
		public string MailState { get; set; }
		public string MailCity { get; set; }
		public string License { get; set; }
		public string CarModel { get; set; }
		public Nullable<decimal> CarCC { get; set; }
		public string PlatNo { get; set; }
		public string PostCode { get; set; }
		public string State { get; set; }
		public bool RememberMe { get; set; }
		public string rowguid { get; set; }
		public DateTime PasswordUpdate { get; set; }

	}

	public class AccountLoginModel
	{
		[Required(ErrorMessage = "Sila masukkan No ID")]
		[Display(Name = "No ID")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Sila masukkan Katalaluan")]
		[DataType(DataType.Password)]
		[Display(Name = "Katalaluan")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Emel")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "{0} Katalaluan {1}", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Display(Name = "Katalaluan Baru")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Sahkan Katalaluan Baru")]
		[Compare("Password", ErrorMessage = "Katalaluan Baru Tidak Sama Dengan Sahkan Katalaluan Baru")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Emel")]
		public string Email { get; set; }
	}
}

