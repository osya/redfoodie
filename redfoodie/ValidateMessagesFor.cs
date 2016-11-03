using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace redfoodie
{
    public static class ValidateMessagesFor
    {
        public static MvcHtmlString ValidationMessagesFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            var propertyName = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).PropertyName;
            var modelState = htmlHelper.ViewData.ModelState;

            
            if (!modelState.ContainsKey(propertyName) || modelState[propertyName].Errors.Count <= 1)
                return htmlHelper.ValidationMessageFor(expression, null,
                    htmlAttributes as IDictionary<string, object> ?? htmlAttributes);
            var msgs = new StringBuilder();
            foreach (var error in modelState[propertyName].Errors)
            {
                msgs.AppendLine(error.ErrorMessage);
            }

            // Return standard ValidationMessageFor, overriding the message with our concatenated list of messages.
            return htmlHelper.ValidationMessageFor(expression, msgs.ToString(), htmlAttributes as IDictionary<string, object> ?? htmlAttributes);
        }
    }
}