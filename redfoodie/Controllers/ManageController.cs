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
                user.FollowMail = model.FollowMail;
                user.ReplyReviewmail = model.ReplyReviewmail;
                user.ThanksFavoritemail = user.ThanksFavoritemail;
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

        public ActionResult ViewProfile()
        {
            return View();
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
        public JsonResult Deactivate(DeactivateViewModel model)
        {
            // TODO: Implement logic
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
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

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