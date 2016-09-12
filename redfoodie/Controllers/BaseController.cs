using System.Linq;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class BaseController: Controller
    {
        protected readonly ApplicationDbContext Context = new ApplicationDbContext();

        protected BaseViewModel UpdateBaseViewModel(City city)
        {
            var currentBaseViewModel = new BaseViewModel
            {
                CityId = city.Id,
                Href = city.Href,
                Name = city.Name,
                Cities = Context.Cities.Where(x => x.Id != city.Id)
            };
            if (Session != null)
            {
                Session["currentBaseViewModel"] = currentBaseViewModel;
            }
            return currentBaseViewModel;
        }

        protected BaseViewModel CurrentViewModel
        {
            get
            {
                if (Session?["currentBaseViewModel"] != null)
                {
                    return (BaseViewModel)Session["currentBaseViewModel"];
                }
                var currentCity = Context.Cities.First(x => x.ParentId == null);
                if (Session != null && Session["currentCity"] == null)
                {
                    Session["currentCity"] = currentCity;
                }
                return UpdateBaseViewModel(currentCity);
            }
        }
    }
}