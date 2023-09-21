using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Fatura.BLL.Utilities;

namespace Fatura.UI.Controllers
{
    public class FirmaController : Controller
    {
        faturaEntities db = null;
        //Utilities util = new Utilities();
        // GET: Firma
        public ActionResult Index()
        {
            return View();
        }
        [ExceptionHandler]
        public ActionResult FirmaListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            //int a = 1;
            //int b = 0;
            //int c = 0;
            //c = a / b; // sıfıra bölme fatası
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (ModelState.IsValid)
            {
                ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                ViewBag.CodeSortParm = sortOrder == "unvan" ? "unvan_desc" : "unvan";
                ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                ViewBag.SearchString = searchString;
                ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                var st = from s in db.firmas
                         where s.isdeleted == false && vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn //yonetici ise bütün şirketler listelenir
                         select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                    st = st.Where(s => s.unvan.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "vkn_desc":
                        st = st.OrderByDescending(s => s.vkn_tckn);
                        break;
                    case "last_update_desc":
                        st = st.OrderByDescending(s => s.last_update);
                        break;
                    case "last_update":
                        st = st.OrderBy(s => s.last_update);
                        break;
                    case "unvan":
                        st = st.OrderBy(s => s.unvan);
                        break;
                    case "unvan_desc":
                        st = st.OrderByDescending(s => s.unvan);
                        break;
                    default:
                        st = st.OrderBy(s => s.id);
                        break;
                }
                int _sayfaNo = SayfaNo ?? 1;
                ViewBag.SayfaNo = _sayfaNo;
                int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                List<firma> firmaList = st.ToList();
                ViewBag.KayitSayisi = firmaList.Count();
                IPagedList<firma> firmaListPaged = firmaList.ToPagedList<firma>(_sayfaNo, _sayfaKayitSayisi);
                if (Request.IsAjaxRequest() || 1 == 1)
                {
                    return View("FirmaListesi", firmaListPaged);
                    //return PartialView("~/Views/Stok/_StokListesi.cshtml", carilarListPaged);
                }
            }
            return View();
        }
        [ExceptionHandler]
        public ActionResult Yeni()
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            var model = new firma();
            model.vkn_tckn = long.Parse(Session["vkn_tckn"].ToString());
            model.lisans_bitis_tarihi = ((kullanici)Session["kullanici"]).kayit_tarihi.Value.AddDays(30);
            model.DropDownList = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
            model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(0, 0), "Key", "Display");
            model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(0, 100), "Key", "Display");
            model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(0, 34), "Key", "Display");
            //ViewBag.YeniFirmaMesaji = "Firma bilgilerinizi girerek sistemi kullanmaya başlayabilirsiniz.";
            ViewBag.YeniFirmaMesaji = TempData["YeniFirmaMesaji"];
            return View("FirmaFormu", model);

        }
        [ExceptionHandler]
        public ActionResult Guncelle(int id, int? SayfaNo, string SortOrder, string SearchString)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)(Session["vkn_tckn"] ?? 0);
            long vknAdmin = Utilities.vknAdmin;
            firma model = null;
            if (vkn > 0 && User.IsInRole("1"))
            {
                model = db.firmas.FirstOrDefault(x => x.id == id && x.isdeleted == false && (vkn == vknAdmin || x.vkn_tckn == vkn));
                if (model != null)
                {
                    ViewBag.SortOrder = SortOrder;
                    ViewBag.SearchString = SearchString;
                    ViewBag.SayfaNo = SayfaNo;
                    model.DropDownList = new SelectList(Utilities.getSelectListForBoolean(model.smtpServerSSLEnable ?? true), "Key", "Display");
                    model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(model.ulke_kodu ?? 0, 0), "Key", "Display");
                    model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(model.il_kodu ?? 0, model.ulke_kodu ?? 100), "Key", "Display");
                    model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(model.ilce_kodu ?? 0, model.il_kodu ?? 34), "Key", "Display");
                    TempData.Clear();
                }
                else
                {
                    ViewBag.Mesaj = "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz, hata tekrar ederse yazılım destek birimini arayınız.";
                }
            }
            else
            {
                ViewBag.Mesaj = "Firma bilgilerini güncellemek için yetkiniz yok.";
            }
            return View("FirmaFormu", model);
        }
        [ExceptionHandler]
        public ActionResult Kaydet(firma gelen, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(gelen.smtpServerSSLEnable ?? true), "Key", "Display");
                gelen.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ulke_kodu ?? 0, 0), "Key", "Display");
                gelen.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(gelen.il_kodu ?? 0, gelen.ulke_kodu ?? 100), "Key", "Display");
                gelen.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ilce_kodu ?? 0, gelen.il_kodu ?? 34), "Key", "Display");
                return View("FirmaFormu", gelen);
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)(Session["vkn_tckn"] ?? 0);
            long vknAdmin = Utilities.vknAdmin;
            kullanici user = (kullanici)Session["kullanici"];
            if (gelen != null && gelen.id == 0) //yeni kayit
            {
                if (gelen.unvan != null && gelen.unvan.Trim() != "")
                {
                    var vergiNoUzunluk = gelen.vkn_tckn.ToString().Length;
                    if (vergiNoUzunluk > 9 && vergiNoUzunluk < 12)
                    {
                        if (gelen.email != null && Utilities.MailAdresiFormataUygunMu(gelen.email))
                        {
                            string logoAdi = "";
                            if (gelen.logoFile != null)
                            {
                                logoAdi = Utilities.DosyaAdiDuzenle(gelen.logoFile.FileName);
                            }
                            string kaseAdi = "";
                            if (gelen.kaseFile != null)
                            {
                                kaseAdi = Utilities.DosyaAdiDuzenle(gelen.kaseFile.FileName);
                            }
                            gelen.logo = logoAdi;
                            gelen.kase = kaseAdi;
                            gelen.last_update = DateTime.Now;
                            gelen.isdeleted = false;
                            gelen.vkn_tckn = (long)user.vkn_tckn;
                            db.firmas.Add(gelen);
                            string firma_dosya_yolu = Server.MapPath("~/Content/") + gelen.vkn_tckn;

                            if (!System.IO.Directory.Exists(firma_dosya_yolu))
                                System.IO.Directory.CreateDirectory(firma_dosya_yolu);
                            string sil = Path.GetFileName(logoAdi);
                            if (gelen.logoFile != null)
                            {
                                gelen.logoFile.SaveAs(Path.Combine(firma_dosya_yolu, sil));
                            }
                            if (gelen.kaseFile != null)
                            {
                                gelen.kaseFile.SaveAs(Path.Combine(firma_dosya_yolu, Path.GetFileName(kaseAdi)));
                            }
                            TempData["firmaEkle"] = "ekle";
                        }
                        else
                            ViewBag.HataMesaj = "Lütfen geçerli bir e-posta adresi giriniz.";
                    }
                    else
                        ViewBag.HataMesaj = "Lütfen firmanın VKN/TCKN numarasını giriniz.";
                }
                else
                    ViewBag.HataMesaj = "Lütfen firmanın ünvanını giriniz.";
                if (ViewBag.HataMesaj != null && ViewBag.HataMesaj != "")
                {
                    ViewBag.SortOrder = sortOrder;
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaNo = SayfaNo;
                    gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(gelen.smtpServerSSLEnable ?? true), "Key", "Display");
                    gelen.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ulke_kodu ?? 0, 0), "Key", "Display");
                    gelen.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(gelen.il_kodu ?? 0, gelen.ulke_kodu ?? 100), "Key", "Display");
                    gelen.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ilce_kodu ?? 0, gelen.il_kodu ?? 34), "Key", "Display");
                    return View("FirmaFormu", gelen);
                }
            }
            else
            {
                if (vkn > 0 && User.IsInRole("1"))
                {
                    if (gelen.unvan != null && gelen.unvan.Trim() != "")
                    {
                        var vergiNoUzunluk = gelen.vkn_tckn.ToString().Length;
                        if (vergiNoUzunluk > 9 && vergiNoUzunluk < 12)
                        {
                            if (gelen.email != null && Utilities.MailAdresiFormataUygunMu(gelen.email))
                            {
                                var guncellenecekKayit = db.firmas.Find(gelen.vkn_tckn);
                                // vkn ve lisans tarihini sadece şirket admin güncelleyebilir
                                if (vkn != vknAdmin)
                                {
                                    if (gelen.vkn_tckn != guncellenecekKayit.vkn_tckn || gelen.lisans_bitis_tarihi != guncellenecekKayit.lisans_bitis_tarihi)
                                    {
                                        gelen.vkn_tckn = guncellenecekKayit.vkn_tckn;
                                        gelen.lisans_bitis_tarihi = guncellenecekKayit.lisans_bitis_tarihi;
                                    }
                                }
                                if (gelen.logoFile != null)
                                {
                                    string resimAdi = Utilities.DosyaAdiDuzenle(gelen.logoFile.FileName);
                                    // resim kaydetme
                                    string firma_dosya_yolu = Server.MapPath("~/Content/") + gelen.vkn_tckn;
                                    if (!System.IO.File.Exists(firma_dosya_yolu))
                                        System.IO.Directory.CreateDirectory(firma_dosya_yolu);
                                    gelen.logoFile.SaveAs(Path.Combine(firma_dosya_yolu, Path.GetFileName(resimAdi)));
                                    gelen.logo = resimAdi;
                                }
                                else
                                {
                                    if (guncellenecekKayit.logo != null)
                                        gelen.logo = guncellenecekKayit.logo;
                                }
                                if (gelen.kaseFile != null)
                                {
                                    string resimAdi = Utilities.DosyaAdiDuzenle(gelen.kaseFile.FileName);
                                    // resim kaydetme
                                    string firma_dosya_yolu = Server.MapPath("~/Content/") + gelen.vkn_tckn;
                                    if (!System.IO.File.Exists(firma_dosya_yolu))
                                        System.IO.Directory.CreateDirectory(firma_dosya_yolu);
                                    gelen.kaseFile.SaveAs(Path.Combine(firma_dosya_yolu, Path.GetFileName(resimAdi)));
                                    gelen.kase = resimAdi;
                                }
                                else
                                {
                                    gelen.kase = guncellenecekKayit.kase;
                                }
                                gelen.last_update = DateTime.Now;
                                gelen.isdeleted = false;
                                //gelen.vkn_tckn = (long)user.vkn_tckn;
                                //gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean((bool)gelen.smtpServerSSLEnable), "Key", "Display");
                                ViewBag.Mesaj = "Firma bilgileri başarıyla güncellenmiştir.";
                                //kayit guncelle
                                db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                                TempData["firmaGuncelle"] = "guncelleme";
                            }
                            else
                                ViewBag.HataMesaj = "Lütfen geçerli bir e-posta adresi giriniz.";
                        }
                        else
                            ViewBag.HataMesaj = "Lütfen firmanın VKN/TCKN numarasını giriniz.";
                    }
                    else
                        ViewBag.HataMesaj = "Lütfen firmanın ünvanını giriniz.";
                }
                if (ViewBag.HataMesaj != null && ViewBag.HataMesaj != "")
                {
                    ViewBag.SortOrder = sortOrder;
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaNo = SayfaNo;
                    gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(gelen.smtpServerSSLEnable ?? true), "Key", "Display");
                    gelen.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ulke_kodu ?? 0, 0), "Key", "Display");
                    gelen.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(gelen.il_kodu ?? 0, gelen.ulke_kodu ?? 100), "Key", "Display");
                    gelen.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(gelen.ilce_kodu ?? 0, gelen.il_kodu ?? 34), "Key", "Display");
                    return View("FirmaFormu", gelen);
                }
            }
            if (vkn == gelen.vkn_tckn)
                Session.Add("firma_logo", gelen.logo);
            db.SaveChanges();
            return RedirectToAction("/FirmaListesi", "Firma", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult AckGetir(int ustkod)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<MyListTable> PList = util.getSelectListForUlkeIlIlce(0, ustkod);
            return Json(PList, JsonRequestBehavior.AllowGet);
        }
    }
}