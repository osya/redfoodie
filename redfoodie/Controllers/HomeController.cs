using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Geocoding.Google;
using Microsoft.AspNet.Identity;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    [AllowAnonymous]
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
                    var currentCity = Db.Cities.Find("DelhiNCR");
                    Session["currentCity"] = currentCity;
                    currentCityId = currentCity?.Id;
                }
                else
                {
                    if (!string.IsNullOrEmpty(cityId))
                    {
                        var currentCity = Db.Cities.Find(cityId);
                        Session["currentCity"] = currentCity;
                        currentCityId = currentCity?.Id;
                    }
                    else
                    {
                        if ((Session["currentCity"] == null) && User.Identity.IsAuthenticated)
                        {
                            var currentCity = Db.Users.Find(User.Identity.GetUserId())?.City;
                            Session["currentCity"] = currentCity;
                            currentCityId = currentCity?.Id;
                        }
                        else
                        {
                            currentCityId = (Session["currentCity"] as City)?.Id;
                        }
                    }
                }
            }

            ApplicationUser user = null;
            if (Request.IsAuthenticated)
            {
                user = Db.Users.Find(User.Identity.GetUserId());
            }

            var places = await Db.Places.Where(p => string.Equals(p.CityId, currentCityId) && p.Restaurants.Any()).OrderByDescending(o => o.Restaurants.Count).Take(11).ToArrayAsync();
            if (Session != null)
            {
                Session["popularLocations"] =
                    places.Take(5)
                        .Select(
                            p =>
                                new PlaceViewModel
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    City = new CityViewModel { Id = p.City.Id, Name = p.City.Name }
                                }).ToArray();
            }

            var rgKeys = new List<string>
            {
                "Trending",
                "NewlyOpened",
                "ButterChicken",
                "MexicanMagic",
                "Cafes",
                "BestBars",
                "Rooftops"
            };
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
                PlacesEven = places.Where((c, i) => i % 2 == 0).ToArray(),
                RestaurantGroups = rgKeys.Select(key => Db.RestaurantGroups.Find(key)).Select(restaurantGroup => new RestaurantGroupViewModel
                {
                    Id = restaurantGroup?.Id,
                    Name = restaurantGroup?.Name,
                    ImageFullFileName = restaurantGroup?.ImageFullFileName
                }).ToArray()
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

        public async Task<JsonResult> ReverseGeocode(double latitude, double longitude)
        {
            var geocoder = new GoogleGeocoder();
            var locRev = (await geocoder.ReverseGeocodeAsync(latitude, longitude))
                .Where(a => a.Type == GoogleAddressType.Political)
                .LastOrDefault(a => a.Components.Any(c => c.Types.First() == GoogleAddressType.Locality))
                ?.Components?.Take(2).Select(c => c.LongName);
            if (locRev == null) return Json(JsonResponseFactory.ErrorResponse("City not found"), JsonRequestBehavior.AllowGet);
            var locArr = locRev.ToArray();
            var cityId = locArr[1].Replace("New Delhi", "DelhiNCR");
            var placeName = locArr[0];
            var place = await Db.Cities.Where(c => string.Equals(c.Id, cityId))
                .Select(
                    c =>
                        new PlaceViewModel
                        {
                            Name = placeName,
                            City = new CityViewModel {Id = c.Id, Name = c.Name}
                        }).FirstOrDefaultAsync();            
            return Json(place != null? JsonResponseFactory.SuccessResponse(place): JsonResponseFactory.ErrorResponse("City not found"), JsonRequestBehavior.AllowGet);
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