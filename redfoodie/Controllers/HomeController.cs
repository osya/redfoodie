using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class HomeController: Controller
    {
        private ApplicationDbContext _db;

        public ApplicationDbContext Db
        {
            private get { return _db ?? (_db = new ApplicationDbContext()); }
            set { _db = value; }
        }

        public ActionResult Index(int? cityId = null)
        {
            if (Session != null)
            {
                if ((cityId == null) && !User.Identity.IsAuthenticated)
                {
                    Session["currentCity"] = Db.Cities.First();
                }
                else
                {
                    if (cityId != null)
                    {
                        Session["currentCity"] = Db.Cities.Find(cityId);
                    }
                    else
                    {
                        if ((Session["currentCity"] == null) && User.Identity.IsAuthenticated)
                        {
                            Session["currentCity"] = Db.Users.Find(User.Identity.GetUserId())?.City;
                        }
                    }
                }
            }

            ApplicationUser user = null;
            if (Request.IsAuthenticated)
            {
                user = Db.Users.Find(User.Identity.GetUserId());
            }

            return View(new HomeViewModel
            {
                SuggestedUsers = Db.Users.OrderByDescending(m => m.Votes.Count)
                        .Take(3)
                        .ToArray()
                        .Select(
                            m =>
                                new UserViewModel
                                {
                                    Id = m.Id,
                                    UserName = m.UserName,
                                    ImageFullFileName = m.ImageFullFileName,
                                    Verified = m.Verified,
                                    Follow = user != null && user.Follows.Any(f => string.Equals(f.FollowUserId, m.Id))
                                })
                        .ToArray()
            });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult Subscribe()
        {
            return Json(JsonResponseFactory.SuccessResponse());
        }

        public ActionResult Verified()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}