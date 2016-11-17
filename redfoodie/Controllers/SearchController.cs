using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Search(SearchViewModel model)
        {
            // TODO: Implement logic
            return Json(ModelState.IsValid ? JsonResponseFactory.SuccessResponse() : JsonResponseFactory.ErrorResponse("Some error"));
        }
    }
}