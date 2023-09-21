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

namespace Fatura.UI.Controllers
{
    public class StokController : Controller
    {
        faturaEntities db = null;
        //Utilities util = new Utilities();
        // GET: Stok
        [ExceptionHandler]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult StokListesi(int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            //int a = 1;
            //int b = 0;
            //int c = 0;
            //c = a / b; // sıfıra bölme hatası
            //string str = "ey edip adanada pide ye";
            //int boy = str.Length / 2;
            //string str1 = str.Substring(0,boy);
            //string str2 = str.Substring(boy+1, str.Length-(boy + 1));
            //char[] ccc = str2.ToCharArray();
            //string str3 = "";
            //for (int i = str2.Length; i>0;i--)
            //{
            //    str3 += ccc[i-1];
            //}
            //if (str1 == str3)
            //    System.Diagnostics.Debug.Write(str1 + "==" + str3);
            //else
            //    System.Diagnostics.Debug.Write(str1 + "!=" + str3);
            if (Session["kullanici"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                    ViewBag.CodeSortParm = sortOrder == "kodu" ? "kodu_desc" : "kodu";
                    ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                    ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                    ViewBag.SearchString = searchString;
                    ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                    long vkn = (long)Session["vkn_tckn"];
                    long vknAdmin = Utilities.vknAdmin;
                    var st = from s in db.stoks
                             where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn && s.isdeleted == false //yonetici ise bütün stok listelenir
                             select s;
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        st = st.Where(s => s.adi.Contains(searchString));
                    }
                    switch (sortOrder)
                    {
                        case "id_desc":
                            st = st.OrderByDescending(s => s.id);
                            break;
                        case "last_update_desc":
                            st = st.OrderByDescending(s => s.last_update);
                            break;
                        case "last_update":
                            st = st.OrderBy(s => s.last_update);
                            break;
                        case "kodu":
                            st = st.OrderBy(s => s.kodu);
                            break;
                        case "kodu_desc":
                            st = st.OrderByDescending(s => s.kodu);
                            break;
                        case "tedarik_yer_adi":
                            st = st.OrderBy(s => s.tedarik_yer_adi);
                            break;
                        case "tedarik_yer_adi_desc":
                            st = st.OrderByDescending(s => s.tedarik_yer_adi);
                            break;
                        case "adi":
                            st = st.OrderBy(s => s.adi);
                            break;
                        case "adi_desc":
                            st = st.OrderByDescending(s => s.adi);
                            break;
                        default:
                            st = st.OrderBy(s => s.adi);
                            break;
                    }
                    int _sayfaNo = SayfaNo ?? 1;
                    ViewBag.SayfaNo = _sayfaNo;
                    int _sayfaKayitSayisi = sayfaKayitSayisi ?? 10;
                    List<stok> stokList = st.ToList();
                    ViewBag.KayitSayisi = stokList.Count();
                    IPagedList<stok> stokListPaged = stokList.ToPagedList<stok>(_sayfaNo, _sayfaKayitSayisi);
                    ViewBag.Mesaj = TempData["Islem"];
                    if (Request.IsAjaxRequest() || 1 == 1)
                    {
                        return View("StokListesi", stokListPaged);
                        //return PartialView("~/Views/Stok/_StokListesi.cshtml", stokListPaged);
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
        public ActionResult Kaydet(stok gelen, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                return View("StokFormu", gelen);
            }
            kullanici user = (kullanici)Session["kullanici"];
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (gelen != null && gelen.id == 0) //yeni kayit
            {                
                string resimAdi = "";
                if (gelen.resimFile != null)
                {
                    resimAdi = Utilities.DosyaAdiDuzenle(gelen.resimFile.FileName);
                }
                gelen.resimler = resimAdi;
                gelen.kayit_tarihi = DateTime.Now;
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                gelen.tedarik_yer_adi = (db.ulke_il_ilce.Find(gelen.tedarik_yer_kodu)).adi;
                gelen.kullanici_id = user.id;
                db.stoks.Add(gelen);
                string stok_dosya_yolu = Server.MapPath("~/Content/") + user.vkn_tckn;
                if (!System.IO.File.Exists(stok_dosya_yolu))
                    System.IO.Directory.CreateDirectory(stok_dosya_yolu);
                if (gelen.resimFile != null)
                {
                    gelen.resimFile.SaveAs(Path.Combine(stok_dosya_yolu, Path.GetFileName(resimAdi)));
                }
                TempData["Islem"] = "Eklendi";
            }
            else
            {
                var guncellenecekKayit = db.stoks.Find(gelen.id);
                if (gelen.resimFile != null)
                {
                    string resimAdi = Utilities.DosyaAdiDuzenle(gelen.resimFile.FileName);

                    if (guncellenecekKayit.resimler != null && guncellenecekKayit.resimler != "")
                        gelen.resimler = resimAdi + "," + guncellenecekKayit.resimler;
                    else
                        gelen.resimler = resimAdi;
                    // resim kaydetme
                    string stok_dosya_yolu = Server.MapPath("~/Content/") + user.vkn_tckn;
                    if (!System.IO.File.Exists(stok_dosya_yolu))
                        System.IO.Directory.CreateDirectory(stok_dosya_yolu);
                    gelen.resimFile.SaveAs(Path.Combine(stok_dosya_yolu, Path.GetFileName(resimAdi)));
                }
                else
                {
                    gelen.resimler = guncellenecekKayit.resimler;
                }
                gelen.kayit_tarihi = guncellenecekKayit.kayit_tarihi;
                gelen.last_update = DateTime.Now;
                gelen.isdeleted = false;
                gelen.vkn_tckn = user.vkn_tckn;
                gelen.tedarik_yer_adi = (db.ulke_il_ilce.Find(gelen.tedarik_yer_kodu)).adi;
                gelen.kullanici_id = user.id;
                ViewBag.Mesaj = "Stok başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                TempData["Islem"] = "Guncellendi";
            }
            db.SaveChanges();
            sortOrder = "last_update_desc";
            return RedirectToAction("/StokListesi", "Stok", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult BirimKaydet(stok_birim model, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                model.DropDownListFor = new SelectList(util.getSelectListForBirimler((int)model.birim_id), "Key", "Display");
                return RedirectToAction("BirimGuncelle", "Stok", new { id = model.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            kullanici user = (kullanici)Session["kullanici"];
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (model != null && model.id == 0) //yeni kayit
            {
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                db.stok_birim.Add(model);
            }
            else
            {
                var guncellenecekKayit = db.stok_birim.Find(model.id);
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                ViewBag.Mesaj = "Birim başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(model);
            }
            sortOrder = "last_update_desc";
            db.SaveChanges();
            return RedirectToAction("Guncelle", "Stok", new { id = model.stok_id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult FiyatKaydet(stok_fiyat model, int stok_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (!ModelState.IsValid) //form doğru dolduruldu mu?
            {
                //model.DropDownListFor = new SelectList(util.getSelectListForBirimler((int)model.birim_id), "Key", "Display");
                return RedirectToAction("FiyatGuncelle", "Stok", new { id = model.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = (kullanici)Session["kullanici"];
            if (model != null && model.id == 0) //yeni kayit
            {
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                db.stok_fiyat.Add(model);
            }
            else
            {
                var guncellenecekKayit = db.stok_fiyat.Find(model.id);
                model.last_update = DateTime.Now;
                model.isdeleted = false;
                ViewBag.Mesaj = "Fiyat başarıyla güncellenmiştir.";
                //kayit guncelle
                db.Entry(guncellenecekKayit).CurrentValues.SetValues(model);
            }
            sortOrder = "last_update_desc";
            db.SaveChanges();
            return RedirectToAction("Guncelle", "Stok", new { id = stok_id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult Yeni()
        {
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            List<stok_birim> stok_birimlerList = Enumerable.Empty<stok_birim>().AsQueryable().ToList();
            ViewBag.KayitSayisi = stok_birimlerList.Count();
            IPagedList<stok_birim> stok_birimlerListPaged = stok_birimlerList.ToPagedList<stok_birim>(1, 4);
            var model = new stok
            {
                DropDownListForMensei = new SelectList(util.getSelectListForUlkeIlIlce(0, 0), "Key", "Display"),
                DropDownListForKdv = new SelectList(util.getSelectListForVergiler(0, 1), "Key", "Display"),
                DropDownListForOtv = new SelectList(util.getSelectListForVergiler(0, 2), "Key", "Display"),
                DropDownListForOiv = new SelectList(util.getSelectListForVergiler(0, 3), "Key", "Display"),
                DropDownListForGvs = new SelectList(util.getSelectListForVergiler(0, 5), "Key", "Display"),
                DropDownListForBtu = new SelectList(util.getSelectListForVergiler(0, 6), "Key", "Display"),
                DropDownListForMfv = new SelectList(util.getSelectListForVergiler(0, 7), "Key", "Display"),
                DropDownListForSgk = new SelectList(util.getSelectListForVergiler(0, 8), "Key", "Display"),
                stok_birimler = stok_birimlerListPaged,
            };
            ViewBag.IslemTipi = "Yeni Stok";
            return View("StokFormu", model);

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
            stok model = null;
            if (id != 0)
            {
                if (vkn == vknAdmin)
                    model = db.stoks.FirstOrDefault(x => x.id == id);
                else
                    model = db.stoks.FirstOrDefault(x => x.id == id && x.isdeleted == false && x.vkn_tckn == vkn);
                ViewBag.SortOrder = SortOrder;
                ViewBag.SearchString = SearchString;
                ViewBag.SayfaNo = SayfaNo;
                int _sayfaNo = SayfaNo ?? 1;
                model.DropDownListForMensei = new SelectList(util.getSelectListForUlkeIlIlce((int)model.tedarik_yer_kodu, 0), "Key", "Display");
                var kdv = model.kdv_orani;
                var kdvint = (int)model.kdv_orani;
                model.DropDownListForKdv = new SelectList(util.getSelectListForVergiler((decimal)model.kdv_orani, 1), "Key", "Display");
                model.DropDownListForOtv = new SelectList(util.getSelectListForVergiler((decimal)model.otv_orani, 2), "Key", "Display");
                model.DropDownListForOiv = new SelectList(util.getSelectListForVergiler((decimal)model.oiv_orani, 3), "Key", "Display");
                model.DropDownListForGvs = new SelectList(util.getSelectListForVergiler((decimal)model.gvs_orani, 5), "Key", "Display");
                model.DropDownListForBtu = new SelectList(util.getSelectListForVergiler((decimal)model.btu_orani, 6), "Key", "Display");
                model.DropDownListForMfv = new SelectList(util.getSelectListForVergiler((decimal)model.mfv_orani, 7), "Key", "Display");
                model.DropDownListForSgk = new SelectList(util.getSelectListForVergiler((decimal)model.sgk_orani, 8), "Key", "Display");
                ViewBag.IslemTipi = "Stok Güncelle";
                var sbirim = from sb in db.stok_birim
                             where sb.stok_id == id && sb.isdeleted == false
                             select sb;
                List<stok_birim> stok_birimlerList = sbirim.ToList();
                ViewBag.KayitSayisi = stok_birimlerList.Count();
                IPagedList<stok_birim> stok_birimlerListPaged = stok_birimlerList.ToPagedList<stok_birim>(1, 10);
                model.stok_birimler = stok_birimlerListPaged;
                foreach (var stok_birim in model.stok_birimler)
                {
                    var sfiyat = from sf in db.stok_fiyat
                                 where sf.stok_birim_id == stok_birim.id && sf.isdeleted == false
                                 select sf;
                    //db.dovizlers.Find(stok)
                    List<stok_fiyat> stok_fiyatlarList = sfiyat.ToList();
                    IPagedList<stok_fiyat> stok_fiyatlarListPaged = stok_fiyatlarList.ToPagedList<stok_fiyat>(1, 10);
                    stok_birim.stok_fiyatlar = stok_fiyatlarListPaged;
                }
            }
            else
            {
                List<stok_birim> stok_birimlerList = Enumerable.Empty<stok_birim>().AsQueryable().ToList();
                ViewBag.KayitSayisi = stok_birimlerList.Count();
                IPagedList<stok_birim> stok_birimlerListPaged = stok_birimlerList.ToPagedList<stok_birim>(1, 4);
                model = new stok
                {
                    DropDownListForMensei = new SelectList(util.getSelectListForUlkeIlIlce(0, 0), "Key", "Display"),
                    DropDownListForKdv = new SelectList(util.getSelectListForVergiler(0, 1), "Key", "Display"),
                    DropDownListForOtv = new SelectList(util.getSelectListForVergiler(0, 2), "Key", "Display"),
                    DropDownListForOiv = new SelectList(util.getSelectListForVergiler(0, 3), "Key", "Display"),
                    DropDownListForGvs = new SelectList(util.getSelectListForVergiler(0, 5), "Key", "Display"),
                    DropDownListForBtu = new SelectList(util.getSelectListForVergiler(0, 6), "Key", "Display"),
                    DropDownListForMfv = new SelectList(util.getSelectListForVergiler(0, 7), "Key", "Display"),
                    DropDownListForSgk = new SelectList(util.getSelectListForVergiler(0, 8), "Key", "Display"),
                    stok_birimler = stok_birimlerListPaged,
                };
                ViewBag.IslemTipi = "Yeni Stok";
            }
            return View("StokFormu", model);
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult BirimGuncelle(int id, int stok_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            stok_birim model = null;
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (id != 0)
            {
                //ViewBag.NameSortParm = sortOrder == "adi" ? "adi_desc" : "adi";
                //ViewBag.CodeSortParm = sortOrder == "cari_vkn" ? "cari_vkn_desc" : "kodu";
                //ViewBag.TedarikYeriSortParm = sortOrder == "tedarik_yer_adi" ? "tedarik_yer_adi_desc" : "tedarik_yer_adi";
                //ViewBag.DateSortParm = sortOrder == "last_update" ? "last_update_desc" : "last_update";
                //ViewBag.SearchString = searchString;
                //ViewBag.SayfaKayitSayisi = sayfaKayitSayisi;
                //int _sayfaNo = SayfaNo ?? 1;
                //ViewBag.SayfaNo = _sayfaNo;
                //int _sayfaKayitSayisi = sayfaKayitSayisi ?? 4;
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;

                if (vkn == vknAdmin)
                    model = db.stok_birim.FirstOrDefault(x => x.id == id);
                else
                    model = db.stok_birim.FirstOrDefault(x => x.id == id && x.isdeleted == false);
                if (model != null)
                {
                    model.DropDownListFor = new SelectList(util.getSelectListForBirimler((int)model.birim_id), "Key", "Display");
                    model.stok_id = stok_id;
                    ViewBag.Baslik = "Birim Güncelle";
                    return View("_StokBirimFormu", model);
                }
                else
                {
                    model = new stok_birim();
                    model.DropDownListFor = new SelectList(util.getSelectListForBirimler(0), "Key", "Display");
                    model.stok_id = stok_id;
                    ViewBag.Baslik = "Yeni Birim";
                    return View("_StokBirimFormu", model);
                }
            }
            else
            {
                model = new stok_birim();
                model.DropDownListFor = new SelectList(util.getSelectListForBirimler(0), "Key", "Display");
                model.stok_id = stok_id;
                ViewBag.Baslik = "Yeni Birim";
                return View("_StokBirimFormu", model);
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult FiyatGuncelle(int id, int stok_birim_id, int stok_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            stok_fiyat model = null;
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (id != 0)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;

                if (vkn == vknAdmin)
                    model = db.stok_fiyat.FirstOrDefault(x => x.id == id);
                else
                    model = db.stok_fiyat.FirstOrDefault(x => x.id == id && x.isdeleted == false);
                if (model != null)
                {
                    model.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz((int)model.doviz_id), "Key", "Display");
                    model.DropDownListForFiyatTipi = new SelectList(util.getSelectListForFiyatTipi((int)model.fiyat_tip), "Key", "Display");
                    model.stok_birim_id = stok_birim_id;
                    model.last_update = DateTime.Now;
                    ViewBag.Baslik = "Fiyat Güncelle";
                    ViewBag.stok_id = stok_id;
                    return View("_StokFiyatFormu", model);
                }
                else
                {
                    model = new stok_fiyat();
                    model.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz(1), "Key", "Display");
                    model.DropDownListForFiyatTipi = new SelectList(util.getSelectListForFiyatTipi(1), "Key", "Display");
                    model.stok_birim_id = stok_birim_id;
                    ViewBag.Baslik = "Yeni Fiyat";
                    return View("_StokFiyatFormu", model);
                }
            }
            else
            {
                model = new stok_fiyat();
                model.DropDownListForDoviz = new SelectList(util.getSelectListForDoviz(1), "Key", "Display");
                model.DropDownListForFiyatTipi = new SelectList(util.getSelectListForFiyatTipi(1), "Key", "Display");
                model.stok_birim_id = stok_birim_id;
                ViewBag.Baslik = "Yeni Fiyat";
                ViewBag.stok_id = stok_id;
                return View("_StokFiyatFormu", model);
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
            var kayit = db.stoks.FirstOrDefault(x => x.id == id && x.isdeleted == false && (x.vkn_tckn == vkn || vkn == vknAdmin));
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
                TempData["Islem"] = "Silindi";
                sortOrder = "last_update_desc";
                return RedirectToAction("/StokListesi", "Stok", new { SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult BirimSil(int id, int stok_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.stok_birim.Find(id);
            var st = db.stoks.Find(kayit.stok_id);
            if ((st.vkn_tckn == vkn || vknAdmin == vkn) && kayit != null)
            {
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                return RedirectToAction("Guncelle", "Stok", new { st.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            else
            {
                TempData.Clear();
                return HttpNotFound();
            }
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult FiyatSil(int id, int stok_birim_id, int stok_id, int? SayfaNo, string sortOrder, string searchString, int? sayfaKayitSayisi)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            var kayit = db.stok_fiyat.Find(id);
            var sb = db.stok_birim.Find(kayit.stok_birim_id);
            var st = db.stoks.Find(sb.stok_id);
            if ((st.vkn_tckn == vkn || vknAdmin == vkn) && kayit != null)
            {
                kayit.isdeleted = true;
                kayit.last_update = DateTime.Now;
                db.Entry(kayit);
                db.SaveChanges();
                return RedirectToAction("Guncelle", "Stok", new { st.id, SayfaNo, sortOrder, searchString, sayfaKayitSayisi });
            }
            else
            {
                TempData.Clear();
                return HttpNotFound();
            }
        }
    }
}