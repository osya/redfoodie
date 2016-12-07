using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<ActionResult> Index(string cityId = null)
        {
            var currentCityId = cityId;
            if (Session != null)
            {
                if (string.IsNullOrEmpty(cityId) && !User.Identity.IsAuthenticated)
                {
                    var currentCity = Db.Cities.First();
                    Session["currentCity"] = currentCity;
                    currentCityId = currentCity.Id;
                }
                else
                {
                    if (!string.IsNullOrEmpty(cityId))
                    {
                        var currentCity = Db.Cities.Find(cityId);
                        Session["currentCity"] = currentCity;
                        currentCityId = currentCity.Id;
                    }
                    else
                    {
                        if ((Session["currentCity"] == null) && User.Identity.IsAuthenticated)
                        {
                            var currentCity = Db.Users.Find(User.Identity.GetUserId()).City;
                            Session["currentCity"] = currentCity;
                            currentCityId = currentCity.Id;
                        }
                    }
                }
            }

            ApplicationUser user = null;
            if (Request.IsAuthenticated)
            {
                user = Db.Users.Find(User.Identity.GetUserId());
            }

            var places = await Db.Places.Where(p => p.CityId == currentCityId && p.Restaurants.Any()).OrderByDescending(o => o.Restaurants.Count).Take(11).ToArrayAsync();
            return View(new HomeViewModel
            {
                SuggestedUsers = (await Db.Users.OrderByDescending(m => m.Votes.Count)
                        .Take(3)
                        .ToArrayAsync())
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
                        .ToArray(),
                CuisinesOdd = new[] { await Db.Cuisines.FindAsync("NorthIndian"), await Db.Cuisines.FindAsync("FastFood"), await Db.Cuisines.FindAsync("SouthIndian"), await Db.Cuisines.FindAsync("StreetFood") },
                CuisinesEven = new[] { await Db.Cuisines.FindAsync("Chinese"), await Db.Cuisines.FindAsync("Desserts"), await Db.Cuisines.FindAsync("Mughlai"), await Db.Cuisines.FindAsync("Bakery") },
                PlacesOdd = places.Where((c, i) => i % 2 != 0).ToArray(),
                PlacesEven = places.Where((c, i) => i % 2 == 0).ToArray()
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

        public async Task<ActionResult> PlacesList()
        {
            var currentCity = Session["currentCity"] as City;
            var places = await Db.Places.Where(p => p.CityId == currentCity.Id && p.Restaurants.Any()).ToArrayAsync();
            var alphaPlaces = places.GroupBy(g => g.Name[0]).ToDictionary(t => t.Key, t => t.ToArray());
            return View(alphaPlaces);
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