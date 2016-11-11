using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace redfoodie.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        /// <summary>
        /// View all User's notifications
        /// </summary>
        /// <returns></returns>
        public ActionResult SeeAll()
        {
            return View();
        }
    }
}