using eSPP.Models.RoleManagement;
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
        //sample code
        #region Sample Code
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

            attributes = GetHtmlAttribute(htmlAttributes);

            return helper.TextBoxFor(expression, attributes);
        }

        private static Dictionary<string, Object> GetHtmlAttribute(object htmlAttributes)
        {
            var attributes = new Dictionary<string, Object>();
            if (htmlAttributes != null)
            {
                RouteValueDictionary htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var thisAttribute in htmlAttrs)
                {
                    attributes.Add(thisAttribute.Key, thisAttribute.Value.ToString());
                }
            }

            //var object1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            //    .Where(s => s.Key == "htmlAttributes").FirstOrDefault();

            //if(object1.Key != null)
            //{
            //    RouteValueDictionary htmlAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(object1.Value);

            //    foreach (var thisAttribute in htmlAttrs)
            //    {
            //        attributes.Add(thisAttribute.Key, thisAttribute.Value.ToString());
            //    }
            //}
            return attributes;
        }

        #endregion

        public static MvcHtmlString RoleTextBoxFor<TModel, TProperty>
            (this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression,
            RoleManager roleManager, string htmlName, string moduleName, object htmlAttributes)
        {
            var attributes = new Dictionary<string, Object>();

            attributes = GetHtmlAttribute(htmlAttributes);

            HtmlRole role = roleManager.GetHtmlRole(htmlName, moduleName);

            if(role.ViewLevel == ViewLevel.NoAccess)
            {
                attributes.Add("style", "display: none");
            }
            else if(role.ViewLevel == ViewLevel.View)
            {
                attributes.Add("disabled", "disabled");
            }
            else
            {
                attributes.Remove("disabled");
            }

            return helper.TextBoxFor(expression, attributes);
        }

        public static MvcHtmlString Button(this HtmlHelper helper,
                                     string innerHtml,
                                     object htmlAttributes)
        {
            return Button(helper, innerHtml,
                          HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
            );
        }

        public static MvcHtmlString Button(this HtmlHelper helper,
                                    string innerHtml,
                                    IDictionary<string, object> htmlAttributes)
        {
            var builder = new TagBuilder("button");
            builder.InnerHtml = innerHtml;
            builder.MergeAttributes(htmlAttributes);
            return MvcHtmlString.Create(builder.ToString());
        }
    }

}