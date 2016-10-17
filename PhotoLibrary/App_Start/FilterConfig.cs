using System.Web.Mvc;
using PhotoLibrary.Common;

namespace PhotoLibrary
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MyHandleErrorAttribute());
            filters.Add(new LogToDataBase());
        }
    }
}