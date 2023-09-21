using Fatura.BLL;
using Fatura.BLL.Uyumsoft;
using Fatura.BLL.UyumsoftIntegration;
using Fatura.BLL.UyumsoftProducerReceipt;
using Fatura.Entity;
using Fatura.UI.Filters;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static Fatura.BLL.Utilities;

namespace Fatura.UI.Controllers
{
    public class FaturaController : Controller
    {
        faturaEntities db = null;        
        // GET: Stok
        [ExceptionHandler]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult FaturaListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    ViewBag.BelgeNoSort = sortOrder == "belgeno" ? "belgeno_desc" : "belgeno";
                    ViewBag.CariAdiSort = sortOrder == "cari_adi" ? "cari_adi_desc" : "cari_adi";
                    ViewBag.ToplamFiyatSort = sortOrder == "toplam_fiyat" ? "toplam_fiyat_desc" : "toplam_fiyat";
                    ViewBag.FaturaTipiSort = sortOrder == "fatura_tipi" ? "fatura_tipi_desc" : "fatura_tipi";
                    ViewBag.FaturaTarihiSort = sortOrder == "fatura_tarihi" ? "fatura_tarihi_desc" : "fatura_tarihi";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    long vkn = (long)Session["vkn_tckn"];
                    long vknAdmin = Utilities.vknAdmin;
                    var st = (from s in db.faturas.AsEnumerable()
                              join c in db.caris.AsEnumerable() on s.cari_id equals c.id
                              where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn && s.isdeleted == false
                              //searchString != null && searchString != '' ? s.belgeno.Contains(searchString) : ''//yonetici ise bütün cari listelenir
                              select new fatura
                              {
                                  id = s.id,
                                  vkn_tckn = (long)s.vkn_tckn,
                                  kullanici_id = (int)s.kullanici_id,
                                  cari_id = (int)s.cari_id,
                                  doviz_id = (int)s.doviz_id,
                                  doviz_kuru = s.doviz_kuru ?? 1,
                                  aratoplam = s.aratoplam ?? 0,
                                  toplam_iskonto = s.toplam_iskonto ?? 0,
                                  toplam_kdvtutar = s.toplam_kdvtutar ?? 0,
                                  toplam_tevkifat = s.toplam_tevkifat ?? 0,
                                  toplam_otvtutar = s.toplam_otvtutar ?? 0,
                                  toplam_oivtutar = s.toplam_oivtutar ?? 0,
                                  toplam_fiyat = s.toplam_fiyat ?? 0,
                                  seri = s.seri ?? "",
                                  sira_no = s.sira_no ?? 0,
                                  belgeno = s.belgeno ?? "",
                                  fatura_tarihi = s.fatura_tarihi ?? DateTime.MinValue,
                                  fatura_onaykontrol = s.fatura_onaykontrol ?? 0,
                                  fatura_durum = s.fatura_durum ?? "",
                                  fatura_alias = s.fatura_alias ?? "",
                                  mail_gittimi = s.mail_gittimi ?? false,
                                  aciklama = s.aciklama ?? "",
                                  fatura_guid = s.fatura_guid ?? "",
                                  kayit_tarihi = s.kayit_tarihi ?? DateTime.MinValue,
                                  entegrator_tipi = s.entegrator_tipi ?? 0,
                                  fatura_tipi = s.fatura_tipi ?? 1,
                                  toplam_gvsmatrah = s.toplam_gvsmatrah ?? 0,
                                  toplam_gvstutar = s.toplam_gvstutar ?? 0,
                                  toplam_btumatrah = s.toplam_btumatrah ?? 0,
                                  toplam_btututar = s.toplam_btututar ?? 0,
                                  toplam_mfvmatrah = s.toplam_mfvmatrah ?? 0,
                                  toplam_mfvtutar = s.toplam_mfvtutar ?? 0,
                                  toplam_sgkmatrah = s.toplam_sgkmatrah ?? 0,
                                  toplam_sgktutar = s.toplam_sgktutar ?? 0,
                                  isdeleted = s.isdeleted ?? false,
                                  last_update = s.last_update ?? DateTime.MinValue,
                                  cari_adi = c.ad + ' ' + c.soyad ?? "",
                              });
                    //mustahsil faturası var mı?
                    var st1 = st.Where(s => s.fatura_tipi == Utilities.EMustahsil).FirstOrDefault();
                    if (st1 != null)
                        ViewBag.MustahsilVar = "MustahsilVar";
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    }
                    //ihracat faturası var mı?
                    var st2 = st.Where(s => s.fatura_tipi == Utilities.EIhracat).FirstOrDefault();
                    if (st2 != null)
                        ViewBag.IhracatVar = "IhracatVar";
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    }
                    //smm faturası var mı?
                    var st3 = st.Where(s => s.fatura_tipi == Utilities.ESMM).FirstOrDefault();
                    if (st3 != null)
                        ViewBag.SMMVar = "SMMVar";
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        st = st.Where(s => s.seri.Contains(searchString) || s.belgeno.Contains(searchString) || s.cari_adi.Contains(searchString) || s.fatura_tarihi.ToString().Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "belgeno":
                            st = st.OrderBy(s => s.belgeno);
                            break;
                        case "belgeno_desc":
                            st = st.OrderByDescending(s => s.belgeno);
                            break;
                        case "cari_adi":
                            st = st.OrderBy(s => s.cari_adi);
                            break;
                        case "cari_adi_desc":
                            st = st.OrderByDescending(s => s.cari_adi);
                            break;
                        case "toplam_fiyat":
                            st = st.OrderBy(s => s.toplam_fiyat);
                            break;
                        case "toplam_fiyat_desc":
                            st = st.OrderByDescending(s => s.toplam_fiyat);
                            break;
                        case "fatura_tipi":
                            st = st.OrderBy(s => s.fatura_tipi);
                            break;
                        case "fatura_tipi_desc":
                            st = st.OrderByDescending(s => s.fatura_tipi);
                            break;
                        case "last_update_desc":
                            st = st.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            st = st.OrderBy(s => s.last_update);
                            break;
                        case "fatura_tarihi_desc":
                            st = st.OrderByDescending(s => s.fatura_tarihi);
                            break;
                        case "fatura_tarihi":
                            st = st.OrderBy(s => s.fatura_tarihi);
                            break;
                        default:
                            st = st.OrderByDescending(s => s.last_update);
                            break;
                    }
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    ViewBag.KayitSayisi = st.Count();
                    IPagedList<fatura> faturaListPaged = st.ToPagedList<fatura>(_sayfaNo, _sayfaKayitSayisi);
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View("FaturaListesi", faturaListPaged);
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
        public ActionResult AyarListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    ViewBag.NameSortParm = sortOrder == "belgeno" ? "belgeno_desc" : "belgeno";
                    ViewBag.CodeSortParm = sortOrder == "toplam_fiyat" ? "toplam_fiyat_desc" : "toplam_fiyat";
                    ViewBag.TedarikYeriSortParm = sortOrder == "vkn_tckn" ? "vkn_tckn_desc" : "vkn_tckn";
                    ViewBag.DateSortParm = sortOrder == "olusturma_tarihi" ? "olusturma_tarihi_desc" : "olusturma_tarihi";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    long vkn = (long)Session["vkn_tckn"];
                    kullanici user = (kullanici)Session["kullanici"];
                    long vknAdmin = Utilities.vknAdmin;
                    //var fa = from s in db.fatura_ayarlari
                    //         where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn && s.kullanici_id == user.id && s.isdeleted == false //yonetici ise bütün kayıtlar listelenir
                    //         select s;

                    var fayar = (from fa in db.fatura_ayarlari.AsEnumerable()
                                 join kln in db.kullanicis.AsEnumerable() on fa.kullanici_id equals kln.id
                                 where vkn == vknAdmin ? fa.vkn_tckn > 0 : fa.vkn_tckn == vkn && fa.isdeleted == false
                                 //searchString != null && searchString != '' ? s.belgeno.Contains(searchString) : ''//yonetici ise bütün cari listelenir
                                 select new fatura_ayarlari
                                 {
                                     id = fa.id,
                                     vkn_tckn = (long)fa.vkn_tckn,
                                     kullanici_id = (int)fa.kullanici_id,
                                     fatura_tipi = fa.fatura_tipi ?? 1,
                                     giden_fatura_tipi = fa.giden_fatura_tipi ?? "",
                                     testmi = fa.testmi ?? true,
                                     entegrator_tipi = fa.entegrator_tipi ?? 0,
                                     test_gndrn_vkn = fa.test_gndrn_vkn ?? 0,
                                     test_alici_vkn = fa.test_alici_vkn ?? 0,
                                     kullanici_adi = fa.kullanici_adi ?? "",
                                     sifre = fa.sifre ?? "",
                                     seri = fa.seri ?? "",
                                     seri_kullanilsinmi = fa.seri_kullanilsinmi ?? true,
                                     isdeleted = fa.isdeleted ?? false,
                                     last_update = fa.last_update ?? DateTime.MinValue,
                                     kullanici_adsoyad = (kln.ad + " " + kln.soyad).Trim() ?? "",
                                 });
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        fayar = fayar.Where(s => s.kullanici_adsoyad.Contains(searchString) || s.fatura_tipi_ack.Contains(searchString) || s.entegrator_tipi_ack.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "id_desc":
                            fayar = fayar.OrderByDescending(s => s.id);
                            break;
                        case "last_update_desc":
                            fayar = fayar.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            fayar = fayar.OrderBy(s => s.last_update);
                            break;
                        default:
                            fayar = fayar.OrderByDescending(s => s.last_update);
                            break;
                    }
                    //List<fatura_ayarlari> ayarList = fa.ToList();
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    ViewBag.KayitSayisi = fayar.Count();
                    IPagedList<fatura_ayarlari> ayarListPaged = fayar.ToPagedList<fatura_ayarlari>(_sayfaNo, _sayfaKayitSayisi);
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View("AyarListesi", ayarListPaged);
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
        public ActionResult AyarEkle(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            fatura_ayarlari ayar = null;
            ViewBag.Baslik = "Fatura Ayarı Ekle";
            long vkn = (long)Session["vkn_tckn"];
            kullanici user = (kullanici)Session["kullanici"];
            long vknAdmin = Utilities.vknAdmin;
            if (id == 0) //yeni kayıt
            {
                ayar = new fatura_ayarlari();
                ayar.vkn_tckn = vkn;
                ayar.kullanici_id = user.id;
                ayar.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi(1), "Key", "Display");
                ayar.DropDownListForFaturaTuru = new SelectList(Utilities.getSelectListForFaturaTuru("TEMELFATURA"), "Key", "Display");
                ayar.DropDownListForEntegratorTipi = new SelectList(Utilities.getSelectListForEntegratorTipi(1), "Key", "Display");
                ayar.DropDownListForTestmi = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                ayar.DropDownListForSeriKullanilsinmi = new SelectList(Utilities.getSelectListForBoolean(true), "Key", "Display");
                ayar.isdeleted = false;
                ayar.last_update = DateTime.Now;
                return View("_FaturaAyarFormu", ayar);
            }
            else //güncelle
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                ayar = db.fatura_ayarlari.Find(id);
                if (ayar != null)
                {
                    if ((ayar.kullanici_id == user.id && ayar.vkn_tckn == vkn) || vkn == vknAdmin)
                    {
                        ayar.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)ayar.fatura_tipi), "Key", "Display");
                        ayar.DropDownListForFaturaTuru = new SelectList(Utilities.getSelectListForFaturaTuru(ayar.giden_fatura_tipi), "Key", "Display");
                        ayar.DropDownListForEntegratorTipi = new SelectList(Utilities.getSelectListForEntegratorTipi((int)ayar.entegrator_tipi), "Key", "Display");
                        ayar.DropDownListForTestmi = new SelectList(Utilities.getSelectListForBoolean((bool)ayar.testmi), "Key", "Display");
                        ayar.DropDownListForSeriKullanilsinmi = new SelectList(Utilities.getSelectListForBoolean((bool)ayar.seri_kullanilsinmi), "Key", "Display");
                        return View("_FaturaAyarFormu", ayar);
                    }
                    else
                        ViewBag.Mesaj = "Beklenmedik bir hata oluştu.";
                }
                else
                    ViewBag.Mesaj = "Beklenmedik bir hata oluştu.";

                return RedirectToAction("/AyarListesi", "Fatura", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult AyarKaydet(fatura_ayarlari gelen, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                return View("AyarFormu", gelen);
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            if (gelen != null && gelen.id == 0) //yeni kayit
            {
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                gelen.kullanici_id = user.id;
                db.fatura_ayarlari.Add(gelen);
                sortOrder = "last_update_desc";
            }
            else
            {
                var guncellenecekKayit = db.fatura_ayarlari.Find(gelen.id);
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                //gelen.dogum_yeri = (db.ulke_il_ilce.Find(gelen.dogum_yeri)).adi;
                gelen.kullanici_id = user.id;
                ViewBag.Mesaj = "Cari başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                TempData["cariGuncelle"] = "guncelleme";
                sortOrder = "last_update_desc";
            }
            db.SaveChanges();
            return RedirectToAction("/AyarListesi", "Fatura", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult AyarSil(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.fatura_ayarlari.FirstOrDefault(x => x.id == id && x.isdeleted == false && (x.vkn_tckn == vkn || vkn == vknAdmin));
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
                return RedirectToAction("AyarListesi", "Fatura", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Kaydet(fatura gelen, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                gelen.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)gelen.doviz_id), "Key", "Display");
                gelen.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)gelen.fatura_tipi), "Key", "Display");
                gelen.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi((int)gelen.islem_tipi), "Key", "Display");
                gelen.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias(gelen.fatura_alias, (long)db.caris.Find(gelen.cari_id).vergi_numarasi, (long)Session["vkn_tckn"]), "Key", "Display");
                gelen.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban(gelen.satis_iban, vkn), "Key", "Display");
                return View("FaturaFormu", gelen);
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            string fat_yetki = "";
            if (gelen.fatura_tipi == Utilities.EArsivNormal || gelen.fatura_tipi == Utilities.EArsivIade)
                fat_yetki = "2"; //Utilities>Yetkiler>E-Arşiv
            if (gelen.fatura_tipi == Utilities.EFaturaNormal || gelen.fatura_tipi == Utilities.EFaturaIade)
                fat_yetki = "3"; //Utilities>Yetkiler>E-Fatura
            if (gelen.fatura_tipi == Utilities.EMustahsil)
                fat_yetki = "4"; //Utilities>Yetkiler>E-Müstahsil
            if (gelen.fatura_tipi == Utilities.EIrsaliye)
                fat_yetki = "5"; //Utilities>Yetkiler>E-Irsaliye
            if (gelen.fatura_tipi == Utilities.EIhracat)
                fat_yetki = "6"; //Utilities>Yetkiler>E-Ihracat
            if (gelen.fatura_tipi == Utilities.ESMM)
                fat_yetki = "7"; //Utilities>Yetkiler>E-SMM

            if (!user.rol_yetki.Contains(fat_yetki))
            {
                ViewBag.Mesaj = "Bu fatura türü için yetkiniz yok. Bu fatura türü ile işlem yapmak için lütfen satış birimimizi arayınız.";
                if (gelen.detays == null || gelen.detays.Count == 0)
                {
                    List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
                    ViewBag.KayitSayisi = fatura_satirList.Count();
                    IPagedList<fatura_satir> cariListPaged = fatura_satirList.ToPagedList<fatura_satir>(1, 4);
                    gelen.detays = cariListPaged;
                }
                gelen.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)gelen.doviz_id), "Key", "Display");
                gelen.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)gelen.fatura_tipi), "Key", "Display");
                gelen.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi((int)gelen.islem_tipi), "Key", "Display");
                gelen.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias(gelen.fatura_alias, (long)db.caris.Find(gelen.cari_id).vergi_numarasi, (long)Session["vkn_tckn"]), "Key", "Display");
                gelen.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban(gelen.satis_iban, vkn), "Key", "Display");
                return View("FaturaFormu", gelen);
            }
            //var islem = 0;
            if (gelen != null && gelen.id == 0) //yeni kayit
            {
                gelen.kayit_tarihi = DateTime.Now;
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                gelen.kullanici_id = user.id;
                gelen.toplam_kdvtutar = 0;
                gelen.toplam_iskonto = 0;
                gelen.toplam_fiyat = 0;
                gelen.toplam_oivtutar = 0;
                gelen.toplam_otvtutar = 0;
                gelen.aratoplam = 0;
                gelen.toplam_fiyat = 0;
                gelen.toplam_tevkifat = 0;
                gelen.toplam_gvsmatrah = 0;
                gelen.toplam_gvstutar = 0;
                gelen.toplam_sgkmatrah = 0;
                gelen.toplam_sgktutar = 0;
                gelen.toplam_mfvmatrah = 0;
                gelen.toplam_mfvtutar = 0;
                gelen.toplam_btumatrah = 0;
                gelen.toplam_btututar = 0;
                if (gelen.fatura_tarihi == null)
                    gelen.fatura_tarihi = DateTime.Now;
                gelen.aciklama = null ?? "";
                gelen.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)gelen.doviz_id), "Key", "Display");
                gelen.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)gelen.fatura_tipi), "Key", "Display");
                gelen.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi((int)gelen.islem_tipi), "Key", "Display");
                gelen.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias(gelen.fatura_alias, (long)db.caris.Find(gelen.cari_id).vergi_numarasi, (long)Session["vkn_tckn"]), "Key", "Display");
                gelen.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban(gelen.satis_iban, vkn), "Key", "Display");
                List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
                ViewBag.KayitSayisi = fatura_satirList.Count();
                IPagedList<fatura_satir> cariListPaged = fatura_satirList.ToPagedList<fatura_satir>(1, 4);
                gelen.detays = cariListPaged;
                var fayar = (from fa in db.fatura_ayarlari
                             where fa.kullanici_id == user.id && fa.fatura_tipi == gelen.fatura_tipi && (vkn == Utilities.vknAdmin || fa.vkn_tckn == vkn)
                             select fa).FirstOrDefault();
                if (fayar == null)
                    throw new Exception(Utilities.FaturaTipleri[(int)gelen.fatura_tipi] + " fatura ayarı hatalı. Lütfen, Yönetici menüsünden E-Fatura Ayarları sayfasında entegratör ve fatura tipine göre ayarları yapınız.");
                gelen.seri = fayar.seri;
                db.faturas.Add(gelen);
                db.SaveChanges();
                var mesaj = "Fatura başarıyla kaydedildi.";
                if (fatura_satirList.Count() == 0)
                    mesaj += " Lütfen faturaya stok ekleyiniz.";
                TempData["mesaj"] = mesaj + "||||success";
                ViewBag.Baslik = "Fatura Güncelle";
                return RedirectToAction("/Guncelle", "Fatura", new { gelen.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            else //güncelle
            {
                decimal net_vergi_yeni = 0;
                var guncellenecekKayit = db.faturas.Find(gelen.id);

                //if (guncellenecekKayit.fatura_tipi != gelen.fatura_tipi
                //        || guncellenecekKayit.doviz_id != gelen.doviz_id
                //        || guncellenecekKayit.cari_id != gelen.cari_id
                //        || guncellenecekKayit.islem_tipi != gelen.islem_tipi)  //fatura_tipi, islem_tipi, doviz_cinsi ve cari tip değişirse vergi tekrar hesaplanıyor.
                //{
                //    islem = 1;
                //}
                var detaylar = db.fatura_satir.Where(x => x.fatura_id == gelen.id).ToList();
                if (detaylar != null && detaylar.Count > 0)
                {
                    gelen.toplam_kdvtutar = 0;
                    gelen.toplam_tevkifat = 0;
                    gelen.toplam_oivtutar = 0;
                    gelen.toplam_otvtutar = 0;
                    gelen.toplam_iskonto = 0;
                    gelen.toplam_fiyat = 0;
                    gelen.aratoplam = 0;
                    gelen.toplam_fiyat = 0;
                    gelen.toplam_gvstutar = 0;
                    gelen.toplam_btututar = 0;
                    gelen.toplam_mfvtutar = 0;
                    gelen.toplam_sgktutar = 0;
                    for (int i = 0; i < detaylar.Count; i++)
                    {
                        fatura_satir detay = (fatura_satir)detaylar[i];
                        if (gelen.fatura_tipi == Utilities.EMustahsil) //Müstahsil ise
                        {
                            gelen.toplam_iskonto += detay.iskonto_toplam * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_gvstutar += detay.gvs_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_sgktutar += detay.sgk_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_mfvtutar += detay.mfv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_btututar += detay.btu_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);

                            detay.kdv_orani = 0;
                            detay.kdv_tutar = 0;
                            detay.tevkifat_orani = 0;
                            detay.tevkifat = 0;
                            detay.otv_orani = 0;
                            detay.otv_tutar = 0;
                            detay.oiv_orani = 0;
                            detay.oiv_tutar = 0;
                            var detay_mustahsil_vergi = detay.gvs_tutar + detay.sgk_tutar + detay.mfv_tutar + detay.btu_tutar;
                            detay.fiyat = (detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam - detay_mustahsil_vergi;
                            db.Entry(detay).State = EntityState.Modified;
                            db.SaveChanges();
                            gelen.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / gelen.doviz_kuru);
                        }
                        else if (gelen.fatura_tipi == Utilities.ESMM) //smm ise
                        {
                            gelen.toplam_iskonto += detay.iskonto_toplam ?? 0 * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_kdvtutar += detay.kdv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_tevkifat += detay.tevkifat * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_gvstutar += detay.gvs_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            //gelen.toplam_otvtutar += detay.otv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            //gelen.toplam_oivtutar += detay.oiv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);

                            //detay.gvs_orani = 0;
                            //detay.gvs_tutar = 0;
                            detay.sgk_orani = 0;
                            detay.sgk_tutar = 0;
                            detay.mfv_orani = 0;
                            detay.mfv_tutar = 0;
                            detay.btu_orani = 0;
                            detay.btu_tutar = 0;
                            var detay_fatura_vergi = detay.kdv_tutar + detay.otv_tutar + detay.oiv_tutar;
                            detay.fiyat = (detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam ?? 0;
                            gelen.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / gelen.doviz_kuru);

                            //net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / gelen.doviz_kuru));
                            net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.gvs_tutar) * (detay.doviz_kuru1 / gelen.doviz_kuru));
                            if (gelen.islem_tipi == 1) //Toptan ise
                                gelen.toplam_fiyat += net_vergi_yeni;
                            else
                                gelen.aratoplam -= net_vergi_yeni;

                            db.Entry(detay).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            gelen.toplam_iskonto += detay.iskonto_toplam * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_kdvtutar += detay.kdv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_tevkifat += detay.tevkifat * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_otvtutar += detay.otv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);
                            gelen.toplam_oivtutar += detay.oiv_tutar * (detay.doviz_kuru1 / gelen.doviz_kuru);

                            detay.gvs_orani = 0;
                            detay.gvs_tutar = 0;
                            detay.sgk_orani = 0;
                            detay.sgk_tutar = 0;
                            detay.mfv_orani = 0;
                            detay.mfv_tutar = 0;
                            detay.btu_orani = 0;
                            detay.btu_tutar = 0;
                            var detay_fatura_vergi = detay.kdv_tutar + detay.otv_tutar + detay.oiv_tutar;
                            detay.fiyat = (detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam;
                            gelen.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / gelen.doviz_kuru);

                            net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / gelen.doviz_kuru));
                            if (gelen.islem_tipi == 1) //Toptan ise
                                gelen.toplam_fiyat += net_vergi_yeni;
                            else
                                gelen.aratoplam -= net_vergi_yeni;

                            db.Entry(detay).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //if (gelen.fatura_tipi != 5 && gelen.islem_tipi == 1) //müstahsil değil ve toptan ise
                    //{
                    //    gelen.toplam_fiyat += gelen.toplam_kdvtutar + gelen.toplam_otvtutar + gelen.toplam_oivtutar;
                    //}
                    gelen.detays = detaylar.ToPagedList(SayfaNo ?? 1, sayfaKayitSayisi ?? 4);
                }
                else // faturada satir yoksa fatura vergilerini sıfırlıyorum. 
                {
                    gelen.toplam_kdvtutar = 0;
                    gelen.toplam_iskonto = 0;
                    gelen.toplam_fiyat = 0;
                    gelen.toplam_oivtutar = 0;
                    gelen.toplam_otvtutar = 0;
                    gelen.aratoplam = 0;
                    gelen.toplam_fiyat = 0;
                    gelen.toplam_tevkifat = 0;
                    gelen.toplam_gvstutar = 0;
                    gelen.toplam_btututar = 0;
                    gelen.toplam_mfvtutar = 0;
                    gelen.toplam_sgktutar = 0;
                }
                gelen.toplam_kdvtutar = decimal.Round((decimal)gelen.toplam_kdvtutar, 8, MidpointRounding.AwayFromZero);
                gelen.toplam_iskonto = decimal.Round((decimal)gelen.toplam_iskonto, 8, MidpointRounding.AwayFromZero);
                gelen.toplam_tevkifat = decimal.Round((decimal)gelen.toplam_tevkifat, 8, MidpointRounding.AwayFromZero);
                gelen.toplam_otvtutar = decimal.Round((decimal)gelen.toplam_otvtutar, 8, MidpointRounding.AwayFromZero);
                gelen.toplam_oivtutar = decimal.Round((decimal)gelen.toplam_oivtutar, 8, MidpointRounding.AwayFromZero);
                gelen.toplam_fiyat = decimal.Round((decimal)gelen.toplam_fiyat, 8, MidpointRounding.AwayFromZero);
                gelen.aratoplam = decimal.Round((decimal)gelen.aratoplam, 8, MidpointRounding.AwayFromZero);

                //FaturaDegerYuvarla(gelen);

                gelen.kayit_tarihi = guncellenecekKayit.kayit_tarihi;
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                gelen.kullanici_id = user.id;
                var fayar = (from fa in db.fatura_ayarlari
                             where fa.kullanici_id == user.id && fa.fatura_tipi == gelen.fatura_tipi && (vkn == Utilities.vknAdmin || fa.vkn_tckn == vkn)
                             select fa).FirstOrDefault();
                gelen.seri = fayar != null ? fayar.seri : "";
                ViewBag.Mesaj = "Fatura başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                db.SaveChanges();
            }
            if (gelen.detays == null || gelen.detays.Count == 0)
            {
                List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
                ViewBag.KayitSayisi = fatura_satirList.Count();
                IPagedList<fatura_satir> cariListPaged = fatura_satirList.ToPagedList<fatura_satir>(1, 4);
                gelen.detays = cariListPaged;
            }

            gelen.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)gelen.doviz_id), "Key", "Display");
            gelen.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)gelen.fatura_tipi), "Key", "Display");
            gelen.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi((int)gelen.islem_tipi), "Key", "Display");
            gelen.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias(gelen.fatura_alias, (long)db.caris.Find(gelen.cari_id).vergi_numarasi, (long)Session["vkn_tckn"]), "Key", "Display");
            gelen.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban(gelen.satis_iban, vkn), "Key", "Display");
            ViewBag.KayitSayisi = gelen.detays.Count();
            ViewBag.Mesaj = "Fatura başarıyla güncellendi.";
            ViewBag.Baslik = "Fatura Güncelle";
            return View("FaturaFormu", gelen);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult SatirKaydet(fatura_satir detay, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            //ModelState.Clear();
            if (!ModelState.IsValid) //form doğru dolduruldu mu? // TODO stok fiyat ve birim bilgilerini güncelle
            {
                detay.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)detay.doviz_id1), "Key", "Display");
                detay.DropDownListForBirimler = new SelectList(util.getSelectListForBirimler((int)detay.birim_id), "Key", "Display");
                detay.DropDownListForKdv = new SelectList(util.getSelectListForVergiler(detay.kdv_orani ?? 0, 1), "Key", "Display");
                detay.DropDownListForTev = new SelectList(util.getSelectListForVergiler(detay.tevkifat_orani ?? 0, 4), "Key", "Display");
                detay.DropDownListForOtv = new SelectList(util.getSelectListForVergiler(detay.otv_orani ?? 0, 2), "Key", "Display");
                detay.DropDownListForOiv = new SelectList(util.getSelectListForVergiler(detay.oiv_orani ?? 0, 3), "Key", "Display");
                detay.DropDownListForGvs = new SelectList(util.getSelectListForVergiler(detay.gvs_orani ?? 0, 5), "Key", "Display");
                detay.DropDownListForGvsSMM = new SelectList(util.getSelectListForVergiler(0, 4), "Key", "Display"); //E-SMM için GVS vergisi
                detay.DropDownListForSgk = new SelectList(util.getSelectListForVergiler(detay.sgk_orani ?? 0, 6), "Key", "Display");
                detay.DropDownListForMfv = new SelectList(util.getSelectListForVergiler(detay.mfv_orani ?? 0, 7), "Key", "Display");
                detay.DropDownListForBtu = new SelectList(util.getSelectListForVergiler(detay.btu_orani ?? 0, 8), "Key", "Display");
                detay.DropDownListForTeslimSekli = new SelectList(util.getSelectListForTeslimSekli(detay.teslim_sarti ?? ""), "Key", "Display");
                detay.DropDownListForGonderilmeSekli = new SelectList(util.getSelectListForGonderilmeSekli((int)(detay.gonderilme_sekli == null ? 0 : detay.gonderilme_sekli)), "Key", "Display");
                detay.DropDownListForKapCinsi = new SelectList(util.getSelectListForKapCinsi(detay.kap_cinsi ?? ""), "Key", "Display");
                ViewBag.Baslik = "Fatura Stok Ekle";
                //ViewBag.FaturaTipi = detay.;
                return PartialView("_FaturaDetayFormu", detay);
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            fatura fat = db.faturas.Find(detay.fatura_id);

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                decimal net_vergi_yeni = 0;
                if (detay != null && detay.id == 0) //yeni kayit
                {
                    detay.last_update = DateTime.Now;
                    detay.iskonto_1 = detay.iskonto_1 ?? 0;
                    CultureInfo usCulture = new CultureInfo("en-US");
                    decimal sil = Convert.ToDecimal(detay.kdv_tutar, usCulture);
                    db.fatura_satir.Add(detay);
                    fat.last_update = DateTime.Now;
                    var kur_katsayisi = Decimal.Divide((decimal)detay.doviz_kuru1, (decimal)fat.doviz_kuru);
                    if (fat.fatura_tipi == Utilities.EMustahsil) //Müstahsil ise
                    {
                        fat.toplam_kdvtutar = 0;
                        fat.toplam_tevkifat = 0;
                        fat.toplam_oivtutar = 0;
                        fat.toplam_otvtutar = 0;
                        detay.kdv_tutar = 0;
                        detay.tevkifat = 0;
                        detay.oiv_tutar = 0;
                        detay.otv_tutar = 0;
                        fat.toplam_iskonto += detay.iskonto_toplam * kur_katsayisi;
                        fat.toplam_gvstutar += Decimal.Multiply((decimal)detay.gvs_tutar, kur_katsayisi);
                        fat.toplam_sgktutar += detay.sgk_tutar * kur_katsayisi;
                        fat.toplam_mfvtutar += detay.mfv_tutar * kur_katsayisi;
                        fat.toplam_btututar += detay.btu_tutar * kur_katsayisi;
                        fat.toplam_fiyat += detay.fiyat * kur_katsayisi;
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * kur_katsayisi;

                        fat.toplam_iskonto = decimal.Round((decimal)fat.toplam_iskonto, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_gvstutar = decimal.Round((decimal)fat.toplam_gvstutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_sgktutar = decimal.Round((decimal)fat.toplam_sgktutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_mfvtutar = decimal.Round((decimal)fat.toplam_mfvtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_btututar = decimal.Round((decimal)fat.toplam_btututar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_fiyat = decimal.Round((decimal)fat.toplam_fiyat, 8, MidpointRounding.AwayFromZero);
                        fat.aratoplam = decimal.Round((decimal)fat.aratoplam, 8, MidpointRounding.AwayFromZero);
                        net_vergi_yeni = (decimal)((detay.gvs_tutar + detay.sgk_tutar + detay.mfv_tutar + detay.btu_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                    }
                    else if (fat.fatura_tipi == Utilities.EIhracat) //ihracat ise
                    {
                        fat.toplam_kdvtutar = 0;
                        fat.toplam_tevkifat = 0;
                        fat.toplam_oivtutar = 0;
                        fat.toplam_otvtutar = 0;
                        detay.kdv_tutar = 0;
                        detay.tevkifat = 0;
                        detay.oiv_tutar = 0;
                        detay.otv_tutar = 0;

                        fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;

                        fat.toplam_iskonto += detay.iskonto_toplam * kur_katsayisi;
                        fat.toplam_fiyat += detay.fiyat * kur_katsayisi;
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * kur_katsayisi;

                        fat.toplam_iskonto = decimal.Round((decimal)fat.toplam_iskonto, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_fiyat = decimal.Round((decimal)fat.toplam_fiyat, 8, MidpointRounding.AwayFromZero);
                        fat.aratoplam = decimal.Round((decimal)fat.aratoplam, 8, MidpointRounding.AwayFromZero);
                    }
                    else if (fat.fatura_tipi == Utilities.ESMM) //ihracat ise
                    {
                        //fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        fat.toplam_oivtutar = 0;
                        fat.toplam_otvtutar = 0;
                        //detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;
                        detay.oiv_tutar = 0;
                        detay.otv_tutar = 0;

                        fat.toplam_kdvtutar += detay.kdv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto += detay.iskonto_toplam ?? 0 * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_tevkifat += detay.tevkifat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_gvstutar += detay.gvs_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //fat.toplam_otvtutar += detay.otv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //fat.toplam_oivtutar += detay.oiv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam ?? 0) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_kdvtutar = decimal.Round((decimal)fat.toplam_kdvtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_iskonto = decimal.Round((decimal)fat.toplam_iskonto, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_tevkifat = decimal.Round((decimal)fat.toplam_tevkifat, 8, MidpointRounding.AwayFromZero);
                        //fat.toplam_otvtutar = decimal.Round((decimal)fat.toplam_otvtutar, 8, MidpointRounding.AwayFromZero);
                        //fat.toplam_oivtutar = decimal.Round((decimal)fat.toplam_oivtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_fiyat = decimal.Round((decimal)fat.toplam_fiyat, 8, MidpointRounding.AwayFromZero);
                        fat.aratoplam = decimal.Round((decimal)fat.aratoplam, 8, MidpointRounding.AwayFromZero);

                        //net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.gvs_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        //if (net_vergi_yeni > 0)
                        //    stok_birim_fiyat_guncelle(fat, detay); //girilen bilgilere göre stok, stok_birim ve stok_fiyat tabloları güncelleniyor. 
                        //if (fat.islem_tipi == 1) //Toptan ise
                        //    fat.toplam_fiyat += net_vergi_yeni;
                        //else
                        //    fat.aratoplam -= net_vergi_yeni;
                    }
                    else
                    {
                        fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;
                        fat.toplam_kdvtutar += detay.kdv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto += detay.iskonto_toplam * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_tevkifat += detay.tevkifat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_otvtutar += detay.otv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_oivtutar += detay.oiv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_kdvtutar = decimal.Round((decimal)fat.toplam_kdvtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_iskonto = decimal.Round((decimal)fat.toplam_iskonto, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_tevkifat = decimal.Round((decimal)fat.toplam_tevkifat, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_otvtutar = decimal.Round((decimal)fat.toplam_otvtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_oivtutar = decimal.Round((decimal)fat.toplam_oivtutar, 8, MidpointRounding.AwayFromZero);
                        fat.toplam_fiyat = decimal.Round((decimal)fat.toplam_fiyat, 8, MidpointRounding.AwayFromZero);
                        fat.aratoplam = decimal.Round((decimal)fat.aratoplam, 8, MidpointRounding.AwayFromZero);

                        net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        //if (net_vergi_yeni > 0)
                        //    stok_birim_fiyat_guncelle(fat, detay); //girilen bilgilere göre stok, stok_birim ve stok_fiyat tabloları güncelleniyor. 
                        if (fat.islem_tipi == 1) //Toptan ise
                            fat.toplam_fiyat += net_vergi_yeni;
                        else
                            fat.aratoplam -= net_vergi_yeni;
                    }
                    TempData["mesaj"] = "Stok faturaya başarıyla eklendi.||||success";
                    db.Entry(fat).State = EntityState.Modified;
                }
                else //güncelle
                {
                    var guncellenecekKayit = db.fatura_satir.Find(detay.id);
                    //eski detayın vergilerini ve fiyatlarını faturadakilerden çıkarıyoruz.
                    if (fat.fatura_tipi == Utilities.EMustahsil) //Müstahsil ise
                    {
                        fat.toplam_kdvtutar = 0;
                        fat.toplam_tevkifat = 0;
                        fat.toplam_oivtutar = 0;
                        fat.toplam_otvtutar = 0;
                        detay.kdv_tutar = 0;
                        detay.tevkifat = 0;
                        detay.oiv_tutar = 0;
                        detay.otv_tutar = 0;
                        fat.toplam_iskonto -= guncellenecekKayit.iskonto_toplam * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto = fat.toplam_iskonto < 0 ? 0 : fat.toplam_iskonto;
                        fat.toplam_gvstutar -= guncellenecekKayit.gvs_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_gvstutar = fat.toplam_gvstutar < 0 ? 0 : fat.toplam_gvstutar;
                        fat.toplam_sgktutar -= guncellenecekKayit.sgk_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_sgktutar = fat.toplam_sgktutar < 0 ? 0 : fat.toplam_sgktutar;
                        fat.toplam_mfvtutar -= guncellenecekKayit.mfv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_mfvtutar = fat.toplam_mfvtutar < 0 ? 0 : fat.toplam_mfvtutar;
                        fat.toplam_btututar -= guncellenecekKayit.btu_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_btututar = fat.toplam_btututar < 0 ? 0 : fat.toplam_btututar;
                        fat.aratoplam -= ((guncellenecekKayit.miktari * guncellenecekKayit.birim_fiyati) - guncellenecekKayit.iskonto_toplam) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam = fat.aratoplam < 0 ? 0 : fat.aratoplam;
                        fat.toplam_fiyat -= guncellenecekKayit.fiyat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat = fat.toplam_fiyat < 0 ? 0 : fat.toplam_fiyat;
                        //yeni detayın vergilerini faturadakilere ekliyoruz.
                        fat.last_update = DateTime.Now;
                        fat.toplam_iskonto += detay.iskonto_toplam * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_gvstutar += detay.gvs_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_sgktutar += detay.sgk_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_mfvtutar += detay.mfv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_btututar += detay.btu_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat += detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto = Math.Round((decimal)fat.toplam_iskonto, 4);
                        net_vergi_yeni = (decimal)((detay.gvs_tutar + detay.sgk_tutar + detay.mfv_tutar + detay.btu_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                    }
                    else if (fat.fatura_tipi == Utilities.EIhracat) //Ihracat ise
                    {
                        fat.toplam_kdvtutar = 0;
                        fat.toplam_tevkifat = 0;
                        fat.toplam_oivtutar = 0;
                        fat.toplam_otvtutar = 0;
                        detay.kdv_tutar = 0;
                        detay.tevkifat = 0;
                        detay.oiv_tutar = 0;
                        detay.otv_tutar = 0;

                        fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;

                        fat.toplam_iskonto -= guncellenecekKayit.iskonto_toplam * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto = fat.toplam_iskonto < 0 ? 0 : fat.toplam_iskonto;
                        fat.toplam_fiyat -= guncellenecekKayit.fiyat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat = fat.toplam_fiyat < 0 ? 0 : fat.toplam_fiyat;
                        fat.aratoplam -= ((guncellenecekKayit.miktari * guncellenecekKayit.birim_fiyati) - guncellenecekKayit.iskonto_toplam) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam = fat.aratoplam < 0 ? 0 : fat.aratoplam;
                        //yeni detayın vergilerini faturadakilere ekliyoruz.
                        fat.last_update = DateTime.Now;
                        decimal kur_carpani = (Decimal.Divide((decimal)detay.doviz_kuru1, (decimal)fat.doviz_kuru));
                        fat.toplam_iskonto += detay.iskonto_toplam * kur_carpani;
                        fat.toplam_fiyat += detay.fiyat * kur_carpani;
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * kur_carpani;
                    }
                    else if (fat.fatura_tipi == Utilities.ESMM) //smm ise
                    {
                        //fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        //detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;
                        fat.toplam_kdvtutar -= guncellenecekKayit.kdv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_kdvtutar = fat.toplam_kdvtutar < 0 ? 0 : fat.toplam_kdvtutar;
                        fat.toplam_iskonto -= guncellenecekKayit.iskonto_toplam ?? 0 * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto = (fat.toplam_iskonto ?? 0) < 0 ? 0 : fat.toplam_iskonto ?? 0;
                        fat.toplam_tevkifat -= guncellenecekKayit.tevkifat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_tevkifat = fat.toplam_tevkifat < 0 ? 0 : fat.toplam_tevkifat;
                        fat.toplam_gvstutar -= guncellenecekKayit.gvs_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_gvstutar = fat.toplam_gvstutar < 0 ? 0 : fat.toplam_gvstutar;
                        //fat.toplam_otvtutar -= guncellenecekKayit.otv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        //fat.toplam_otvtutar = fat.toplam_otvtutar < 0 ? 0 : fat.toplam_otvtutar;
                        //fat.toplam_oivtutar -= guncellenecekKayit.oiv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        //fat.toplam_oivtutar = fat.toplam_oivtutar < 0 ? 0 : fat.toplam_oivtutar;
                        //var net_vergi_eski = ((guncellenecekKayit.kdv_tutar - guncellenecekKayit.tevkifat) + guncellenecekKayit.otv_tutar + guncellenecekKayit.oiv_tutar) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        var net_vergi_eski = ((guncellenecekKayit.kdv_tutar - guncellenecekKayit.tevkifat) + guncellenecekKayit.gvs_tutar) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat -= guncellenecekKayit.fiyat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        //if (fat.islem_tipi == 1) // toptan ise
                        //    fat.toplam_fiyat -= net_vergi_eski;
                        fat.toplam_fiyat = fat.toplam_fiyat < 0 ? 0 : fat.toplam_fiyat;
                        fat.aratoplam -= ((guncellenecekKayit.miktari * guncellenecekKayit.birim_fiyati) - guncellenecekKayit.iskonto_toplam) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam = fat.aratoplam < 0 ? 0 : fat.aratoplam;

                        //yeni detayın vergilerini faturadakilere ekliyoruz.
                        fat.last_update = DateTime.Now;
                        decimal kur_carpani = (Decimal.Divide((decimal)detay.doviz_kuru1, (decimal)fat.doviz_kuru));
                        fat.toplam_kdvtutar += detay.kdv_tutar * kur_carpani;
                        fat.toplam_iskonto += detay.iskonto_toplam ?? 0 * kur_carpani;
                        fat.toplam_tevkifat += detay.tevkifat * kur_carpani;
                        fat.toplam_gvstutar += detay.gvs_tutar * kur_carpani;
                        //fat.toplam_otvtutar += detay.otv_tutar * kur_carpani;
                        //fat.toplam_oivtutar += detay.oiv_tutar * kur_carpani;
                        fat.toplam_fiyat += detay.fiyat * kur_carpani;
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam ?? 0) * kur_carpani;
                        //net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.gvs_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        //if (fat.islem_tipi == 1) //toptan ise                      
                        //    fat.toplam_fiyat += net_vergi_yeni;
                        //if (fat.islem_tipi == 2) // perakende ise
                        //    fat.aratoplam -= net_vergi_yeni;
                    }
                    else
                    {
                        fat.toplam_gvstutar = 0;
                        fat.toplam_btututar = 0;
                        fat.toplam_mfvtutar = 0;
                        fat.toplam_sgktutar = 0;
                        detay.gvs_tutar = 0;
                        detay.btu_tutar = 0;
                        detay.mfv_tutar = 0;
                        detay.sgk_tutar = 0;
                        fat.toplam_kdvtutar -= guncellenecekKayit.kdv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_kdvtutar = fat.toplam_kdvtutar < 0 ? 0 : fat.toplam_kdvtutar;
                        fat.toplam_iskonto -= guncellenecekKayit.iskonto_toplam * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_iskonto = fat.toplam_iskonto < 0 ? 0 : fat.toplam_iskonto;
                        fat.toplam_tevkifat -= guncellenecekKayit.tevkifat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_tevkifat = fat.toplam_tevkifat < 0 ? 0 : fat.toplam_tevkifat;
                        fat.toplam_otvtutar -= guncellenecekKayit.otv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_otvtutar = fat.toplam_otvtutar < 0 ? 0 : fat.toplam_otvtutar;
                        fat.toplam_oivtutar -= guncellenecekKayit.oiv_tutar * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_oivtutar = fat.toplam_oivtutar < 0 ? 0 : fat.toplam_oivtutar;
                        var net_vergi_eski = ((guncellenecekKayit.kdv_tutar - guncellenecekKayit.tevkifat) + guncellenecekKayit.otv_tutar + guncellenecekKayit.oiv_tutar) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.toplam_fiyat -= guncellenecekKayit.fiyat * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.islem_tipi == 1) // toptan ise
                            fat.toplam_fiyat -= net_vergi_eski;
                        fat.toplam_fiyat = fat.toplam_fiyat < 0 ? 0 : fat.toplam_fiyat;
                        fat.aratoplam -= ((guncellenecekKayit.miktari * guncellenecekKayit.birim_fiyati) - guncellenecekKayit.iskonto_toplam) * (guncellenecekKayit.doviz_kuru1 / fat.doviz_kuru);
                        fat.aratoplam = fat.aratoplam < 0 ? 0 : fat.aratoplam;

                        //yeni detayın vergilerini faturadakilere ekliyoruz.
                        fat.last_update = DateTime.Now;
                        decimal kur_carpani = (Decimal.Divide((decimal)detay.doviz_kuru1, (decimal)fat.doviz_kuru));
                        fat.toplam_kdvtutar += detay.kdv_tutar * kur_carpani;
                        fat.toplam_iskonto += detay.iskonto_toplam * kur_carpani;
                        fat.toplam_tevkifat += detay.tevkifat * kur_carpani;
                        fat.toplam_otvtutar += detay.otv_tutar * kur_carpani;
                        fat.toplam_oivtutar += detay.oiv_tutar * kur_carpani;
                        fat.toplam_fiyat += detay.fiyat * kur_carpani;
                        fat.aratoplam += ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * kur_carpani;
                        net_vergi_yeni = (decimal)(((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru));
                        if (fat.islem_tipi == 1) //toptan ise                      
                            fat.toplam_fiyat += net_vergi_yeni;
                        if (fat.islem_tipi == 2) // perakende ise
                            fat.aratoplam -= net_vergi_yeni;
                    }
                    detay.last_update = DateTime.Now;
                    TempData["mesaj"] = "Stok başarıyla güncellendi.||||success";
                    db.Entry(guncellenecekKayit).CurrentValues.SetValues(detay);
                    db.Entry(fat).State = EntityState.Modified;
                }
                if (net_vergi_yeni > 0)
                    stok_birim_fiyat_guncelle(fat, detay); //girilen bilgilere göre stok, stok_birim ve stok_fiyat tabloları güncelleniyor. 
                db.SaveChanges();
                transaction.Commit();
            }
            return RedirectToAction("/Guncelle", "Fatura", new { id = detay.fatura_id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Yeni()
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            kullanici user = (kullanici)Session["kullanici"];
            var gelen = new fatura { };
            gelen.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz(1), "Key", "Display");
            gelen.doviz_kuru = 1;
            gelen.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi(1), "Key", "Display");
            gelen.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi(1), "Key", "Display");
            gelen.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias("", 0, (long)Session["vkn_tckn"]), "Key", "Display");
            gelen.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban("", vkn), "Key", "Display");
            List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
            ViewBag.KayitSayisi = fatura_satirList.Count();
            IPagedList<fatura_satir> cariListPaged = fatura_satirList.ToPagedList<fatura_satir>(1, 4);
            gelen.detays = cariListPaged;
            var fayar = (from fa in db.fatura_ayarlari
                         where fa.kullanici_id == user.id && fa.fatura_tipi == 1 && (vkn == Utilities.vknAdmin || fa.vkn_tckn == vkn)
                         select fa).FirstOrDefault();
            gelen.seri = fayar.seri;
            gelen.fatura_tarihi = DateTime.Now;
            ViewBag.Baslik = "Yeni Fatura";
            return View("FaturaFormu", gelen);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Guncelle(int id, int? SayfaNo, string SortOrder, string SearchString)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            kullanici user = (kullanici)Session["kullanici"];
            int _sayfaNo = SayfaNo ?? 1;
            fatura model = null;
            if (vkn == vknAdmin)
                model = db.faturas.FirstOrDefault(x => x.id == id);
            else
                model = db.faturas.FirstOrDefault(x => x.id == id && x.isdeleted == false && x.vkn_tckn == vkn);
            if (model != null)
            {
                var fdetay = from fd in db.fatura_satir
                             where fd.fatura_id == id
                             select fd;
                switch (SortOrder)
                {
                    case "id_desc":
                        fdetay = fdetay.OrderByDescending(s => s.id);
                        break;
                    case "last_update_desc":
                        fdetay = fdetay.OrderByDescending(s => s.last_update);
                        break;
                    case "last_update":
                        fdetay = fdetay.OrderBy(s => s.last_update);
                        break;
                    default:
                        fdetay = fdetay.OrderByDescending(s => s.last_update);
                        break;
                }
                List<fatura_satir> faturadetaylarList = fdetay.ToList();
                ViewBag.KayitSayisi = faturadetaylarList.Count();
                IPagedList<fatura_satir> faturaListPaged = faturadetaylarList.ToPagedList<fatura_satir>(_sayfaNo, 4);
                model.detays = faturaListPaged;
            }
            model.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)model.doviz_id), "Key", "Display");
            model.DropDownListForFaturaTipi = new SelectList(Utilities.getSelectListForFaturaTipi((int)model.fatura_tipi), "Key", "Display");
            model.DropDownListForIslemTipi = new SelectList(Utilities.getSelectListForIslemTipi((int)model.islem_tipi), "Key", "Display");
            model.DropDownListForFaturaAlias = new SelectList(util.getSelectListForFaturaAlias(model.fatura_alias, (long)db.caris.Find(model.cari_id).vergi_numarasi, (long)Session["vkn_tckn"]), "Key", "Display");
            model.DropDownListForSatisIban = new SelectList(util.getSelectListForSatisIban(model.satis_iban, vkn), "Key", "Display");
            model.cari_adi = (db.caris.Find(model.cari_id).ad + " " + db.caris.Find(model.cari_id).soyad).Trim();
            var fayar = (from fa in db.fatura_ayarlari
                         where fa.kullanici_id == user.id && fa.fatura_tipi == model.fatura_tipi && (vkn == Utilities.vknAdmin || fa.vkn_tckn == vkn)
                         select fa).FirstOrDefault();
            model.seri = fayar.seri;
            ViewBag.SortOrder = SortOrder;
            ViewBag.SearchString = SearchString;
            ViewBag.SayfaNo = SayfaNo;
            ViewBag.Baslik = "Fatura Güncelle";
            return View("FaturaFormu", model);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Sil(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.faturas.FirstOrDefault(x => x.id == id && x.isdeleted == false && (x.fatura_onaykontrol == null || x.fatura_onaykontrol == 0) && x.vkn_tckn == vkn);
            if (kayit == null)
            {
                TempData.Clear();
                return HttpNotFound();
            }
            else
            { //TODO satırlar silinecek mi?
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                return RedirectToAction("/FaturaListesi", "Fatura", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult SatirSil(int id, int fatura_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var detay = db.fatura_satir.Find(id);
            var fat = db.faturas.Find(detay.fatura_id);
            if (detay != null && fat != null)
            {
                if (((int)detay.fatura_id == fatura_id && fat.vkn_tckn == vkn) || vkn == vknAdmin)
                {
                    if (fat.fatura_tipi == Utilities.EMustahsil) //Müstahsil ise
                    {
                        fat.last_update = DateTime.Now;
                        fat.toplam_iskonto -= detay.iskonto_toplam * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_iskonto < 0)
                            fat.toplam_iskonto = 0;
                        fat.toplam_gvstutar -= detay.gvs_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_gvstutar < 0)
                            fat.toplam_gvstutar = 0;
                        fat.toplam_sgktutar -= detay.sgk_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_sgktutar < 0)
                            fat.toplam_sgktutar = 0;
                        fat.toplam_mfvtutar -= detay.mfv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_mfvtutar < 0)
                            fat.toplam_mfvtutar = 0;
                        fat.toplam_btututar -= detay.btu_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_btututar < 0)
                            fat.toplam_btututar = 0;
                        fat.toplam_fiyat -= detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_fiyat < 0)
                            fat.toplam_fiyat = 0;
                        fat.aratoplam -= ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.aratoplam < 0)
                            fat.aratoplam = 0;
                        //db.Entry(fat).State = EntityState.Modified;
                        //db.fatura_satir.Remove(detay);
                    }
                    else if (fat.fatura_tipi == Utilities.ESMM) //Müstahsil ise
                    {
                        fat.last_update = DateTime.Now;
                        fat.toplam_kdvtutar -= detay.kdv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_kdvtutar < 0)
                            fat.toplam_kdvtutar = 0;
                        fat.toplam_iskonto -= (detay.iskonto_toplam ?? 0) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_iskonto < 0)
                            fat.toplam_iskonto = 0;
                        fat.toplam_tevkifat -= detay.tevkifat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_tevkifat < 0)
                            fat.toplam_tevkifat = 0;
                        fat.toplam_gvstutar -= detay.gvs_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_gvstutar < 0)
                            fat.toplam_gvstutar = 0;
                        //fat.toplam_otvtutar -= detay.otv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //if (fat.toplam_otvtutar < 0)
                        //    fat.toplam_otvtutar = 0;
                        //fat.toplam_oivtutar -= detay.oiv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //if (fat.toplam_oivtutar < 0)
                        //    fat.toplam_oivtutar = 0;
                        fat.aratoplam -= ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam ?? 0) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.aratoplam < 0)
                            fat.aratoplam = 0;
                        fat.toplam_fiyat -= detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_fiyat < 0)
                            fat.toplam_fiyat = 0;
                        //var net_vergi = ((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //var net_vergi = ((detay.kdv_tutar - detay.tevkifat) + detay.gvs_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //if (fat.islem_tipi == 1) //Toptan ise
                        //{
                        //    fat.toplam_fiyat -= net_vergi;
                        //}
                        //else //Perakende ise
                        //{
                        //    fat.toplam_fiyat -= detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //}
                        if (fat.toplam_fiyat < 0)
                            fat.toplam_fiyat = 0;
                        //db.Entry(fat).State = EntityState.Modified;
                        //db.fatura_satir.Remove(detay);
                    }
                    else
                    {
                        fat.last_update = DateTime.Now;
                        fat.toplam_kdvtutar -= detay.kdv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_kdvtutar < 0)
                            fat.toplam_kdvtutar = 0;
                        fat.toplam_iskonto -= detay.iskonto_toplam * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_iskonto < 0)
                            fat.toplam_iskonto = 0;
                        fat.toplam_tevkifat -= detay.tevkifat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_tevkifat < 0)
                            fat.toplam_tevkifat = 0;
                        fat.toplam_otvtutar -= detay.otv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_otvtutar < 0)
                            fat.toplam_otvtutar = 0;
                        fat.toplam_oivtutar -= detay.oiv_tutar * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_oivtutar < 0)
                            fat.toplam_oivtutar = 0;
                        fat.aratoplam -= ((detay.miktari * detay.birim_fiyati) - detay.iskonto_toplam) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.aratoplam < 0)
                            fat.aratoplam = 0;
                        fat.toplam_fiyat -= detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.toplam_fiyat < 0)
                            fat.toplam_fiyat = 0;
                        var net_vergi = ((detay.kdv_tutar - detay.tevkifat) + detay.otv_tutar + detay.oiv_tutar) * (detay.doviz_kuru1 / fat.doviz_kuru);
                        if (fat.islem_tipi == 1) //Toptan ise
                        {
                            fat.toplam_fiyat -= net_vergi;
                        }
                        //else //Perakende ise
                        //{
                        //    fat.toplam_fiyat -= detay.fiyat * (detay.doviz_kuru1 / fat.doviz_kuru);
                        //}
                        if (fat.toplam_fiyat < 0)
                            fat.toplam_fiyat = 0;
                        //db.Entry(fat).State = EntityState.Modified;
                        //db.fatura_satir.Remove(detay);
                    }
                    TempData["mesaj"] = "Stok faturadan başarıyla silindi.||||success";
                    db.Entry(fat).State = EntityState.Modified;
                    db.fatura_satir.Remove(detay);
                    db.SaveChanges();
                }
                else
                    ViewBag.Mesaj = "Silinecek kayıt bulunamadı.";
            }
            else
                ViewBag.Mesaj = "Silinecek kayıt bulunamadı.";
            return RedirectToAction("/Guncelle", "Fatura", new { id = detay.fatura_id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult SatirEkle(int id, int fatura_id, int fatura_tipi, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            fatura_satir satir = null;
            if (id == 0) //yeni kayıt
            {
                satir = new fatura_satir();
                satir.fatura_id = fatura_id;
                satir.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz(1), "Key", "Display");
                satir.doviz_kuru1 = 1;
                satir.DropDownListForBirimler = new SelectList(util.getSelectListForBirimler(1), "Key", "Display");
                satir.DropDownListForKdv = new SelectList(util.getSelectListForVergiler(0, 1), "Key", "Display");
                satir.DropDownListForTev = new SelectList(util.getSelectListForVergiler(0, 4), "Key", "Display");
                satir.DropDownListForOtv = new SelectList(util.getSelectListForVergiler(0, 2), "Key", "Display");
                satir.DropDownListForOiv = new SelectList(util.getSelectListForVergiler(0, 3), "Key", "Display");
                satir.DropDownListForGvs = new SelectList(util.getSelectListForVergiler(0, 5), "Key", "Display");
                satir.DropDownListForGvsSMM = new SelectList(util.getSelectListForVergiler(0, 4), "Key", "Display"); //E-SMM için GVS vergisi
                satir.DropDownListForSgk = new SelectList(util.getSelectListForVergiler(0, 6), "Key", "Display");
                satir.DropDownListForMfv = new SelectList(util.getSelectListForVergiler(0, 7), "Key", "Display");
                satir.DropDownListForBtu = new SelectList(util.getSelectListForVergiler(0, 8), "Key", "Display");
                satir.DropDownListForTeslimSekli = new SelectList(util.getSelectListForTeslimSekli(""), "Key", "Display");
                satir.DropDownListForGonderilmeSekli = new SelectList(util.getSelectListForGonderilmeSekli(0), "Key", "Display");
                satir.DropDownListForKapCinsi = new SelectList(util.getSelectListForKapCinsi(""), "Key", "Display");
                satir.iskonto_1 = 0;
                ViewBag.Baslik = "Fatura Stok Ekle";
                ViewBag.FaturaTipi = fatura_tipi;
                return View("_FaturaDetayFormu", satir);
            }
            else //güncelle
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                satir = db.fatura_satir.Find(id);
                var fat = db.faturas.Find(satir.fatura_id);
                if (satir != null && fat != null)
                {
                    if (((int)satir.fatura_id == fatura_id && fat.vkn_tckn == vkn) || vkn == vknAdmin)
                    {
                        satir.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)satir.doviz_id1), "Key", "Display");
                        satir.DropDownListForBirimler = new SelectList(util.getSelectListForBirimler((int)satir.birim_id), "Key", "Display");
                        satir.DropDownListForKdv = new SelectList(util.getSelectListForVergiler(satir.kdv_orani ?? 0, 1), "Key", "Display");
                        satir.DropDownListForTev = new SelectList(util.getSelectListForVergiler(satir.tevkifat_orani ?? 0, 4), "Key", "Display");
                        satir.DropDownListForOtv = new SelectList(util.getSelectListForVergiler(satir.otv_orani ?? 0, 2), "Key", "Display");
                        satir.DropDownListForOiv = new SelectList(util.getSelectListForVergiler(satir.oiv_orani ?? 0, 3), "Key", "Display");
                        satir.DropDownListForGvs = new SelectList(util.getSelectListForVergiler(satir.gvs_orani ?? 0, 5), "Key", "Display");
                        satir.DropDownListForGvsSMM = new SelectList(util.getSelectListForVergiler(0, 4), "Key", "Display"); //E-SMM için GVS vergisi
                        satir.DropDownListForSgk = new SelectList(util.getSelectListForVergiler(satir.sgk_orani ?? 0, 6), "Key", "Display");
                        satir.DropDownListForMfv = new SelectList(util.getSelectListForVergiler(satir.mfv_orani ?? 0, 7), "Key", "Display");
                        satir.DropDownListForBtu = new SelectList(util.getSelectListForVergiler(satir.btu_orani ?? 0, 8), "Key", "Display");
                        satir.DropDownListForTeslimSekli = new SelectList(util.getSelectListForTeslimSekli(satir.teslim_sarti ?? ""), "Key", "Display");
                        satir.DropDownListForGonderilmeSekli = new SelectList(util.getSelectListForGonderilmeSekli((int)(satir.gonderilme_sekli == null ? 0 : satir.gonderilme_sekli)), "Key", "Display");
                        satir.DropDownListForKapCinsi = new SelectList(util.getSelectListForKapCinsi(satir.kap_cinsi ?? ""), "Key", "Display");
                        ViewBag.FaturaTipi = fatura_tipi;
                        return View("_FaturaDetayFormu", satir);
                    }
                    else
                        ViewBag.Mesaj = "Beklenmedik bir hata oluştu.";
                }
                else
                    ViewBag.Mesaj = "Beklenmedik bir hata oluştu.";

                return RedirectToAction("/FaturaListesi", "Fatura", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public ActionResult CariBul(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            IPagedList<cari> cariListPaged = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                kullanici user = (kullanici)Session["kullanici"];
                long vknAdmin = Utilities.vknAdmin;
                //cari model = new cari { };
                ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                ViewBag.CodeSortParm = sortOrder == "kodu" ? "kodu_desc" : "kodu";
                ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                ViewBag.SearchString = searchString = searchString ?? "";
                ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;

                //IPagedList<cari> cariListPaged = null;
                //List<cari> cariList = null;

                //if (String.IsNullOrEmpty(cari_adi))
                //{
                //    cariList = Enumerable.Empty<cari>().AsQueryable().ToList();
                //    ViewBag.KayitSayisi = cariList.Count();
                //    //cariListPaged = cariList.ToPagedList<cari>(1, 4);
                //}
                //else
                //{ 
                var cariList = db.caris.Where(s => (s.ad.Contains(searchString) || s.soyad.Contains(searchString) || (s.ad + s.soyad).Contains(searchString) || (s.ad + " " + s.soyad).Contains(searchString)) && s.isdeleted == false && s.vkn_tckn == vkn);
                //var cariList1 = from cari in db.caris.Where(s => (s.ad.Contains(searchString) || s.soyad.Contains(searchString) || string.Concat(s.ad, s.soyad).Contains(searchString)) && s.isdeleted == false && s.vkn_tckn == vkn) select new { adsoyad = cari.ad + " " + cari.soyad};
                switch (sortOrder)
                {
                    case "last_update_desc":
                        cariList = cariList.OrderByDescending(s => s.last_update);
                        break;
                    case "last_update":
                        cariList = cariList.OrderBy(s => s.last_update);
                        break;
                    case "adi":
                        cariList = cariList.OrderBy(s => s.ad);
                        break;
                    case "adi_desc":
                        cariList = cariList.OrderByDescending(s => s.ad);
                        break;
                    default:
                        cariList = cariList.OrderBy(s => s.ad);
                        break;
                }
                int _sayfaNo = SayfaNo ?? 1;
                ViewBag.SayfaNo = _sayfaNo;
                int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                ViewBag.KayitSayisi = cariList.ToList().Count();
                cariListPaged = cariList.ToPagedList<cari>(_sayfaNo, _sayfaKayitSayisi);
                //}
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_CariBul", cariListPaged);
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
            return View("CariBul", cariListPaged);
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult CariBulBir(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi) //ürün adına stok kodu ya da adı tam olarak girilirse ve 1 kayıt dönerse, bu kayıt modal stok bul açılmadan fatura_satir_formu na yazılır.
        {
            List<cari> cariList = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                kullanici user = (kullanici)Session["kullanici"];
                long vknAdmin = Utilities.vknAdmin;
                cari model = new cari { };
                //if (String.IsNullOrEmpty(searchString))
                //cariList = Enumerable.Empty<cari>().AsQueryable().ToList();
                //else if (cari_adi.Length < 3)
                //{
                //    cariList = Enumerable.Empty<cari>().AsQueryable().ToList();
                //    ViewBag.Mesaj = "Ürün adı alanına en az 3 harf giriniz.";
                //}
                //else
                //{
                //if (searchString.Length > 50)
                //    cari_adi = cari_adi.Substring(0, 50);
                //StringBuilder query = new StringBuilder("select id,vkn_tckn,adi,birim_id_son,doviz_id_son,fiyat_son,kdv_son,otv_son,oiv_son from " +
                //    "(select st.id,st.vkn_tckn,st.adi,isnull(sb.birim_id,0) as birim_id_son,isnull(sf.doviz_id,0) as doviz_id_son,isnull(sf.fiyat,0) as fiyat_son," +
                //    "isnull(st.kdv_orani,0) as kdv_son, isnull(st.otv_orani,0) as otv_son, isnull(st.oiv_orani,0) as oiv_son,sb.katsayi,sb.agirlik from test.dbo.stok st " +
                //    "cross apply (select sb1.id,sb1.birim_id,sb1.katsayi,sb1.agirlik " +
                //    "from stok_birim sb1 where (st.adi = '").Append(stok_adi).Append("' or st.kodu = '").Append(stok_adi).Append("') and st.id = sb1.stok_id) sb " +
                //    "CROSS APPLY (select top 1 sf1.doviz_id,sf1.fiyat,sf1.last_update " +
                //    "from stok_fiyat sf1 where sb.id = sf1.stok_birim_id order by sf1.last_update desc) sf) ss");
                cariList = db.caris.Where(x => x.ad.Contains(searchString) && x.isdeleted == false && x.vkn_tckn == vkn).ToList();
                ViewBag.KayitSayisi = cariList.Count();
                //stokList = db.Database.SqlQuery<stok_birim_fiyat>(query.ToString()).ToList();
                if (cariList != null && cariList.Count() == 1)
                {
                    return Json(cariList, JsonRequestBehavior.AllowGet);
                    //return PartialView("_FaturaDetayFormu", stokList);
                }
                //}
                cariList = Enumerable.Empty<cari>().AsQueryable().ToList();
                //ViewBag.Mesaj = "Ürün adı alanına en az 3 harf giriniz.";
            }
            return Json(cariList, JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult StokBul(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            //List<stok_birim_fiyat> stokList = null;
            IPagedList<stok_birim_fiyat> stokListPaged = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                kullanici user = (kullanici)Session["kullanici"];
                long vknAdmin = Utilities.vknAdmin;
                stok model = new stok { };
                ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                ViewBag.CodeSortParm = sortOrder == "kodu" ? "kodu_desc" : "kodu";
                ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                ViewBag.SearchString = searchString = searchString ?? "";
                ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                //if (String.IsNullOrEmpty(stok_adi))
                //    stokList = Enumerable.Empty<stok_birim_fiyat>().AsQueryable().ToList();
                //else if (stok_adi.Length < 3)
                //{
                //    stokList = Enumerable.Empty<stok_birim_fiyat>().AsQueryable().ToList();
                //    ViewBag.Mesaj = "Ürün adı alanına en az 3 harf giriniz.";
                //}
                //else
                //{
                //var cariList = db.caris.Where(s => s.adi.Contains(searchString) && s.isdeleted == false && (s.vkn_tckn == vkn || vkn == vknAdmin));


                //}

                if (searchString.Length > 50)
                    searchString = searchString.Substring(0, 50);
                StringBuilder query = new StringBuilder("select id,vkn_tckn,adi,gtip_no,birim_id_son,doviz_id_son,fiyat_son,kdv_son,otv_son,oiv_son,gvs_son,sgk_son,mfv_son,btu_son from " +
                    "(select st.id,st.vkn_tckn,st.adi,isnull(st.gtip_no,0) as gtip_no,isnull(sb.birim_id,0) as birim_id_son,isnull(sf.doviz_id,0) as doviz_id_son,isnull(sf.fiyat,0) as fiyat_son," +
                    "isnull(st.kdv_orani,0) as kdv_son, isnull(st.otv_orani,0) as otv_son, isnull(st.oiv_orani,0) as oiv_son," +
                    "isnull(st.gvs_orani,0) as gvs_son, isnull(st.sgk_orani,0) as sgk_son, isnull(st.mfv_orani,0) as mfv_son, isnull(st.btu_orani,0) as btu_son," +
                    "sb.katsayi, sb.agirlik from stok st " +
                    "cross apply (select sb1.id,sb1.birim_id,sb1.katsayi,sb1.agirlik " +
                    "from stok_birim sb1 where (st.adi like '%");
                query.Append(searchString);
                query.Append("%' or st.kodu like '%");
                query.Append(searchString);
                query.Append("%') and st.id = sb1.stok_id and st.isdeleted = 0 and sb1.isdeleted = 0) sb " +
                    "CROSS APPLY (select top 1 sf1.doviz_id,sf1.fiyat " +
                    "from stok_fiyat sf1 where sb.id = sf1.stok_birim_id and sf1.isdeleted = 0 order by sf1.last_update desc) sf) ss");
                var stokList = db.Database.SqlQuery<stok_birim_fiyat>(query.ToString()).Where(x => x.vkn_tckn == vkn);
                switch (sortOrder)
                {
                    //case "last_update_desc":
                    //    stoklar = stoklar.OrderByDescending(s => s.last_update);
                    //    break;
                    //case "last_update":
                    //    stoklar = stoklar.OrderBy(s => s.last_update);
                    //    break;
                    case "adi":
                        stokList = stokList.OrderBy(s => s.adi).ToList();
                        break;
                    case "adi_desc":
                        stokList = stokList.OrderByDescending(s => s.adi).ToList();
                        break;
                    default:
                        stokList = stokList.OrderBy(s => s.adi).ToList();
                        break;
                }
                //stokList = stoklar.ToList();

                int _sayfaNo = SayfaNo ?? 1;
                ViewBag.SayfaNo = _sayfaNo;
                int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                ViewBag.KayitSayisi = stokList.ToList().Count();
                stokListPaged = stokList.ToPagedList<stok_birim_fiyat>(_sayfaNo, _sayfaKayitSayisi);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_StokBul", stokListPaged);
                }
                //cariListPaged = cariList.ToPagedList<cari>(_sayfaNo, _sayfaKayitSayisi);
                //}
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
            return PartialView("StokBul", stokListPaged);
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult StokBulBir(string stok_adi) //ürün adına stok kodu ya da adı tam olarak girilirse ve 1 kayıt dönerse, bu kayıt modal stok bul açılmadan fatura_satir_formu na yazılır.
        {
            List<stok_birim_fiyat> stokList = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                kullanici user = (kullanici)Session["kullanici"];
                long vknAdmin = Utilities.vknAdmin;
                stok model = new stok { };
                if (String.IsNullOrEmpty(stok_adi))
                    stokList = Enumerable.Empty<stok_birim_fiyat>().AsQueryable().ToList();
                else if (stok_adi.Length < 3)
                {
                    stokList = Enumerable.Empty<stok_birim_fiyat>().AsQueryable().ToList();
                    ViewBag.Mesaj = "Ürün adı alanına en az 3 harf giriniz.";
                }
                else
                {
                    if (stok_adi.Length > 50)
                        stok_adi = stok_adi.Substring(0, 50);
                    StringBuilder query = new StringBuilder("select id,vkn_tckn,adi,gtip_no,birim_id_son,doviz_id_son,fiyat_son,kdv_son,otv_son,oiv_son,gvs_son,sgk_son,mfv_son,btu_son from " +
                        "(select st.id,st.vkn_tckn,st.adi,isnull(st.gtip_no,0) as gtip_no,isnull(sb.birim_id,0) as birim_id_son,isnull(sf.doviz_id,0) as doviz_id_son,isnull(sf.fiyat,0) as fiyat_son," +
                        "isnull(st.kdv_orani,0) as kdv_son, isnull(st.otv_orani,0) as otv_son, isnull(st.oiv_orani,0) as oiv_son," +
                        "isnull(st.gvs_orani,0) as gvs_son, isnull(st.sgk_orani,0) as sgk_son, isnull(st.mfv_orani,0) as mfv_son, isnull(st.btu_orani,0) as btu_son,sb.katsayi,sb.agirlik from stok st " +
                        "cross apply (select sb1.id,sb1.birim_id,sb1.katsayi,sb1.agirlik " +
                        "from stok_birim sb1 where (st.adi = '");
                    query.Append(stok_adi);
                    query.Append("' or st.kodu = '");
                    query.Append(stok_adi);
                    query.Append("') and st.id = sb1.stok_id and st.isdeleted = 0 and sb1.isdeleted = 0) sb " +
                        "CROSS APPLY (select top 1 sf1.doviz_id,sf1.fiyat,sf1.last_update " +
                        "from stok_fiyat sf1 where sb.id = sf1.stok_birim_id and sf1.isdeleted = 0 order by sf1.last_update desc) sf) ss");

                    stokList = db.Database.SqlQuery<stok_birim_fiyat>(query.ToString()).ToList();
                    stokList = stokList.Where(x => x.vkn_tckn == vkn).ToList();
                    if (stokList != null && stokList.Count() == 1)
                    {
                        return Json(stokList, JsonRequestBehavior.AllowGet);
                        //return PartialView("_FaturaDetayFormu", stokList);
                    }
                }
                stokList = Enumerable.Empty<stok_birim_fiyat>().AsQueryable().ToList();
                ViewBag.Mesaj = "Ürün adı alanına en az 3 harf giriniz.";
            }
            return Json(stokList, JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult DovizKuruGetir(int id)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            var doviz = db.dovizlers.Where(x => x.id == id);
            return Json(doviz.ToList(), JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult GelenIrsaliyeYanitla(string guid)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            fatura_gelen fg = db.fatura_gelen.Where(x => x.guid == guid).FirstOrDefault(); //TODO
            List<fatura_gelen_satir> fgsList = db.fatura_gelen_satir.Where(x => x.guid == guid).ToList();
            fg.fgs = fgsList;
            return View("GelenIrsaliyeYanitla", fg);
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult GelenFaturaTeslimAl(string[] guids)
        {
            long vkn = (long)Session["vkn_tckn"];
            kullanici user = (kullanici)Session["kullanici"];
            long vknAdmin = Utilities.vknAdmin;
            string path = Server.MapPath("~/Content/assets/");
            List<AjaxResult> arList = new List<AjaxResult>();
            if (guids.Count() > 0)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                string guid = guids[0]; // birden fazla guid gönderince her guid için status true ya da false dönmüyor, hepsi için tek status dönüyor. tek tek göndermek mantıklı.
                //string guids1 = String.Join(",", guids.Select(p => p.ToString()).ToArray());
                fatura_gelen fg = db.fatura_gelen.Where(x => x.guid == guid).FirstOrDefault();
                EFatura efatura = new EFatura(fg.entegrator_tipi, fg.fatura_tipi, vkn, path);
                arList = efatura.GelenFaturaTeslimAl(guids);
                if (arList[0].status)
                {
                    fg.durum = 1;
                    db.Entry(fg).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else
            {
                arList[0].status = false;
                arList[0].errordescription = "Lütfen teslim almak için bir fatura seçiniz.";
            }
            return Json(arList, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //[ExceptionHandler]
        //[Authorize]
        //public JsonResult OnaylaIrsaliyeInsert(fatura_gelen_satir fgs)
        //{
        //    fgs.last_update = DateTime.Now;
        //    db.fatura_gelen_satir.Add(fgs);            
        //    db.SaveChanges();
        //    return Json(fgs);
        //}
        [HttpPost]
        [ExceptionHandler]
        [Authorize]
        public ActionResult GelenIrsaliyeYanitlaUpdate(fatura_gelen fg)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            fatura_gelen updatedFg = (from c in db.fatura_gelen
                                      where c.id == fg.id
                                      select c).FirstOrDefault();
            updatedFg.not = fg.not;
            updatedFg.teslim_alan = fg.teslim_alan;
            updatedFg.teslim_eden = fg.teslim_eden;
            updatedFg.teslim_tarihi = fg.teslim_tarihi;
            updatedFg.last_update = DateTime.Now;
            db.SaveChanges();

            return View("GelenIrsaliye");
        }
        [HttpPost]
        [ExceptionHandler]
        [Authorize]
        public JsonResult GelenIrsaliyeYanitlaUpdate1(fatura_gelen fg)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            kullanici user = (kullanici)Session["kullanici"];
            long vknAdmin = Utilities.vknAdmin;
            string path = Server.MapPath("~/Content/assets/");
            AjaxResult ar = new AjaxResult();
            fatura_gelen updatedFg = (from c in db.fatura_gelen
                                      where c.id == fg.id
                                      select c).FirstOrDefault();
            updatedFg.not = fg.not ?? "";
            updatedFg.teslim_alan = fg.teslim_alan ?? "";
            updatedFg.teslim_eden = fg.teslim_eden ?? "";
            updatedFg.teslim_tarihi = fg.teslim_tarihi;
            updatedFg.last_update = DateTime.Now;
            db.SaveChanges();
            List<fatura_gelen_satir> fgsList = null;
            EIrsaliye eirsaliye = null;
            if (updatedFg.teslim_alan != "" && updatedFg.teslim_eden != "" && updatedFg.teslim_tarihi != null)
            {
                fgsList = db.fatura_gelen_satir.Where(x => x.guid == updatedFg.guid).ToList();
                bool yanitlansinMi = false;
                foreach (fatura_gelen_satir fgs in fgsList)
                {
                    var sikayet = fgs.sikayet ?? "";
                    var teslim_adet = fgs.teslim_adet == null ? 0 : fgs.teslim_adet;
                    var red_adet = fgs.red_adet == null ? 0 : fgs.red_adet;
                    var red_kodu = fgs.red_kodu ?? "";
                    var red_ack = fgs.red_ack ?? "";
                    var eksik_adet = fgs.eksik_adet == null ? 0 : fgs.eksik_adet;
                    var fazla_adet = fgs.fazla_adet == null ? 0 : fgs.fazla_adet;
                    if (sikayet != "" || red_ack != "" || red_adet > 0 || teslim_adet > 0)
                    {
                        yanitlansinMi = true;
                    }
                }
                if (yanitlansinMi)
                {
                    eirsaliye = new EIrsaliye(updatedFg.entegrator_tipi, updatedFg.fatura_tipi, vkn, path); //TODO updatedFg.entegrator_tipi, updatedFg.fatura_tipi null
                    ar = eirsaliye.SendDespatchResponse(updatedFg);
                }
                else
                {
                    ar.status = false;
                    ar.errordescription = "Lütfen satır bilgilerinde şikayet, red açıklaması alanlarını giriniz.";
                }
            }

            return Json(ar, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ExceptionHandler]
        [Authorize]
        public ActionResult GelenIrsaliyeYanitlaSatirUpdate(fatura_gelen_satir fgs)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            fatura_gelen_satir updatedFgs = (from c in db.fatura_gelen_satir
                                             where c.id == fgs.id
                                             select c).FirstOrDefault();
            //updatedFgs.satirno = fgs.satirno;
            updatedFgs.sikayet = fgs.sikayet ?? "";
            updatedFgs.teslim_adet = fgs.teslim_adet == null ? 0 : fgs.teslim_adet;
            updatedFgs.red_adet = fgs.red_adet == null ? 0 : fgs.red_adet;
            updatedFgs.red_kodu = fgs.red_kodu ?? "";
            updatedFgs.red_ack = fgs.red_ack ?? "";
            updatedFgs.eksik_adet = fgs.eksik_adet == null ? 0 : fgs.eksik_adet;
            updatedFgs.fazla_adet = fgs.fazla_adet == null ? 0 : fgs.fazla_adet;
            updatedFgs.last_update = DateTime.Now;
            db.SaveChanges();

            return new EmptyResult();
        }

        //[HttpPost]
        //public ActionResult OnaylaIrsaliyeDelete(int customerId)
        //{
        //    using (CustomersEntities entities = new CustomersEntities())
        //    {
        //        Customer customer = (from c in entities.Customers
        //                             where c.CustomerId == customerId
        //                             select c).FirstOrDefault();
        //        entities.Customers.Remove(customer);
        //        entities.SaveChanges();
        //    }

        //    return new EmptyResult();
        //}
        public void FaturaVergiHesapla(fatura f, fatura_satir fd)
        {

        }
        public void stok_birim_fiyat_guncelle(fatura fat, fatura_satir detay)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            //girilen stok bilgilerine göre stok, stok_birim ve stok_fiyat tabloları güncelleniyor. 
            stok st = db.stoks.Find(detay.stok_id);
            if (st != null)
            {
                st.isdeleted = false;
                st.last_update = DateTime.Now;
                if (fat.fatura_tipi == Utilities.EMustahsil) //Müstahsil ise
                {
                    st.gvs_orani = detay.gvs_orani;
                    st.sgk_orani = detay.sgk_orani;
                    st.mfv_orani = detay.mfv_orani;
                    st.btu_orani = detay.btu_orani;
                }
                else if (fat.fatura_tipi == Utilities.ESMM) //SMM ise
                {
                    st.gvs_orani = detay.gvs_orani;
                    st.kdv_orani = detay.kdv_orani;
                }
                else
                {
                    st.kdv_orani = detay.kdv_orani;
                    st.otv_orani = detay.otv_orani;
                    st.oiv_orani = detay.oiv_orani;
                }
            }
            db.Entry(st).State = System.Data.Entity.EntityState.Modified;
            stok_birim sb = db.stok_birim.Where(x => x.stok_id == st.id).FirstOrDefault();
            if (sb != null)
            {
                sb.isdeleted = false;
                sb.last_update = DateTime.Now;
            }
            db.Entry(sb).State = System.Data.Entity.EntityState.Modified;
            stok_fiyat sf = db.stok_fiyat.Where(x => x.stok_birim_id == sb.id).FirstOrDefault();
            if (sf != null)
            {
                sf.isdeleted = false;
                sf.last_update = DateTime.Now;
                sf.doviz_id = detay.doviz_id1;
                sf.fiyat = detay.birim_fiyati;
            }
            db.Entry(sf).State = System.Data.Entity.EntityState.Modified;
            //girilen stok bilgilerine göre stok, stok_birim ve stok_fiyat tabloları güncellendi. 
        }
        public void FaturaDegerYuvarla(fatura fat)
        {

        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult MukellefListesi(int id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    //List<mukellef> mukellefListesi = null;
                    ViewBag.VKNSort = sortOrder == "VergiKimlikNo" ? "VergiKimlikNo_desc" : "VergiKimlikNo";
                    ViewBag.UnvanSort = sortOrder == "Unvan" ? "Unvan_desc" : "Unvan";
                    ViewBag.EPostaSort = sortOrder == "EPosta" ? "EPosta_desc" : "EPosta";
                    ViewBag.TarihSort = sortOrder == "Tarih" ? "Tarih_desc" : "Tarih";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    long vkn = (long)Session["vkn_tckn"];
                    kullanici user = (kullanici)Session["kullanici"];
                    long vknAdmin = Utilities.vknAdmin;
                    string path = Server.MapPath("~/Content/assets/" + vkn);
                    IPagedList<mukellef> mukellefListPaged = null;
                    //if (vkn == vknAdmin) //Sadece bizim sirket admini mukellef guncelleme yapabilir.(kayit sayisi cok fazla oldugu icin)
                    //{
                    try
                    {
                        var mu = (from muk in db.mukellefs.AsEnumerable()
                                  select new mukellef
                                  {
                                      Title = muk.Title ?? "",
                                      VergiKimlikNo = muk.VergiKimlikNo ?? 0,
                                      PostBoxAlias = muk.PostBoxAlias ?? "",
                                      SenderBoxAlias = muk.SenderBoxAlias ?? "",
                                      Tipi = muk.Tipi ?? "",
                                      OlusturmaTarihi = muk.OlusturmaTarihi ?? DateTime.Now,
                                      recorddate = muk.recorddate ?? DateTime.Now,
                                      last_update = muk.last_update ?? DateTime.Now,
                                      id = muk.id,
                                  });
                        if (mu == null || mu.Count() == 0 || id == 1) //id = 1 ise Mükellef güncelle butonuna basılmıştır.
                        {
                            if (vkn == vknAdmin) //Sadece sirket admin guncelleyebilir
                            {
                                fatura_ayarlari fa = db.fatura_ayarlari.FirstOrDefault(x => x.fatura_tipi == 1);
                                if (fa.entegrator_tipi == Utilities.Uyumsoft)
                                {
                                    EFatura efat = new EFatura(1, 1, vkn, path); //Uyumsoft dan mukellef listesi cekiyoruz
                                    try
                                    {
                                        if (efat.MukellefListesiGuncelle())
                                        {
                                            mu = db.mukellefs.AsEnumerable().ToList();
                                            ViewBag.Mesaj = "Guncellendi";
                                        }
                                        else
                                            ViewBag.Mesaj = "Guncel";
                                    }
                                    catch (Exception ex)
                                    {
                                        //Manager.ErrorHandler.Instance.ReportError(ex);
                                        ViewBag.Hata = "Beklenmedik bir hata oluştu." + ex.ToString();
                                    }
                                }
                                else if (fa.entegrator_tipi == 2)
                                {

                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(searchString))
                        {
                            mu = mu.Where(s => s.VergiKimlikNo.ToString().Contains(searchString) || s.Title.Contains(searchString) || s.PostBoxAlias.Contains(searchString) || s.SenderBoxAlias.Contains(searchString));
                        }
                        switch (sortOrder)
                        {
                            case "Unvan":
                                mu = mu.OrderBy(s => s.Title);
                                break;
                            case "Unvan_desc":
                                mu = mu.OrderByDescending(s => s.Title);
                                break;
                            case "VergiKimlikNo":
                                mu = mu.OrderBy(s => s.VergiKimlikNo);
                                break;
                            case "VergiKimlikNo_desc":
                                mu = mu.OrderByDescending(s => s.VergiKimlikNo);
                                break;
                            case "EPosta":
                                mu = mu.OrderBy(s => s.PostBoxAlias);
                                break;
                            case "EPosta_desc":
                                mu = mu.OrderByDescending(s => s.PostBoxAlias);
                                break;
                            case "Tarih":
                                mu = mu.OrderBy(s => s.OlusturmaTarihi);
                                break;
                            case "Tarih_desc":
                                mu = mu.OrderByDescending(s => s.OlusturmaTarihi);
                                break;
                            default:
                                mu = mu.OrderByDescending(s => s.OlusturmaTarihi);
                                break;
                        }
                        //List<fatura_ayarlari> ayarList = fa.ToList();
                        int _sayfaNo = SayfaNo ?? 1;
                        ViewBag.SayfaNo = _sayfaNo;
                        int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                        ViewBag.KayitSayisi = mu.Count();
                        mukellefListPaged = mu.ToPagedList<mukellef>(_sayfaNo, _sayfaKayitSayisi);
                        //}
                        if (Request.IsAjaxRequest() || 1 == 1)
                        {
                            return View(mukellefListPaged);
                            //return PartialView("~/Views/Stok/_StokListesi.cshtml", cariListPaged);
                        }
                    }
                    catch (Exception ex)
                    {

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
        public ActionResult GelenFatura(string baslangic, string bitis, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            EFatura efatura = null;
            IPagedList<fatura_gelen> fgListPaged = new PagedList<fatura_gelen>(null, 1, 10);
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    long vkn = (long)Session["vkn_tckn"];
                    long vknAdmin = Utilities.vknAdmin;
                    kullanici user = (kullanici)Session["kullanici"];
                    string path = Server.MapPath("~/Content/assets/");
                    //DateTime baslangicTarih = new DateTime();
                    //DateTime bitisTarih = new DateTime();
                    //baslangic = "30.04.2020";
                    //bitis = "30.04.2020";
                    List<fatura_gelen> fgList = Enumerable.Empty<fatura_gelen>().AsQueryable().ToList();
                    fgList = db.fatura_gelen.Where(x => x.fatura_turu.Equals("TEMELFATURA") || x.fatura_turu.Equals("TICARIFATURA") && x.gonderen_vkn == vkn).ToList();
                    if (baslangic != null && baslangic != "" && bitis != null && bitis != "")
                    {
                        //baslangicTarih = DateTime.ParseExact(baslangic, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        //bitisTarih = DateTime.ParseExact(bitis, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        var fayar = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.fatura_tipi == Utilities.EArsivNormal).FirstOrDefault();
                        //fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();

                        List<FaturaItem> faturalarList = new List<FaturaItem>();
                        if (fayar.entegrator_tipi == Utilities.Uyumsoft)
                        {
                            efatura = new EFatura((int)fayar.entegrator_tipi, (int)fayar.fatura_tipi, (long)fayar.vkn_tckn, path);
                            faturalarList = efatura.GelenFatura(Convert.ToDateTime(baslangic), Convert.ToDateTime(bitis).AddDays(1)).ToList();
                        }
                        //DbCommand cmd = GenericDataAccess.CreateCommand();
                        fatura_gelen fg = null;

                        //List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        for (int i = 0; i < faturalarList.Count; i++)
                        {
                            long vkn1 = long.Parse(faturalarList[i].GonderenMusteri.VergiKimlikNo);
                            var belgeno = faturalarList[i].BelgeNo ?? "";
                            var guid = faturalarList[i].Guid;
                            var fg1 = db.fatura_gelen.Where(x => x.gonderen_vkn == vkn1 && x.belgeno == belgeno && x.guid == guid).FirstOrDefault();
                            if (fg1 == null)
                            {
                                fg = new fatura_gelen();
                                fg.gonderen_vkn = long.Parse(faturalarList[i].GonderenMusteri.VergiKimlikNo);
                                fg.gonderen_unvan = faturalarList[i].GonderenMusteri.Unvan ?? "";
                                fg.gonderen_sehir = faturalarList[i].GonderenMusteri.Sehir ?? "";
                                fg.fatura_turu = faturalarList[i].FaturaTuru ?? "";
                                fg.olusturma_tarihi = faturalarList[i].FaturaOlusturmaTarihi;
                                fg.invoicetypecodetype = faturalarList[i].InvoiceTypeCodeType ?? "";
                                fg.faturatarihi = faturalarList[i].FaturaTarihi;
                                fg.faturano = faturalarList[i].FaturaNo ?? "";
                                fg.belgeno = faturalarList[i].BelgeNo ?? "";
                                fg.meblag = faturalarList[i].Meblag;
                                fg.aratoplam = faturalarList[i].AraToplam;
                                fg.vergitutar = faturalarList[i].VergiTutar;
                                fg.iskontotutar = faturalarList[i].IskontoTutar;
                                fg.guid = faturalarList[i].Guid ?? "";
                                fg.fatura_notu = string.IsNullOrEmpty(faturalarList[i].FaturaNotu) ? "" : faturalarList[i].FaturaNotu.Replace("'", "`");
                                fg.aktarimmesaji = faturalarList[i].AktarimMesaji ?? "";
                                fg.issucceded = faturalarList[i].IsSucceded ? true : false;
                                fg.otvtutar = faturalarList[i].OtvTutar;
                                fg.alicimusteriobj = AjaxResult.CreateJSON(faturalarList[i].AliciMusteri).Replace("'", "`") ?? "";
                                fg.gonderenmusteriobj = AjaxResult.CreateJSON(faturalarList[i].GonderenMusteri).Replace("'", "`") ?? "";
                                fg.kdvtoplamobj = AjaxResult.CreateJSON(faturalarList[i].KdvToplam).Replace("'", "`");
                                fg.hareketsatirlariobj = AjaxResult.CreateJSON(faturalarList[i].HareketSatirlari).Replace("'", "`") ?? "";
                                fg.doviz = faturalarList[i].doviz ?? "";
                                fg.kur_carpani = faturalarList[i].kur_carpani;
                                fg.entegrator_tipi = faturalarList[i].EntegratorTipi;
                                fg.fatura_tipi = faturalarList[i].EEvrakTipi;
                                fg.recorddate = DateTime.Now;
                                fg.last_update = DateTime.Now;
                                fg.durum = 0;
                                fg.mesaj = "";
                                fg.not = "";
                                fg.teslim_alan = "";
                                fg.teslim_eden = "";
                                fg.teslim_tarihi = DateTime.Now;
                                fatura_gelen_satir fgs = null;
                                foreach (var item in faturalarList[i].HareketSatirlari)
                                {
                                    fgs = new fatura_gelen_satir();
                                    fgs.guid = faturalarList[i].Guid;
                                    fgs.stok_adi = item.StokItem.Adi;
                                    fgs.birimi = item.StokItem.Birim;
                                    fgs.miktari = item.Miktar;
                                    fgs.doviz_cinsi = item.doviz;
                                    fgs.doviz_kuru = 1;
                                    fgs.kdv_orani = item.Kdv.Oran;
                                    fgs.kdv_tutar = item.Kdv.Tutar;
                                    fgs.otv_orani = item.Kdv.OtvOran;
                                    fgs.otv_tutar = item.Kdv.OtvTutar;
                                    fgs.oiv_orani = 0;
                                    fgs.oiv_tutar = 0;
                                    fgs.tevkifat_orani = item.Kdv.TevkifatOrani;
                                    fgs.tevkifat_tutar = item.Kdv.TevkifatTutar;
                                    fgs.iskonto_orani = item.Iskonto1;
                                    fgs.iskonto_tutar = item.IskontoTutar;
                                    fgs.fiyat = item.Fiyat;
                                    fgs.satirno = "";
                                    fgs.sikayet = "";
                                    fgs.teslim_adet = 0;
                                    fgs.red_adet = 0;
                                    fgs.red_kodu = "";
                                    fgs.red_ack = "";
                                    fgs.eksik_adet = 0;
                                    fgs.fazla_adet = 0;
                                    fgs.last_update = DateTime.Now;
                                    db.fatura_gelen_satir.Add(fgs);
                                    db.SaveChanges();
                                }
                                db.fatura_gelen.Add(fg);
                                db.SaveChanges();
                            }
                            fgList.Add(fg);
                        }
                    }
                    ViewBag.VKNSort = sortOrder == "VergiKimlikNo" ? "VergiKimlikNo_desc" : "VergiKimlikNo";
                    ViewBag.UnvanSort = sortOrder == "Unvan" ? "Unvan_desc" : "Unvan";
                    ViewBag.SehirSort = sortOrder == "Sehir" ? "Sehir_desc" : "Sehir";
                    ViewBag.MeblagSort = sortOrder == "Meblag" ? "Meblag_desc" : "Meblag";
                    ViewBag.TarihSort = sortOrder == "Tarih" ? "Tarih_desc" : "Tarih";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        fgList = fgList.Where(s => s.faturatarihi.ToString().Contains(searchString)).ToList();
                    }
                    switch (sortOrder)
                    {
                        case "Unvan":
                            fgList = fgList.OrderBy(s => s.gonderen_unvan).ToList();
                            break;
                        case "Unvan_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_unvan).ToList();
                            break;
                        case "VergiKimlikNo":
                            fgList = fgList.OrderBy(s => s.gonderen_vkn).ToList();
                            break;
                        case "VergiKimlikNo_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_vkn).ToList();
                            break;
                        case "Sehir":
                            fgList = fgList.OrderBy(s => s.gonderen_sehir).ToList();
                            break;
                        case "Sehir_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_sehir).ToList();
                            break;
                        case "Meblag":
                            fgList = fgList.OrderBy(s => s.meblag).ToList();
                            break;
                        case "Meblag_desc":
                            fgList = fgList.OrderByDescending(s => s.meblag).ToList();
                            break;
                        case "Tarih":
                            fgList = fgList.OrderBy(s => s.faturatarihi).ToList();
                            break;
                        case "Tarih_desc":
                            fgList = fgList.OrderByDescending(s => s.faturatarihi).ToList();
                            break;
                        default:
                            fgList = fgList.OrderByDescending(s => s.faturatarihi).ToList();
                            break;
                    }
                    //List<fatura_ayarlari> ayarList = fa.ToList();
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    ViewBag.KayitSayisi = fgList.Count();
                    //var sil = from s in fgList.AsEnumerable() select s;
                    fgListPaged = fgList.ToPagedList<fatura_gelen>(_sayfaNo, _sayfaKayitSayisi);
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View(fgListPaged);
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
        public ActionResult GelenIrsaliye(string baslangic, string bitis, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            EIrsaliye eirsaliye = null;
            IPagedList<fatura_gelen> fgListPaged = new PagedList<fatura_gelen>(null, 1, 10);
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    long vkn = (long)Session["vkn_tckn"];
                    long vknAdmin = Utilities.vknAdmin;
                    kullanici user = (kullanici)Session["kullanici"];
                    string path = Server.MapPath("~/Content/assets/");
                    //DateTime baslangicTarih = new DateTime();
                    //DateTime bitisTarih = new DateTime();
                    //baslangic = "30.04.2020";
                    //bitis = "30.04.2020";
                    List<fatura_gelen> fgList = Enumerable.Empty<fatura_gelen>().AsQueryable().ToList();
                    fgList = db.fatura_gelen.Where(x => x.fatura_turu.Contains("IRSALIYE") && x.gonderen_vkn == vkn).ToList();
                    if (baslangic != null && baslangic != "" && bitis != null && bitis != "")
                    {
                        //baslangicTarih = DateTime.ParseExact(baslangic, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        //bitisTarih = DateTime.ParseExact(bitis, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        var fayar = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.fatura_tipi == Utilities.EArsivNormal).FirstOrDefault();
                        //fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();

                        List<FaturaItem> faturalarList = new List<FaturaItem>();
                        if (fayar.entegrator_tipi == Utilities.Uyumsoft)
                        {
                            eirsaliye = new EIrsaliye((int)fayar.entegrator_tipi, (int)fayar.fatura_tipi, (long)fayar.vkn_tckn, path);
                            faturalarList = eirsaliye.GelenIrsaliye(Convert.ToDateTime(baslangic), Convert.ToDateTime(bitis).AddDays(1)).ToList();
                        }
                        //DbCommand cmd = GenericDataAccess.CreateCommand();
                        fatura_gelen fg = null;

                        //List<fatura_satir> fatura_satirList = Enumerable.Empty<fatura_satir>().AsQueryable().ToList();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        for (int i = 0; i < faturalarList.Count; i++)
                        {
                            long vkn1 = long.Parse(faturalarList[i].GonderenMusteri.VergiKimlikNo);
                            var belgeno = faturalarList[i].BelgeNo ?? "";
                            var guid = faturalarList[i].Guid;
                            var fg1 = db.fatura_gelen.Where(x => x.gonderen_vkn == vkn1 && x.belgeno == belgeno && x.guid == guid).FirstOrDefault();
                            if (fg1 == null)
                            {
                                fg = new fatura_gelen();
                                fg.gonderen_vkn = long.Parse(faturalarList[i].GonderenMusteri.VergiKimlikNo);
                                fg.gonderen_unvan = faturalarList[i].GonderenMusteri.Unvan ?? "";
                                fg.gonderen_sehir = faturalarList[i].GonderenMusteri.Sehir ?? "";
                                fg.fatura_turu = faturalarList[i].FaturaTuru ?? "";
                                fg.olusturma_tarihi = faturalarList[i].FaturaOlusturmaTarihi;
                                fg.invoicetypecodetype = faturalarList[i].InvoiceTypeCodeType ?? "";
                                fg.faturatarihi = faturalarList[i].FaturaTarihi;
                                fg.faturano = faturalarList[i].FaturaNo ?? "";
                                fg.belgeno = faturalarList[i].BelgeNo ?? "";
                                fg.meblag = faturalarList[i].Meblag;
                                fg.aratoplam = faturalarList[i].AraToplam;
                                fg.vergitutar = faturalarList[i].VergiTutar;
                                fg.iskontotutar = faturalarList[i].IskontoTutar;
                                fg.guid = faturalarList[i].Guid ?? "";
                                fg.fatura_notu = string.IsNullOrEmpty(faturalarList[i].FaturaNotu) ? "" : faturalarList[i].FaturaNotu.Replace("'", "`");
                                fg.aktarimmesaji = faturalarList[i].AktarimMesaji ?? "";
                                fg.issucceded = faturalarList[i].IsSucceded ? true : false;
                                fg.otvtutar = faturalarList[i].OtvTutar;
                                fg.alicimusteriobj = AjaxResult.CreateJSON(faturalarList[i].AliciMusteri).Replace("'", "`") ?? "";
                                fg.gonderenmusteriobj = AjaxResult.CreateJSON(faturalarList[i].GonderenMusteri).Replace("'", "`") ?? "";
                                fg.kdvtoplamobj = AjaxResult.CreateJSON(faturalarList[i].KdvToplam).Replace("'", "`");
                                fg.hareketsatirlariobj = AjaxResult.CreateJSON(faturalarList[i].HareketSatirlari).Replace("'", "`") ?? "";
                                fg.doviz = faturalarList[i].doviz ?? "";
                                fg.kur_carpani = faturalarList[i].kur_carpani;
                                fg.entegrator_tipi = faturalarList[i].EntegratorTipi;
                                fg.fatura_tipi = faturalarList[i].EEvrakTipi;
                                fg.recorddate = DateTime.Now;
                                fg.last_update = DateTime.Now;
                                fg.durum = 0;
                                fg.mesaj = "";
                                fg.not = "";
                                fg.teslim_alan = "";
                                fg.teslim_eden = "";
                                fg.teslim_tarihi = DateTime.Now;
                                fatura_gelen_satir fgs = null;
                                foreach (var item in faturalarList[i].HareketSatirlari)
                                {
                                    fgs = new fatura_gelen_satir();
                                    fgs.guid = faturalarList[i].Guid;
                                    fgs.stok_adi = item.StokItem.Adi;
                                    fgs.birimi = item.StokItem.Birim;
                                    fgs.miktari = item.Miktar;
                                    fgs.doviz_cinsi = item.doviz;
                                    fgs.doviz_kuru = 1;
                                    fgs.kdv_orani = item.Kdv.Oran;
                                    fgs.kdv_tutar = item.Kdv.Tutar;
                                    fgs.otv_orani = item.Kdv.OtvOran;
                                    fgs.otv_tutar = item.Kdv.OtvTutar;
                                    fgs.oiv_orani = 0;
                                    fgs.oiv_tutar = 0;
                                    fgs.tevkifat_orani = item.Kdv.TevkifatOrani;
                                    fgs.tevkifat_tutar = item.Kdv.TevkifatTutar;
                                    fgs.iskonto_orani = item.Iskonto1;
                                    fgs.iskonto_tutar = item.IskontoTutar;
                                    fgs.fiyat = item.Fiyat;
                                    fgs.satirno = item.satirno;
                                    fgs.sikayet = "";
                                    fgs.teslim_adet = 0;
                                    fgs.red_adet = 0;
                                    fgs.red_kodu = "";
                                    fgs.red_ack = "";
                                    fgs.eksik_adet = 0;
                                    fgs.fazla_adet = 0;
                                    fgs.last_update = DateTime.Now;
                                    db.fatura_gelen_satir.Add(fgs);
                                    db.SaveChanges();
                                }
                                db.fatura_gelen.Add(fg);
                                db.SaveChanges();
                            }
                            fgList.Add(fg);
                        }
                    }
                    ViewBag.VKNSort = sortOrder == "VergiKimlikNo" ? "VergiKimlikNo_desc" : "VergiKimlikNo";
                    ViewBag.UnvanSort = sortOrder == "Unvan" ? "Unvan_desc" : "Unvan";
                    ViewBag.SehirSort = sortOrder == "Sehir" ? "Sehir_desc" : "Sehir";
                    ViewBag.MeblagSort = sortOrder == "Meblag" ? "Meblag_desc" : "Meblag";
                    ViewBag.TarihSort = sortOrder == "Tarih" ? "Tarih_desc" : "Tarih";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        fgList = fgList.Where(s => s.faturatarihi.ToString().Contains(searchString)).ToList();
                    }
                    switch (sortOrder)
                    {
                        case "Unvan":
                            fgList = fgList.OrderBy(s => s.gonderen_unvan).ToList();
                            break;
                        case "Unvan_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_unvan).ToList();
                            break;
                        case "VergiKimlikNo":
                            fgList = fgList.OrderBy(s => s.gonderen_vkn).ToList();
                            break;
                        case "VergiKimlikNo_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_vkn).ToList();
                            break;
                        case "Sehir":
                            fgList = fgList.OrderBy(s => s.gonderen_sehir).ToList();
                            break;
                        case "Sehir_desc":
                            fgList = fgList.OrderByDescending(s => s.gonderen_sehir).ToList();
                            break;
                        case "Meblag":
                            fgList = fgList.OrderBy(s => s.meblag).ToList();
                            break;
                        case "Meblag_desc":
                            fgList = fgList.OrderByDescending(s => s.meblag).ToList();
                            break;
                        case "Tarih":
                            fgList = fgList.OrderBy(s => s.faturatarihi).ToList();
                            break;
                        case "Tarih_desc":
                            fgList = fgList.OrderByDescending(s => s.faturatarihi).ToList();
                            break;
                        default:
                            fgList = fgList.OrderByDescending(s => s.faturatarihi).ToList();
                            break;
                    }
                    //List<fatura_ayarlari> ayarList = fa.ToList();
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    ViewBag.KayitSayisi = fgList.Count();
                    //var sil = from s in fgList.AsEnumerable() select s;
                    fgListPaged = fgList.ToPagedList<fatura_gelen>(_sayfaNo, _sayfaKayitSayisi);
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View(fgListPaged);
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
        public JsonResult FaturaIptal(int[] ids)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            string path = Server.MapPath("~/Content/assets/");
            List<AjaxResult> arList = new List<AjaxResult>();
            AjaxResult ar = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                string ids1 = String.Join(",", ids.Select(p => p.ToString()).ToArray());
                StringBuilder query = new StringBuilder();
                query.Append("select * from fatura where id in (");
                query.Append(ids1);
                query.Append(")");
                //query.Append(") and ISNULL(fatura_guid, '') = '' and fatura_onaykontrol is null) or fatura_onaykontrol = 12");
                //query.Append(") and ISNULL(fatura_guid, '') = '' and fatura_onaykontrol is null and ISNULL(belgeno, '') = ''");
                List<fatura> fats = db.Database.SqlQuery<fatura>(query.ToString()).ToList();
                List<SendInvoiceStatus> sendstatuses = null;
                foreach (fatura item in fats.ToList())
                {
                    string fguid = item.fatura_guid ?? "";
                    int? fokontrol = item.fatura_onaykontrol == null ? 0 : item.fatura_onaykontrol;
                    string fdurum = item.fatura_durum ?? "";
                    //var vergiMustahsil = item.toplam_gvstutar + item.toplam_btututar + item.toplam_mfvtutar + item.toplam_sgktutar;
                    //var vergiESMM = item.toplam_kdvtutar + item.toplam_gvstutar;
                    fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == item.fatura_tipi && x.vkn_tckn == item.vkn_tckn).FirstOrDefault();
                    if (item.fatura_durum == "Onaylandı" || item.fatura_durum == "İmzalandı")
                    {
                        try
                        {
                            if (item.fatura_tipi.Equals(Utilities.ESMM))
                            {
                                ESMM esmm = new ESMM(fa, path);
                                sendstatuses = esmm.SMMIptalUyumsoft(item.fatura_guid).ToList();
                                foreach (var senditem in sendstatuses)
                                {
                                    ar = new AjaxResult();
                                    if (senditem != null && senditem.IsSucceded)
                                    {
                                        //fatura fat1 = db.faturas.Where(x => x.belgeno == senditem.FaturaNo).FirstOrDefault();
                                        //fat1.fatura_guid = senditem.Guid;
                                        //fat1.belgeno = senditem.FaturaNo;
                                        //fat1.seri = senditem.FaturaNo.Substring(0, 3);
                                        //fat1.sira_no = Convert.ToInt32(senditem.FaturaNo.Substring(6, senditem.FaturaNo.Length - 6));
                                        //fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                                        //fat1.fatura_durum = senditem.Mesaj == "" ? "Kuyrukta" : senditem.Mesaj;
                                        //fat1.last_update = DateTime.Now;

                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        item.fatura_durum = "İptal Edildi";
                                        item.last_update = DateTime.Now;
                                        item.fatura_onaykontrol = 99; //iptal
                                        db.Entry(item).State = EntityState.Modified;
                                        db.SaveChanges();
                                        ar.status = senditem.IsSucceded;
                                        ar.errordescription = senditem.Mesaj == "" ? "Fatura başarıyla iptal edildi." : senditem.Mesaj;
                                        Match match = null;
                                        string alici_mail = "";

                                        if ((item.fatura_alias ?? "") != "" && regex.Match(item.fatura_alias).Success)
                                            alici_mail = item.fatura_alias;
                                        else
                                        {
                                            var cariAlias = db.caris.Find(item.cari_id);
                                            match = regex.Match(cariAlias.cari_mail ?? "");
                                            if (match.Success)
                                                alici_mail = cariAlias.cari_mail;
                                        }
                                        if (alici_mail != "")
                                        {
                                            //efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                            //string str = efatura.GetHtmlFromFaturaItem(faturaItem);
                                            //ContentResult cr = Content(str);
                                            firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == item.vkn_tckn); //firma smtp bilgileri aliniyor 
                                            SenderInfo sInfo = null;
                                            try
                                            {
                                                sInfo = new SenderInfo
                                                {
                                                    senderMail = dSirket.emailUserName,
                                                    senderPass = dSirket.emailUserPassword,
                                                    senderDisplayName = dSirket.emailDisplayName,
                                                    senderSmtpServer = dSirket.smtpServerName,
                                                    senderSmtpPort = (int)dSirket.smtpServerPort,
                                                    senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                sInfo = null;
                                                try
                                                {
                                                    sInfo = new SenderInfo
                                                    {
                                                        senderMail = dSirket.emailUserName,
                                                        senderPass = dSirket.emailUserPassword,
                                                        senderDisplayName = dSirket.emailDisplayName,
                                                        senderSmtpServer = dSirket.smtpServerName,
                                                        senderSmtpPort = (int)dSirket.smtpServerPort,
                                                        senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                    };
                                                }
                                                catch (Exception ex1)
                                                {
                                                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                    var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                    throw new Exception("SMTP ayarları eksik. " + hata);
                                                }
                                            }
                                            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                            IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-Fatura", "Sayın yetkili; <BR>" + item.belgeno + " numaralı fatura iptal edilmiştir.<BR>Bilgilerinize sunar iyi günler dileriz.");
                                            //if (iSonuc.hataliMi)
                                            //    fat.mail_gittimi = false;
                                            //else
                                            //    fat.mail_gittimi = true;
                                        }
                                    }
                                    else
                                    {
                                        ar.status = senditem.IsSucceded;
                                        ar.errordescription = senditem.Mesaj;
                                    }
                                    arList.Add(ar);
                                }
                            }
                            else
                            {
                                EFatura efatura = new EFatura(fa, path);
                                sendstatuses = efatura.FaturaIptalUyumsoft(item.fatura_guid).ToList();
                                foreach (var senditem in sendstatuses)
                                {
                                    ar = new AjaxResult();
                                    if (senditem != null && senditem.IsSucceded)
                                    {
                                        //fatura fat1 = db.faturas.Where(x => x.belgeno == senditem.FaturaNo).FirstOrDefault();
                                        //fat1.fatura_guid = senditem.Guid;
                                        //fat1.belgeno = senditem.FaturaNo;
                                        //fat1.seri = senditem.FaturaNo.Substring(0, 3);
                                        //fat1.sira_no = Convert.ToInt32(senditem.FaturaNo.Substring(6, senditem.FaturaNo.Length - 6));
                                        //fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                                        //fat1.fatura_durum = senditem.Mesaj == "" ? "Kuyrukta" : senditem.Mesaj;
                                        //fat1.last_update = DateTime.Now;

                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        item.fatura_durum = "İptal Edildi";
                                        item.last_update = DateTime.Now;
                                        item.fatura_onaykontrol = 99; //iptal
                                        db.Entry(item).State = EntityState.Modified;
                                        db.SaveChanges();
                                        ar.status = senditem.IsSucceded;
                                        ar.errordescription = senditem.Mesaj == "" ? "Fatura başarıyla iptal edildi." : senditem.Mesaj;
                                        Match match = null;
                                        string alici_mail = "";

                                        if ((item.fatura_alias ?? "") != "" && regex.Match(item.fatura_alias).Success)
                                            alici_mail = item.fatura_alias;
                                        else
                                        {
                                            var cariAlias = db.caris.Find(item.cari_id);
                                            match = regex.Match(cariAlias.cari_mail ?? "");
                                            if (match.Success)
                                                alici_mail = cariAlias.cari_mail;
                                        }
                                        if (alici_mail != "")
                                        {
                                            //efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                            //string str = efatura.GetHtmlFromFaturaItem(faturaItem);
                                            //ContentResult cr = Content(str);
                                            firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == item.vkn_tckn); //firma smtp bilgileri aliniyor 
                                            SenderInfo sInfo = null;
                                            try
                                            {
                                                sInfo = new SenderInfo
                                                {
                                                    senderMail = dSirket.emailUserName,
                                                    senderPass = dSirket.emailUserPassword,
                                                    senderDisplayName = dSirket.emailDisplayName,
                                                    senderSmtpServer = dSirket.smtpServerName,
                                                    senderSmtpPort = (int)dSirket.smtpServerPort,
                                                    senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                sInfo = null;
                                                try
                                                {
                                                    sInfo = new SenderInfo
                                                    {
                                                        senderMail = dSirket.emailUserName,
                                                        senderPass = dSirket.emailUserPassword,
                                                        senderDisplayName = dSirket.emailDisplayName,
                                                        senderSmtpServer = dSirket.smtpServerName,
                                                        senderSmtpPort = (int)dSirket.smtpServerPort,
                                                        senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                    };
                                                }
                                                catch (Exception ex1)
                                                {
                                                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                    var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                    throw new Exception("SMTP ayarları eksik. " + hata);
                                                }
                                            }
                                            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                            IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-Fatura", "Sayın yetkili; <BR>" + item.belgeno + " numaralı fatura iptal edilmiştir.<BR>Bilgilerinize sunar iyi günler dileriz.");
                                            //if (iSonuc.hataliMi)
                                            //    fat.mail_gittimi = false;
                                            //else
                                            //    fat.mail_gittimi = true;
                                        }
                                    }
                                    else
                                    {
                                        ar.status = senditem.IsSucceded;
                                        ar.errordescription = senditem.Mesaj;
                                    }
                                    arList.Add(ar);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ar = new AjaxResult();
                            //var hata = "";
                            //if (ex.InnerException == null)
                            //{
                            //    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                            //    hata = tra.GetFrame(0).GetMethod().ReflectedType.FullName;
                            //    hata += tra.GetFrame(0).GetFileLineNumber().ToString();
                            //    //throw new Exception("aaa:>" + a + "bbb:>" + b, ex);
                            //}
                            //else
                            //{
                            //    hata = ex.InnerException.ToString();
                            //}
                            Logger.log(ex, Request.Url.AbsoluteUri, Request.UserHostAddress, RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), "", "");
                            ar.errordescription = "Fatura iptal edilemedi. Beklenmedik bir hata oluştu. " + ex.Message;
                            ar.status = false;
                            arList.Add(ar);
                            //return View();
                            //return RedirectToAction("/FaturaListesi", "Fatura");

                            //throw ex;
                        }
                    }
                    else
                        fats.Remove(item);
                }
                var sil = fats.ToList().Count();
                var sil2 = arList.Count();
                if (arList.Count() == 0)
                {
                    ar = new AjaxResult();
                    ar.status = false;
                    ar.errordescription = "Lütfen iptal için uygun fatura seçiniz.";
                    arList.Add(ar);
                }
            }
            return Json(arList, JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult GetFaturaForPrint(int id)
        {
            EMustahsil emustahsil = null;
            MustahsilItem mustahsilItem = null;
            EFatura efatura = null;
            EIrsaliye eirsaliye = null;
            ESMM esmm = null;
            FaturaItem faturaItem = null;
            ContentResult cr = null;
            string str = "";
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                string path = Server.MapPath("~/Content/assets/");
                //string xmlFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/assets/Mustahsil1.xml")); //Reading data from xml file
                //string xsltFile = Server.MapPath("~/Content/assets/Mustahsil.xslt"); //path of xslt file
                //TransformationModel model = new TransformationModel();
                //model.XMLFilename = xmlFile;
                //model.StylesheetFilename = xsltFile;
                fatura fat = db.faturas.Find(id);
                fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();
                if (fa.entegrator_tipi == Utilities.Uyumsoft)
                {
                    if (fat.fatura_tipi == Utilities.EMustahsil)
                    {
                        mustahsilItem = GetMustahsilItem(fat, fa);
                        emustahsil = new EMustahsil((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                        str = emustahsil.GetHtmlFromMustahsilItem(mustahsilItem);
                        cr = Content(str);
                    }
                    else if (fat.fatura_tipi == Utilities.EIrsaliye)
                    {
                        faturaItem = GetFaturaItem(fat, fa);
                        eirsaliye = new EIrsaliye((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                        str = eirsaliye.GetHtmlFromFaturaItem(faturaItem);
                        cr = Content(str);
                    }
                    else if (fat.fatura_tipi == Utilities.ESMM)
                    {
                        faturaItem = GetFaturaItem(fat, fa);
                        esmm = new ESMM((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                        str = esmm.GetHtmlFromFaturaItem(faturaItem);
                        cr = Content(str);
                    }
                    else
                    {
                        faturaItem = GetFaturaItem(fat, fa);
                        efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                        str = efatura.GetHtmlFromFaturaItem(faturaItem);
                        cr = Content(str);
                    }
                }
                if (fa.entegrator_tipi == Utilities.Veriban)
                {

                }
            }
            //return new ContentResult
            //{
            //    ContentType = "text/xml",
            //    Content = Encoding.UTF8.GetString(str),
            //    ContentEncoding = System.Text.Encoding.UTF8
            //};
            return cr;
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult GelenFaturaForPrint(long gonderen_vkn, string belgeno, string guid)
        {
            //EMustahsil emustahsil = null;
            //MustahsilItem mustahsilItem = null;
            //EFatura efatura = null;
            //FaturaItem faturaItem = null;
            ContentResult cr = null;
            //string str = "";
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                //string xmlFile = System.IO.File.ReadAllText(Server.MapPath("~/Content/assets/Mustahsil1.xml")); //Reading data from xml file
                //string xsltFile = Server.MapPath("~/Content/assets/Mustahsil.xslt"); //path of xslt file
                //TransformationModel model = new TransformationModel();
                //model.XMLFilename = xmlFile;
                //model.StylesheetFilename = xsltFile;
                fatura_gelen fat = db.fatura_gelen.Where(x => x.gonderen_vkn == gonderen_vkn && x.belgeno == belgeno && x.guid == guid).FirstOrDefault();
                //TODO
                //fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();
                //if (fa.entegrator_tipi == Utilities.Uyumsoft)
                //{
                //    if (fat.fatura_tipi == Utilities.EMustahsil)
                //    {
                //        mustahsilItem = GetMustahsilItem(fat, fa);
                //        emustahsil = new EMustahsil((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn);
                //        str = emustahsil.GetHtmlFromMustahsilItem(mustahsilItem);
                //        cr = Content(str);
                //    }
                //    else
                //    {
                //        faturaItem = GetFaturaItem(fat, fa);
                //        efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn);
                //        str = efatura.GetHtmlFromFaturaItem(faturaItem);
                //        cr = Content(str);
                //    }
                //}
                //if (fa.entegrator_tipi == Utilities.Veriban)
                //{

                //}
            }
            //return new ContentResult
            //{
            //    ContentType = "text/xml",
            //    Content = Encoding.UTF8.GetString(str),
            //    ContentEncoding = System.Text.Encoding.UTF8
            //};
            return cr;
        }
        //[ExceptionHandler]
        //[Authorize]
        //public ActionResult FaturaDurumGuncelle(int id)
        //{
        //    EFatura efatura = null;
        //    fatura fat = db.faturas.Find(id);
        //    fatura_ayarlari fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();

        //    if (fa.entegrator_tipi == Utilities.Uyumsoft)
        //    {
        //        if (fat.fatura_tipi == Utilities.EMustahsil)
        //        {
        //            efatura = new EFatura((int)fat.fatura_tipi, (long)fat.vkn_tckn);
        //            efatura.FaturaDurumGuncelle(secilmisfaturalar[i].eevrak_tipi, tip);//Uyumsoft
        //        }
        //        else
        //        {

        //        }
        //    }

        //    AjaxResult result = new AjaxResult();
        //    EFatura efatura = null;
        //    EMustahsil emustahsil = null;
        //    int entegrator_tipi = 0;
        //    bool efatura_test = true;
        //    for (int i = 0; i < secilmisfaturalar.Count; i++)
        //    {
        //        List<int> secilmisfatura = new List<int>();
        //        secilmisfatura.Add(secilmisfaturalar[i].id);
        //        efatura_musteribilgileriEntity efm1 = efatura_musteribilgileriProvider.GetByEEvrakTipi(secilmisfaturalar[i].eevrak_tipi);
        //        entegrator_tipi = efm1.entegrator_tipi;
        //        efatura_test = efm1.efatura_test;
        //        //if (secilmisfaturalar[i].entegrator_tipi == null || secilmisfaturalar[i].entegrator_tipi == 0)
        //        //{
        //        //    secilmisfaturalar[i].entegrator_tipi = efatura_musteribilgileriProvider.GetByEEvrakTipi(secilmisfaturalar[i].eevrak_tipi).entegrator_tipi;
        //        //}
        //        if (secilmisfaturalar[i].entegrator_tipi == Utilities.Uyumsoft)
        //        {
        //            if (secilmisfaturalar[i].eevrak_tipi == 1 || secilmisfaturalar[i].eevrak_tipi == 2 || secilmisfaturalar[i].eevrak_tipi == 5)
        //            {
        //                efatura = new EFatura(secilmisfaturalar[i].eevrak_tipi);
        //                efatura.DurumGuncelle(secilmisfaturalar[i].eevrak_tipi, tip);//Uyumsoft
        //            }
        //            else if (secilmisfaturalar[i].eevrak_tipi == 3)
        //            {
        //                emustahsil = new EMustahsil(secilmisfaturalar[i].eevrak_tipi);
        //                emustahsil.DurumGuncelleMustahsilForUyumsoft(secilmisfaturalar[i].eevrak_tipi, tip, secilmisfatura);//Uyumsoft
        //            }
        //        }
        //        else if (secilmisfaturalar[i].entegrator_tipi == Utilities.Veriban)
        //        {
        //            if (secilmisfaturalar[i].eevrak_tipi == 1 || secilmisfaturalar[i].eevrak_tipi == 2 || secilmisfaturalar[i].eevrak_tipi == 5)
        //            {
        //                DurumGuncelleForVeriban(tip, secilmisfatura, efatura_test);
        //            }
        //            else if (secilmisfaturalar[i].eevrak_tipi == 3)
        //            {
        //                DurumGuncelleMustahsilForVeriban(tip, secilmisfatura, efatura_test);
        //            }
        //        }
        //        else if (secilmisfaturalar[i].entegrator_tipi == Utilities.Mikro)
        //        {
        //            if (secilmisfaturalar[i].eevrak_tipi == 1 || secilmisfaturalar[i].eevrak_tipi == 2 || secilmisfaturalar[i].eevrak_tipi == 5)
        //            {
        //                DurumGuncelleForMikro(tip, secilmisfatura, efatura_test);
        //            }
        //            else if (secilmisfaturalar[i].eevrak_tipi == 3)
        //            {
        //                DurumGuncelleMustahsilForMikro(tip, secilmisfatura, efatura_test);
        //            }
        //        }
        //        else
        //        {
        //            result.status = false;
        //            result.errordescription = "Beklenmedik bir hata oluştu.";
        //            result.returnvalue = "Evrak tipi ya da entegratör tipi bilgisi eksik.";
        //        }

        //    }
        //    return AjaxResult.CreateJSON(result);
        //}
        [ExceptionHandler]
        [Authorize]
        public JsonResult FaturaGonder(int[] ids)
        {
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            List<AjaxResult> arList = new List<AjaxResult>();
            AjaxResult ar = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                string ids1 = String.Join(",", ids.Select(p => p.ToString()).ToArray());
                StringBuilder query = new StringBuilder();
                try
                {
                    query.Append("select * from fatura where id in (");
                    query.Append(ids1);
                    query.Append(")");
                    //query.Append(") and ISNULL(fatura_guid, '') = '' and fatura_onaykontrol is null) or fatura_onaykontrol = 12");
                    //query.Append(") and ISNULL(fatura_guid, '') = '' and fatura_onaykontrol is null and ISNULL(belgeno, '') = ''");
                    List<fatura> fats = db.Database.SqlQuery<fatura>(query.ToString()).ToList();
                    foreach (fatura item in fats.ToList())
                    {
                        string fguid = item.fatura_guid ?? "";
                        int? fokontrol = item.fatura_onaykontrol == null ? 0 : item.fatura_onaykontrol;
                        string fdurum = item.fatura_durum ?? "";
                        //var vergiMustahsil = item.toplam_gvstutar + item.toplam_btututar + item.toplam_mfvtutar + item.toplam_sgktutar;
                        var vergiESMM = item.toplam_kdvtutar + item.toplam_gvstutar;
                        if ((item.fatura_tipi != Utilities.EMustahsil && item.fatura_tipi != Utilities.ESMM && item.toplam_fiyat > 0)
                            || (item.fatura_tipi == Utilities.EMustahsil && item.toplam_gvstutar > 0 && item.toplam_btututar > 0 && item.toplam_mfvtutar > 0 && item.toplam_sgktutar > 0)
                            //|| (item.fatura_tipi != Utilities.ESMM && item.toplam_fiyat > 0) 
                            || (item.fatura_tipi == Utilities.ESMM && vergiESMM > 0))
                            if (fguid == "" || fdurum == "Hata")
                                if (fokontrol != Utilities.FaturaIptal)
                                    continue;
                                else
                                    fats.Remove(item);
                            else
                                fats.Remove(item);
                        else
                            fats.Remove(item);
                    }
                    //var fats = db.faturas.Where(x => ids.ToString().Contains(x.id.ToString())).ToList();

                    if (fats != null && fats.Count > 0)
                        arList = EFaturaOlustur(fats);
                    else
                    {
                        ar = new AjaxResult();
                        ar.status = false;
                        ar.errordescription = "Fatura gönderilemedi. Lütfen daha önce gönderilmemiş, stok ve vergileri girilmiş gönderilmeye uygun fatura seçiniz.";
                        arList.Add(ar);
                    }
                    //int arListCount = arList.Count();
                    //foreach (var item in arList)
                    //{
                    //    if (arListCount == 1)
                    //    {

                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Logger.log(ex, Request.Url.AbsoluteUri, Request.UserHostAddress, RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), "", "");
                    //TempData["mesaj"] = "Beklenmedik bir hata oluştu." + ex.Message + "||||error";
                    ar = new AjaxResult();
                    //var hata = "";
                    //if (ex.InnerException == null)
                    //{
                    //    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    //    hata = tra.GetFrame(0).GetMethod().ReflectedType.FullName;
                    //    hata += tra.GetFrame(0).GetFileLineNumber().ToString();
                    //    //throw new Exception("aaa:>" + a + "bbb:>" + b, ex);
                    //}
                    //else
                    //{
                    //    hata = ex.InnerException.ToString();
                    //}
                    ar.errordescription = "Fatura gönderilemedi. Beklenmedik bir hata oluştu. " + ex.Message;
                    ar.status = false;
                    arList.Add(ar);
                    //return View();
                    //return RedirectToAction("/FaturaListesi", "Fatura");

                    //throw ex;
                }
            }
            return Json(arList, JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult DurumGuncelle(int[] ids)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            string path = Server.MapPath("~/Content/assets/");
            kullanici user = (kullanici)Session["kullanici"];
            List<AjaxResult> arList = new List<AjaxResult>();
            AjaxResult ar = null;
            if (Session["kullanici"] != null)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                try
                {
                    if (ids != null && ids.Count() > 0)
                    {
                        string ids1 = String.Join(",", ids.Select(p => p.ToString()).ToArray());
                        StringBuilder query = new StringBuilder();
                        query.Append("select * from fatura where id in (");
                        query.Append(ids1);
                        query.Append(")");
                        //query.Append(") and ISNULL(fatura_guid, '') != '' and fatura_onaykontrol is not null and ((fatura_tipi = 5 and fatura_onaykontrol != 4) or (fatura_tipi != 5 and fatura_onaykontrol != 7)) and (fatura_durum != 'Onaylandı' or fatura_durum != 'İmzalandı')");
                        List<fatura> fats = db.Database.SqlQuery<fatura>(query.ToString()).ToList();
                        foreach (fatura item in fats.ToList())
                        {
                            string fguid = item.fatura_guid ?? "";
                            int? fokontrol = item.fatura_onaykontrol == null ? 0 : item.fatura_onaykontrol;
                            string fdurum = item.fatura_durum ?? "";
                            if (fdurum != "Onaylandı" && fdurum != "İmzalandı" && fdurum != "Hata")
                                if (fokontrol != Utilities.FaturaIptal) //Fatura iptal ise durum sorgulamaya gerek yok
                                    continue;
                                else
                                    fats.Remove(item);
                            else
                                fats.Remove(item);
                        }
                        if (fats != null && fats.Count > 0)
                        {
                            OutboxInvoiceStatus uyumFaturaStatus = null;
                            List<OutboxInvoiceStatus> uyumFaturaStatuses = new List<OutboxInvoiceStatus>();
                            OutboxInvoiceStatus uyumIrsaliyeStatus = null;
                            List<OutboxInvoiceStatus> uyumIrsaliyeStatuses = new List<OutboxInvoiceStatus>();
                            OutboxInvoiceStatus uyumESMMStatus = null;
                            List<OutboxInvoiceStatus> uyumESMMStatuses = new List<OutboxInvoiceStatus>();
                            //ProducerReceiptStatusInfo uyumProducerReceiptStatus = null; 
                            //List<ProducerReceiptStatusInfo> uyumProducerReceiptStatuses = new List<ProducerReceiptStatusInfo>();
                            //StringBuilder sb = new StringBuilder();
                            //string[] receiptIds = new string[] { }; // empty string
                            List<string> uyumMustahsilIds = new List<string>();
                            foreach (fatura item in fats)
                            {
                                if (item.entegrator_tipi == Utilities.Uyumsoft)
                                {
                                    if (item.fatura_tipi == Utilities.EMustahsil)
                                    {
                                        uyumMustahsilIds.Add(item.fatura_guid);
                                    }
                                    else if (item.fatura_tipi == Utilities.EIrsaliye)
                                    {
                                        uyumIrsaliyeStatus = new OutboxInvoiceStatus();
                                        uyumIrsaliyeStatus.FaturaID = (int)item.id;
                                        uyumIrsaliyeStatus.Guid = item.fatura_guid;
                                        uyumIrsaliyeStatuses.Add(uyumIrsaliyeStatus);
                                    }
                                    else if (item.fatura_tipi == Utilities.ESMM)
                                    {
                                        uyumESMMStatus = new OutboxInvoiceStatus();
                                        uyumESMMStatus.FaturaID = (int)item.id;
                                        uyumESMMStatus.Guid = item.fatura_guid;
                                        uyumESMMStatuses.Add(uyumESMMStatus);
                                    }
                                    else
                                    {
                                        uyumFaturaStatus = new OutboxInvoiceStatus();
                                        uyumFaturaStatus.FaturaID = (int)item.id;
                                        uyumFaturaStatus.Guid = item.fatura_guid;
                                        uyumFaturaStatuses.Add(uyumFaturaStatus);
                                    }
                                }
                                if (item.entegrator_tipi == Utilities.Veriban)
                                {
                                    if (item.fatura_tipi != Utilities.EMustahsil)
                                    {

                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            if (uyumFaturaStatuses != null && uyumFaturaStatuses.Count > 0)
                            {
                                EFatura efatura = new EFatura();
                                List<OutboxInvoiceStatus> statuses = new List<OutboxInvoiceStatus>();
                                statuses = efatura.FaturaDurumGuncelle(uyumFaturaStatuses).ToList();
                                fatura_ayarlari fa = null;
                                foreach (var statusItem in statuses)
                                {
                                    ar = new AjaxResult();
                                    fatura fat = db.faturas.Where(x => x.fatura_guid == statusItem.Guid).FirstOrDefault();
                                    fa = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.kullanici_id == user.id && x.entegrator_tipi == fat.entegrator_tipi && x.fatura_tipi == fat.fatura_tipi).FirstOrDefault();
                                    if (fat != null)
                                    {

                                        fat.fatura_guid = statusItem.Guid;
                                        fat.fatura_onaykontrol = statusItem.OnayKontrol;
                                        fat.fatura_durum = statusItem.Mesaj == "" ? "Kuyrukta" : statusItem.Mesaj;

                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        if (statusItem.OnayKontrol == 12 || statusItem.Mesaj == "Hata")
                                        {
                                            ar.status = false;
                                            fat.mail_gittimi = false;
                                        }
                                        else
                                            ar.status = true;
                                        if (statusItem.OnayKontrol == 7)
                                        {
                                            if (fat.mail_gittimi == null || fat.mail_gittimi == false)
                                            {
                                                //mail gönderiliyor
                                                Match match = null;
                                                FaturaItem faturaItem = GetFaturaItem(fat, fa);

                                                string alici_mail = "";

                                                if (faturaItem != null && faturaItem.FaturaAlias != null && faturaItem.FaturaAlias != "" && regex.Match(faturaItem.FaturaAlias).Success)
                                                    alici_mail = faturaItem.FaturaAlias;
                                                else
                                                {
                                                    string vkn2 = faturaItem.AliciMusteri.VergiKimlikNo;
                                                    long vkn1 = long.Parse(vkn2.Trim());
                                                    var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn1 && x.vkn_tckn == vkn).FirstOrDefault();
                                                    match = regex.Match(cariAlias.cari_mail ?? "");
                                                    if (match.Success)
                                                        alici_mail = cariAlias.cari_mail;
                                                }
                                                if (alici_mail != "")
                                                {
                                                    efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                                    string str = efatura.GetHtmlFromFaturaItem(faturaItem);
                                                    ContentResult cr = Content(str);
                                                    firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vkn); //firma smtp bilgileri aliniyor
                                                    SenderInfo sInfo = null;
                                                    try
                                                    {
                                                        sInfo = new SenderInfo
                                                        {
                                                            senderMail = dSirket.emailUserName,
                                                            senderPass = dSirket.emailUserPassword,
                                                            senderDisplayName = dSirket.emailDisplayName,
                                                            senderSmtpServer = dSirket.smtpServerName,
                                                            senderSmtpPort = (int)dSirket.smtpServerPort,
                                                            senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                        };
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                        sInfo = null;
                                                        try
                                                        {
                                                            sInfo = new SenderInfo
                                                            {
                                                                senderMail = dSirket.emailUserName,
                                                                senderPass = dSirket.emailUserPassword,
                                                                senderDisplayName = dSirket.emailDisplayName,
                                                                senderSmtpServer = dSirket.smtpServerName,
                                                                senderSmtpPort = (int)dSirket.smtpServerPort,
                                                                senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                            };
                                                        }
                                                        catch (Exception ex1)
                                                        {
                                                            System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                            var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                            hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                            throw new Exception("SMTP ayarları eksik. " + hata);
                                                        }
                                                    }
                                                    Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                                    IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-Fatura", str + "<BR><BR><BR><BR><BR><BR>Bizi tercih ettiğiniz için teşekkürler, huzurlu günler dileriz.");
                                                    if (iSonuc.hataliMi)
                                                        fat.mail_gittimi = false;
                                                    else
                                                        fat.mail_gittimi = true;
                                                }

                                            }
                                        }
                                        db.Entry(fat).State = EntityState.Modified;
                                        db.SaveChanges();
                                        ar.id = fat.id.ToString();
                                        ar.returnvalue = fat.belgeno + "||||" + fat.fatura_durum + "||||" + fat.fatura_onaykontrol + "||||" + fat.mail_gittimi + "||||" + fat.fatura_tipi;
                                        ar.errordescription = statusItem.Mesaj; // == "" ? "Fatura durumu başarıyla güncellendi." : statusItem.Mesaj;
                                        arList.Add(ar);
                                    }
                                }
                            }
                            if (uyumESMMStatuses != null && uyumESMMStatuses.Count > 0)
                            {
                                ESMM esmm = new ESMM();
                                List<OutboxInvoiceStatus> statuses = new List<OutboxInvoiceStatus>();
                                statuses = esmm.SMMDurumGuncelle(uyumESMMStatuses).ToList();
                                fatura_ayarlari fa = null;
                                foreach (var statusItem in statuses)
                                {
                                    ar = new AjaxResult();
                                    fatura fat = db.faturas.Where(x => x.fatura_guid == statusItem.Guid).FirstOrDefault();
                                    fa = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.kullanici_id == user.id && x.entegrator_tipi == fat.entegrator_tipi && x.fatura_tipi == fat.fatura_tipi).FirstOrDefault();
                                    if (fat != null)
                                    {

                                        fat.fatura_guid = statusItem.Guid;
                                        fat.fatura_onaykontrol = statusItem.OnayKontrol;
                                        fat.fatura_durum = statusItem.Mesaj == "" ? "Kuyrukta" : statusItem.Mesaj;

                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        if (statusItem.OnayKontrol == 6 || statusItem.Mesaj == "Hata")
                                        {
                                            ar.status = false;
                                            fat.mail_gittimi = false;
                                        }
                                        else
                                            ar.status = true;
                                        if (statusItem.OnayKontrol == 4)
                                        {
                                            if (fat.mail_gittimi == null || fat.mail_gittimi == false)
                                            {
                                                //mail gönderiliyor
                                                Match match = null;
                                                FaturaItem faturaItem = GetFaturaItem(fat, fa);

                                                string alici_mail = "";
                                                if (faturaItem != null && faturaItem.FaturaAlias != null && faturaItem.FaturaAlias != "" && regex.Match(faturaItem.FaturaAlias).Success)
                                                    alici_mail = faturaItem.FaturaAlias;
                                                else
                                                {
                                                    long vkn1 = long.Parse(faturaItem.AliciMusteri.VergiKimlikNo.Trim());
                                                    var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn1 && x.vkn_tckn == vkn).FirstOrDefault();
                                                    match = regex.Match(cariAlias.cari_mail ?? "");
                                                    if (match.Success)
                                                        alici_mail = cariAlias.cari_mail;
                                                }
                                                if (alici_mail != "")
                                                {
                                                    esmm = new ESMM((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                                    string str = esmm.GetHtmlFromFaturaItem(faturaItem);
                                                    ContentResult cr = Content(str);
                                                    firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vkn); //firma smtp bilgileri aliniyor 
                                                    SenderInfo sInfo = null;
                                                    try
                                                    {
                                                        sInfo = new SenderInfo
                                                        {
                                                            senderMail = dSirket.emailUserName,
                                                            senderPass = dSirket.emailUserPassword,
                                                            senderDisplayName = dSirket.emailDisplayName,
                                                            senderSmtpServer = dSirket.smtpServerName,
                                                            senderSmtpPort = (int)dSirket.smtpServerPort,
                                                            senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                        };
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                        sInfo = null;
                                                        try
                                                        {
                                                            sInfo = new SenderInfo
                                                            {
                                                                senderMail = dSirket.emailUserName,
                                                                senderPass = dSirket.emailUserPassword,
                                                                senderDisplayName = dSirket.emailDisplayName,
                                                                senderSmtpServer = dSirket.smtpServerName,
                                                                senderSmtpPort = (int)dSirket.smtpServerPort,
                                                                senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                            };
                                                        }
                                                        catch (Exception ex1)
                                                        {
                                                            System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                            var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                            hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                            throw new Exception("SMTP ayarları eksik. " + hata);
                                                        }
                                                    }
                                                    Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                                    IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-SMM", str + "<BR><BR><BR><BR><BR><BR>Bizi tercih ettiğiniz için teşekkürler, huzurlu günler dileriz.");
                                                    if (iSonuc.hataliMi)
                                                        fat.mail_gittimi = false;
                                                    else
                                                        fat.mail_gittimi = true;
                                                }
                                            }
                                        }
                                        db.Entry(fat).State = EntityState.Modified;
                                        db.SaveChanges();
                                        ar.id = fat.id.ToString();
                                        ar.returnvalue = fat.belgeno + "||||" + fat.fatura_durum + "||||" + fat.fatura_onaykontrol + "||||" + fat.mail_gittimi + "||||" + fat.fatura_tipi;
                                        ar.errordescription = statusItem.Mesaj; // == "" ? "Fatura durumu başarıyla güncellendi." : statusItem.Mesaj;
                                        arList.Add(ar);
                                    }
                                }
                            }
                            if (uyumIrsaliyeStatuses != null && uyumIrsaliyeStatuses.Count > 0)
                            {
                                EIrsaliye eirsaliye = new EIrsaliye();
                                List<OutboxInvoiceStatus> statuses = new List<OutboxInvoiceStatus>();
                                statuses = eirsaliye.IrsaliyeDurumGuncelle(uyumIrsaliyeStatuses).ToList();
                                fatura_ayarlari fa = null;
                                foreach (var statusItem in statuses)
                                {
                                    ar = new AjaxResult();
                                    fatura fat = db.faturas.Where(x => x.fatura_guid == statusItem.Guid).FirstOrDefault();
                                    fa = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.kullanici_id == user.id && x.entegrator_tipi == fat.entegrator_tipi && x.fatura_tipi == fat.fatura_tipi).FirstOrDefault();
                                    if (fat != null)
                                    {

                                        fat.fatura_guid = statusItem.Guid;
                                        fat.fatura_onaykontrol = statusItem.OnayKontrol;
                                        fat.fatura_durum = statusItem.Mesaj == "" ? "Kuyrukta" : statusItem.Mesaj;

                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        if (statusItem.OnayKontrol == 9 || statusItem.Mesaj == "Hata")
                                        {
                                            ar.status = false;
                                            fat.mail_gittimi = false;
                                        }
                                        else
                                            ar.status = true;
                                        if (statusItem.OnayKontrol == 6)
                                        {
                                            if (fat.mail_gittimi == null || fat.mail_gittimi == false)
                                            {
                                                //mail gönderiliyor
                                                Match match = null;
                                                FaturaItem faturaItem = GetFaturaItem(fat, fa);

                                                string alici_mail = "";
                                                if (faturaItem != null && faturaItem.FaturaAlias != null && faturaItem.FaturaAlias != "" && regex.Match(faturaItem.FaturaAlias).Success)
                                                    alici_mail = faturaItem.FaturaAlias;
                                                else
                                                {
                                                    long vkn1 = long.Parse(faturaItem.AliciMusteri.VergiKimlikNo.Trim());
                                                    var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn1 && x.vkn_tckn == vkn).FirstOrDefault();
                                                    match = regex.Match(cariAlias.cari_mail ?? "");
                                                    if (match.Success)
                                                        alici_mail = cariAlias.cari_mail;
                                                }
                                                if (alici_mail != "")
                                                {
                                                    eirsaliye = new EIrsaliye((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                                    string str = eirsaliye.GetHtmlFromFaturaItem(faturaItem);
                                                    ContentResult cr = Content(str);
                                                    firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vkn); //firma smtp bilgileri aliniyor 
                                                    SenderInfo sInfo = null;
                                                    try
                                                    {
                                                        sInfo = new SenderInfo
                                                        {
                                                            senderMail = dSirket.emailUserName,
                                                            senderPass = dSirket.emailUserPassword,
                                                            senderDisplayName = dSirket.emailDisplayName,
                                                            senderSmtpServer = dSirket.smtpServerName,
                                                            senderSmtpPort = (int)dSirket.smtpServerPort,
                                                            senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                        };
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                        sInfo = null;
                                                        try
                                                        {
                                                            sInfo = new SenderInfo
                                                            {
                                                                senderMail = dSirket.emailUserName,
                                                                senderPass = dSirket.emailUserPassword,
                                                                senderDisplayName = dSirket.emailDisplayName,
                                                                senderSmtpServer = dSirket.smtpServerName,
                                                                senderSmtpPort = (int)dSirket.smtpServerPort,
                                                                senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                            };
                                                        }
                                                        catch (Exception ex1)
                                                        {
                                                            System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                            var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                            hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                            throw new Exception("SMTP ayarları eksik. " + hata);
                                                        }
                                                    }
                                                    Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                                    IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-Fatura", str + "<BR><BR><BR><BR><BR><BR>Bizi tercih ettiğiniz için teşekkürler, huzurlu günler dileriz.");
                                                    if (iSonuc.hataliMi)
                                                        fat.mail_gittimi = false;
                                                    else
                                                        fat.mail_gittimi = true;
                                                }
                                            }
                                        }
                                        db.Entry(fat).State = EntityState.Modified;
                                        db.SaveChanges();
                                        ar.id = fat.id.ToString();
                                        ar.returnvalue = fat.belgeno + "||||" + fat.fatura_durum + "||||" + fat.fatura_onaykontrol + "||||" + fat.mail_gittimi + "||||" + fat.fatura_tipi;
                                        ar.errordescription = statusItem.Mesaj; // == "" ? "Fatura durumu başarıyla güncellendi." : statusItem.Mesaj;
                                        arList.Add(ar);
                                    }
                                }
                            }
                            if (uyumMustahsilIds.Count() > 0)
                            {
                                //EFatura efatura = new EFatura();
                                string resultMesaj = "";
                                byte resultOnayKontrol = 0;
                                EMustahsil emustahsil = new EMustahsil(Utilities.Uyumsoft, Utilities.EMustahsil, vkn, path);
                                List<ProducerReceiptStatusInfo> statuses = new List<ProducerReceiptStatusInfo>();
                                statuses = emustahsil.GetQueryProducerReceiptStatus(uyumMustahsilIds.ToArray()).ToList();
                                fatura_ayarlari fa = null;
                                foreach (var item in statuses)
                                {
                                    var status = Conversion.Parse<byte>(item.Status);
                                    switch (item.Status)
                                    {
                                        case ProducerReceiptStatus.Draft:
                                            resultMesaj = "Taslak";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Draft;
                                            break;
                                        case ProducerReceiptStatus.Canceled:
                                            resultMesaj = "İptalEdildi";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Canceled;
                                            break;
                                        case ProducerReceiptStatus.PReceiptCanceled:
                                            resultMesaj = "İptalEdildi";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.PReceiptCanceled;
                                            break;
                                        case ProducerReceiptStatus.Queued:
                                            resultMesaj = "Kuyrukta";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Queued;
                                            break;
                                        case ProducerReceiptStatus.Processing:
                                            resultMesaj = "İşleniyor";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Processing;
                                            break;
                                        case ProducerReceiptStatus.Deleted:
                                            resultMesaj = "Silindi";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Deleted;
                                            break;
                                        case ProducerReceiptStatus.Signed:
                                            resultMesaj = "İmzalandı";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Signed;
                                            break;
                                        case ProducerReceiptStatus.Error:
                                            resultMesaj = "Hata";
                                            resultOnayKontrol = (byte)ProducerReceiptStatus.Error;
                                            break;
                                    }
                                    ar = new AjaxResult();
                                    fatura fat = db.faturas.Where(x => x.fatura_guid == item.DocumentId).FirstOrDefault();
                                    fa = db.fatura_ayarlari.Where(x => x.vkn_tckn == vkn && x.kullanici_id == user.id && x.entegrator_tipi == fat.entegrator_tipi && x.fatura_tipi == fat.fatura_tipi).FirstOrDefault();
                                    if (fat != null)
                                    {
                                        //fat.fatura_guid = item.DocumentId;
                                        fat.fatura_onaykontrol = Conversion.Parse<byte>(item.Status);
                                        fat.fatura_durum = resultMesaj;
                                        if (resultOnayKontrol == 6 || resultMesaj == "Hata")
                                        {
                                            ar.status = false;
                                            fat.mail_gittimi = false;
                                        }
                                        else
                                            ar.status = true;
                                        if (resultOnayKontrol == 4)
                                        {
                                            if (fat.mail_gittimi == null || fat.mail_gittimi == false)
                                            {
                                                //mail gönderiliyor
                                                Match match = null;
                                                MustahsilItem mustahsilItem = GetMustahsilItem(fat, fa);

                                                string alici_mail = "";
                                                if (mustahsilItem != null && mustahsilItem.FaturaAlias != null && mustahsilItem.FaturaAlias != "" && regex.Match(mustahsilItem.FaturaAlias).Success)
                                                    alici_mail = mustahsilItem.FaturaAlias;
                                                else
                                                {
                                                    long vkn1 = long.Parse(mustahsilItem.AliciMusteri.VergiKimlikNo.Trim());
                                                    var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn1 && x.vkn_tckn == vkn).FirstOrDefault();
                                                    match = regex.Match(cariAlias.cari_mail ?? "");
                                                    if (match.Success)
                                                        alici_mail = cariAlias.cari_mail;
                                                }
                                                if (alici_mail != "")
                                                {
                                                    emustahsil = new EMustahsil((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                                                    string str = emustahsil.GetHtmlFromMustahsilItem(mustahsilItem);
                                                    ContentResult cr = Content(str);
                                                    firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vkn); //firma smtp bilgileri aliniyor 
                                                    SenderInfo sInfo = null;
                                                    try
                                                    {
                                                        sInfo = new SenderInfo
                                                        {
                                                            senderMail = dSirket.emailUserName,
                                                            senderPass = dSirket.emailUserPassword,
                                                            senderDisplayName = dSirket.emailDisplayName,
                                                            senderSmtpServer = dSirket.smtpServerName,
                                                            senderSmtpPort = (int)dSirket.smtpServerPort,
                                                            senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                        };
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vknAdmin); //firmanın smtp tanımlı değilse bizim firmanın smtp bilgileri kullanılıyor
                                                        sInfo = null;
                                                        try
                                                        {
                                                            sInfo = new SenderInfo
                                                            {
                                                                senderMail = dSirket.emailUserName,
                                                                senderPass = dSirket.emailUserPassword,
                                                                senderDisplayName = dSirket.emailDisplayName,
                                                                senderSmtpServer = dSirket.smtpServerName,
                                                                senderSmtpPort = (int)dSirket.smtpServerPort,
                                                                senderSSL = (bool)dSirket.smtpServerSSLEnable
                                                            };
                                                        }
                                                        catch (Exception ex1)
                                                        {
                                                            System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex1, true);
                                                            var hata = ex1.Message + ">" + tra.GetFrame(0).GetFileName();
                                                            hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                                                            throw new Exception("SMTP ayarları eksik. " + hata);
                                                        }
                                                    }
                                                    Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
                                                    IslemSonuc iSonuc = util.SendEmail(sInfo, alici_mail, "E-Mustahsil", str + "<BR><BR><BR><BR><BR><BR>Bizi tercih ettiğiniz için teşekkürler, huzurlu günler dileriz.");
                                                    if (iSonuc.hataliMi)
                                                        fat.mail_gittimi = false;
                                                    else
                                                        fat.mail_gittimi = true;
                                                }
                                            }
                                        }
                                        db.Entry(fat).State = EntityState.Modified;
                                        db.SaveChanges();
                                        //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                                        if (status == 6 || resultMesaj == "Hata")
                                            ar.status = false;
                                        else
                                            ar.status = true;
                                        ar.id = fat.id.ToString();
                                        ar.returnvalue = fat.belgeno + "||||" + fat.fatura_durum + "||||" + fat.fatura_onaykontrol + "||||" + fat.mail_gittimi + "||||" + fat.fatura_tipi;
                                        ar.errordescription = resultMesaj; // == "" ? "Fatura durumu başarıyla güncellendi." : statusItem.Mesaj;
                                        arList.Add(ar);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ar = new AjaxResult();
                            ar.status = false;
                            ar.errordescription = "Lütfen durum güncelleme için uygun fatura seçiniz.";
                            arList.Add(ar);
                        }
                        int arListCount = arList.Count();
                        foreach (var item in arList)
                        {
                            if (arListCount == 1)
                            {

                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        ar = new AjaxResult();
                        ar.status = false;
                        ar.errordescription = "Lütfen durum güncelleme için uygun fatura seçiniz.";
                        arList.Add(ar);
                    }
                }
                catch (Exception ex)
                {
                    Logger.log(ex, Request.Url.AbsoluteUri, Request.UserHostAddress, RouteData.Values["controller"].ToString(), RouteData.Values["action"].ToString(), "", "");
                    ar = new AjaxResult();
                    //var hata = "";
                    //if (ex.InnerException == null)
                    //{
                    //    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    //    hata = tra.GetFrame(0).GetMethod().ReflectedType.FullName;
                    //    hata += tra.GetFrame(0).GetFileLineNumber().ToString();
                    //    //throw new Exception("aaa:>" + a + "bbb:>" + b, ex);
                    //}
                    //else
                    //{
                    //    hata = ex.InnerException.ToString();
                    //}
                    ar.errordescription = "" + ex.Message;
                    ar.status = false;
                    arList.Add(ar);
                }
            }
            return Json(arList, JsonRequestBehavior.AllowGet);
        }
        [ExceptionHandler]
        [Authorize]
        public List<AjaxResult> EFaturaOlustur(List<fatura> fats)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            string path = Server.MapPath("~/Content/assets/");
            MyListFatura myListUyumsoftFatura = null;
            MyListIrsaliye myListUyumsoftIrsaliye = null;
            MyListMustahsil myListUyumsoftMustahsil = null;
            MyListSMM myListUyumsoftSMM = null;
            MyListFatura myListVeribanFatura = null;
            MyListMustahsil myListVeribanMustahsil = null;
            MyListIrsaliye myListVeribanIrsaliye = null;
            MyListSMM myListVeribanSMM = null;
            EMustahsil emustahsil = null;
            EFatura efatura = null;
            EIrsaliye eirsaliye = null;
            ESMM esmm = null;
            List<AjaxResult> arList = new List<AjaxResult>();
            AjaxResult ar = null;
            fatura_ayarlari fa = null;
            FaturaItem fi = null;
            FaturaItem ii = null; //irsaliye icin
            FaturaItem si = null; //smm icin
            MustahsilItem mi = null;
            fatura fat = null;
            for (int i = 0; i < fats.Count(); i++)
            {
                try
                {
                    fat = fats[i];
                    fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();
                    if (fa == null)
                    {
                        arList.Add(new AjaxResult { status = false, errordescription = Utilities.FaturaTipleri[(int)fat.fatura_tipi] + " ayarı hatalı. Lütfen, Yönetici menüsünden E-Fatura Ayarları sayfasında entegratör ve fatura tipine göre ayarları yapınız." });
                        return arList;
                    }
                    if (!(bool)fa.testmi)
                    {
                        StringBuilder query = new StringBuilder("select * from cari c join mukellef m on c.vergi_numarasi = m.VergiKimlikNo where c.id=" + fat.cari_id);
                        var cari = db.Database.SqlQuery<cari>(query.ToString()).FirstOrDefault();
                        if (cari != null) //cari mukellef listesinde ise fatura_tipi efatura olarak set ediliyor.
                        {
                            if (fat.fatura_tipi == Utilities.EArsivIade)
                                fat.fatura_tipi = Utilities.EFaturaIade;
                            if (fat.fatura_tipi == Utilities.EArsivNormal)
                                fat.fatura_tipi = Utilities.EFaturaNormal;
                        }
                        else //cari mukellef listesinde değil ise fatura_tipi earsiv olarak set ediliyor.
                        {
                            if (fat.fatura_tipi == Utilities.EFaturaIade)
                                fat.fatura_tipi = Utilities.EArsivIade;
                            if (fat.fatura_tipi == Utilities.EFaturaNormal)
                                fat.fatura_tipi = Utilities.EArsivNormal;
                        }
                        fa = db.fatura_ayarlari.Where(x => x.fatura_tipi == fat.fatura_tipi && x.vkn_tckn == fat.vkn_tckn).FirstOrDefault();
                        if (fa == null)
                        {
                            arList.Add(new AjaxResult { status = false, errordescription = Utilities.FaturaTipleri[(int)fat.fatura_tipi] + " ayarı hatalı. Lütfen, Yönetici menüsünden E-Fatura Ayarları sayfasında entegratör ve fatura tipine göre ayarları yapınız." });
                            return arList;
                        }
                    }
                    efatura = new EFatura(fa, path);
                    BelgeNoOlustur(fat, fa, efatura);
                    if (fa.entegrator_tipi == Utilities.Uyumsoft)
                    {
                        if (fat.fatura_tipi == Utilities.EMustahsil)
                        {
                            mi = GetMustahsilItem(fat, fa);
                            if (mi != null)
                            {
                                if (myListUyumsoftMustahsil == null)
                                {
                                    myListUyumsoftMustahsil = new MyListMustahsil();
                                    myListUyumsoftMustahsil.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListUyumsoftMustahsil.FaturaTipi = (int)fa.fatura_tipi;
                                    myListUyumsoftMustahsil.MustahsilItems = new List<MustahsilItem>();
                                }
                                myListUyumsoftMustahsil.MustahsilItems.Add(mi);
                            }
                        }
                        else if (fat.fatura_tipi == Utilities.EIrsaliye)
                        {
                            ii = GetFaturaItem(fat, fa);
                            if (ii != null)
                            {
                                if (myListUyumsoftIrsaliye == null)
                                {
                                    myListUyumsoftIrsaliye = new MyListIrsaliye();
                                    myListUyumsoftIrsaliye.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListUyumsoftIrsaliye.FaturaTipi = (int)fa.fatura_tipi;
                                    myListUyumsoftIrsaliye.FaturaItems = new List<FaturaItem>();
                                }
                                myListUyumsoftIrsaliye.FaturaItems.Add(ii);
                            }
                        }
                        else if (fat.fatura_tipi == Utilities.ESMM)
                        {
                            si = GetFaturaItem(fat, fa);
                            if (si != null)
                            {
                                if (myListUyumsoftSMM == null)
                                {
                                    myListUyumsoftSMM = new MyListSMM();
                                    myListUyumsoftSMM.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListUyumsoftSMM.FaturaTipi = (int)fa.fatura_tipi;
                                    myListUyumsoftSMM.FaturaItems = new List<FaturaItem>();
                                }
                                myListUyumsoftSMM.FaturaItems.Add(si);
                            }
                        }
                        else
                        {
                            fi = GetFaturaItem(fat, fa);
                            if (fi != null)
                            {
                                if (myListUyumsoftFatura == null)
                                {
                                    myListUyumsoftFatura = new MyListFatura();
                                    myListUyumsoftFatura.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListUyumsoftFatura.FaturaTipi = (int)fa.fatura_tipi;
                                    myListUyumsoftFatura.FaturaItems = new List<FaturaItem>();
                                }
                                myListUyumsoftFatura.FaturaItems.Add(fi);
                            }
                        }
                    }
                    if (fa.entegrator_tipi == Utilities.Veriban)
                    {
                        if (fat.fatura_tipi == Utilities.EMustahsil)
                        {
                            mi = GetMustahsilItem(fat, fa);
                            if (mi != null)
                            {
                                if (myListVeribanMustahsil == null)
                                {
                                    myListVeribanMustahsil = new MyListMustahsil();
                                    myListVeribanMustahsil.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListVeribanMustahsil.FaturaTipi = (int)fa.fatura_tipi;
                                    myListVeribanMustahsil.MustahsilItems = new List<MustahsilItem>();
                                }
                                myListVeribanMustahsil.MustahsilItems.Add(mi);
                            }
                        }
                        else if (fat.fatura_tipi == Utilities.EIrsaliye)
                        {
                            ii = GetFaturaItem(fat, fa);
                            if (ii != null)
                            {
                                if (myListUyumsoftIrsaliye == null)
                                {
                                    myListUyumsoftIrsaliye = new MyListIrsaliye();
                                    myListUyumsoftIrsaliye.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListUyumsoftIrsaliye.FaturaTipi = (int)fa.fatura_tipi;
                                    myListUyumsoftIrsaliye.FaturaItems = new List<FaturaItem>();
                                }
                                myListUyumsoftIrsaliye.FaturaItems.Add(ii);
                            }
                        }
                        else
                        {
                            fi = GetFaturaItem(fat, fa);
                            if (fi != null)
                            {
                                if (myListVeribanFatura == null)
                                {
                                    myListVeribanFatura = new MyListFatura();
                                    myListVeribanFatura.EntegratorTipi = (int)fa.entegrator_tipi;
                                    myListVeribanFatura.FaturaTipi = (int)fa.fatura_tipi;
                                    myListVeribanFatura.FaturaItems = new List<FaturaItem>();
                                }
                                myListVeribanFatura.FaturaItems.Add(fi);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                    throw new Exception(hata);
                }
            }
            if (myListUyumsoftFatura != null && myListUyumsoftFatura.FaturaItems.Count > 0)
            {
                List<SendInvoiceStatus> sendstatuses = null;
                try
                {
                    sendstatuses = efatura.FaturaGonderUyumsoft(myListUyumsoftFatura.FaturaItems).ToList();
                    foreach (var senditem in sendstatuses)
                    {
                        ar = new AjaxResult();
                        if (senditem != null && senditem.IsSucceded)
                        {
                            fatura fat1 = db.faturas.Where(x => x.belgeno == senditem.FaturaNo).FirstOrDefault();
                            fat1.fatura_guid = senditem.Guid;
                            fat1.belgeno = senditem.FaturaNo;
                            fat1.seri = senditem.FaturaNo.Substring(0, 3);
                            fat1.sira_no = Convert.ToInt32(senditem.FaturaNo.Substring(6, senditem.FaturaNo.Length - 6));
                            fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                            fat1.fatura_durum = senditem.Mesaj == "" ? "Kuyrukta" : senditem.Mesaj;
                            fat1.last_update = DateTime.Now;

                            //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                            db.Entry(fat1).State = EntityState.Modified;
                            db.SaveChanges();
                            ar.status = senditem.IsSucceded;
                            ar.id = fat1.id.ToString();
                            ar.returnvalue = senditem.FaturaNo;
                            ar.errordescription = senditem.Mesaj == "" ? "Fatura başarıyla gönderildi." : senditem.Mesaj;
                        }
                        else
                        {
                            SendInvoiceStatus sil = senditem;
                            foreach (FaturaItem item in myListUyumsoftFatura.FaturaItems)
                            {
                                fatura fat1 = db.faturas.Find(item.FaturaID);
                                if (fat1 != null)
                                {
                                    fat1.fatura_guid = null;
                                    db.Entry(fat1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            ar.status = senditem.IsSucceded;
                            ar.errordescription = senditem.Mesaj;
                        }
                        arList.Add(ar);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                    throw new Exception(hata);
                }
            }
            if (myListUyumsoftIrsaliye != null && myListUyumsoftIrsaliye.FaturaItems.Count > 0)
            {
                List<SendInvoiceStatus> sendstatuses = null;
                try
                {
                    if (eirsaliye == null)
                        eirsaliye = new EIrsaliye(fa);
                    sendstatuses = eirsaliye.IrsaliyeGonderUyumsoft(myListUyumsoftIrsaliye.FaturaItems).ToList();
                    foreach (var senditem in sendstatuses)
                    {
                        ar = new AjaxResult();
                        if (senditem != null && senditem.IsSucceded)
                        {
                            fatura fat1 = db.faturas.Where(x => x.belgeno == senditem.FaturaNo).FirstOrDefault();
                            fat1.fatura_guid = senditem.Guid;
                            fat1.belgeno = senditem.FaturaNo;
                            fat1.seri = senditem.FaturaNo.Substring(0, 3);
                            fat1.sira_no = Convert.ToInt32(senditem.FaturaNo.Substring(6, senditem.FaturaNo.Length - 6));
                            fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                            fat1.fatura_durum = senditem.Mesaj == "" ? "Kuyrukta" : senditem.Mesaj;
                            fat1.last_update = DateTime.Now;

                            //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                            db.Entry(fat1).State = EntityState.Modified;
                            db.SaveChanges();
                            ar.status = senditem.IsSucceded;
                            ar.id = fat1.id.ToString();
                            ar.returnvalue = senditem.FaturaNo;
                            ar.errordescription = senditem.Mesaj == "" ? "Fatura başarıyla gönderildi." : senditem.Mesaj;
                        }
                        else
                        {
                            foreach (FaturaItem item in myListUyumsoftIrsaliye.FaturaItems)
                            {
                                fatura fat1 = db.faturas.Find(item.FaturaID);
                                if (fat1 != null)
                                {
                                    fat1.fatura_guid = null;
                                    db.Entry(fat1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            ar.status = senditem.IsSucceded;
                            ar.errordescription = senditem.Mesaj;
                        }
                        arList.Add(ar);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                    throw new Exception(hata);
                }
            }
            if (myListUyumsoftMustahsil != null && myListUyumsoftMustahsil.MustahsilItems.Count > 0)
            {
                List<SendProducerReceiptStatus> sendstatuses = null;
                try
                {
                    if (emustahsil == null)
                        emustahsil = new EMustahsil(fa, path);
                    sendstatuses = emustahsil.MustahsilGonderUyumsoft(myListUyumsoftMustahsil.MustahsilItems).ToList();
                    foreach (SendProducerReceiptStatus senditem1 in sendstatuses)
                    {
                        ar = new AjaxResult();
                        if (senditem1 != null && senditem1.IsSucceded)
                        {
                            fatura fat1 = db.faturas.Where(x => x.belgeno == senditem1.FaturaNo).FirstOrDefault();
                            fat1.fatura_guid = senditem1.Guid;
                            fat1.belgeno = senditem1.FaturaNo;
                            fat1.seri = senditem1.FaturaNo.Substring(0, 3);
                            fat1.sira_no = Convert.ToInt32(senditem1.FaturaNo.Substring(6, senditem1.FaturaNo.Length - 6));
                            fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                            fat1.fatura_durum = senditem1.Mesaj == "" ? "Kuyrukta" : senditem1.Mesaj;
                            fat1.last_update = DateTime.Now;

                            //MustahsilItem mustahsilItem = myListUyumsoftMustahsil.MustahsilItems.Where(x => x.FaturaID == fat1.id).FirstOrDefault();
                            //string mailMesaj = "";
                            //string alici_mail = "";
                            //Match match = regex.Match(mustahsilItem.FaturaAlias);
                            //if (match.Success)
                            //    alici_mail = mustahsilItem.FaturaAlias;
                            //else
                            //{
                            //    long vkn1 = long.Parse(mustahsilItem.AliciMusteri.VergiKimlikNo.Trim());
                            //    var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn1 && x.vkn_tckn == vkn).FirstOrDefault();
                            //    match = regex.Match(cariAlias.email);
                            //    if (match.Success)
                            //        alici_mail = cariAlias.email;
                            //}
                            //if (alici_mail != "")
                            //{
                            //    emustahsil = new EMustahsil((int)fat1.entegrator_tipi, (int)fat1.fatura_tipi, (long)fat1.vkn_tckn);
                            //    string str = emustahsil.GetHtmlFromMustahsilItem(mustahsilItem);
                            //    ContentResult cr = Content(str);
                            //    firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == vkn); //firma smtp bilgileri aliniyor 
                            //    SenderInfo sInfo = new SenderInfo
                            //    {
                            //        senderMail = dSirket.emailUserName,
                            //        senderPass = dSirket.emailUserPassword,
                            //        senderDisplayName = dSirket.emailDisplayName,
                            //        senderSmtpServer = dSirket.smtpServerName,
                            //        senderSmtpPort = (int)dSirket.smtpServerPort,
                            //        senderSSL = (bool)dSirket.smtpServerSSLEnable
                            //    };
                            //    Utilities util = new Utilities();
                            //    IslemSonuc iSonuc = util.SendEmail(sInfo, mustahsilItem.FaturaAlias, "E-Fatura", str + "<BR><BR><BR>Bizi tercih ettiğiniz için teşekkürler, huzurlu günler dileriz.");
                            //    if (iSonuc.hataliMi)
                            //    {
                            //        mailMesaj = "Mail gönderilemedi.";
                            //        fat1.mail_gittimi = false;
                            //    }
                            //    else
                            //    {
                            //        mailMesaj = "Mail gönderildi.";
                            //        fat1.mail_gittimi = true;
                            //    }
                            //}
                            //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                            db.Entry(fat1).State = EntityState.Modified;
                            db.SaveChanges();
                            ar.status = senditem1.IsSucceded;
                            ar.id = fat1.id.ToString();
                            ar.returnvalue = senditem1.FaturaNo;
                            ar.errordescription = senditem1.Mesaj == "" ? "Fatura başarıyla gönderildi." : senditem1.Mesaj;
                        }
                        else
                        {
                            foreach (MustahsilItem item in myListUyumsoftMustahsil.MustahsilItems)
                            {
                                fatura fat1 = db.faturas.Find(item.FaturaID);
                                if (fat1 != null)
                                {
                                    fat1.fatura_guid = null;
                                    db.Entry(fat1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            ar.status = senditem1.IsSucceded;
                            ar.errordescription = senditem1.Mesaj;
                            //fatura fat = db.faturas.Where(x => x.belgeno == senditem1.FaturaNo).FirstOrDefault();
                            //fat.belgeno = "";
                            //db.Entry(fat).State = EntityState.Modified;
                            //db.SaveChanges();
                            //ar.status = senditem1.IsSucceded;
                            //ar.returnvalue = senditem1.FaturaNo;
                            //ar.errordescription = senditem1.Mesaj;
                        }
                        arList.Add(ar);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                    throw new Exception(hata);
                }
            }
            if (myListUyumsoftSMM != null && myListUyumsoftSMM.FaturaItems.Count > 0)
            {
                List<SendInvoiceStatus> sendstatuses = null;
                try
                {
                    if (esmm == null)
                        esmm = new ESMM(fa, path);
                    sendstatuses = esmm.SMMGonderUyumsoft(myListUyumsoftSMM.FaturaItems).ToList();
                    foreach (var senditem in sendstatuses)
                    {
                        ar = new AjaxResult();
                        if (senditem != null && senditem.IsSucceded)
                        {
                            fatura fat1 = db.faturas.Where(x => x.belgeno == senditem.FaturaNo).FirstOrDefault();
                            fat1.fatura_guid = senditem.Guid;
                            fat1.belgeno = senditem.FaturaNo;
                            fat1.seri = senditem.FaturaNo.Substring(0, 3);
                            fat1.sira_no = Convert.ToInt32(senditem.FaturaNo.Substring(6, senditem.FaturaNo.Length - 6));
                            fat1.fatura_onaykontrol = (int)InvoiceStatus.WaitingForAprovement;
                            fat1.fatura_durum = senditem.Mesaj == "" ? "Kuyrukta" : senditem.Mesaj;
                            fat1.last_update = DateTime.Now;

                            //Utilities.AddLog(faturalar[k].FaturaID, Utilities.kullanici.id, "alsat_hareket", Utilities.translator("E-Fatura Gönder", "efatura_gonder"), "", "");
                            db.Entry(fat1).State = EntityState.Modified;
                            db.SaveChanges();
                            ar.status = senditem.IsSucceded;
                            ar.id = fat1.id.ToString();
                            ar.returnvalue = senditem.FaturaNo;
                            ar.errordescription = senditem.Mesaj == "" ? "Fatura başarıyla gönderildi." : senditem.Mesaj;
                        }
                        else
                        {
                            SendInvoiceStatus sil = senditem;
                            foreach (FaturaItem item in myListUyumsoftSMM.FaturaItems)
                            {
                                fatura fat1 = db.faturas.Find(item.FaturaID);
                                if (fat1 != null)
                                {
                                    fat1.fatura_guid = null;
                                    db.Entry(fat1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            ar.status = senditem.IsSucceded;
                            ar.errordescription = senditem.Mesaj;
                        }
                        arList.Add(ar);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                    var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                    hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                    throw new Exception(hata);
                }
            }
            return arList;
        }
        public FaturaItem GetFaturaItem(fatura fat, fatura_ayarlari fa)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            FaturaItem fatura = null;
            try
            {
                List<fatura_satir> fdList = null;
                fatura = new FaturaItem();
                fatura.EntegratorTipi = (int)fa.entegrator_tipi;
                //fatura.vade = secilmisfaturalar[i].vade;
                fatura.AktarimMesaji = "";
                fatura.FaturaNotu = fat.aciklama;
                fatura.AliciMusteri = MusteriGetir((int)fat.cari_id, fat.fatura_alias);
                if (fa.testmi == true)
                    fatura.test_alici_vkn = (fa.test_alici_vkn ?? 0).ToString();
                fatura.AraToplam = decimal.Round(fat.aratoplam ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.FaturaNo = fat.belgeno;
                fatura.FaturaID = fat.id;
                fatura.IslemTipi = (int)fat.islem_tipi;

                fatura.FaturaNumarasi = fat.seri + fat.sira_no;
                fatura.FaturaOlusturmaTarihi = (DateTime)fat.kayit_tarihi;
                fatura.FaturaTarihi = (DateTime)fat.fatura_tarihi;
                fatura.FaturaTuru = fa.giden_fatura_tipi;
                fatura.GonderenMusteri = FirmaGetir((long)fat.vkn_tckn);
                if (fa.testmi == true)
                    fatura.test_gndrn_vkn = (fa.test_gndrn_vkn ?? 0).ToString();
                if (fat.fatura_durum != null && (fat.fatura_durum == "Taslak" || fat.fatura_durum == "Hata"))
                    fatura.Guid = fat.fatura_guid;
                else
                    fatura.Guid = Guid.NewGuid().ToString();
                fatura.IskontoTutar = decimal.Round(fat.toplam_iskonto ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.Meblag = decimal.Round(fat.toplam_fiyat ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.KdvTutar = decimal.Round(fat.toplam_kdvtutar ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.TevkifatTutar = decimal.Round(fat.toplam_tevkifat ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.OtvTutar = decimal.Round(fat.toplam_otvtutar ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.OivTutar = decimal.Round(fat.toplam_oivtutar ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.GVSTutar = decimal.Round(fat.toplam_gvstutar ?? 0, 2, MidpointRounding.AwayFromZero);
                fatura.VergiTutar = (fatura.KdvTutar - fatura.TevkifatTutar) + fatura.OtvTutar + fatura.OivTutar;
                fatura.VergiTutar = decimal.Round((decimal?)fatura.VergiTutar ?? 0, 2, MidpointRounding.AwayFromZero);
                var doviz = db.dovizlers.Find(fat.doviz_id);
                fatura.doviz = doviz.kod;
                //mustahsil.doviz_kodu = (dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod == "TL" ? "TRY" : dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod);
                fatura.doviz_kodu = doviz.kod == "TL" ? "TRY" : doviz.kod;
                //fatura.doviz = dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod;
                //fatura.doviz_kodu = (dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod == "TL" ? "TRY" : dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod);
                fatura.kur_carpani = (decimal)doviz.kur_carpani;
                fatura.SatisIban = fat.satis_iban;
                fatura.FaturaAlias = fat.fatura_alias;
                fatura.EEvrakTipi = (int)fat.fatura_tipi;
                fatura.EntegratorTipi = (int)fa.entegrator_tipi;
                fatura.efatura_test = (bool)fa.testmi;
                string evrak_tipi = "";
                if (fat.fatura_tipi == Utilities.EIhracat) //E-İhracat
                {
                    evrak_tipi = "ISTISNA";
                    fatura.FaturaTuru = "IHRACAT";
                }
                else if (fat.fatura_tipi == Utilities.EArsivNormal)
                {
                    fatura.IsArsiv = true;
                    evrak_tipi = "SATIS";
                    fatura.FaturaTuru = "EARSIVFATURA";
                }
                else if (fat.fatura_tipi == Utilities.EFaturaNormal)
                {
                    fatura.IsArsiv = false;
                    evrak_tipi = "SATIS";
                    fatura.FaturaTuru = fa.giden_fatura_tipi;
                }
                else if (fat.fatura_tipi == Utilities.EMustahsil) //E-Müstahsil
                {
                    fatura.FaturaTuru = "EARSIVBELGE";
                }
                else if (fat.fatura_tipi == Utilities.EArsivIade || fat.fatura_tipi == Utilities.EFaturaIade || fat.fatura_tipi == Utilities.EFiyatFarki)
                {
                    evrak_tipi = "IADE";
                    fatura.FaturaTuru = "TEMELFATURA";
                }
                else
                {
                    evrak_tipi = "SATIS";
                }
                fatura.InvoiceTypeCodeType = evrak_tipi;
                fatura.IsSucceded = false;
                fdList = db.fatura_satir.Where(x => x.fatura_id == fat.id).ToList();
                if (fat.fatura_tipi != Utilities.EMustahsil && fat.fatura_tipi != Utilities.EIhracat)
                    fatura.KdvToplam = KDVleriGetir(fdList, fatura.doviz_kodu);
                fatura.HareketSatirlari = HareketSatirlari(fdList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return fatura;
        }
        public MustahsilItem GetMustahsilItem(fatura fat, fatura_ayarlari fa)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            MustahsilItem mustahsil = null;
            try
            {
                List<fatura_satir> fdList = null;
                mustahsil = new MustahsilItem();
                //EMustahsil emustahsil = null;
                mustahsil.EntegratorTipi = (int)fa.entegrator_tipi;
                //emustahsil = new EMustahsil((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (long)fat.vkn_tckn, path);
                mustahsil.AktarimMesaji = "";
                mustahsil.FaturaNotu = fat.aciklama;
                mustahsil.AliciMusteri = MusteriGetir((int)fat.cari_id, fat.fatura_alias);
                if (fa.testmi == true)
                    mustahsil.test_alici_vkn = (fa.test_alici_vkn ?? 0).ToString();
                mustahsil.AraToplam = (decimal)fat.aratoplam;
                mustahsil.FaturaNo = fat.belgeno;
                mustahsil.FaturaID = fat.id;
                mustahsil.IslemTipi = (int)fat.islem_tipi;
                mustahsil.FaturaNumarasi = fat.seri + fat.sira_no;
                mustahsil.FaturaOlusturmaTarihi = (DateTime)fat.kayit_tarihi;
                mustahsil.FaturaTarihi = (DateTime)fat.fatura_tarihi;
                mustahsil.FaturaTuru = fa.giden_fatura_tipi;
                mustahsil.GonderenMusteri = FirmaGetir((long)fat.vkn_tckn);
                if (fa.testmi == true)
                    mustahsil.test_gndrn_vkn = (fa.test_gndrn_vkn ?? 0).ToString();
                if (fat.fatura_durum != null && (fat.fatura_durum == "Taslak" || fat.fatura_durum == "Hata"))
                    mustahsil.Guid = fat.fatura_guid;
                else
                    mustahsil.Guid = Guid.NewGuid().ToString();
                mustahsil.IskontoTutar = decimal.Round(fat.toplam_iskonto ?? 0, 2, MidpointRounding.AwayFromZero);
                mustahsil.Meblag = decimal.Round(fat.toplam_fiyat ?? 0, 2, MidpointRounding.AwayFromZero);
                var doviz = db.dovizlers.Find(fat.doviz_id);
                mustahsil.doviz = doviz.kod;
                //mustahsil.doviz_kodu = (dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod == "TL" ? "TRY" : dovizlerProvider.GetById(secilmisfaturalar[i].doviz_id).kod);
                mustahsil.doviz_kodu = doviz.kod == "TL" ? "TRY" : doviz.kod;
                mustahsil.kur_carpani = (decimal)doviz.kur_carpani;
                mustahsil.SatisIban = fat.satis_iban;
                mustahsil.FaturaAlias = fat.fatura_alias;
                mustahsil.EEvrakTipi = (int)fat.fatura_tipi;
                mustahsil.EntegratorTipi = (int)fa.entegrator_tipi;
                mustahsil.GvsTutar = decimal.Round(fat.toplam_gvstutar ?? 0, 2, MidpointRounding.AwayFromZero);
                mustahsil.MfvTutar = decimal.Round(fat.toplam_mfvtutar ?? 0, 2, MidpointRounding.AwayFromZero);
                mustahsil.SgkTutar = decimal.Round(fat.toplam_sgktutar ?? 0, 2, MidpointRounding.AwayFromZero);
                mustahsil.BtuTutar = decimal.Round(fat.toplam_btututar ?? 0, 2, MidpointRounding.AwayFromZero);
                mustahsil.FaturaTuru = "EARSIVBELGE";
                //mustahsil.InvoiceTypeCodeType = evrak_tipi;
                mustahsil.efatura_test = (bool)fa.testmi;
                mustahsil.IsArsiv = false;
                mustahsil.IsSucceded = false;
                //mustahsil.KdvToplam = KDVleriGetir(secilmisfaturalar[i].hareketdetaylari, mustahsil.doviz_kodu);
                fdList = db.fatura_satir.Where(x => x.fatura_id == fat.id).ToList();
                mustahsil.HareketSatirlari = MustahsilHareketSatirlari(fdList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return mustahsil;
        }
        public void BelgeNoOlustur(fatura fat, fatura_ayarlari fa, EFatura efatura)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            //EFatura efatura = new EFatura((int)fat.entegrator_tipi, (int)fat.fatura_tipi, (int)fat.vkn_tckn);            
            fatura fat1 = null;
            int sira_no = 1;
            string seri = "";
            if ((bool)fa.seri_kullanilsinmi)    //fatura tablosunda seriye göre en son kayıt alınıyor, en son kaydın fatura tipi ile gelen faturanın fatura tipi aynı ise sira nosu 1 artılıyor. 
            {                                   //fatura tipleri aynı değilse, faturadaki seri ye göre arama yapılıyor. en son kaydın fatura tipi ile gelen faturanın fatura tipi aynı ise sira nosu 1 artılıyor. 
                seri = fa.seri;                 //fatura tipleri aynı değilse, hata dönderiliyor.    
                                                //seri = fa.seri.Substring(0, 3);
                fat1 = db.faturas.Where(x => x.vkn_tckn == fat.vkn_tckn && x.kullanici_id == fat.kullanici_id
                    && x.fatura_onaykontrol != null && x.fatura_guid != null && x.fatura_guid != ""
                    && x.belgeno.StartsWith(seri)).OrderByDescending(x => x.belgeno).FirstOrDefault();
                if (fat1 != null)
                {
                    if (fat1.fatura_durum != "Hata")
                    {
                        if (fat1.fatura_tipi != fat.fatura_tipi)
                        {
                            seri = fat.seri;
                            //seri = fat.seri.Substring(0, 3);
                            fat1 = db.faturas.Where(x => x.vkn_tckn == fat.vkn_tckn && x.kullanici_id == fat.kullanici_id
                                && x.fatura_onaykontrol != null && x.fatura_guid != null && x.fatura_guid != ""
                                && x.belgeno.StartsWith(seri)).OrderByDescending(x => x.belgeno).FirstOrDefault();
                            if (fat1 != null)
                            {
                                if (fat1.fatura_tipi != fat.fatura_tipi)
                                    throw new Exception("Belge numarası üretilemedi. Fatura ayarlarını kontrol edip tekrar deneyiniz. Aynı hatayı alırsanız yazılım destek birimini arayınız.");
                                else
                                    sira_no = (int)fat1.sira_no + 1;
                            }
                        }
                        else
                            sira_no = (int)fat1.sira_no + 1;
                    }
                    else
                        sira_no = (int)fat1.sira_no;
                }
            }
            else
            {
                seri = fat.seri;
                //seri = fat.seri.Substring(0, 3);
                fat1 = db.faturas.Where(x => x.vkn_tckn == fat.vkn_tckn && x.kullanici_id == fat.kullanici_id
                    && x.fatura_onaykontrol != null && x.fatura_guid != null && x.fatura_guid != ""
                    && x.belgeno.StartsWith(seri)).OrderByDescending(x => x.belgeno).FirstOrDefault();
                if (fat1 != null)
                {
                    if (fat1.fatura_durum != null && fat1.fatura_durum != "Hata")
                    {
                        if (fat1.fatura_tipi != fat.fatura_tipi)
                        {
                            seri = fa.seri;
                            //seri = fa.seri.Substring(0, 3);
                            fat1 = db.faturas.Where(x => x.vkn_tckn == fat.vkn_tckn && x.kullanici_id == fat.kullanici_id
                                && x.fatura_onaykontrol != null && x.fatura_guid != null && x.fatura_guid != ""
                                && x.belgeno.StartsWith(seri)).OrderByDescending(x => x.belgeno).FirstOrDefault();
                            if (fat1 != null)
                            {
                                if (fat1.fatura_tipi != fat.fatura_tipi)
                                    throw new Exception("Belge numarası üretilemedi. Fatura ayarlarını kontrol edip tekrar deneyiniz. Aynı hatayı alırsanız yazılım destek birimini arayınız.");
                                else
                                    sira_no = (int)fat1.sira_no + 1;
                            }
                        }
                        else
                            sira_no = (int)fat1.sira_no + 1;
                    }
                    else
                        sira_no = (int)fat1.sira_no;
                }
            }
            fat.belgeno = efatura.FaturaNoOlustur(seri, sira_no.ToString(), ((DateTime)fat.fatura_tarihi).Year);
            var fat2 = db.faturas.Where(x => x.vkn_tckn == fat.vkn_tckn && x.kullanici_id == fat.kullanici_id
                    && x.fatura_onaykontrol == null && x.fatura_guid == null
                    && x.belgeno.StartsWith(seri)).OrderByDescending(x => x.belgeno).FirstOrDefault();
            if (fat2 != null)
            {
                if (fat2.fatura_durum != null && fat2.fatura_durum != "Hata")
                {
                    if (string.Compare(fat.belgeno, fat2.belgeno) != 1)
                    {
                        sira_no = (int)fat2.sira_no + 1;
                        fat.belgeno = efatura.FaturaNoOlustur(seri, sira_no.ToString(), ((DateTime)fat.fatura_tarihi).Year);
                    }
                }
                else
                    sira_no = (int)fat2.sira_no;
            }
            fat.seri = seri;
            fat.sira_no = sira_no;
            fat.entegrator_tipi = fa.entegrator_tipi;
            if (fat2 != null)
                db.Entry(fat2).State = EntityState.Detached;
            if (fat1 != null)
                db.Entry(fat1).State = EntityState.Detached;
            db.Entry(fat).State = EntityState.Modified; //fatura_tipi, belgeno, seri, sira_no set ediliyor
            db.SaveChanges();
        }
        [ExceptionHandler]
        [Authorize]
        public Musteri MusteriGetir(int id, string alias)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            cari c = db.caris.Find(id);
            List<cari_adres> caList = db.cari_adres.Where(x => x.cari_id == id).ToList();
            Musteri musteri = new Musteri();
            try
            {
                if (c != null)
                {
                    if (string.IsNullOrEmpty(alias))
                    {
                        var mukellef = db.mukellefs.Where(x => x.VergiKimlikNo == c.vergi_numarasi).FirstOrDefault();
                        //ef_mukellef = efatura_emukellefcariProvider.GetByVergiNo(cari.vergi_numarasi);
                        if (mukellef != null)
                            alias = mukellef.PostBoxAlias ?? "";
                    }
                    musteri.Alias = alias ?? "";
                    var cafList = caList.Where(x => x.faturaadresimi == true).ToList();
                    cari_adres ca = null;
                    if (cafList.Count() > 0)
                    {
                        ca = cafList.FirstOrDefault();
                        musteri.BinaNo = "";
                        musteri.CaddeSokakBulvar = ca.adres;
                        musteri.Fax = ca.fax;
                        musteri.IlceSemtMahalle = ca.ilce_adi;
                        musteri.KapiNo = "";
                        musteri.Sehir = ca.il_adi;
                        musteri.TelNo = ca.telefon_no;
                        musteri.Ulke = ca.ulke_adi;
                    }
                    else
                    {
                        if (caList.Count() > 0)
                        {
                            ca = caList.FirstOrDefault();
                            musteri.BinaNo = "";
                            musteri.CaddeSokakBulvar = ca.adres;
                            musteri.Fax = ca.fax;
                            musteri.IlceSemtMahalle = ca.ilce_adi;
                            musteri.KapiNo = "";
                            musteri.Sehir = ca.il_adi;
                            musteri.TelNo = ca.telefon_no;
                            musteri.Ulke = ca.ulke_adi;
                        }
                    }
                    musteri.Unvan = c.ad + " " + c.soyad;
                    musteri.Ad = c.ad;
                    musteri.Soyad = c.soyad;
                    musteri.SicilNo = "";
                    musteri.VergiDairesi = c.vergi_dairesi;
                    musteri.VergiKimlikNo = c.vergi_numarasi.ToString();

                    musteri.WebAdresi = "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return musteri;
        }
        [ExceptionHandler]
        [Authorize]
        public Musteri FirmaGetir(long vkn_tckn)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            firma firm = null;
            Musteri musteri = null;
            string firma_dosya_yolu = Server.MapPath("~/Content/") + vkn_tckn;
            try
            {
                firm = db.firmas.Find(vkn_tckn);
                musteri = new Musteri();
                musteri.Alias = firm.email;
                musteri.BinaNo = "";
                musteri.CaddeSokakBulvar = firm.adres;
                musteri.Fax = firm.fax;
                musteri.IlceSemtMahalle = firm.ilce_adi;
                musteri.KapiNo = "";
                musteri.Sehir = firm.il_adi;
                musteri.SicilNo = firm.ticaret_sicil_no;
                musteri.TelNo = firm.telefon;
                musteri.Ulke = firm.ulke_adi;
                musteri.Unvan = firm.unvan;
                musteri.VergiDairesi = firm.vergi_dairesi;
                musteri.VergiKimlikNo = firm.vkn_tckn.ToString();
                musteri.WebAdresi = firm.web_sitesi;
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                musteri.logoPath = Path.Combine(firma_dosya_yolu, Path.GetFileName(firm.logo));
                musteri.kasePath = Path.Combine(firma_dosya_yolu, Path.GetFileName(firm.kase));
                musteri.logoUrl = baseUrl + "/Content/" + vkn_tckn + "/" + firm.logo;
                musteri.kaseUrl = baseUrl + "/Content/" + vkn_tckn + "/" + firm.kase;
                string logocontentType = MimeMapping.GetMimeMapping(musteri.logoPath);
                string kasecontentType = MimeMapping.GetMimeMapping(musteri.kasePath);
                musteri.logoBase64 = "data:" + logocontentType + ";base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(musteri.logoPath));
                musteri.kaseBase64 = "data:" + kasecontentType + ";base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(musteri.kasePath));
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return musteri;
        }
        [ExceptionHandler]
        [Authorize]
        private List<FaturaHareketSatirlari> HareketSatirlari(List<fatura_satir> fd)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<FaturaHareketSatirlari> itemList = new List<FaturaHareketSatirlari>();

            FaturaHareketSatirlari item = null;
            try
            {
                for (int i = 0; i < fd.Count; i++)
                {
                    item = new FaturaHareketSatirlari();
                    item.Aciklama = fd[i].stok_adi;
                    item.AliciKodu = "";
                    item.Fiyat = decimal.Round(fd[i].birim_fiyati ?? 0, 2, MidpointRounding.AwayFromZero);
                    //item.IskontoOran = 0; // Oran ortalaması
                    //item.IskontoTutar = doviz_kodu == "TRY" ? hareket[i].iskonto_toplam * hareket[i].kur_carpani : hareket[i].iskonto_toplam;
                    item.IskontoOran = fd[i].iskonto_1 ?? 0;
                    item.IskontoTutar = decimal.Round(fd[i].iskonto_toplam ?? 0, 2, MidpointRounding.AwayFromZero);
                    //item.Kdv = KdvItem(hareket[i], doviz_kodu); //TODO
                    item.Miktar = (decimal)fd[i].miktari;
                    item.kdv_tutar = decimal.Round(fd[i].kdv_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.otv_tutar = decimal.Round(fd[i].otv_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.oiv_tutar = decimal.Round(fd[i].oiv_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.gvs_tutar = decimal.Round(fd[i].gvs_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.tevkifat_tutar = decimal.Round(fd[i].tevkifat ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.kdv_orani = decimal.Round(fd[i].kdv_orani ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.otv_orani = decimal.Round(fd[i].otv_orani ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.oiv_orani = decimal.Round(fd[i].oiv_orani ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.gvs_orani = decimal.Round(fd[i].oiv_orani ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.tevkifat_orani = decimal.Round(fd[i].tevkifat_orani ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.SaticiKodu = "";
                    item.SatirNotu = "";
                    //item.doviz = hareket[i].doviz;
                    //item.doviz_id = hareket[i].doviz_id;
                    //item.kur_carpani = hareket[i].kur_carpani;
                    item.doviz = fd[i].doviz_cinsi_ack;
                    item.doviz_id = (int)fd[i].doviz_id1;
                    item.kur_carpani = (decimal)fd[i].doviz_kuru1;
                    //item.seri_no = new List<string>();
                    //item.StokItem = StokGetir(hareket[i].stok_id, hareket[i].birim_id); //TODO
                    item.stok_adi = fd[i].stok_adi;
                    item.birimi = db.birimlers.Find(fd[i].birim_id).donusum_kodu;
                    item.birim_fiyat = decimal.Round(fd[i].birim_fiyati ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.Tutar = decimal.Round(fd[i].fiyat ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.UreticiKodu = "";

                    item.Iskonto1 = fd[i].iskonto_1 ?? 0;

                    item.teslim_sarti = fd[i].teslim_sarti ?? "";
                    item.kap_cinsi = fd[i].kap_cinsi ?? "";
                    item.kap_no = fd[i].kap_no ?? "";
                    item.kap_adedi = fd[i].kap_adedi ?? 0;
                    item.gonderilme_sekli = fd[i].gonderilme_sekli ?? 0;
                    item.stok_id = fd[i].stok_id ?? 0;
                    item.stok_adi = fd[i].stok_adi ?? "";
                    item.gtip = (db.stoks.Find((int)fd[i].stok_id).gtip_no ?? 0).ToString();
                    itemList.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return itemList;
        }
        [ExceptionHandler]
        [Authorize]
        private List<MustahsilHareketSatirlari> MustahsilHareketSatirlari(List<fatura_satir> fd)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<MustahsilHareketSatirlari> itemList = new List<MustahsilHareketSatirlari>();

            MustahsilHareketSatirlari item = null;
            try
            {
                for (int i = 0; i < fd.Count; i++)
                {
                    item = new MustahsilHareketSatirlari();
                    item.Aciklama = fd[i].stok_adi;
                    item.AliciKodu = "";
                    //item.Fiyat = doviz_kodu == "TRY" ? hareket[i].birim_fiyat * hareket[i].kur_carpani : hareket[i].birim_fiyat;
                    item.Fiyat = decimal.Round(fd[i].fiyat ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.IskontoOran = (decimal)fd[i].iskonto_1;
                    //item.IskontoTutar = doviz_kodu == "TRY" ? hareket[i].iskonto_toplam * hareket[i].kur_carpani : hareket[i].iskonto_toplam;
                    item.IskontoTutar = decimal.Round(fd[i].iskonto_toplam ?? 0, 2, MidpointRounding.AwayFromZero);
                    //item.Kdv = KdvItem(hareket[i], doviz_kodu);
                    item.Miktar = (decimal)fd[i].miktari;
                    //item.OtvTutar = doviz_kodu == "TRY" ? hareket[i].otv_tutar * hareket[i].kur_carpani : hareket[i].otv_tutar;
                    item.SaticiKodu = "";
                    item.SatirNotu = "";
                    item.doviz = fd[i].doviz_cinsi_ack;
                    item.doviz_id = (int)fd[i].doviz_id1;
                    item.kur_carpani = (decimal)fd[i].doviz_kuru1;
                    //item.seri_no = new List<string>();
                    //item.StokItem = db.stoks.Find(fd[i].stok_id); //TODO
                    item.stok_id = (int)fd[i].stok_id;
                    item.stok_adi = fd[i].stok_adi;
                    item.birimi = db.birimlers.Find(fd[i].birim_id).donusum_kodu;
                    item.birim_fiyat = decimal.Round(fd[i].birim_fiyati ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.Tutar = decimal.Round(fd[i].fiyat ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.UreticiKodu = "";

                    item.Iskonto1 = (decimal)fd[i].iskonto_1;
                    item.GvsOrani = (decimal)fd[i].gvs_orani;
                    item.GvsTutar = decimal.Round(fd[i].gvs_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.MfvOrani = (decimal)fd[i].mfv_orani;
                    item.MfvTutar = decimal.Round(fd[i].mfv_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.SgkOrani = (decimal)fd[i].sgk_orani;
                    item.SgkTutar = decimal.Round(fd[i].sgk_tutar ?? 0, 2, MidpointRounding.AwayFromZero);
                    item.BtuOrani = (decimal)fd[i].btu_orani;
                    item.BtuTutar = decimal.Round(fd[i].btu_tutar ?? 0, 2, MidpointRounding.AwayFromZero);

                    itemList.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace tra = new System.Diagnostics.StackTrace(ex, true);
                var hata = ex.Message + ">" + tra.GetFrame(0).GetFileName();
                hata += ">Satir No:" + tra.GetFrame(0).GetFileLineNumber();
                throw new Exception(hata);
            }
            return itemList;
        }
        [ExceptionHandler]
        [Authorize]
        public JsonResult AliasDoldur(long cari_vkn)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<MyListTable> PList = util.getSelectListForFaturaAlias("", cari_vkn, (long)Session["vkn_tckn"]);
            return Json(PList, JsonRequestBehavior.AllowGet);
        }
        private static List<KdvDurum> KDVleriGetir(List<fatura_satir> hareket, string doviz_kodu)
        {
            List<KdvDurum> kdvler = new List<KdvDurum>();

            for (int i = 0; i < hareket.Count; i++)
            {
                kdvler.Add(KdvItem(hareket[i], doviz_kodu));
            }
            return kdvler;
        }
        private static KdvDurum KdvItem(fatura_satir hareket, string doviz_kodu)
        {
            KdvDurum kdv = new KdvDurum();


            kdv.KdvDahilTutar = (decimal)hareket.fiyat;
            //kdv.KdvID = (decimal)hareket.kdv_orani;
            kdv.Oran = hareket.kdv_orani ?? 0;
            kdv.OtvOran = hareket.otv_orani ?? 0;
            kdv.OtvTutar = hareket.otv_tutar ?? 0;
            kdv.TevkifatOrani = hareket.tevkifat_orani ?? 0;
            kdv.TevkifatTutar = decimal.Round(hareket.tevkifat ?? 0, 2, MidpointRounding.AwayFromZero);
            kdv.Tutar = decimal.Round(hareket.kdv_tutar ?? 0, 2, MidpointRounding.AwayFromZero); //doviz_kodu == "TRY" ? hareket.kdv_tutar * hareket.kur_carpani : hareket.kdv_tutar;

            return kdv;
        }
        public ActionResult SatisGrafik()
        {
            ViewBag.Data = "Value,Value1,Value2,Value3"; //list of strings that you need to show on the chart. as mentioned in the example from c-sharpcorner
            ViewBag.ObjectName = "Test,Test1,Test2,Test3";
            return View();
        }
        public ActionResult SatisGrafik1()
        {
            ViewBag.Data = "Value,Value1,Value2,Value3"; //list of strings that you need to show on the chart. as mentioned in the example from c-sharpcorner
            ViewBag.ObjectName = "Test,Test1,Test2,Test3";
            return View();
        }
    }
}