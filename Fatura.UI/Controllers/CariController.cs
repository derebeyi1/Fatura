using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Fatura.BLL.Utilities;

namespace Fatura.UI.Controllers
{
    public class CariController : Controller
    {
        faturaEntities db = null;
        //Utilities util = new Utilities();
        // GET: Cari
        [ExceptionHandler]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult CariListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    ViewBag.CariAdiSort = sortOrder == "adi" ? "adi_desc" : "adi";
                    ViewBag.VKNSort = sortOrder == "vkn_tckn" ? "vkn_tckn_desc" : "vkn_tckn";
                    ViewBag.LastUpdateSort = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    long vkn = (long)Session["vkn_tckn"];
                    long vknAdmin = Utilities.vknAdmin;
                    var cari = from s in db.caris
                               where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn && s.isdeleted == false //yonetici ise bütün cari listelenir
                               select s;
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        cari = cari.Where(s => s.ad.Contains(searchString) || s.vkn_tckn.ToString().Contains(searchString) || s.cari_mail.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "last_update_desc":
                            cari = cari.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            cari = cari.OrderBy(s => s.last_update);
                            break;
                        case "vkn_tckn_desc":
                            cari = cari.OrderByDescending(s => s.vergi_numarasi);
                            break;
                        case "vkn_tckn":
                            cari = cari.OrderBy(s => s.vergi_numarasi);
                            break;
                        //case "cep":
                        //    st = st.OrderBy(s => s.cep_no);
                        //    break;
                        //case "cep_desc":
                        //    st = st.OrderByDescending(s => s.cep_no);
                        //    break;
                        case "adi":
                            cari = cari.OrderBy(s => s.ad);
                            break;
                        case "adi_desc":
                            cari = cari.OrderByDescending(s => s.ad);
                            break;
                        default:
                            cari = cari.OrderByDescending(s => s.last_update);
                            break;
                    }
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    List<cari> cariList = cari.ToList();
                    ViewBag.KayitSayisi = cariList.Count();
                    ViewBag.Mesaj = TempData["Islem"];
                    IPagedList<cari> cariListPaged = cariList.ToPagedList<cari>(_sayfaNo, _sayfaKayitSayisi);
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View("CariListesi", cariListPaged);
                        //return PartialView("~/Views/Stok/_StokListesi.cshtml", cariListPaged);
                    }
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
        public ActionResult Kaydet(cari gelen, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                return View("CariFormu", gelen);
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            if (gelen != null && gelen.id == 0) //yeni kayit
            {
                //var adiParcala = gelen.ad.Trim().Split(' ');
                if (gelen.ad != null && gelen.ad.Trim() != "" && gelen.soyad != null && gelen.soyad.Trim() != "")
                {
                    var vergiNoUzunluk = (gelen.vergi_numarasi ?? 0).ToString().Length;
                    if (gelen.vergi_numarasi != null && vergiNoUzunluk > 9 && vergiNoUzunluk < 12)
                    {
                        if (gelen.cari_mail != null && Utilities.MailAdresiFormataUygunMu(gelen.cari_mail))
                        {
                            gelen.kayit_tarihi = DateTime.Now;
                            gelen.last_update = DateTime.Now;
                            gelen.isdeleted = false;
                            gelen.vkn_tckn = user.vkn_tckn;
                            //gelen.kullanici_id = user.id;
                            db.caris.Add(gelen);
                            TempData["Islem"] = "Eklendi";
                            sortOrder = "last_update_desc";
                        }
                        else
                            ViewBag.HataMesaj = "Lütfen geçerli bir e-posta adresi giriniz.";
                    }
                    else
                        ViewBag.HataMesaj = "Lütfen carinin VKN/TCKN numarasını giriniz.";
                }
                else
                    ViewBag.HataMesaj = "Lütfen carinin adını ve soyadını tam olarak giriniz.";
                if (ViewBag.HataMesaj != null && ViewBag.HataMesaj != "")
                {
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 4;
                    gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                    gelen.DropDownListForCariTipi = new SelectList(Utilities.getSelectListForCariTipi((int)gelen.tip), "Key", "Display");
                    var cadres = from ca in db.cari_adres
                                 where ca.cari_id == gelen.id && ca.isdeleted == false //yonetici ise bütün cari listelenir
                                 select ca;
                    switch (sortOrder)
                    {
                        case "id_desc":
                            cadres = cadres.OrderByDescending(s => s.id);
                            break;
                        case "last_update_desc":
                            cadres = cadres.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            cadres = cadres.OrderBy(s => s.last_update);
                            break;
                        default:
                            cadres = cadres.OrderBy(s => s.adres);
                            break;
                    }
                    List<cari_adres> cariadreslerList = cadres.ToList();
                    ViewBag.KayitSayisi = cariadreslerList.Count();
                    IPagedList<cari_adres> cariListPaged = cariadreslerList.ToPagedList<cari_adres>(_sayfaNo, 4);
                    gelen.cari_adresler = cariListPaged;
                    return View("CariFormu", gelen);
                }
            }
            else
            {
                //var adiParcala = gelen.ad.Trim().Split(' ');
                if (gelen.ad != null && gelen.ad.Trim() != "" && gelen.soyad != null && gelen.soyad.Trim() != "")
                {
                    var vergiNoUzunluk = (gelen.vergi_numarasi ?? 0).ToString().Length;
                    if (gelen.vergi_numarasi != null && vergiNoUzunluk > 9 && vergiNoUzunluk < 12)
                    {
                        if (gelen.cari_mail != null && Utilities.MailAdresiFormataUygunMu(gelen.cari_mail))
                        {
                            var guncellenecekKayit = db.caris.Find(gelen.id);
                            gelen.kayit_tarihi = guncellenecekKayit.kayit_tarihi;
                            gelen.last_update = DateTime.Now;
                            gelen.isdeleted = false;
                            gelen.vkn_tckn = user.vkn_tckn;
                            //gelen.dogum_yeri = (db.ulke_il_ilce.Find(gelen.dogum_yeri)).adi;
                            //gelen.kullanici_id = user.id;
                            ViewBag.Mesaj = "Cari başarıyla güncellendi.";
                            //kayit guncelle
                            db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                            TempData["Islem"] = "Guncellendi";
                            sortOrder = "last_update_desc";
                        }
                        else
                            ViewBag.HataMesaj = "Lütfen geçerli bir e-posta adresi giriniz.";
                    }
                    else
                        ViewBag.HataMesaj = "Lütfen carinin VKN/TCKN numarasını giriniz.";
                }
                else
                    ViewBag.HataMesaj = "Lütfen carinin adını ve soyadını tam olarak giriniz.";
                if (ViewBag.HataMesaj != null && ViewBag.HataMesaj != "")
                {
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 4;
                    gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                    gelen.DropDownListForCariTipi = new SelectList(Utilities.getSelectListForCariTipi((int)gelen.tip), "Key", "Display");
                    var cadres = from ca in db.cari_adres
                                 where ca.cari_id == gelen.id && ca.isdeleted == false //yonetici ise bütün cari listelenir
                                 select ca;
                    switch (sortOrder)
                    {
                        case "id_desc":
                            cadres = cadres.OrderByDescending(s => s.id);
                            break;
                        case "last_update_desc":
                            cadres = cadres.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            cadres = cadres.OrderBy(s => s.last_update);
                            break;
                        default:
                            cadres = cadres.OrderBy(s => s.adres);
                            break;
                    }
                    List<cari_adres> cariadreslerList = cadres.ToList();
                    ViewBag.KayitSayisi = cariadreslerList.Count();
                    IPagedList<cari_adres> cariListPaged = cariadreslerList.ToPagedList<cari_adres>(_sayfaNo, 4);
                    gelen.cari_adresler = cariListPaged;
                    return View("CariFormu", gelen);
                }
            }
            db.SaveChanges();
            return RedirectToAction("/CariListesi", "Cari", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult AdresKaydet(cari_adres model, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                model.DropDownListForAdr = new SelectList(Utilities.getSelectListForBoolean((bool)model.faturaadresimi), "Key", "Display");
                model.DropDownListForSevk = new SelectList(Utilities.getSelectListForBoolean((bool)model.sevkadresimi), "Key", "Display");
                model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(model.ulke_kodu ?? 0, 0), "Key", "Display");
                model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(model.il_kodu ?? 0, model.ulke_kodu ?? 0), "Key", "Display");
                model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(model.ilce_kodu ?? 0, model.il_kodu ?? 0), "Key", "Display");
                return RedirectToAction("AdresGuncelle", "Cari", new { id = model.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            if (model != null && model.id == 0) //yeni kayit
            {
                model.kayit_tarihi = DateTime.Now;
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                db.cari_adres.Add(model);
            }
            else
            {
                var guncellenecekKayit = db.cari_adres.Find(model.id);
                model.kayit_tarihi = guncellenecekKayit.kayit_tarihi;
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                ViewBag.Mesaj = "Cari başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(model);
            }
            sortOrder = "last_update_desc";
            db.SaveChanges();
            return RedirectToAction("Guncelle", "Cari", new { id = model.cari_id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Yeni()
        {
            //var model = new cari();
            List<cari_adres> cariadreslerList = Enumerable.Empty<cari_adres>().AsQueryable().ToList();
            ViewBag.KayitSayisi = cariadreslerList.Count();
            IPagedList<cari_adres> cariListPaged = cariadreslerList.ToPagedList<cari_adres>(1, 4);
            var model = new cari
            {
                //DropDownListForKilitlimi = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display"),
                DropDownList = new SelectList(Utilities.getSelectListForBoolean(false), "Key", "Display"),
                DropDownListForCariTipi = new SelectList(Utilities.getSelectListForCariTipi(0), "Key", "Display"),
                cari_adresler = cariListPaged,
            };
            TempData.Clear();
            ViewBag.IslemTipi = "Yeni Cari";
            return View("CariFormu", model);

        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Guncelle(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
            ViewBag.CodeSortParm = sortOrder == "cari_vkn" ? "cari_vkn_desc" : "kodu";
            ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
            ViewBag.SearchString = searchString;
            ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
            int _sayfaNo = SayfaNo ?? 1;
            ViewBag.SayfaNo = _sayfaNo;
            int _sayfaKayitSayisi = sayfaKayitSayisi ?? 4;
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            cari model = null;
            if (vkn == vknAdmin)
                model = db.caris.FirstOrDefault(x => x.id == id);
            else
                model = db.caris.FirstOrDefault(x => x.id == id && x.isdeleted == false && x.vkn_tckn == vkn);
            //IQueryable cadres = null; 
            if (model != null)
            {
                var cadres = from ca in db.cari_adres
                             where ca.cari_id == id && ca.isdeleted == false //yonetici ise bütün cari listelenir
                             select ca;
                switch (sortOrder)
                {
                    case "id_desc":
                        cadres = cadres.OrderByDescending(s => s.id);
                        break;
                    case "last_update_desc":
                        cadres = cadres.OrderByDescending(s => s.last_update);
                        break;
                    case "last_update":
                        cadres = cadres.OrderBy(s => s.last_update);
                        break;
                    default:
                        cadres = cadres.OrderBy(s => s.adres);
                        break;
                }
                List<cari_adres> cariadreslerList = cadres.ToList();
                ViewBag.KayitSayisi = cariadreslerList.Count();
                IPagedList<cari_adres> cariListPaged = cariadreslerList.ToPagedList<cari_adres>(_sayfaNo, 4);
                model.cari_adresler = cariListPaged;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.SayfaNo = SayfaNo;
            model.DropDownList = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
            model.DropDownListForCariTipi = new SelectList(Utilities.getSelectListForCariTipi((int)model.tip), "Key", "Display");
            ViewBag.IslemTipi = "Cari Güncelle";
            return View("CariFormu", model);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult AdresGuncelle(int id, int cari_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            cari_adres model = null;
            if (id != 0)
            {
                ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                ViewBag.CodeSortParm = sortOrder == "cari_vkn" ? "cari_vkn_desc" : "kodu";
                ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                ViewBag.SearchString = searchString;
                ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                int _sayfaNo = SayfaNo ?? 1;
                ViewBag.SayfaNo = _sayfaNo;
                int _sayfaKayitSayisi = sayfaKayitSayisi ?? 4;
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;

                if (vkn == vknAdmin)
                    model = db.cari_adres.FirstOrDefault(x => x.id == id);
                else
                    model = db.cari_adres.FirstOrDefault(x => x.id == id && x.isdeleted == false);
                //IQueryable cadres = null; 
                //cari ca = db.caris.Find(model.cari_id);
                if (model != null)
                {
                    model.DropDownListForAdr = new SelectList(Utilities.getSelectListForBoolean((bool)model.faturaadresimi), "Key", "Display");
                    model.DropDownListForSevk = new SelectList(Utilities.getSelectListForBoolean((bool)model.sevkadresimi), "Key", "Display");
                    model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce((int)model.ulke_kodu, 0), "Key", "Display");
                    model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce((int)model.il_kodu, (int?)model.ulke_kodu ?? 100), "Key", "Display");
                    model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce((int)model.ilce_kodu, (int?)model.il_kodu ?? 34), "Key", "Display");
                    model.cari_id = cari_id;
                    ViewBag.Baslik = "Adres Güncelle";
                    return View("_CariAdresFormu", model);
                }
                else
                {
                    model = new cari_adres();
                    model.cari_id = cari_id;
                    model.DropDownListForAdr = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                    model.DropDownListForSevk = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                    model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(0, 0), "Key", "Display");
                    model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(0, 100), "Key", "Display");
                    model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(0, 34), "Key", "Display");
                    model.cari_id = cari_id;
                    ViewBag.Baslik = "Yeni Adres";
                    return View("_CariAdresFormu", model);
                }
            }
            else
            {
                model = new cari_adres();
                model.DropDownListForAdr = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                model.DropDownListForSevk = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                model.DropDownListForUlke = new SelectList(util.getSelectListForUlkeIlIlce(0, 0), "Key", "Display");
                model.DropDownListForIl = new SelectList(util.getSelectListForUlkeIlIlce(0, 100), "Key", "Display");
                model.DropDownListForIlce = new SelectList(util.getSelectListForUlkeIlIlce(0, 34), "Key", "Display");
                model.cari_id = cari_id;
                ViewBag.Baslik = "Yeni Adres";
                return View("_CariAdresFormu", model);
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Sil(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.caris.FirstOrDefault(x => x.id == id && x.isdeleted == false && x.vkn_tckn == vkn);
            if (kayit != null)
            {
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                TempData["Islem"] = "Silindi";
                sortOrder = "last_update_desc";
            }
            return RedirectToAction("CariListesi", "Cari", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult CariSil(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.cari_adres.FirstOrDefault(x => x.id == id && x.isdeleted == false);
            if (kayit == null)
            {
                TempData.Clear();
                return HttpNotFound();
            }
            else
            {
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                TempData["cariSil"] = "silindi";
                return RedirectToAction("Guncelle", "Cari", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult AdresSil(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.cari_adres.Find(id);
            var ca = db.caris.Find(kayit.cari_id);
            if ((ca.vkn_tckn == vkn || vknAdmin == vkn) && kayit != null)
            {
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                return RedirectToAction("Guncelle", "Cari", new { ca.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            else
            {
                TempData.Clear();
                return HttpNotFound();
            }
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