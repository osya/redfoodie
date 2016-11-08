namespace redfoodie.Controllers
{
    public static class JsonResponseFactory
    {
        public static object ErrorResponse(string error)
        {
            return new { Success = false, ErrorMessage = error };
        }

        public static object ErrorResponse(object modelState)
        {
            return new { Success = false, ModelState = modelState };
        }

        public static object SuccessResponse()
        {
            return new { Success = true };
        }

        public static object SuccessResponse(object referenceObject)
        {
            return new { Success = true, Object = referenceObject };
        }
    }
}