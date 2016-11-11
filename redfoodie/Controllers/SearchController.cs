using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace redfoodie.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Search(string searchTxt)
        {
            return Json(JsonResponseFactory.SuccessResponse());
        }
    }
}