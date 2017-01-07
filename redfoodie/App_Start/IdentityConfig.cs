using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using redfoodie.Models;
using SparkPost;

namespace redfoodie
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var transmission = new Transmission
            {
                Content =
                {
                    From = new Address {Name = "Redfoodie", Email = "restaurants@vosipov.com"},
                    Subject = message.Subject,
                    Html = message.Body
                }
            };

            var recipient = new Recipient
            {
                Address = new Address { Email = message.Destination }
            };
            transmission.Recipients.Add(recipient);

            var client = new Client(Environment.GetEnvironmentVariable("Redfoodie_SparkPost_Password"));
            return client.Transmissions.Send(transmission);
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
            // Change UserValidator to be able to create Users with space in UserName
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
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

        /// <summary>Sign in the user in using the user name and password</summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="isPersistent"></param>
        /// <param name="shouldLockout"></param>
        /// <returns></returns>
        public override async Task<SignInStatus> PasswordSignInAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            if (UserManager == null)
                return SignInStatus.Failure;
            var user = await UserManager.FindByEmailAsync(email).WithCurrentCulture();
            if (user == null)
                return SignInStatus.Failure;
            if (await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                return SignInStatus.LockedOut;
            if (await UserManager.CheckPasswordAsync(user, password).WithCurrentCulture())
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id).WithCurrentCulture();
                return await SignInOrTwoFactor(user, isPersistent).WithCurrentCulture();
            }
            if (!shouldLockout) return SignInStatus.Failure;
            await UserManager.AccessFailedAsync(user.Id).WithCurrentCulture();
            if (await UserManager.IsLockedOutAsync(user.Id).WithCurrentCulture())
                return SignInStatus.LockedOut;
            return SignInStatus.Failure;
        }

        /// <summary>
        /// Copied from SignInManage.SignInOrTwoFactor due it is a private method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        private async Task<SignInStatus> SignInOrTwoFactor(ApplicationUser user, bool isPersistent)
        {
            var id = Convert.ToString((object)user.Id);
            if (await UserManager.GetTwoFactorEnabledAsync(user.Id).WithCurrentCulture())
            {
                if ((await UserManager.GetValidTwoFactorProvidersAsync(user.Id).WithCurrentCulture()).Count > 0)
                {
                    if (!await AuthenticationManager.TwoFactorBrowserRememberedAsync(id).WithCurrentCulture())
                    {
                        var claimsIdentity = new ClaimsIdentity("TwoFactorCookie");
                        claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", id));
                        AuthenticationManager.SignIn(claimsIdentity);
                        return SignInStatus.RequiresVerification;
                    }
                }
            }
            await SignInAsync(user, isPersistent, false).WithCurrentCulture();
            return SignInStatus.Success;
        }
    }
}
