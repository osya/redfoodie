using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        private BaseViewModel UpdateBaseViewModel(City city)
        {
            var currentBaseViewModel = new BaseViewModel
            {
                CityId = city.Id,
                Href = city.Href,
                Name = city.Name,
                Cities = _context.Cities.Where(x => x.Id != city.Id)
            };
            if (Session != null)
            {
                Session["currentBaseViewModel"] = currentBaseViewModel;
            }
            return currentBaseViewModel;
        }

        private BaseViewModel CurrentViewModel
        {
            get
            {
                if (Session?["currentBaseViewModel"] != null)
                {
                    return (BaseViewModel) Session["currentBaseViewModel"];
                }
                var currentCity = _context.Cities.First(x => x.ParentId == null);
                if (Session!= null && Session["currentCity"] == null)
                {
                    Session["currentCity"] = currentCity;
                }
                return UpdateBaseViewModel(currentCity);
            }
        }

        public ActionResult Index(string city = null)
        {
            var currentCity = city != null ? _context.Cities.First(x => x.Name == city) : _context.Cities.First(x => x.ParentId == null);
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