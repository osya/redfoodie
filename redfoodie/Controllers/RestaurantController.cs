using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, BindingFlags flags)
        {
            var typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        private static bool IsIEnumerable(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            var t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            return t[0];
        }

        /// <summary>
        /// based on http://stackoverflow.com/questions/326321/how-do-i-create-an-expression-tree-calling-ienumerabletsource-any
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static Expression CallAny(Expression collection, Expression predicate)
        {
            var cType = GetIEnumerableImpl(collection.Type);
            collection = Expression.Convert(collection, cType);

            var elemType = cType.GetGenericArguments()[0];
            var predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

            // Enumerable.Any<T>(IEnumerable<T>, Func<T,bool>)
            var anyMethod = (MethodInfo)
                GetGenericMethod(typeof(Enumerable), "Any", new[] { elemType },
                    new[] { cType, predType }, BindingFlags.Static);

            return Expression.Call(
                anyMethod,
                    collection,
                        predicate);
        }

        /// <summary>
        /// Search of Restaurants by different filters
        /// </summary>
        /// <param name="cuisineId"></param>
        /// <param name="cityId"></param>
        /// <param name="groupId"></param>
        /// <param name="placeId"></param>
        /// <param name="count"></param>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        private IQueryable<Restaurant> _SearchBusiness(string cuisineId = null, string cityId = null, string groupId = null, int? placeId = null, int? count = null, string searchWord = null)
        {
            var rParam = Expression.Parameter(typeof(Restaurant), "r");
            var filterLambda = placeId != null
                ? Expression.Lambda<Func<Restaurant, bool>>(
                    Expression.Equal(
                        Expression.Property(Expression.Property(rParam, "Place"), "Id"),
                        Expression.Constant(placeId)
                    ),
                    rParam
                )
                : !string.IsNullOrEmpty(cityId)
                    ? Expression.Lambda<Func<Restaurant, bool>>(
                        Expression.Equal(
                            Expression.Property(Expression.Property(Expression.Property(rParam, "Place"), "City"), "Id"),
                            Expression.Constant(cityId)
                        ),
                        rParam
                    )
                    : Expression.Lambda<Func<Restaurant, bool>>(Expression.Constant(true), rParam);

            if (!string.IsNullOrEmpty(cuisineId))
            {
                var cParam = Expression.Parameter(typeof(Cuisine), "c");
                var anyPredicatExpr = Expression.Call(
                    null,
                    typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }),
                    Expression.Property(cParam, "Id"),
                    Expression.Constant(cuisineId));

                var cuiPredicate = Expression.Lambda(anyPredicatExpr, cParam);
                var cuiExpr = CallAny(Expression.Property(rParam, "Cuisines"), cuiPredicate);
                var cuiLambda = Expression.Lambda<Func<Restaurant, bool>>(cuiExpr, rParam);
                filterLambda = placeId != null || !string.IsNullOrEmpty(cityId) ?
                    Expression.Lambda<Func<Restaurant, bool>>(Expression.AndAlso(filterLambda.Body, cuiLambda.Body), rParam) : cuiLambda;
            }

            if (!string.IsNullOrEmpty(groupId))
            {
                var gParam = Expression.Parameter(typeof(RestaurantGroup), "g");
                var anyPredicatExpr = Expression.Call(
                    null,
                    typeof(string).GetMethod("Equals", new[] { typeof(string), typeof(string) }),
                    Expression.Property(gParam, "Id"),
                    Expression.Constant(groupId));

                var groupPredicate = Expression.Lambda(anyPredicatExpr, gParam);
                var groupExpr = CallAny(Expression.Property(rParam, "Groups"), groupPredicate);
                var groupLambda = Expression.Lambda<Func<Restaurant, bool>>(groupExpr, rParam);
                filterLambda = placeId != null || !string.IsNullOrEmpty(cityId) || !string.IsNullOrEmpty(cuisineId) ?
                    Expression.Lambda<Func<Restaurant, bool>>(Expression.AndAlso(filterLambda.Body, groupLambda.Body), rParam) :
                    groupLambda;
            }

            if (!string.IsNullOrEmpty(searchWord))
            {
                var keywordExpr = Expression.Call(
                    Expression.Property(rParam, "Name"),
                    typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                    Expression.Constant(searchWord));
                var keywordLambda = Expression.Lambda<Func<Restaurant, bool>>(keywordExpr, rParam);
                filterLambda = placeId != null || !string.IsNullOrEmpty(cityId) || !string.IsNullOrEmpty(cuisineId) || !string.IsNullOrEmpty(groupId) ?
                    Expression.Lambda<Func<Restaurant, bool>>(Expression.AndAlso(filterLambda.Body, keywordLambda.Body), rParam) :
                    keywordLambda;
            }

            var restaurants = _db.Restaurants.Where(filterLambda).Include(r => r.Cuisines).Include(r => r.Groups);
            if (count != null)
                restaurants = restaurants.Take((int)count);

            return restaurants;
        }

        /// <summary>
        /// It is in separate methid to avoid Implicitly captured closure warning
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        private IQueryable<Cuisine> _searchCuisines(string searchWord)
        {
            return _db.Cuisines.Where(c => c.Name.Contains(searchWord));
        }

        // GET: Restaurants
        public async Task<ActionResult> Index(RestaurantSearchViewModel modelIn)
        {
            // TODO: In case of "Detect current location" searchTxtLoc value passed here, but not processed. Because it is not PlaceId. It shold be somehow matched to PlaceId, but it is messy
            var currentCityId = modelIn.CityId ?? (Session["currentCity"] as City)?.Id;
            var model = (await _SearchBusiness(modelIn.CuisineId, currentCityId, modelIn.GroupId, modelIn.PlaceId).ToArrayAsync()).Select(
                    r =>
                        new RestaurantViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            ImageFullFileName = r.ImageFullFileName,
                            Place = new PlaceViewModel { Id = r.Place.Id, Name = r.Place.Name, City = new CityViewModel { Id = r.Place.City.Id, Name = r.Place.City.Name } },
                            Cuisines = r.Cuisines.Select(c => new CuisineViewModel { Name = c.Name }).ToArray(),
                            Groups = r.Groups.Select(g => new RestaurantGroupViewModel { Id = g.Id, Name = g.Name}).ToArray(),
                            Votes = r.Votes.Select(v => new VoteViewModel { ReviewText = v.ReviewText, ApplicationUser = new UserViewModel { Id = v.ApplicationUser.Id, UserName = v.ApplicationUser.UserName, ImageFullFileName = v.ApplicationUser.ImageFullFileName, ReviewsCount = v.ApplicationUser.Votes.Count } }).OrderByDescending(o => o.ApplicationUser.ReviewsCount).ToArray(),
                            PercentRate = r.PercentRate
                        }).OrderByDescending(o => o.PercentRate);

            // Creating ViewBag.Title
            var sb = new StringBuilder();
            sb.Append(!string.IsNullOrEmpty(modelIn.GroupId) ? $"{_db.RestaurantGroups.Find(modelIn.GroupId).Name} restaurants" : "Restaurants");
            if (!string.IsNullOrEmpty(modelIn.CuisineId))
            {
                sb.Append($" with {_db.Cuisines.Find(modelIn.CuisineId).Name}");
            }
            if (modelIn.PlaceId != null)
            {
                sb.Append($" in {_db.Places.Find(modelIn.PlaceId).Name}");
            }
            else if (!string.IsNullOrEmpty(currentCityId))
            {
                var cityName = string.IsNullOrEmpty(modelIn.CityId) ||
                               (!string.IsNullOrEmpty(modelIn.CityId) &&
                                string.Equals(modelIn.CityId, (Session["currentCity"] as City)?.Id))
                    ? (Session["currentCity"] as City)?.Name
                    : _db.Cities.Find(modelIn.CityId).Name;
                sb.Append($" in {cityName} city");
            }
            else
            {
                sb.Append($" in all cities");
            }
            ViewBag.Title = sb.ToString();

            return View(model);
        }

        public async Task<JsonResult> SearchBusiness(string cityId = null, string groupId = null, int? count = null, string searchWord = null)
        {
            var currentCityId = cityId ?? (Session["currentCity"] as City)?.Id;
            var restaurants = (await _SearchBusiness(cityId: currentCityId, groupId: groupId, count: count, searchWord: searchWord).ToArrayAsync()).Select(
                    r =>
                        new RestaurantViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            ImageFullFileName = r.ImageFullFileName,
                            Place = new PlaceViewModel { Id = r.Place.Id, Name = r.Place.Name, City = new CityViewModel { Id = r.Place.City.Id, Name = r.Place.City.Name } },
                            Cuisines = r.Cuisines.Select(c => new CuisineViewModel { Name = c.Name }).ToArray(),
                            Groups = r.Groups.Select(g => new RestaurantGroupViewModel { Id = g.Id, Name = g.Name }).ToArray(),
                            Votes = r.Votes.Select(v => new VoteViewModel { ReviewText = v.ReviewText, ApplicationUser = new UserViewModel { Id = v.ApplicationUser.Id, UserName = v.ApplicationUser.UserName, ImageFullFileName = v.ApplicationUser.ImageFullFileName, ReviewsCount = v.ApplicationUser.Votes.Count } }).OrderByDescending(o => o.ApplicationUser.ReviewsCount).ToArray(),
                            PercentRate = r.PercentRate
                        }).OrderByDescending(o => o.PercentRate);

            return Json(JsonResponseFactory.SuccessResponse(restaurants), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> SearchByKeyword(string searchWord = null, string cityId = null, string groupId = null, string type = null)
        {
            var currentCityId = cityId ?? (Session["currentCity"] as City)?.Id;
            var places = _db.Places.Where(r => r.Name.Contains(searchWord) && string.Equals(r.City.Id, currentCityId))
                .Select(p => new SerpItem
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Type = SerpItemType.Neighbourhood.ToString(),
                    PlaceName = string.Empty
                });

            var res = await (string.Equals(type, "loc") ? places : 
                places
                .Union(_SearchBusiness(cityId: currentCityId, searchWord: searchWord, groupId: groupId).Select(r => new SerpItem
                    {
                        Id = r.Id.ToString(),
                        Name = r.Name,
                        Type = SerpItemType.Business.ToString(),
                        PlaceName = r.Place.Name
                    }))
                .Union(_searchCuisines(searchWord).Select(c => new SerpItem
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Type = SerpItemType.Cuisine.ToString(),
                        PlaceName = string.Empty
                    }))
                ).ToArrayAsync();

            return Json(JsonResponseFactory.SuccessResponse(res), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult>RestaurantGroupsList()
        {
            var model =
                (await _db.RestaurantGroups.ToArrayAsync()).Select(r => new RestaurantGroupViewModel {Id = r.Id, Name = r.Name, ImageFullFileName = r.ImageFullFileName }).ToArray();
            return View(model);
        }

        public ActionResult CuisinesList()
        {
            var model = _db.Cuisines.ToArray();
            return View(model);
        }

        // GET: Restaurants/Details/5
        public async Task<ActionResult> Details(int restaurantId)
        {
            var restaurant = await _db.Restaurants.FindAsync(restaurantId);
            
            if (restaurant == null)
            {
                return HttpNotFound();
            }
            return View(new RestaurantViewModel
            {
                Id = restaurant.Id, Name = restaurant.Name, ImageFullFileName = restaurant.ImageFullFileName, Place = new PlaceViewModel { Id = restaurant.Place.Id, Name = restaurant.Place.Name, City = new CityViewModel { Id = restaurant.Place.City.Id, Name = restaurant.Place.City.Name } },
                Groups = restaurant.Groups.Select(g => new RestaurantGroupViewModel { Id = g.Id, Name = g.Name}).ToArray(),
                Votes = restaurant.Votes.Select(v => new VoteViewModel { ReviewText = v.ReviewText, ApplicationUser = new UserViewModel { Id = v.ApplicationUser.Id, UserName = v.ApplicationUser.UserName, ImageFullFileName = v.ApplicationUser.ImageFullFileName } }).ToArray(),
                Location = restaurant.Location
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
