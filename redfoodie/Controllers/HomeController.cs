using System.Linq;
using System.Web.Mvc;

namespace redfoodie.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string city = null)
        {
            var currentCity = city != null ? Context.Cities.First(x => x.Name == city) : Context.Cities.First(x => x.ParentId == null);
            if (Session != null)
            {
                Session["currentCity"] = currentCity;
            }
            return View(UpdateBaseViewModel(currentCity));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View(CurrentViewModel);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(CurrentViewModel);
        }
    }
}