using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using redfoodie.Controllers;
using redfoodie.Models;

namespace redfoodie.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests
    {
        /// <summary>
        /// get() method of UserManager based on ApplicationUserManager.Create(). UserManager implements Singleton pattern
        /// </summary>
        private static ApplicationUserManager _userManager;
        private static ApplicationUserManager UserManager
        {           
            get
            {
                if (_userManager != null) return _userManager;
                _userManager = new ApplicationUserManager(new MemoryUserStore<ApplicationUser>());
                // Configure validation logic for usernames
                _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

                // Configure validation logic for passwords
                _userManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    RequireNonLetterOrDigit = true,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true
                };

                // Configure user lockout defaults
                _userManager.UserLockoutEnabledByDefault = false;
                _userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                _userManager.MaxFailedAccessAttemptsBeforeLockout = 5;

                // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
                // You can write your own provider and plug it in here.
                _userManager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
                {
                    MessageFormat = "Your security code is {0}"
                });
                _userManager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is {0}"
                });
                _userManager.EmailService = new EmailService();
                _userManager.SmsService = new SmsService();
                return _userManager;
            }
        }
        private static readonly RegisterViewModel DummyModel = new RegisterViewModel { Email = "my_email@ya.ru", UserName = "my_fullname", Password = "Aa123#" };

        [TestMethod]
        public async Task RegisterByLoginPasswordTest()
        {
            // Arrange
            var signInManager = new ApplicationSignInManager(UserManager, new Mock<IAuthenticationManager>().Object);
            var controller = new AccountController(UserManager, signInManager);

            var mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s["currentCity"]).Returns(new City { Id = 1, ParentId = null, Name = "Delhi NCR" });
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            controller.ControllerContext = mockControllerContext.Object;

            // Act
            var actual = await controller.Register(DummyModel);

            // Assert
            Debug.Assert(actual != null, "actual != null");
            Assert.AreEqual(true, actual.Data.GetType().GetProperty("Success").GetValue(actual.Data, null));

            var user = UserManager.FindByName(DummyModel.UserName);
            Assert.AreEqual(DummyModel.UserName, user.UserName);
        }

        [TestMethod]
        public async Task RegisterExistedUserByLoginPasswordTest()
        {
            // Arrange
            var signInManager = new ApplicationSignInManager(UserManager, new Mock<IAuthenticationManager>().Object);
            var controller = new AccountController(UserManager, signInManager);

            var mockControllerContext = new Mock<ControllerContext>();
            var mockSession = new Mock<HttpSessionStateBase>();
            mockSession.SetupGet(s => s["currentCity"]).Returns(new City { Id = 1, ParentId = null, Name = "Delhi NCR" });
            mockControllerContext.Setup(p => p.HttpContext.Session).Returns(mockSession.Object);
            controller.ControllerContext = mockControllerContext.Object;

            // Act
            var actual = await controller.Register(DummyModel);

            // Assert
            Debug.Assert(actual != null, "result != null");
            Assert.AreEqual(false, actual.Data.GetType().GetProperty("Success").GetValue(actual.Data, null));

            var modelState = actual.Data.GetType().GetProperty("ModelState").GetValue(actual.Data, null);
            foreach (var el in (System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>)
                modelState)
            {
                var name = DummyModel.GetType().GetProperty(el.Key).GetValue(DummyModel, null);
                StringAssert.Matches(el.Value.First(), new Regex($"\\w '?{name}'? is already taken."));
            }
        }

        [TestMethod]
        public async Task LoginTest()
        {
            // Arrange
            var signInManager = new ApplicationSignInManager(UserManager, new Mock<IAuthenticationManager>().Object);
            var controller = new AccountController(UserManager, signInManager);
            var loginVm = new LoginViewModel { Email = DummyModel.Email, Password = DummyModel.Password};

            // Act
            var actual = await controller.Login(loginVm);

            // Assert
            Debug.Assert(actual != null, "result != null");
            Assert.AreEqual(true, actual.Data.GetType().GetProperty("Success").GetValue(actual.Data, null));
        }

        [TestMethod]
        public async Task LoginTestWithFailPassword()
        {
            // Arrange
            var signInManager = new ApplicationSignInManager(UserManager, new Mock<IAuthenticationManager>().Object);
            var controller = new AccountController(UserManager, signInManager);
            var loginVm = new LoginViewModel { Email = DummyModel.Email, Password = string.Empty };

            // Act
            var actual = await controller.Login(loginVm);

            // Assert
            Debug.Assert(actual != null, "result != null");
            Assert.AreEqual(false, actual.Data.GetType().GetProperty("Success").GetValue(actual.Data, null));
        }
    }
}