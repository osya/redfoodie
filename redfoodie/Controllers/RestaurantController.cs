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

        private async Task<IEnumerable<RestaurantViewModel>> Search(string cuisineId = null, string cityId = null, string groupId = null, int? placeId = null, int? count = null)
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

            var restaurants = _db.Restaurants.Where(filterLambda).Include(r => r.Cuisines).Include(r => r.Groups);
            if (count != null)
                restaurants = restaurants.Take((int)count);

            return (await restaurants.ToArrayAsync()).Select(
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
        }

        // GET: Restaurants
        public async Task<ActionResult> Index(string cuisineId, string cityId, string groupId, int? placeId = null)
        {
            var currentCityId = cityId ?? (Session["currentCity"] as City)?.Id;
            var model = await Search(cuisineId, currentCityId, groupId, placeId);

            // Creating ViewBag.Title
            var sb = new StringBuilder();
            sb.Append(!string.IsNullOrEmpty(groupId) ? $"{_db.RestaurantGroups.Find(groupId).Name} restaurants" : "Restaurants");
            if (!string.IsNullOrEmpty(cuisineId))
            {
                sb.Append($" with {_db.Cuisines.Find(cuisineId).Name}");
            }
            if (placeId != null)
            {
                sb.Append($" in {_db.Places.Find(placeId).Name} place");
            }
            else if (!string.IsNullOrEmpty(currentCityId))
            {
                var cityName = string.IsNullOrEmpty(cityId) ||
                               (!string.IsNullOrEmpty(cityId) &&
                                string.Equals(cityId, (Session["currentCity"] as City)?.Id))
                    ? (Session["currentCity"] as City)?.Name
                    : _db.Cities.Find(cityId).Name;
                sb.Append($" in {cityName} city");
            }
            else
            {
                sb.Append($" in all cities");
            }
            ViewBag.Title = sb.ToString();

            return View(model);
        }

        public async Task<JsonResult> Group(string groupId, int? count = null)
        {
            return Json(JsonResponseFactory.SuccessResponse(await Search(groupId: groupId, count: count)), JsonRequestBehavior.AllowGet);
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

        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        public JsonResult Search(RestaurantSearchViewModel model)
        //        {
        //            // TODO: Implement logic. Отдельный контроллер для поиска, видимо, нужен, так как он будет возвращать возможно JSON
        //            return Json(ModelState.IsValid ? JsonResponseFactory.SuccessResponse() : JsonResponseFactory.ErrorResponse("Some error"));
        //        }

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
                Votes = restaurant.Votes.Select(v => new VoteViewModel { ReviewText = v.ReviewText, ApplicationUser = new UserViewModel { Id = v.ApplicationUser.Id, UserName = v.ApplicationUser.UserName, ImageFullFileName = v.ApplicationUser.ImageFullFileName } }).ToArray()
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
