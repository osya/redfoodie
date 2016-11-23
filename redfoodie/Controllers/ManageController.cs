using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set 
            { 
                _signInManager = value; 
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        private string UserPath => Path.Combine("~/Content/thumbs/user/", User.Identity.GetUserName());

        //
        // GET: /Manage/ProfileSettings
        public async Task<ActionResult> ProfileSettings()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var model = new ProfileSettingsViewModel
            {
                UserName = User.Identity.GetUserName(),
                Cities = Session["citiesList"] as SelectList,
                SelectedCity = user.CityId,
                Twitter = user.Twitter,
                Facebook = user.Facebook,
                Website = user.Website,
                Bio = user.Bio,
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                ShortUrl = user.ShortUrl,
                FollowMail = user.FollowMail,
                ReplyReviewmail = user.ReplyReviewmail,
                ThanksFavoritemail = user.ThanksFavoritemail
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ProfileSettings(ProfileSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.UserName = model.UserName;
                user.CityId = model.SelectedCity;
                user.Twitter = model.Twitter;
                user.Facebook = model.Facebook;
                user.Website = model.Website;
                user.PhoneNumber = model.PhoneNumber;
                user.Bio = model.Bio;
                var updateResult = await UserManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return Json(JsonResponseFactory.SuccessResponse());
                }
                AddErrors(updateResult);
            }
            return Json(JsonResponseFactory.ErrorResponse(ModelState.Where(pair => pair.Value.Errors.Count > 0)
                    .ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(error => error.ErrorMessage))));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ShortUrl(ShortUrlViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.ShortUrl = model.ShortUrl;
                var updateResult = await UserManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    AddErrors(updateResult);
                }
            }
            return PartialView("_ShortUrlBody", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> NotificationSettings(NotificationSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.FollowMail = model.FollowMail;
                user.ReplyReviewmail = model.ReplyReviewmail;
                user.ThanksFavoritemail = model.ThanksFavoritemail;
                var updateResult = await UserManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return Json(JsonResponseFactory.SuccessResponse());
                }
                AddErrors(updateResult);
            }
            return Json(JsonResponseFactory.ErrorResponse(ModelState.Where(pair => pair.Value.Errors.Count > 0)
                    .ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(error => error.ErrorMessage))));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ViewProfile(string username, string shortUrl)
        {
            var dbContext = new ApplicationDbContext();
            ApplicationUser user;
            if ((username != null) && string.Equals(username, User.Identity.GetUserName()))
            {
                user = await UserManager.FindByNameAsync(username);
                return View(new ViewProfileViewModel { UserName = username, ImagePath = user.ImageFileName != null ? Path.Combine(UserPath, user.ImageFileName) : null });
            }

            // UserManager not used here because searching by ShortUrl
            var username2 = username ?? User.Identity.GetUserName();
            var userQ = dbContext.Users.Where(m => string.Equals(m.UserName, username2) || (shortUrl != null && string.Equals(m.ShortUrl, shortUrl)));
            if (!await userQ.AnyAsync() || await userQ.CountAsync() > 1) return View("Error");
            user = userQ.First();
            var imagePath = Session["imageFileName"] as string ?? (user.ImageFileName != null
                ? Path.Combine(UserPath, user.ImageFileName) : "/Content/collections/imgs/user.svg");
            return View(new ViewProfileViewModel { UserName = user.UserName, ImagePath = imagePath });
        }

        public ActionResult InviteFriends()
        {
            return View();
        }        

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, false, false);
                    }
                    return Json(JsonResponseFactory.SuccessResponse());
                }
                AddErrors(result);
            }
            return Json(JsonResponseFactory.ErrorResponse(ModelState.Where(pair => pair.Value.Errors.Count > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(error => error.ErrorMessage))));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateProfilePicture(ImagePopupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if ((model.UserFile.ContentLength > 0) && (model.UserFile.FileName != null))
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    var updateResult = await UpdateProfilePicture(model.UserFile.FileName, model.UserFile.InputStream, user);
                    if (updateResult.Succeeded)
                    {
                        return Json(JsonResponseFactory.SuccessResponse());
                    }
                    AddErrors(updateResult);
                }
                ModelState.AddModelError("", "ContentLength == 0 or FileName is null");
            }
            return Json(JsonResponseFactory.ErrorResponse(ModelState.Where(pair => pair.Value.Errors.Count > 0)
                    .ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(error => error.ErrorMessage))));
        }

        internal async Task<IdentityResult> UpdateProfilePicture(string filename, Stream inputStream, ApplicationUser user)
        {
            // User.Identity.GetUserId() is null when this method called from ExternalLoginConfirmation. 
            // Thats why UserPath property is not used here and `user` exists in method's parameters
            var userPath = Path.Combine("~/Content/thumbs/user/", user.UserName);
            var userPathLocal = Server.MapPath(userPath);
            try
            {
                if (!Directory.Exists(userPathLocal))
                {
                    Directory.CreateDirectory(userPathLocal);
                }
            }
            catch (IOException ioex)
            {
                //TODO: Add NLog here
                Console.WriteLine(ioex.Message);
            }

            // Delete old file
            if (user.ImageFileName != null)
            {
                var oldFilePath = Path.Combine(userPathLocal, user.ImageFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Save new file
            Debug.Assert(filename != null);
            using (var fileStream = System.IO.File.Create(Path.Combine(userPathLocal, Path.GetFileName(filename))))
            {
                inputStream.CopyTo(fileStream);
                fileStream.Close();
            }
            
            user.ImageFileName = filename;
            Session["imageFileName"] = Path.Combine(userPath, filename);
            var updateResult = await UserManager.UpdateAsync(user);
            return updateResult;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Deactivate(DeactivateViewModel model)
        {
            // TODO: Add removing from Roles, Logins, Users posts. Store model data (Reason) somewhere in database
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    var result = await UserManager.DeleteAsync(user);
                    AddErrors(result);
                }
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            }
            return Json(ModelState.IsValid ? JsonResponseFactory.SuccessResponse() : 
                JsonResponseFactory.ErrorResponse(ModelState.Where(pair => pair.Value.Errors.Count > 0).ToDictionary(pair => pair.Key, pair => pair.Value.Errors.Select(error => error.ErrorMessage))));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
#endregion
    }
}