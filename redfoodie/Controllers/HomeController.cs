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
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var defaultCity = context.Cities.First(x => x.ParentId == null);

            var redFoodieViewModel = new RedFoodieViewModel
            {
                CityId = defaultCity.Id,
                Href = defaultCity.Href,
                Name = defaultCity.Name,
                Cities = context.Cities.Where(x => x.ParentId != null)
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