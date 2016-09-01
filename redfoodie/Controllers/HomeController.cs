using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string city)
        {
            var context = new ApplicationDbContext();

            var defaultCity = city != null ? context.Cities.First(x => x.Name == city) : context.Cities.First(x => x.ParentId == null);

            var redFoodieViewModel = new RedFoodieViewModel
            {
                CityId = defaultCity.Id,
                Href = defaultCity.Href,
                Name = defaultCity.Name,
                Cities = context.Cities.Where(x => x.Id != defaultCity.Id)
            };

            return View(redFoodieViewModel);
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
    }
}