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

        // GET: Restaurants
        public async Task<ActionResult> Index(string cuisineId, string cityId, int? placeId = null)
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
                    typeof(string).GetMethod("Equals", new[] {typeof(string), typeof(string)}),
                    Expression.Property(cParam, "Id"),
                    Expression.Constant(cuisineId));

                var cuiPredicate = Expression.Lambda(anyPredicatExpr, cParam);
                var cuiExpr = CallAny(Expression.Property(rParam, "Cuisines"), cuiPredicate);
                var cuiLambda = Expression.Lambda<Func<Restaurant, bool>>(cuiExpr, rParam);
                filterLambda = placeId != null || !string.IsNullOrEmpty(cityId) ? 
                    Expression.Lambda<Func<Restaurant, bool>>(Expression.AndAlso(filterLambda.Body, cuiLambda.Body), rParam) : cuiLambda;
            }

            var restaurants = await _db.Restaurants.Where(filterLambda).Include(r => r.Cuisines).ToArrayAsync();
            var model = restaurants.Select(r => new RestaurantViewModel {Id = r.Id, Name = r.Name, ImageFullFileName = r.ImageFullFileName, Place = r.Place, Cuisines = r.Cuisines });


            // Creating ViewBag.Title
            var sb = new StringBuilder();
            sb.Append("Restaurants");
            if (!string.IsNullOrEmpty(cuisineId))
            {
                sb.Append($" with {_db.Cuisines.Find(cuisineId).Name}");
            }
            if (placeId != null)
            {
                sb.Append($" in {_db.Places.Find(placeId).Name} place");
            }
            else if (!string.IsNullOrEmpty(cityId))
            {
                sb.Append($" in {_db.Cities.Find(cityId).Name} city");
            }
            else
            {
                sb.Append($" in all cities");
            }
            ViewBag.Title = sb.ToString();

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
            return View(new RestaurantViewModel { Id = restaurant.Id, Name = restaurant.Name, ImageFullFileName = restaurant.ImageFullFileName, Place = restaurant.Place });
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
