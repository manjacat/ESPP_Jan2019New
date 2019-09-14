using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using eSPP.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace eSPP
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            /*#region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            string html = "e - Hartanah LPKtn Reset Katalaluan<br />";

            html += HttpUtility.HtmlEncode(@"" + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("lpktn@rsakhidmat.com.my");
            msg.To.Add(new MailAddress(message.Destination));
            msg.IsBodyHtml = true;
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("mail.rsakhidmat.com.my", Convert.ToInt32(26));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("lpktn@rsakhidmat.com.my", "Icon,123#");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = false;
            smtpClient.Send(msg);
            return Task.FromResult(0);*/
            string fromAddress = "espp_support@janasys.com.my";
            string passwd = "MbpJ1234#";

            var smtp = new SmtpClient
            {
                Host = "mail.janasys.com.my",
                Port = 587,//143
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, passwd)
            };

            smtp.SendCompleted += (s, e) =>
            {
                smtp.Dispose();
            };

            var mail = new MailMessage(fromAddress, message.Destination);
            mail.IsBodyHtml = true;
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            return smtp.SendMailAsync(mail);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            PasswordHasher = new CustomPasswordHasher();
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        internal Task UpdateAsync(ApplicationUser user, string password)
        {
            throw new NotImplementedException();
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
