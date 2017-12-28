using System.Web.Mvc;
using redfoodie.Models;

namespace redfoodie.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Post(MessagePostViewModel model)
        {
            // TODO: Implement logic
            return Json(!ModelState.IsValid ? JsonResponseFactory.SuccessResponse() : JsonResponseFactory.ErrorResponse("Some error"));
        }
    }
}