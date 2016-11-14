using System;
using System.Linq;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class HomeController: Controller
    {
        public ApplicationDbContext Context = new ApplicationDbContext();

        public ActionResult Index(string city = null)
        {
            if (Session != null)
            {
                Session["currentCity"] = city != null ? Context.Cities.First(x => x.Name.Equals(city, StringComparison.InvariantCultureIgnoreCase)).Name : Context.Cities.First(x => x.ParentId == null).Name;
            }
            return View();
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
    }
}