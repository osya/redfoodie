using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class RestaurantController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Restaurants
//        public async Task<ActionResult> Index()
//        {
//            var restaurants = _db.Restaurants.Include(r => r.City);
//            return View(await restaurants.ToListAsync());
//        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Search(RestaurantSearchViewModel model)
        {
            // TODO: Implement logic
            return Json(ModelState.IsValid ? JsonResponseFactory.SuccessResponse() : JsonResponseFactory.ErrorResponse("Some error"));
        }

        // GET: Restaurants/Details/5
        public async Task<ActionResult> Details(string uniqueName)
        {
            if (string.IsNullOrEmpty(uniqueName))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var restaurant = _db.Restaurants.Where(m => string.Equals(m.UniqueName, uniqueName));
            
            if (!await restaurant.AnyAsync())
            {
                return HttpNotFound();
            }
            return View(restaurant.First());
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
