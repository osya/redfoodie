using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class HomeController: Controller
    {
        public ApplicationDbContext Context = new ApplicationDbContext();

        public ActionResult Index(int? cityId = null)
        {
            if (Session != null)
            {
                if ((cityId == null) && !User.Identity.IsAuthenticated)
                {
                    Session["currentCity"] = Context.Cities.First(x => x.ParentId == null);
                }
                else
                {
                    if (cityId != null)
                    {
                        Session["currentCity"] = Context.Cities.Find(cityId);
                    }
                    else
                    {
                        if ((Session["currentCity"] == null) && User.Identity.IsAuthenticated)
                        {
                            Session["currentCity"] = Context.Users.Find(User.Identity.GetUserId())?.City;
                        }
                    }
                }
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