using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Fatura.UI.Controllers
{
    public class AdminController : Controller
    {
        faturaEntities db = null;
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [ExceptionHandler]
        public ActionResult Login(string mail)
        {
            ViewBag.Mesaj = TempData["mesaj"];
            //return View();
            return View(new kullanici { kullanici_mail = mail });
        }
        [HttpPost]
        [AllowAnonymous]
        [ExceptionHandler]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string ReturnUrl, kullanici gelen, string captcha, string captchaFileName)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);

            string captcha1 = Session["captchaValue"] == null ? "" : Session["captchaValue"].ToString();
            kullanici kln = null;

            //var sqlConnectionDbs = util.getConfigSqlConnections().SqlConnection.Where(x => x.Company == kln.vkn_tckn.ToString());
            //var myconnection = sqlConnectionDbs.First();

            string path = Server.MapPath("~/Content/");
            if (gelen.kullanici_mail != null && gelen.kullanici_mail != "")
                kln = db.kullanicis.Find(gelen.kullanici_mail);
            if (captcha != null && !captcha.Equals(captcha1)) //captcha sorulmuşsa ve işlem cevabı yanlış ise db ye gitmeden yeni captcha oluşturuluyor.
            {
                ViewBag.Mesaj = "İşlem sonucu hatalı.";
                CaptchaTempResimSil(captchaFileName);
                CaptchaEkle();
                return View(gelen);
            }
            else if (captcha != null && captcha.Equals(captcha1))
                CaptchaTempResimSil(captchaFileName);
            if (kln != null)
            {
                if (kln.isdeleted != null && kln.isdeleted == false)
                {
                    if (kln.sifre.Equals(gelen.sifre))
                    {
                        Session.Remove("captchaValue");
                        Session.Remove("hatali_mail_deneme_sayisi");
                        kln.passattempts = 0;
                        db.SaveChanges();
                        if (kln.isactivated == null || kln.isactivated == false)
                        {
                            ViewBag.Mesaj = "Hesabın aktivasyonu yapılmamıştır. Aktivasyon kodunu mailinize tekrar gönderiniz ya da firma yetkilinize başvurunuz.";
                        }
                        else
                        {
                            firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                            DateTime lisansBitisTarihi;
                            if (dSirket != null && dSirket.lisans_bitis_tarihi != null)
                            {
                                lisansBitisTarihi = dSirket.lisans_bitis_tarihi.Value;
                            }
                            else
                            {
                                lisansBitisTarihi = kln.kayit_tarihi.Value.AddDays(31); // 1 ay deneme süresi
                            }
                            //stok stok = db.stoks.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                            //if (stok == null)
                            //    klnTestStokOlustur((long)kln.vkn_tckn); //test stok verisi 15 adet 
                            //cari cari = db.caris.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                            //if (cari == null)
                            //{
                            //    klnTestCariOlustur((long)kln.vkn_tckn); //test cari verisi 15 adet
                            //    List<cari> caris = db.caris.Where(x => x.vkn_tckn == kln.vkn_tckn).ToList();
                            //    int i = 0;
                            //    foreach (cari ca in caris)
                            //    {
                            //        if (i == 0)
                            //        {

                            //            ca.ad = "emustahsil carisi";
                            //            ca.tip = 1;
                            //            ca.mukellefmi = false;
                            //            ca.vergi_numarasi = ca.vergi_numarasi ?? 12345678901;
                            //            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                            //            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                            //            ca.last_update = DateTime.Now;
                            //            db.Entry(ca);
                            //            db.SaveChanges();
                            //        }
                            //        if (i == 1)
                            //        {
                            //            ca.ad = "efatura carisi";
                            //            ca.tip = 0;
                            //            ca.mukellefmi = true;
                            //            ca.vergi_numarasi = 9000068418; //mukellef cari
                            //            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                            //            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                            //            ca.last_update = DateTime.Now;
                            //            db.Entry(ca);
                            //            db.SaveChanges();
                            //        }
                            //        if (i == 2)
                            //        {
                            //            ca.ad = "earsiv carisi";
                            //            ca.tip = 0;
                            //            ca.mukellefmi = false;
                            //            ca.vergi_numarasi = ca.vergi_numarasi ?? 12345678901;
                            //            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                            //            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                            //            ca.last_update = DateTime.Now;
                            //            db.Entry(ca);
                            //            db.SaveChanges();
                            //        }
                            //        i += 1;
                            //    }
                            //}
                            //fatura_ayarlari fa = db.fatura_ayarlari.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                            //if (fa == null)
                            //    klnFaturaAyarlariOlustur(null); //fatura ayarları oluşturuluyor
                            if (DateTime.Now > lisansBitisTarihi)
                            {
                                ViewBag.Mesaj = "Firmanızın lisans süresi dolmuştur. Firma yetkilinize başvurunuz.";
                            }
                            else
                            {
                                FormsAuthentication.SetAuthCookie(kln.kullanici_mail, false);
                                Session.Add("kullanici", null);
                                Session.Add("vkn_tckn", kln.vkn_tckn);
                                Session.Add("ad_soyad", kln.ad + " " + kln.soyad);
                                Session.Add("kln_email", kln.kullanici_mail);
                                int kalanSure = lisansBitisTarihi.Subtract(DateTime.Now).Days;
                                if (kalanSure < 31)
                                    Session.Add("kalanSure", kalanSure);

                                if (dSirket != null)
                                {
                                    Session.Add("firma_unvan", dSirket.unvan ?? "");
                                    if (dSirket.logo != null && (dSirket.logo ?? "") != "")
                                        Session.Add("firma_logo", dSirket.logo ?? "");
                                    if (ReturnUrl != null && ReturnUrl != "")
                                    {
                                        var url = ReturnUrl.Split('/');
                                        var control = url[1];
                                        var method = url[2];
                                        return RedirectToAction(method, control);
                                    }
                                    return RedirectToAction("Index", "Anasayfa");
                                }
                                else
                                {
                                    TempData["YeniFirmaMesaji"] = "Firma bilgilerinizi girerek sistemi kullanmaya başlayabilirsiniz.";
                                    return RedirectToAction("Yeni", "Firma");
                                }
                            }
                        }
                    }
                    else
                    { // Şifre 3 kez hatalı girilirse captcha soruluyor.
                        ViewBag.Mesaj = "Kullanıcı adı ya da şifre hatalı.";
                        kln.passattempts += 1;
                        db.SaveChanges();
                        if (kln.passattempts > 2) //kullanicinin parola deneme sayısı 2 yi geçerse captcha soruluyor
                        {
                            ViewBag.Mesaj = "Çok fazla hatalı deneme yaptınız. Şifrenizi tekrar girin ve aşağıdaki işlemi yapınız.";
                            CaptchaEkle();
                        }
                    }
                }
                else
                {
                    ViewBag.Mesaj = "Kullanıcı hesabınız kapalı durumdadır. Firma yetkilinize başvurunuz.";
                }
            }
            else
            { //Olmayan bir mail denenirse 3 deneme sonunda captcha soruluyor
                ViewBag.Mesaj = "Kullanıcı adı ya da şifre hatalı.";
                int sayi = Session["hatali_mail_deneme_sayisi"] == null ? 0 : (int)Session["hatali_mail_deneme_sayisi"];
                if (sayi > 2)
                {
                    ViewBag.Mesaj = "Çok fazla hatalı deneme yaptınız. Şifrenizi tekrar girin ve aşağıdaki işlemi yapınız.";
                    CaptchaEkle();
                }
                else
                    Session.Add("hatali_mail_deneme_sayisi", sayi + 1);
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        [ExceptionHandler]
        public ActionResult KullaniciKayit(int id)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici model = null;
            if (id > 0)
            {
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                if (vkn == vknAdmin)
                    model = db.kullanicis.FirstOrDefault(x => x.id == id);
                else
                    model = db.kullanicis.FirstOrDefault(x => x.id == id && x.vkn_tckn == vkn);
                //ViewBag.SortOrder = SortOrder;
                //ViewBag.SearchString = SearchString;
                //ViewBag.SayfaNo = SayfaNo;
                TempData.Clear();
                //return View("KullaniciKayit", "Security", model);
                if (model != null)
                {
                    ViewBag.IslemTipi = "Kullanıcı Güncelle";
                    model.DropDownList = new SelectList(Utilities.getSelectListForBoolean(model.isdeleted != null ? (bool)model.isdeleted : false), "Key", "Display");
                    ViewBag.Yetkiler = Utilities.getCheckBoxForYetkiler(model.rol_yetki);
                }
                else
                {
                    Exception custException = null;
                    if (Session["kullanici"] == null)
                        custException = new Exception("Lütfen giriş yapınız.");
                    else
                        custException = new Exception("Beklenmedik bir hata oluştu. Tekrar deneyiniz.");
                    var model1 = new HandleErrorInfo(custException, "403", "403");
                    return View("Error", model1);
                }
                return View(model);
            }
            else
            {
                model = new kullanici();
                model.DropDownList = new SelectList(Utilities.getSelectListForBoolean(false), "Key", "Display");
                ViewBag.IslemTipi = "Yeni Kullanıcı";
                ViewBag.Yetkiler = Utilities.getCheckBoxForYetkiler("");
                CaptchaEkle();
            }
            return View(model);
        }
        [AllowAnonymous]
        [ExceptionHandler]
        [ValidateAntiForgeryToken]
        public ActionResult KullaniciKayit(kullanici gelen, string sifre2, string captcha, string captchaFileName)
        {
            if (ModelState.IsValid)
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);

                string captcha1 = Session["captchaValue"] == null ? "" : Session["captchaValue"].ToString();
                if (gelen != null && gelen.id == 0) //yeni kayit
                {
                    if (captcha != null && !captcha.Equals(captcha1)) //captcha sorulmuşsa ve işlem cevabı yanlış ise db ye gitmeden yeni captcha oluşturuluyor.
                    {
                        ViewBag.Mesaj = "İşlem sonucu hatalı.";
                        CaptchaTempResimSil(captchaFileName);
                        CaptchaEkle();
                        return View(gelen);
                    }
                    else if (captcha != null && captcha.Equals(captcha1))
                    {
                        CaptchaTempResimSil(captchaFileName);
                    }
                    var kln = db.kullanicis.Find(gelen.kullanici_mail);
                    if (kln == null)
                    {
                        kln = db.kullanicis.FirstOrDefault(x => x.vkn_tckn == gelen.vkn_tckn);
                        if (kln == null || Session["kullanici"] != null)
                        {
                            if (gelen.sifre != null && !gelen.sifre.Equals(sifre2))
                            {
                                ViewBag.Mesaj = "Girdiğiniz şifreler birbirinden farklı. Tekrar giriniz.";
                                CaptchaEkle();
                            }
                            else
                            {
                                try
                                {
                                    gelen.isdeleted = false;
                                    gelen.kayit_tarihi = DateTime.Now;
                                    gelen.last_update = DateTime.Now;
                                    gelen.isactivated = false;
                                    gelen.passattempts = 0;
                                    gelen.activationcode1 = Guid.NewGuid().ToString();
                                    if (gelen.rol_yetki == null)
                                    {
                                        if (gelen.vkn_tckn != Utilities.vknAdmin)
                                            gelen.rol_yetki = "0,2,"; //earsiv yetkisi default olarak veriliyor.
                                        else
                                            gelen.rol_yetki = "0,1,2,3,4,5,6,7,";
                                    }
                                    //if (Session["kullanici"] != null && (long)Session["vkn_tckn"] != Utilities.vknAdmin) //Firma yetkilisi kullanıcı oluşturursa firma vkn si set edilir.
                                    //{
                                    //    gelen.vkn_tckn = (long)Session["vkn_tckn"];
                                    //}
                                    bool sonuc = Utilities.isVKN("9000068418");
                                    bool sonuc1 = Utilities.isTC("10292363028");
                                    db.kullanicis.Add(gelen);
                                    db.SaveChanges();
                                    //klnFirmaOlustur(gelen);
                                    TempData["Islem"] = "Eklendi";
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Kayıt işleminde hata oluştu.", ex.InnerException);
                                }
                                TempData["Mesaj"] = "Kullanıcı bilgileri başarıyla kaydedildi. Lütfen mailinize gelen doğrulama kodu ile mailinizi doğrulayınız.";
                                return RedirectToAction("Dogrula", "Security", gelen);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("vkn_tckn", "Girdiğiniz VKN/TCKN sistemde kayıtlı. Yeni kullanıcı açtırmak için sistem destek birimini arayınız ya da farklı bir VKN/TCKN giriniz.");
                            ViewBag.Mesaj = "Girdiğiniz VKN/TCKN sistemde kayıtlı. Yeni kullanıcı açtırmak için sistem destek birimini arayınız ya da farklı bir VKN/TCKN giriniz.";
                            ViewBag.IslemTipi = "Yeni Kullanıcı";
                            CaptchaEkle();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("kullanici_mail", "Girdiğiniz mail adresi sistemde kayıtlı. Bu mail adresi size aitse ve daha önce kaydolduysanız lütfen Giriş Yap sayfasından giriş yapınız ya da farklı bir mail adresi giriniz.");
                        ViewBag.Mesaj = "Girdiğiniz mail adresi sistemde kayıtlı. Bu mail adresi size aitse ve daha önce kaydolduysanız lütfen Giriş Yap sayfasından giriş yapınız ya da farklı bir mail adresi giriniz.";
                        ViewBag.IslemTipi = "Yeni Kullanıcı";
                        CaptchaEkle();
                    }
                    ViewBag.Yetkiler = Utilities.getCheckBoxForYetkiler(gelen.rol_yetki ?? "");
                    ViewBag.Baslik = "Yeni Kullanıcı";
                    return View(gelen);
                    //db.kullanicis.Add(gelen);
                    //db.SaveChanges();
                    //ViewBag.Mesaj = "Kaydetme İşlemi Başarı ile Gerçekleşmiştir...";
                    ////return PartialView("_Onay");
                    //return RedirectToAction("Login");
                }
                else //güncelleme
                {
                    long vkn = (long)Session["vkn_tckn"];
                    if (Session["kullanici"] != null && User.IsInRole("1"))
                    {
                        var guncellenecekKayit = db.kullanicis.FirstOrDefault(x => x.id == gelen.id);

                        if (vkn != Utilities.vknAdmin)
                        {
                            if (gelen.vkn_tckn != guncellenecekKayit.vkn_tckn)
                            {
                                ViewBag.Mesaj = "VKN/TCKN değiştirme yetkiniz yoktur.";
                                ViewBag.IslemTipi = "Kullanıcı Güncelle";
                                //gelen.isdeleted != null ? (bool)gelen.isdeleted : false
                                gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean(gelen.isdeleted ?? false), "Key", "Display");
                                ViewBag.Yetkiler = Utilities.getCheckBoxForYetkiler(gelen.rol_yetki);
                                gelen.vkn_tckn = guncellenecekKayit.vkn_tckn;
                                return View(gelen);
                            }
                            //fatura yetkisi(2,3,4,5,6,7) sadece şirket admini tarafından verilir ve güncellenebilir.
                            if ((gelen.rol_yetki ?? "0,") != (guncellenecekKayit.rol_yetki ?? "0,"))
                            {
                                string[] onceki_roles = guncellenecekKayit.rol_yetki.Split(',');
                                string[] gelen_roles = gelen.rol_yetki.Split(',');
                                List<string> son_roles = new List<string>();
                                for (int i = 0; i < gelen_roles.Length; i++)
                                {
                                    if (gelen_roles[i] == "0" || gelen_roles[i] == "1")
                                    {
                                        son_roles.Add(gelen_roles[i]);
                                    }
                                }
                                for (int j = 0; j < onceki_roles.Length; j++)
                                {
                                    if (onceki_roles[j] != "0" && onceki_roles[j] != "1")
                                    {
                                        son_roles.Add(onceki_roles[j]);
                                    }
                                }
                                gelen.rol_yetki = String.Join(",", son_roles.ToArray());
                            }
                        }
                        gelen.kayit_tarihi = guncellenecekKayit.kayit_tarihi;
                        gelen.last_update = DateTime.Now;
                        gelen.activationcode1 = guncellenecekKayit.activationcode1;
                        gelen.activationcode2 = guncellenecekKayit.activationcode2;
                        gelen.isactivated = guncellenecekKayit.isactivated;
                        gelen.sifre = guncellenecekKayit.sifre;
                        if (gelen.isdeleted == null)
                            gelen.isdeleted = guncellenecekKayit.isdeleted;
                        if (gelen.passattempts == null)
                            gelen.passattempts = guncellenecekKayit.passattempts;
                        ViewBag.Mesaj = "Kullanıcı başarıyla güncellenmiştir.";
                        if (Session["kullanici"] != null && (long)Session["vkn_tckn"] != Utilities.vknAdmin) //Firma yetkilisi kullanıcı oluşturursa firma vkn si set edilir.
                        {
                            gelen.vkn_tckn = (long)Session["vkn_tckn"];
                        }
                        db.Entry(guncellenecekKayit).CurrentValues.SetValues(gelen);
                        db.SaveChanges();
                        ViewBag.Baslik = "Kullanıcı Güncelle";
                        TempData["Islem"] = "Guncellendi";
                        return RedirectToAction("KullaniciListesi", "Security");
                    }
                }
            }
            else
            {
                gelen.DropDownList = new SelectList(Utilities.getSelectListForBoolean((bool)gelen.isdeleted), "Key", "Display");
            }
            return View();
        }
        public void CaptchaEkle()
        {
            string pathTemp = Server.MapPath("~/Content/temp/");
            string path = Server.MapPath("~/Content/captcha/");
            var files = Directory.GetFiles(path, "*.*").Where(s => Regex.Match(s, @"\.(jpg|gif|png)$").Success);
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            string randFileName = files.ToList()[util.Generator.Next(0, files.Count())];
            string fileName = Path.GetFileName(randFileName);
            string fileName1 = Path.GetFileNameWithoutExtension(randFileName);
            string captchaValue = fileName1.Split('_')[0]; // captcha resim dosyasının adının ilk degeri captcha degeri yani cevap olarak alınıyor
            Session.Add("captchaValue", captchaValue);
            string sourceFile = System.IO.Path.Combine(path, fileName);
            string destFile1 = fileName.Replace(fileName1, Guid.NewGuid().ToString().Substring(0, 13));//captcha resminin ismi random olarak değiştiriliyor 
            string destFile = System.IO.Path.Combine(pathTemp, destFile1);
            ViewBag.captchaResmi = destFile1;
            System.IO.File.Copy(sourceFile, destFile, true);    //captcha resmi temp klasörüne kopyalanıyor. kullanıcıya bu gösterilecek.
        }
        public void CaptchaTempResimSil(string captchaFileName)
        {
            string pathTemp = Server.MapPath("~/Content/temp/");
            if (captchaFileName != null && captchaFileName != "")
            {
                string deleteFile = System.IO.Path.Combine(pathTemp, captchaFileName);
                if (System.IO.File.Exists(deleteFile)) //temp deki resim siliniyor.
                {
                    System.IO.File.Delete(deleteFile);
                }
            }
        }
    }
}