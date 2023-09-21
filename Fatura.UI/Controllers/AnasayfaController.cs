using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatura.UI.Controllers
{
    public class AnasayfaController : Controller
    {
        // GET: Anasayfa
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}