//using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatura.UI.Filters
{
    public class ExceptionHandler : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }
            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            Exception custException = filterContext.Exception;
            //Log.Error(filterContext.Exception.Message + " in " + controllerName);
            var model = new HandleErrorInfo(custException, controllerName, actionName);
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
        }
    }
}