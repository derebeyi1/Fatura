using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI;
using Fatura.UI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fatura.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        faturaEntities db = new faturaEntities();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalFilters.Filters.Add(new AuthorizeAttribute());
            //GlobalFilters.Filters.Add(new ExceptionHandler());
            //GlobalFilters.Filters.Add(new ElmahExceptionFilter());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError(); //Oluşan hatayı değişkene atadık.
                                                   //Eğer hata kaydı (log) tutulacak ise gerekli kodlar buraya.
            var httpException = exception as HttpException;
            Response.Clear();
            Server.ClearError(); //Sunucudaki hatayı temizledik.
            Response.TrySkipIisCustomErrors = true; //IIS'in tipik hata sayfalarını görmezden geldik.
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error"; //Hata mesajlarını yöneteceğimiz Controller ismi
            routeData.Values["action"] = "Index"; //Controller içindeki default Action ismi
            routeData.Values["exception"] = exception;
            Response.StatusCode = 500;

            Logger.log(exception, "", "", "", "", "", "Global.asax.cs/Application_Error");

            if (httpException != null)
            {
                Response.StatusCode = httpException.GetHttpCode();

                switch (Response.StatusCode)
                {
                    case 403: //Eğer 403 hatası meydana gelmiş ise Http403 Action'ı devreye girecek.
                        routeData.Values["action"] = "Http403";
                        break;

                    case 404: //Eğer 404 hatası meydana gelmiş ise Http404 Action'ı devreye girecek.
                        routeData.Values["action"] = "Http404";
                        break;
                }
            }
            IController errorsController = new Controllers.ErrorController();
            var rc = new RequestContext(new HttpContextWrapper(Context), routeData);
            errorsController.Execute(rc);
        }
    }
}
