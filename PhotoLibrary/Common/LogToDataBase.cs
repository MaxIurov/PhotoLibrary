using PhotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PhotoLibrary.Common
{
    public class LogToDataBase : ActionFilterAttribute, IExceptionFilter
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string myMessage = "";
            foreach (var parameter in filterContext.ActionParameters)
            {
                if (parameter.Key != null)
                {
                    myMessage = myMessage + " | key:" + parameter.Key.ToString();
                }
                if (parameter.Value != null)
                {
                    myMessage = myMessage + ", value: " + parameter.Value.ToString();
                }
            }
            LogMessage m = new LogMessage
            {
                Controller = filterContext.RouteData.Values["controller"].ToString(),
                Action = filterContext.RouteData.Values["action"].ToString(),
                Message = myMessage,
                Time = DateTime.Now
            };
            LogMessageToDB(m);
            base.OnActionExecuting(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            LogMessage m = new LogMessage
            {
                Controller = filterContext.RouteData.Values["controller"].ToString(),
                Action = filterContext.RouteData.Values["action"].ToString(),
                Message = "OnResultExecuted",
                Time = DateTime.Now
            };
            LogMessageToDB(m);
            base.OnResultExecuted(filterContext);
        }
        public void OnException(ExceptionContext context)
        {
            LogMessage m = new LogMessage
            {
                Controller = context.RouteData.Values["controller"].ToString(),
                Action = context.RouteData.Values["action"].ToString(),
                Message = context.Exception.Message,
                Time = DateTime.Now
            };
            LogMessageToDB(m);
        }
        private void LogMessageToDB(LogMessage mes)
        {
            db.LogMessages.Add(mes);
            db.SaveChanges();
        }
    }
}