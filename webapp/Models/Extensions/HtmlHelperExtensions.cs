using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace eSPP.Models.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString AutoSizedTextBoxFor<TModel, TProperty>
            (this HtmlHelper<TModel> helper,Expression<Func<TModel, TProperty>> expression, 
            object htmlAttributes)
        {
            var attributes = new Dictionary<string, Object>();
            var memberAccessExpression = (MemberExpression)expression.Body;
            var stringLengthAttribs = memberAccessExpression.Member.GetCustomAttributes(
              typeof(StringLengthAttribute), true);

            if (stringLengthAttribs.Length > 0)
            {
                var length = ((StringLengthAttribute)stringLengthAttribs[0]).MaximumLength;

                if (length > 0)
                {
                    attributes.Add("size", length);
                    attributes.Add("maxlength", length);
                }
            }

            RouteValueDictionary htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var thisAttribute in htmlAttrs)
            {
                attributes.Add(thisAttribute.Key, thisAttribute.Value.ToString());
            }

            return helper.TextBoxFor(expression, attributes);
        }
    }
}