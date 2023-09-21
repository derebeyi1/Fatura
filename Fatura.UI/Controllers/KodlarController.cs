using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fatura.UI.Controllers
{
    public class KodlarController : Controller
    {
        faturaEntities db = null;
        // GET: Kodlar
        public ActionResult Index()
        {
            ViewBag.Tree = GetAllCategoriesForTree();
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult KodListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            //int a = 1;
            //int b = 0;
            //int c = 0;
            //c = a / b; // sıfıra bölme fatası
            //if (ModelState.IsValid)
            //{
            //    ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
            //    ViewBag.CodeSortParm = sortOrder == "kodu" ? "kodu_desc" : "kodu";
            //    ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
            //    ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
            //    ViewBag.SearchString = searchString;
            //    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
            //    long vkn = (long)Session["vkn_tckn"];
            //    long vknAdmin = Utilities.vknAdmin;
            //    var st = from s in db.carilars
            //             where s.isdeleted == false && vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn //yonetici ise bütün şirketler listelenir
            //             select s;
            //    if (!String.IsNullOrEmpty(searchString))
            //    {
            //        st = st.Where(s => s.adi.Contains(searchString));
            //    }
            //    switch (sortOrder)
            //    {
            //        case "id_desc":
            //            st = st.OrderByDescending(s => s.id);
            //            break;
            //        case "last_update_desc":
            //            st = st.OrderByDescending(s => s.last_update);
            //            break;
            //        case "last_update":
            //            st = st.OrderBy(s => s.last_update);
            //            break;
            //        case "kodu":
            //            st = st.OrderBy(s => s.kodu);
            //            break;
            //        case "kodu_desc":
            //            st = st.OrderByDescending(s => s.kodu);
            //            break;
            //        case "tedarik_yer_adi":
            //            st = st.OrderBy(s => s.tedarik_yer_adi);
            //            break;
            //        case "tedarik_yer_adi_desc":
            //            st = st.OrderByDescending(s => s.tedarik_yer_adi);
            //            break;
            //        case "adi":
            //            st = st.OrderBy(s => s.adi);
            //            break;
            //        case "adi_desc":
            //            st = st.OrderByDescending(s => s.adi);
            //            break;
            //        default:
            //            st = st.OrderBy(s => s.adi);
            //            break;
            //    }
            //    int _sayfaNo = SayfaNo ?? 1;
            //    ViewBag.SayfaNo = _sayfaNo;
            //    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
            //    List<carilar> carilarList = st.ToList();
            //    ViewBag.KayitSayisi = carilarList.Count();
            //    IPagedList<carilar> carilarListPaged = carilarList.ToPagedList<carilar>(_sayfaNo, _sayfaKayitSayisi);
            //    if (Request.IsAjaxRequest() || 1 == 1)
            //    {
            //        return View("StokListesi", carilarListPaged);
            //        //return PartialView("~/Views/Stok/_StokListesi.cshtml", carilarListPaged);
            //    }
            //}
            return View();
        }
        public string GetAllCategoriesForTree()
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<kodlar> categories = new List<kodlar>();
            //DataTable dt = new HomeBAL().GetAllCategories();
            var dt = db.kodlars.Where(x => x.id > 0 && x.isdeleted == 0);
            int sayi = dt.ToList().Count;
            if (dt != null && dt.ToList().Count > 0)
            {
                //foreach (kodlar row in dt.ToList())
                //{
                //    categories.Add(
                //        new kodlar
                //        {
                //            kod = Convert.ToInt32(row["CategoryId"]),
                //            adi = row["CategoryName"].ToString(),
                //            ustkod = (Convert.ToInt32(row["ParentCategoryId"]) != 0) ? Convert.ToInt32(row["ParentCategoryId"]) : (int?)null
                //        });
                //}

                List<Utilities.TreeNode> headerTree = FillRecursive(dt.ToList(), null);

                #region BindingHeaderMenus  

                string root_li = string.Empty;
                string down1_names = string.Empty;
                string down2_names = string.Empty;

                foreach (var item in headerTree)
                {
                    root_li += "<li class=\"dropdown mega-menu-fullwidth\">"
                                + "<a href=\"/Product/ListProduct?cat=" + item.CategoryId + "\" class=\"dropdown-toggle\" data-hover=\"dropdown\" data-toggle=\"dropdown\">" + item.CategoryName + "</a>";

                    down1_names = "";
                    foreach (var down1 in item.Children)
                    {
                        down2_names = "";
                        foreach (var down2 in down1.Children)
                        {
                            down2_names += "<li><a href=\"/Product/ListProduct?cat=" + down2.CategoryId + "\">" + down2.CategoryName + "</a></li>";
                        }
                        down1_names += "<div class=\"col-md-2 col-sm-6\">"
                                        + "<h3 class=\"mega-menu-heading\"><a href=\"/Product/ListProduct?cat=" + down1.CategoryId + "\">" + down1.CategoryName + "</a></h3>"
                                        + "<ul class=\"list-unstyled style-list\">"
                                        + down2_names
                                        + "</ul>"
                                        + "</div>";
                    }
                    root_li += "<ul class=\"dropdown-menu\">"
                                + "<li>"
                                    + "<div class=\"mega-menu-content\">"
                                        + "<div class=\"container\">"
                                            + "<div class=\"row\">"
                                                + down1_names
                                            + "</div>"
                                        + "</div>"
                                    + "<div>"
                                + "</li>"
                                + "</ul>"
                            + "</li>";
                }
                #endregion

                return "<ul class=\"nav navbar-nav\">" + root_li + "</ul>";
            }
            return "Record Not Found!!";
        }
        private static List<Utilities.TreeNode> FillRecursive(List<kodlar> flatObjects, int? parentId)
        {
            return flatObjects.Where(x => x.ustkod.Equals(parentId == null ? parentId = 0 : parentId)).Select(item => new Utilities.TreeNode
            {
                CategoryName = item.adi,
                CategoryId = item.kod,
                Children = FillRecursive(flatObjects, item.kod)
            }).ToList();
        }
    }
}