using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatura.UI.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ViewResult Http404()
        {
            Exception custException = new Exception("Safya bulunamadı.");
            var model = new HandleErrorInfo(custException, "404", "404");
            return View("Error", model);
        }
        [AllowAnonymous]
        public ViewResult Http403()
        {
            Exception custException = new Exception("Bu sayfayı görüntülemeye yetkiniz yok.");
            var model = new HandleErrorInfo(custException, "403", "403");
            return View("Error", model);
        }
    }
}