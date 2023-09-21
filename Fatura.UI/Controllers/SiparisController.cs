using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Fatura.UI.Controllers
{
    public class SiparisController : Controller
    {
        faturaEntities db = null;

        // GET: Siparis
        public ActionResult Index()
        {
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult SiparisListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    N11Controller n11 = new N11Controller();
                    ViewBag.CategoryList = n11.LoadCategoryList(1);
                    ViewBag.SubCategoryList = ViewBag.CategoryList;
                    ViewBag.SubSubCategoryList = ViewBag.CategoryList;
                    ViewBag.OrderList = n11.GetOrderList();
                    ViewBag.ProductList = n11.GetProductList();
                    ViewBag.ProductStockList = n11.GetProductStockList(); 

                    //ViewBag.BelgeNoSort = sortOrder == "belgeno" ? "belgeno_desc" : "belgeno";
                    //ViewBag.CariAdiSort = sortOrder == "cari_adi" ? "cari_adi_desc" : "cari_adi";
                    //ViewBag.ToplamFiyatSort = sortOrder == "toplam_fiyat" ? "toplam_fiyat_desc" : "toplam_fiyat";
                    //ViewBag.FaturaTipiSort = sortOrder == "fatura_tipi" ? "fatura_tipi_desc" : "fatura_tipi";
                    //ViewBag.FaturaTarihiSort = sortOrder == "fatura_tarihi" ? "fatura_tarihi_desc" : "fatura_tarihi";
                    //ViewBag.SearchString = searchString;
                    //ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    //long vkn = (long)Session["vkn_tckn"];
                    //long vknAdmin = Utilities.vknAdmin;
                    //var st = (from s in db.faturas.AsEnumerable()
                    //          join c in db.caris.AsEnumerable() on s.cari_id equals c.id
                    //          where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn && s.isdeleted == false
                    //          //searchString != null && searchString != '' ? s.belgeno.Contains(searchString) : ''//yonetici ise bütün cari listelenir
                    //          select new fatura
                    //          {
                    //              id = s.id,
                    //              vkn_tckn = (long)s.vkn_tckn,
                    //              kullanici_id = (int)s.kullanici_id,
                    //              cari_id = (int)s.cari_id,
                    //              doviz_id = (int)s.doviz_id,
                    //              doviz_kuru = s.doviz_kuru ?? 1,
                    //              aratoplam = s.aratoplam ?? 0,
                    //              toplam_iskonto = s.toplam_iskonto ?? 0,
                    //              toplam_kdvtutar = s.toplam_kdvtutar ?? 0,
                    //              toplam_tevkifat = s.toplam_tevkifat ?? 0,
                    //              toplam_otvtutar = s.toplam_otvtutar ?? 0,
                    //              toplam_oivtutar = s.toplam_oivtutar ?? 0,
                    //              toplam_fiyat = s.toplam_fiyat ?? 0,
                    //              seri = s.seri ?? "",
                    //              sira_no = s.sira_no ?? 0,
                    //              belgeno = s.belgeno ?? "",
                    //              fatura_tarihi = s.fatura_tarihi ?? DateTime.MinValue,
                    //              fatura_onaykontrol = s.fatura_onaykontrol ?? 0,
                    //              fatura_durum = s.fatura_durum ?? "",
                    //              fatura_alias = s.fatura_alias ?? "",
                    //              mail_gittimi = s.mail_gittimi ?? false,
                    //              aciklama = s.aciklama ?? "",
                    //              fatura_guid = s.fatura_guid ?? "",
                    //              kayit_tarihi = s.kayit_tarihi ?? DateTime.MinValue,
                    //              entegrator_tipi = s.entegrator_tipi ?? 0,
                    //              fatura_tipi = s.fatura_tipi ?? 1,
                    //              toplam_gvsmatrah = s.toplam_gvsmatrah ?? 0,
                    //              toplam_gvstutar = s.toplam_gvstutar ?? 0,
                    //              toplam_btumatrah = s.toplam_btumatrah ?? 0,
                    //              toplam_btututar = s.toplam_btututar ?? 0,
                    //              toplam_mfvmatrah = s.toplam_mfvmatrah ?? 0,
                    //              toplam_mfvtutar = s.toplam_mfvtutar ?? 0,
                    //              toplam_sgkmatrah = s.toplam_sgkmatrah ?? 0,
                    //              toplam_sgktutar = s.toplam_sgktutar ?? 0,
                    //              isdeleted = s.isdeleted ?? false,
                    //              last_update = s.last_update ?? DateTime.MinValue,
                    //              cari_adi = c.ad + ' ' + c.soyad ?? "",
                    //          });
                    ////mustahsil faturası var mı?
                    //var st1 = st.Where(s => s.fatura_tipi == Utilities.EMustahsil).FirstOrDefault();
                    //if (st1 != null)
                    //    ViewBag.MustahsilVar = "MustahsilVar";
                    //if (!String.IsNullOrEmpty(searchString))
                    //{
                    //    st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    //}
                    ////ihracat faturası var mı?
                    //var st2 = st.Where(s => s.fatura_tipi == Utilities.EIhracat).FirstOrDefault();
                    //if (st2 != null)
                    //    ViewBag.IhracatVar = "IhracatVar";
                    //if (!String.IsNullOrEmpty(searchString))
                    //{
                    //    st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    //}
                    ////smm faturası var mı?
                    //var st3 = st.Where(s => s.fatura_tipi == Utilities.ESMM).FirstOrDefault();
                    //if (st3 != null)
                    //    ViewBag.SMMVar = "SMMVar";
                    //if (!String.IsNullOrEmpty(searchString))
                    //{
                    //    st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    //}
                    //switch (sortOrder)
                    //{
                    //    case "belgeno":
                    //        st = st.OrderBy(s => s.belgeno);
                    //        break;
                    //    case "belgeno_desc":
                    //        st = st.OrderByDescending(s => s.belgeno);
                    //        break;
                    //    case "cari_adi":
                    //        st = st.OrderBy(s => s.cari_adi);
                    //        break;
                    //    case "cari_adi_desc":
                    //        st = st.OrderByDescending(s => s.cari_adi);
                    //        break;
                    //    case "toplam_fiyat":
                    //        st = st.OrderBy(s => s.toplam_fiyat);
                    //        break;
                    //    case "toplam_fiyat_desc":
                    //        st = st.OrderByDescending(s => s.toplam_fiyat);
                    //        break;
                    //    case "fatura_tipi":
                    //        st = st.OrderBy(s => s.fatura_tipi);
                    //        break;
                    //    case "fatura_tipi_desc":
                    //        st = st.OrderByDescending(s => s.fatura_tipi);
                    //        break;
                    //    case "last_update_desc":
                    //        st = st.OrderByDescending(s => s.last_update);
                    //        break;
                    //    case "last_update":
                    //        st = st.OrderBy(s => s.last_update);
                    //        break;
                    //    case "fatura_tarihi_desc":
                    //        st = st.OrderByDescending(s => s.fatura_tarihi);
                    //        break;
                    //    case "fatura_tarihi":
                    //        st = st.OrderBy(s => s.fatura_tarihi);
                    //        break;
                    //    default:
                    //        st = st.OrderByDescending(s => s.last_update);
                    //        break;
                    //}
                    //int _sayfaNo = SayfaNo ?? 1;
                    //ViewBag.SayfaNo = _sayfaNo;
                    //int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    //ViewBag.KayitSayisi = st.Count();
                    //IPagedList<fatura> faturaListPaged = st.ToPagedList<fatura>(_sayfaNo, _sayfaKayitSayisi);
                    //if (Request.IsAjaxRequest() || 1 == 1)
                    //{
                    //    return View("FaturaListesi", faturaListPaged);
                    //}
                }
            }
            else
            {
                Exception custException = null;
                if (Session["kullanici"] == null)
                    custException = new Exception("Lütfen giriş yapınız.");
                else if (!User.IsInRole("1"))
                    custException = new Exception("Yetkiniz yok.");
                var model = new HandleErrorInfo(custException, "403", "403");
                return View("Error", model);
            }
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult AckGetir(int CategoryId)
        {
                Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                N11Controller n11 = new N11Controller();
                List<SelectListItem> SubCategories = n11.LoadSubCategoryList(CategoryId);
                //ViewBag.SubSubCategoryList = ViewBag.CategoryList;

                //string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
                //string strAppSecret = "SpzrFSGJje8cJ4IW";
                ////string strAppKey = keys.N11AppKey;
                ////string strAppSecret = keys.N11AppSecretKey;
                //var authentication = new BLL.N11CategoryService.Authentication();
                //authentication.appKey = strAppKey; //api anahtarınız
                //authentication.appSecret = strAppSecret;//api şifeniz


                //CategoryServicePortClient proxy = new BLL.N11CategoryService.CategoryServicePortClient();
                //var request = new BLL.N11CategoryService.GetSubCategoriesRequest();
                //request.auth = authentication;
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //request.categoryId = CategoryId;


                //var SubCategories = proxy.GetSubCategories(request);


                //List<SelectListItem> SubCatItem = new List<SelectListItem>();

                ////SubCatItem.Add(new SelectListItem { Text = "Seçiniz", Value = "" });
                //foreach (var item in SubCategories.category[0].subCategoryList)
                //{
                //    bool SelectedState = false;
                //    if (CategoryId == item.id) { SelectedState = true; }
                //    SubCatItem.Add(new SelectListItem { Text = "-" + item.name, Selected = SelectedState, Value = item.id.ToString() });
                //}


                //if (SubCategories.result.status == "success")
                //{


                    return Json(SubCategories, JsonRequestBehavior.AllowGet);

            //}
            //else
            //{
            //    return Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}