using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PhotoLibrary.Helpers
{
    public static class MyLinkButtonHelper
    {
        public static MvcHtmlString MyLinkButton(this HtmlHelper html, string linkText, string action, string controller, object routeValues, bool enable = true)
        {
            UrlHelper u = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string url = "";
            if (routeValues==null)
            {
                url = u.Action(action, controller);
            }
            else
            {
                url = u.Action(action, controller, routeValues);
            }
            string strDisable = "";
            if (!enable) strDisable = "disabled ";
            string buttonTag = String.Format("<input type = \"button\" {0}value=\"{1}\" onclick=\"location.href='{2}'\" />",
                strDisable, linkText, url);
            return MvcHtmlString.Create(buttonTag);
        }
        public static MvcHtmlString MyLinkButton(this HtmlHelper html, string linkText, string action, string controller, bool enable = true)
        {
            return MyLinkButton(html, linkText, action, controller, null, enable);
        }
        //public static MvcHtmlString MyLinkButton(this HtmlHelper html, string linkText, string action, string controller, bool enable = true)
        //{
        //    string strDisable = "";
        //    if (!enable) strDisable = "disabled ";
        //    string buttonTag = String.Format("<input type = \"button\" {0}value=\"{1}\" onclick=\"location.href='/{2}/{3}'\" />",
        //        strDisable, linkText, controller, action);
        //    return MvcHtmlString.Create(buttonTag);
        //}
        //<input type = "button" disabled value = "Go" onclick="location.href='@Url.Action("Contact", "Home")'" />
    }
}