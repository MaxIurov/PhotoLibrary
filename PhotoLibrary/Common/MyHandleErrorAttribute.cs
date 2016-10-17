using System.Web.Mvc;

namespace PhotoLibrary.Common
{
    public class MyHandleErrorAttribute:HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = false;
        }
    }
}