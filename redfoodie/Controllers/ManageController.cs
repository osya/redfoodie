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
using RazorEngine;
using RazorEngine.Templating;

namespace redfoodie.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db = new ApplicationDbContext();

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
            var user = await UserManager.FindByIdAsync(userId);
            var model = new ProfileSettingsViewModel
            {
                UserName = User.Identity.GetUserName(),
                Cities = (Session["citiesList"] as City[])?.Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }),
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
        public async Task<ActionResult> ViewProfile(string userId, string shortUrl)
        {   
            ApplicationUser user;
            Tuple<int, int, int, int, RestaurantViewModel[]> likes;
            if (!string.IsNullOrEmpty(userId) && string.Equals(userId, User.Identity.GetUserId()))
            {
                user = await UserManager.FindByIdAsync(userId);
                if (user == null) return View("Error");
                likes = await CalculateLikes(user);
                return View(new ViewProfileViewModel
                {
                    User = user,
                    Likes = likes.Item1,
                    Dislikes = likes.Item2,
                    LikesPercent = likes.Item3,
                    DislikesPercent = likes.Item4,
                    LikesRestaurant = likes.Item5,
                    SuggestedUsers = (await _db.Users.Where(m => !string.Equals(m.Id, user.Id)).OrderBy(m => Guid.NewGuid()).Take(4).ToArrayAsync()).Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray(),
                    Followers = (await _db.Users.Where(u => u.Follows.Any(f => string.Equals(f.FollowUserId, user.Id))).ToArrayAsync()).Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray(),
                    Followings = user.Follows.Select(m => m.FollowUser).ToArray().Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = true }).ToArray()
                });
            }

            // UserManager not used here because searching by ShortUrl
            var userQ = _db.Users.Where(m => (!string.IsNullOrEmpty(userId) && string.Equals(m.Id, userId)) || (!string.IsNullOrEmpty(shortUrl) && string.Equals(m.ShortUrl, shortUrl)));
            if (!await userQ.AnyAsync() || await userQ.CountAsync() > 1) return View("Error");
            user = userQ.First();
            if (user == null) return View("Error");  
            likes = await CalculateLikes(user);
            return View(new ViewProfileViewModel
            {
                User = user,
                Likes = likes.Item1,
                Dislikes = likes.Item2,
                LikesPercent = likes.Item3,
                DislikesPercent = likes.Item4,
                LikesRestaurant = likes.Item5,
                SuggestedUsers = (await _db.Users.Where(m => m.Id != user.Id).OrderBy(m => Guid.NewGuid()).Take(4).ToArrayAsync()).Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray(),
                Followers = (await _db.Users.Where(u => u.Follows.Any(f => string.Equals(f.FollowUserId, user.Id))).ToArrayAsync()).Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray(),
                Followings = user.Follows.Select(m => m.User).ToArray().Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = true }).ToArray()
            });
        }

        public async Task<JsonResult> Follow(string userId)
        {
            var userQ = _db.Users.Where(m => string.Equals(m.Id, userId));
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (!await userQ.AnyAsync() || await userQ.CountAsync() > 1 || user == null) return Json(JsonResponseFactory.ErrorResponse("User not found or there are many users with these userName and Year of Birth")); ;
            var followUser = userQ.First();

            var follow = user.Follows.Any(m => string.Equals(m.FollowUserId, followUser.Id));
            if (follow)
            {
                var rec = _db.Follows.First(m => string.Equals(m.FollowUserId, followUser.Id));
                _db.Follows.Remove(rec);
                await _db.SaveChangesAsync();
                return Json(JsonResponseFactory.SuccessResponse(new {Follow = false} ));
            }
            user.Follows.Add(new Follow { FollowUserId = followUser.Id });
            var updateResult = await UserManager.UpdateAsync(user);
            return Json(updateResult.Succeeded ?
                JsonResponseFactory.SuccessResponse(new { Follow = true }) :
                JsonResponseFactory.ErrorResponse(new { Follow = false, updateResult.Errors }));
        }

        private async Task<Tuple<int, int, int, int, RestaurantViewModel[]>> CalculateLikes(ApplicationUser user)
        {
            var likesRestaurants = (await _db.Restaurants.Where(r => r.Votes.Any(v => v.Value && string.Equals(v.UserId, user.Id))).
                ToArrayAsync()).Select(m => new RestaurantViewModel {Id = m.Id, Name = m.Name,
                    PercentRate = m.PercentRate,
                    ImageFullFileName = m.ImageFullFileName, Place = new PlaceViewModel { Id = m.Place.Id, Name = m.Place.Name, City = new CityViewModel { Id = m.Place.City.Id, Name = m.Place.City.Name } },
                    Votes = m.Votes.Select(v => new VoteViewModel { ReviewText = v.ReviewText, ApplicationUser = new UserViewModel { Id = v.ApplicationUser.Id, UserName = v.ApplicationUser.UserName, ImageFullFileName = v.ApplicationUser.ImageFullFileName } }).ToArray()
                }).ToArray();
            var likes = likesRestaurants.Length;
            var total = user.Votes.Count;
            var dislikes = total - likes;
            var likesPercent = (int) (likes * 100.0 / total);
            var dislikesPercent = 100 - likesPercent;

            return new Tuple<int, int, int, int, RestaurantViewModel[]>(likes, dislikes, likesPercent, dislikesPercent, likesRestaurants);
        }

        public async Task<ActionResult> InviteFriends()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            return View(new InviteFriendsViewModel
            {
                SuggestedUsers = _db.Users.Where(m => !string.Equals(m.Id, user.Id)).OrderBy(m => Guid.NewGuid()).Take(8).ToArray().Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray()
            });
        }

        public async Task<JsonResult> SendInvitationEmail(InviteFriendsViewModel model)
        {
            var emailService = new EmailService();

            string body;
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var userModel = new UserViewModel { Id = user.Id, UserName = user.UserName, ImageFullFileName = user.ImageFullFileName };
            using (var sr = new StreamReader(Server.MapPath("\\Views\\InvitationEmail.cshtml")))
            {
                var source = sr.ReadToEnd();
                body = Engine.Razor.RunCompile(source, "InvitationEmail", typeof(UserViewModel), userModel);
            }
            await emailService.SendAsync(new IdentityMessage
            {
                Destination = model.Email,
                Subject = "E mail Invitation From Redfoodie",
                Body = body
            });
            return Json(JsonResponseFactory.SuccessResponse("Ok"));
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
            var userPathLocal = Server.MapPath(user.UserPath);
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
                var oldFilePath = Server.MapPath(user.ImageFullFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Save new file
            user.ImageFileName = filename;
            using (var fileStream = System.IO.File.Create(Server.MapPath(user.ImageFullFileName)))
            {
                inputStream.CopyTo(fileStream);
                fileStream.Close();
            }
            
            Session["imageFileName"] = user.ImageFullFileName;
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

        /// <summary>
        /// View all User's notifications
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SeeAll()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
 
            return View(new SuggestedUsersViewModel
            {
                SuggestedUsers = _db.Users.Where(m => !string.Equals(m.Id, user.Id)).OrderBy(m => Guid.NewGuid()).Take(4).ToArray().Select(m => new UserViewModel { Id = m.Id, UserName = m.UserName, ImageFullFileName = m.ImageFullFileName, Verified = m.Verified, Follow = user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id)) }).ToArray()
            });
        }
    
        public async Task<JsonResult> SearchUsers(InviteFriendsViewModel model)
        {
            var users = (await _db.Users.Where(u => u.UserName.Contains(model.SearchUsers)).ToArrayAsync()).Select(u => new UserViewModel { Id = u.Id, UserName = u.UserName, ImageFullFileName = u.ImageFullFileName }).ToArray();

            return Json(JsonResponseFactory.SuccessResponse(users));
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
                    _userManager = null;
                }
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
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