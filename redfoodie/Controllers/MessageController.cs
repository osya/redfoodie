using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    public class MessageController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Post(MessagePostViewModel model)
        {
            return Json(JsonResponseFactory.SuccessResponse());
        }
    }
}