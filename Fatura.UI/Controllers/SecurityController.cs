using Fatura.BLL;
using Fatura.Entity;
using Fatura.UI.Filters;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services;
using static Fatura.BLL.Utilities;

namespace Fatura.UI.Controllers
{
    public class SecurityController : Controller
    {
        faturaEntities db = null;
        public SecurityController()
        {
            //kullanici user = null;
            //if (Session != null && Session["kullanici"] != null)
            //    user = (kullanici)Session["kullanici"];
            //if (user != null)
            //{
            //    db = new faturaEntities(user.vkn_tckn.ToString());
            //}
            //string url = Request.Url.AbsoluteUri; //http://en.domain.com/
            //var sil = Request.RawUrl;
            //var url = HttpContext.Request.Url.Host;

            //var index = url.IndexOf(".");
            //Regex regex = new Regex(@"/(?:http[s]*\:\/\/)*(.*?)\.(?=[^\/]*\..{2,5})/", RegexOptions.IgnoreCase);
            //GroupCollection results = regex.Match(url).Groups;
            //Group result = results[0];
        }

        //Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
        // GET: Security
        [AllowAnonymous]
        [ExceptionHandler]
        public ActionResult Login(string mail)
        {
            ViewBag.Mesaj = TempData["mesaj"];
            //return View();
            return View("_Login", new kullanici { kullanici_mail = mail });
        }
        [HttpPost]
        [AllowAnonymous]
        [ExceptionHandler]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string ReturnUrl, kullanici gelen, string captcha, string captchaFileName)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (HttpContext.Request.Url.Host.Split('.')[0] == Utilities.rootURL)
            {
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
                                stok stok = db.stoks.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (stok == null)
                                    klnTestStokOlustur((long)kln.vkn_tckn); //test stok verisi 15 adet 
                                cari cari = db.caris.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (cari == null)
                                {
                                    klnTestCariOlustur((long)kln.vkn_tckn); //test cari verisi 15 adet
                                    List<cari> caris = db.caris.Where(x => x.vkn_tckn == kln.vkn_tckn).ToList();
                                    int i = 0;
                                    foreach (cari ca in caris)
                                    {
                                        if (i == 0)
                                        {

                                            ca.ad = "emustahsil";
                                            ca.soyad = "carisi";
                                            ca.tip = 1;
                                            ca.mukellefmi = false;
                                            ca.vergi_numarasi = ca.vergi_numarasi ?? 12345678901;
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        if (i == 1)
                                        {
                                            ca.ad = "efatura";
                                            ca.soyad = "carisi";
                                            ca.tip = 0;
                                            ca.mukellefmi = true;
                                            ca.vergi_numarasi = 9000068418; //mukellef cari
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        if (i == 2)
                                        {
                                            ca.ad = "earsiv";
                                            ca.soyad = "carisi";
                                            ca.tip = 0;
                                            ca.mukellefmi = false;
                                            ca.vergi_numarasi = 12345678901;
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        i += 1;
                                    }
                                }
                                fatura_ayarlari fa = db.fatura_ayarlari.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (fa == null)
                                    klnFaturaAyarlariOlustur(kln); //fatura ayarları oluşturuluyor
                                if (DateTime.Now > lisansBitisTarihi)
                                {
                                    ViewBag.Mesaj = "Firmanızın lisans süresi dolmuştur. Firma yetkilinize başvurunuz.";
                                }
                                else
                                {
                                    FormsAuthentication.SetAuthCookie(kln.kullanici_mail, false);
                                    Session.Add("kullanici", kln);//TODO kullanici olarak session null atanmışım neden?
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
            }
            else
            {
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
                                stok stok = db.stoks.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (stok == null)
                                    klnTestStokOlustur((long)kln.vkn_tckn); //test stok verisi 15 adet 
                                cari cari = db.caris.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (cari == null)
                                {
                                    klnTestCariOlustur((long)kln.vkn_tckn); //test cari verisi 15 adet
                                    List<cari> caris = db.caris.Where(x => x.vkn_tckn == kln.vkn_tckn).ToList();
                                    int i = 0;
                                    foreach (cari ca in caris)
                                    {
                                        if (i == 0)
                                        {

                                            ca.ad = "emustahsil";
                                            ca.soyad = "carisi";
                                            ca.tip = 1;
                                            ca.mukellefmi = false;
                                            ca.vergi_numarasi = ca.vergi_numarasi ?? 12345678901;
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        if (i == 1)
                                        {
                                            ca.ad = "efatura";
                                            ca.soyad = "carisi";
                                            ca.tip = 0;
                                            ca.mukellefmi = true;
                                            ca.vergi_numarasi = 9000068418; //mukellef cari
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        if (i == 2)
                                        {
                                            ca.ad = "earsiv";
                                            ca.soyad = "carisi";
                                            ca.tip = 0;
                                            ca.mukellefmi = false;
                                            ca.vergi_numarasi = ca.vergi_numarasi ?? 12345678901;
                                            ca.cari_mail = ca.cari_mail ?? gelen.kullanici_mail;
                                            ca.cari_mail = ca.cari_mail == "" ? gelen.kullanici_mail : ca.cari_mail;
                                            ca.last_update = DateTime.Now;
                                            db.Entry(ca);
                                            db.SaveChanges();
                                        }
                                        i += 1;
                                    }
                                }
                                fatura_ayarlari fa = db.fatura_ayarlari.FirstOrDefault(x => x.vkn_tckn == kln.vkn_tckn);
                                if (fa == null)
                                    klnFaturaAyarlariOlustur(kln); //fatura ayarları oluşturuluyor
                                if (DateTime.Now > lisansBitisTarihi)
                                {
                                    ViewBag.Mesaj = "Firmanızın lisans süresi dolmuştur. Firma yetkilinize başvurunuz.";
                                }
                                else
                                {
                                    FormsAuthentication.SetAuthCookie(kln.kullanici_mail, false);
                                    Session.Add("kullanici", kln);
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
            }
            return View();
        }
        public bool klnTestStokOlustur(long vkn)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            StringBuilder query = new StringBuilder();
            query.Append("update stok set vkn_tckn = @param1 where id in (select top 15 st.id from stok st left join firma fr on st.vkn_tckn = fr.vkn_tckn where fr.id is null)");
            int sil = db.Database.ExecuteSqlCommand(query.ToString(), new SqlParameter("param1", vkn));
            if (sil > 0)
                return true;
            else
                return false;
        }
        public bool klnTestCariOlustur(long vkn)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            StringBuilder query = new StringBuilder();
            query.Append("update cari set vkn_tckn = @param1 where id in (select top 15 c.id from cari c left join firma fr on c.vkn_tckn = fr.vkn_tckn where fr.id is null)");
            int sil = db.Database.ExecuteSqlCommand(query.ToString(), new SqlParameter("param1", vkn));
            if (sil > 0)
                return true;
            else
            {
                query = new StringBuilder();
                query.Append("insert into cari values(@param1,1,'earsiv@mail.com','sifre1',9000068418,'E-ARSIV','CARISI','www.earsiv.com','05553333333','02162222222','hatay',null,null,'','',null,'',0,0,0,0,'MALTEPE',9000068418,'','','',0,CAST(N'2020-07-10T07:51:46.660' AS DateTime),'Jan  1 1900 12:00AM',0);");
                query.Append("insert into cari values(@param1,1,'earsiv@mail.com','sifre1',9000068418,'E-ARSIV','CARISI','www.earsiv.com','05553333333','02162222222','hatay',null,null,'','',null,'',0,0,0,0,'MALTEPE',9000068418,'','','',0,CAST(N'2020-07-10T07:51:46.660' AS DateTime),'Jan  1 1900 12:00AM',0);");
                query.Append("insert into cari values(@param1,1,'earsiv@mail.com','sifre1',9000068418,'E-ARSIV','CARISI','www.earsiv.com','05553333333','02162222222','hatay',null,null,'','',null,'',0,0,0,0,'MALTEPE',9000068418,'','','',0,CAST(N'2020-07-10T07:51:46.660' AS DateTime),'Jan  1 1900 12:00AM',0);");
                //query.Append("insert into cari values(@param1,1,'earsiv@mail.com','sifre1',9000068418,'E-ARSIV','CARISI','www.earsiv.com','05553333333','02162222222','hatay',null,null,'','',null,'',0,0,0,0,'MALTEPE',9000068418,'','','',0,CAST(N'2020-07-10T07:51:46.660' AS DateTime),'Jan  1 1900 12:00AM',0)");
                //query.Append("insert into cari values(@param1,1,'earsiv@mail.com','sifre1',9000068418,'E-ARSIV','CARISI','www.earsiv.com','05553333333','02162222222','hatay',null,null,'','',null,'',0,0,0,0,'MALTEPE',9000068418,'','','',0,CAST(N'2020-07-10T07:51:46.660' AS DateTime),'Jan  1 1900 12:00AM',0);");
                int sil1 = db.Database.ExecuteSqlCommand(query.ToString(), new SqlParameter("param1", vkn));
                if (sil > 0)
                    return true;
                else
                    return false;
            }
        }
        public void klnFirmaOlustur(kullanici kln)
        {
            //StringBuilder query1 = new StringBuilder();
            //query1.Append("insert into firma values(@p1,@p2, 'ÜMRANİYE', '', 'Adresi',100,'TÜRKİYE',34,'İSTANBUL',0,'',@p3, 'www.enucuzefatura.com', @p4, @p5,'TR01010101010101','TR02020202020202','TR030303030303','notlar',dateadd(day, +30, getdate()),'dene9876543210@gmail.com','your password','E-Fatura Sistemi','smtp.gmail.com',587,1,'','',GETDATE(),0);");
            //StringBuilder query2 = new StringBuilder();
            //query2.Append("insert into firma values(@p1,@p2, '', '', '',100,'TÜRKİYE',34,'İSTANBUL',0,'',@p3, '', @p4, @p5,'TR01','TR02','TR03','notlar',dateadd(day, +30, getdate()),'','','','',587,1,'','',GETDATE(),0);");
            //int sonuc = 0;
            //if (kln.vkn_tckn == vknAdmin) //bizim firma kaydı ekleniyor
            //{
            //    var sp1 = new SqlParameter("p1", kln.vkn_tckn);
            //    var sp2 = new SqlParameter("p2", kln.ad ?? "" + " " + kln.soyad ?? "");
            //    var sp3 = new SqlParameter("p3", kln.kullanici_mail ?? "");
            //    var sp4 = new SqlParameter("p4", kln.cep_no ?? "");
            //    var sp5 = new SqlParameter("p5", kln.telefon_no ?? "");
            //    sonuc = db.Database.ExecuteSqlCommand(query1.ToString(), sp1, sp2, sp3, sp4, sp5);
            //}
            //else
            //{
            //    var sp1 = new SqlParameter("p1", kln.vkn_tckn);
            //    var sp2 = new SqlParameter("p2", kln.ad ?? "" + " " + kln.soyad ?? "");
            //    var sp3 = new SqlParameter("p3", kln.kullanici_mail ?? "");
            //    var sp4 = new SqlParameter("p4", kln.cep_no ?? "");
            //    var sp5 = new SqlParameter("p5", kln.telefon_no ?? "");
            //    sonuc = db.Database.ExecuteSqlCommand(query2.ToString(), sp1);
            //}
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            string path = Server.MapPath("~/Content/");
            firma dSirket = db.firmas.Find(kln.vkn_tckn); //ilk kez login olan kullanıcı için firma kaydı oluşturuluyor.
            try
            {
                if (dSirket == null)
                {
                    dSirket = new firma();
                    if ((kln.vkn_tckn ?? 0) != 0 && ((kln.vkn_tckn ?? 0).ToString().Length == 10 || (kln.vkn_tckn ?? 0).ToString().Length == 11))
                    {
                        if (kln.vkn_tckn == Utilities.vknAdmin)//bizim firma kaydı ekleniyor
                        {
                            dSirket.emailUserName = "dene9876543210@gmail.com";
                            dSirket.emailUserPassword = "your password";
                            dSirket.emailDisplayName = "E-Fatura Sistemi";
                            dSirket.smtpServerName = "smtp.gmail.com";
                            dSirket.smtpServerPort = 587;
                            dSirket.smtpServerSSLEnable = true;
                        }
                        dSirket.vkn_tckn = kln.vkn_tckn ?? 0;
                        string ad = kln.ad ?? "";
                        string soyad = kln.soyad ?? "";
                        if (ad != "" || soyad != "")
                            dSirket.unvan = kln.ad + " " + kln.soyad;
                        else
                            dSirket.unvan = kln.vkn_tckn.ToString().Length == 10 ? "VKN:" : "TCKN:" + kln.vkn_tckn;
                        dSirket.email = kln.kullanici_mail;
                        dSirket.vergi_dairesi = "VERGIDAIRESI";
                        dSirket.telefon = kln.telefon_no;
                        dSirket.lisans_bitis_tarihi = kln.kayit_tarihi.Value.AddDays(31); // 1 ay deneme süresi
                        string logo1 = path + "assets\\default_logo.jpg";
                        string kase1 = path + "assets\\default_kase.jpg";
                        string sil = path + kln.vkn_tckn;
                        string logo2 = sil + "\\" + kln.vkn_tckn + "_default_logo.jpg";
                        string kase2 = sil + "\\" + kln.vkn_tckn + "_default_kase.jpg";
                        bool dirVarMi = Directory.Exists(sil);
                        if (!dirVarMi)
                            Directory.CreateDirectory(sil);
                        System.IO.File.Copy(logo1, logo2, true);
                        System.IO.File.Copy(kase1, kase2, true);
                        dSirket.logo = kln.vkn_tckn + "_default_logo.jpg";
                        dSirket.kase = kln.vkn_tckn + "_default_kase.jpg";
                        dSirket.isdeleted = false;
                        dSirket.last_update = DateTime.Now;
                        dSirket.iban1 = "TR0101010101";
                        dSirket.iban2 = "TR0202020202";
                        dSirket.iban3 = "TR0303030303";
                        db.firmas.Add(dSirket);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Kayıt işleminde hata oluştu." + ex.Message.ToString());
            }
            //if (sonuc > 0)
            //    return true;
            //else
            //    return false;
        }
        public bool klnFaturaAyarlariOlustur(kullanici kln)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            StringBuilder query = null;
            //List<fatura_ayarlari> faList = db.fatura_ayarlari.Where(x => x.vkn_tckn == 9000068418).ToList(); //9000068418 ayarları kopyalanıyor.
            //insert into fatura_ayarlari values(3090308658,1,1,1,1,'Uyumsoft', 'Uyumsoft', 'EFA', 1, 'TEMELFATURA', 0, GETDATE());
            int sayi = 0;
            //seri oluşturuluyor
            string ad = kln.ad ?? "";
            string soyad = kln.soyad ?? "";
            var seri = "";
            if (ad != "")
                seri += kln.ad.Substring(0, 1).ToUpper();
            if (soyad != "")
                seri += kln.soyad.Substring(0, 1).ToUpper();
            if (seri.Length < 2)
            {
                if (seri.Length == 0)
                {
                    seri = Utilities.RastgeleStringGetir(2);
                }
                if (seri.Length == 1)
                {
                    seri += Utilities.RastgeleStringGetir(1);
                }
            };
            for (int i = 1; i < Utilities.FaturaTipleri.Length; i++)
            {
                string seri1 = "";
                query = new StringBuilder();
                query.Append("insert into fatura_ayarlari values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14);");
                var sp1 = new SqlParameter("p1", kln.vkn_tckn);
                var sp2 = new SqlParameter("p2", kln.id);
                var sp3 = new SqlParameter("p3", i); //Fatura Tipi
                var sp4 = new SqlParameter("p4", 1); //testmi test
                SqlParameter sp5 = null;
                SqlParameter sp6 = null;
                if (i == 1 || i == 3)
                {
                    sp5 = new SqlParameter("p5", "9000068418"); //Uyumsoft test ayarı
                    sp6 = new SqlParameter("p6", "11111111111"); //Uyumsoft test ayarı
                }
                else
                {
                    sp5 = new SqlParameter("p5", "9000068418"); //Uyumsoft test ayarı
                    sp6 = new SqlParameter("p6", "9000068418"); //Uyumsoft test ayarı
                }
                var sp7 = new SqlParameter("p7", 1); //Uyumsoft
                var sp8 = new SqlParameter("p8", "Uyumsoft");
                var sp9 = new SqlParameter("p9", "Uyumsoft");

                switch (i)
                {
                    case 1:
                        seri1 = seri + "A";
                        break;
                    case 2:
                        seri1 = seri + "F";
                        break;
                    case 3:
                        seri1 = seri + "B";
                        break;
                    case 4:
                        seri1 = seri + "G";
                        break;
                    case 5:
                        seri1 = seri + "M";
                        break;
                    case 6:
                        seri1 = seri + "I";
                        break;
                    case 7:
                        seri1 = seri + "H";
                        break;
                    case 8:
                        seri1 = seri + "S";
                        break;
                    case 9:
                        seri1 = seri + "X";
                        break;
                    case 10:
                        seri1 = seri + "W";
                        break;
                }
                var sp10 = new SqlParameter("p10", seri1);
                var sp11 = new SqlParameter("p11", 1);
                var sp12 = new SqlParameter("p12", "TEMELFATURA");
                var sp13 = new SqlParameter("p13", false);
                var sp14 = new SqlParameter("p14", DateTime.Now);
                int sil = db.Database.ExecuteSqlCommand(query.ToString(), sp1, sp2, sp3, sp4, sp5, sp6, sp7, sp8, sp9, sp10, sp11, sp12, sp13, sp14);
                sayi += sil;
            }
            //foreach (fatura_ayarlari fa in faList)
            //{
            //    string seri1 = "";
            //    query = new StringBuilder();
            //    query.Append("insert into fatura_ayarlari values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12);");
            //    var sp1 = new SqlParameter("p1", kln.vkn_tckn);
            //    var sp2 = new SqlParameter("p2", kln.id);
            //    var sp3 = new SqlParameter("p3", fa.fatura_tipi);
            //    var sp4 = new SqlParameter("p4", 1); //testmi
            //    var sp5 = new SqlParameter("p5", fa.entegrator_tipi);
            //    var sp6 = new SqlParameter("p6", fa.kullanici_adi);
            //    var sp7 = new SqlParameter("p7", fa.sifre);

            //    switch (fa.fatura_tipi)
            //    {
            //        case 1:
            //            seri1 = seri + "A";
            //            break;
            //        case 2:
            //            seri1 = seri + "F";
            //            break;
            //        case 5:
            //            seri1 = seri + "M";
            //            break;
            //        case 6:
            //            seri1 = seri + "I";
            //            break;
            //        case 7:
            //            seri1 = seri + "H";
            //            break;
            //        case 8:
            //            seri1 = seri + "S";
            //            break;
            //    }
            //    var sp8 = new SqlParameter("p8", seri1);
            //    var sp9 = new SqlParameter("p9", fa.seri_kullanilsinmi);
            //    var sp10 = new SqlParameter("p10", fa.giden_fatura_tipi);
            //    var sp11 = new SqlParameter("p11", false);
            //    var sp12 = new SqlParameter("p12", DateTime.Now);
            //    int sil = db.Database.ExecuteSqlCommand(query.ToString(), sp1, sp2, sp3, sp4, sp5, sp6, sp7, sp8, sp9, sp10, sp11, sp12);
            //    sayi += sil;
            //}
            //query.Append("insert into fatura_ayarlari values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12);");
            //sil = db.Database.ExecuteSqlCommand(query.ToString(), new SqlParameter("param1", vkn));
            if (sayi > 0)
                return true;
            else
                return false;
        }
        [ExceptionHandler]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [ExceptionHandler]
        [Authorize]
        public ActionResult KullaniciSil(int id, int? SayfaNo, string SortOrder, string SearchString)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            long vkn = (long)Session["vkn_tckn"];
            long vknAdmin = Utilities.vknAdmin;
            kullanici model = null;
            if (vkn == vknAdmin)
                model = db.kullanicis.FirstOrDefault(x => x.id == id);
            else
                model = db.kullanicis.FirstOrDefault(x => x.id == id && x.vkn_tckn == vkn);
            SortOrder = "last_update_desc";
            ViewBag.SortOrder = SortOrder;
            ViewBag.SearchString = SearchString;
            ViewBag.SayfaNo = SayfaNo;
            model.isdeleted = true;
            db.SaveChanges();
            TempData["Islem"] = "Silindi";
            return View("KullaniciListesi", "Security", model);
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
            return View("_KullaniciKayit", model);
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
                                    klnFirmaOlustur(gelen);
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
        [Authorize]
        [ExceptionHandler]
        public ActionResult KullaniciListesi(int sayfa = 1)
        {
            if (Session["kullanici"] != null && User.IsInRole("1"))
            {
                if (db == null)
                    db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                //var model = from d in db.kullanicis select d;
                long vkn = (long)Session["vkn_tckn"];
                long vknAdmin = Utilities.vknAdmin;
                var model = from s in db.kullanicis
                            where vkn == vknAdmin ? s.vkn_tckn > 0 : s.vkn_tckn == vkn //yonetici ise kendi vkn sine bağlı tüm kullanıcılar listelenir
                            select s;

                //if (!string.IsNullOrEmpty(p))
                //{
                //    long vkn = long.Parse(p);
                //    model = model.Where(a => a.vkn_tckn == vkn);
                //}
                ViewBag.Mesaj = TempData["Islem"];
                return View(model.ToList().ToPagedList(sayfa, 10));
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
        }
        [AllowAnonymous]
        [ExceptionHandler]
        //[ValidateAntiForgeryToken]
        public ActionResult Dogrula(kullanici user)
        {
            if (user.activationcode2 != null && user.activationcode2 != "")
            {
                if (user.activationcode1.Equals(user.activationcode2.Trim()))
                {
                    if (db == null)
                        db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
                    var guncellenecekKayit = db.kullanicis.Find(user.kullanici_mail);
                    user.isdeleted = false; //aktivasyon yapıldı
                    user.last_update = DateTime.Now;
                    user.isactivated = true;
                    db.Entry(guncellenecekKayit).CurrentValues.SetValues(user);
                    db.SaveChanges();
                    TempData["mesaj"] = "Aktivasyon işlemi tamamlandı. Mail adresiniz ve şifreniz ile sisteme giriş yapabilirsiniz.";
                    if (Session["kullanici"] != null && User.IsInRole("1"))
                        return RedirectToAction("KullaniciListesi", "Security");
                    else
                        return RedirectToAction("Login", new { mail = guncellenecekKayit.kullanici_mail });
                }
                else
                {
                    ViewBag.Mesaj = "Aktivasyon kodu hatalı.";
                }
            }
            else
            {
                AktivasyonKoduMailGonder(user); //aktivasyon kodu gonderiliyor
            }
            if (TempData["Mesaj"] != null && TempData["Mesaj"].ToString() != "")
                ViewBag.Mesaj = TempData["Mesaj"];
            TempData.Clear();
            return View(user);
        }
        [AllowAnonymous]
        [ExceptionHandler]
        public ActionResult TekrarDogrula(string mail)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            kullanici user = db.kullanicis.Find(mail);
            //if (user != null)
            //{
            //    if (user.activationcode2 != null && user.activationcode2 != "")
            //    {
            //        if (user.activationcode1.Equals(user.activationcode2))
            //        {
            //            //var guncellenecekKayit = db.kullanicis.Find(user.kullanici_mail);
            //            user.isdeleted = false; //aktivasyon yapıldı
            //            user.last_update = DateTime.Now;
            //            user.isactivated = true;
            //            //db.Entry(guncellenecekKayit).CurrentValues.SetValues(user);
            //            db.SaveChanges();
            //            TempData["mesaj"] = "Aktivasyon işlemi tamamlandı. Mail adresiniz ve şifreniz ile sisteme giriş yapabilirsiniz.";
            //            if (Session["kullanici"] != null && User.IsInRole("1"))
            //                return RedirectToAction("KullaniciListesi", "Security");
            //            else
            //                return RedirectToAction("Login", new { mail = user.kullanici_mail });
            //        }
            //        else
            //        {
            //            ViewBag.Mesaj = "Aktivasyon kodu hatalı.";
            //        }
            //    }
            //    else
            //    {
            //        AktivasyonKoduMailGonder(user); //aktivasyon kodu gonderiliyor
            //    }
            //} else
            //{
            //    ViewBag.Mesaj = "Beklenmedik bir hata oluştu. Lütfen hesabınızla ilgili yazılım destek birimi ile irtibata geçiniz.";
            //    return View();
            //}
            //if (TempData["Mesaj"] != null && TempData["Mesaj"].ToString() != "")
            //    ViewBag.Mesaj = TempData["Mesaj"];
            ////TempData.Clear();
            return RedirectToAction("Dogrula", "Security", user);
        }
        [AllowAnonymous]
        [ExceptionHandler]
        public ActionResult SifremiUnuttum(kullanici gelen, string sifre2, string captcha, string captchaFileName, string kullanici_mail)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            string captcha1 = Session["captchaValue"] == null ? "" : Session["captchaValue"].ToString();
            string mail = Session["kullanici_mail"] == null ? "" : Session["kullanici_mail"].ToString();
            kullanici kln = null;
            if (gelen.kullanici_mail != null && gelen.kullanici_mail != "")
                kln = db.kullanicis.Find(gelen.kullanici_mail);
            else
                return View(gelen);
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
                kln.passattempts = 0;
                db.SaveChanges();
            }
            if (kln != null)
            {
                if (kln.passattempts > 2)
                {
                    if (captcha != null)
                        CaptchaTempResimSil(captchaFileName);
                    CaptchaEkle();
                    //ViewBag.Mesaj = "Çok fazla hatalı deneme yaptınız.";
                }
                else
                {
                    if (Session["kullanici_mail"] != null && Session["kullanici_mail"].ToString() != "")
                    {
                        if ((gelen.sifre == null || gelen.sifre == "") && (sifre2 == null || sifre2 == ""))
                        {
                            if (gelen.resetpasscode == null || gelen.resetpasscode == "")
                            {
                                Session.Add("kullanici_mail", kln.kullanici_mail);
                                SifirlamaKoduMailGonder(kln);
                            }
                            else
                            {
                                Session.Add("resetPassCode", gelen.resetpasscode);
                                if (kln.resetpasscode == gelen.resetpasscode && gelen.resetpasscode.Equals(Session["resetPassCode"].ToString()))
                                {
                                    ViewBag.sifreMesaj = "Yeni şifrenizi giriniz.";
                                }
                                else
                                {
                                    ViewBag.Mesaj = "Şifre sıfırlama kodunu yanlış girdiniz.";
                                    kln.passattempts += 1;
                                    db.SaveChanges();
                                    if (kln.passattempts > 2)
                                        ViewBag.Mesaj = ViewBag.Mesaj + " Çok fazla hatalı giriş yaptınız.";
                                }
                            }
                        }
                        else
                        {
                            string ma = Session["kullanici_mail"].ToString();
                            string pa = Session["resetPassCode"].ToString();
                            if (kln.kullanici_mail != null && kln.kullanici_mail.Equals(ma) && kln.resetpasscode != null && kln.resetpasscode.Equals(pa))
                            {
                                if (kln.sifre != gelen.sifre)
                                {
                                    if (gelen.sifre == sifre2)
                                    {
                                        kln.sifre = sifre2;
                                        kln.passattempts = 0;
                                        db.SaveChanges();
                                        ViewBag.sifreDegistiMesaj = "Şifreniz başarıyla güncellendi.";
                                        Session.Remove("kullanici_mail");
                                        Session.Remove("resetPassCode");
                                    }
                                    else
                                    {
                                        ViewBag.sifreMesaj = "Yeni şifrenizi giriniz.";
                                        ViewBag.Mesaj = "Girdiğiniz iki şifre birbirinden farklı.";
                                    }
                                }
                                else
                                {
                                    ViewBag.sifreMesaj = "Yeni şifrenizi giriniz.";
                                    ViewBag.Mesaj = "Eski şifrenizden farklı bir şifre belirleyiniz.";
                                }
                            }
                            else
                                ViewBag.Mesaj = "Beklenmedik bir hata oluştu. Mail adresinizi kontrol ederek tekrar deneyiniz. MD01";
                        }
                    }
                    else
                        Session.Add("kullanici_mail", gelen.kullanici_mail);
                }
            }
            else
                ViewBag.Mesaj = "Beklenmedik bir hata oluştu. Mail adresinizi kontrol ederek tekrar deneyiniz. MY01";
            return View(gelen);
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
        public void SifirlamaKoduMailGonder(kullanici kln)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == Utilities.vknAdmin); //6666666666 bizim sirket, smtp bilgileri aliniyor 
            SenderInfo sInfo = new SenderInfo
            {
                senderMail = dSirket.emailUserName,
                senderPass = dSirket.emailUserPassword,
                senderDisplayName = dSirket.emailDisplayName,
                senderSmtpServer = dSirket.smtpServerName,
                senderSmtpPort = (int)dSirket.smtpServerPort,
                senderSSL = (bool)dSirket.smtpServerSSLEnable
            };
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            string resetPassCode = Guid.NewGuid().ToString().Substring(0, 5);
            kln.resetpasscode = resetPassCode;
            Session.Add("resetPassCode", resetPassCode);
            db.SaveChanges();
            IslemSonuc iSonuc = util.SendEmail(sInfo, kln.kullanici_mail, "Şifre Sıfırlama", "Şifre Sıfırlama Kodunuz: " + resetPassCode);
            if (iSonuc.hataliMi)
            {
                ViewBag.mailMesaj = "Şifre sıfırlama kodunuz mail adresinize gönderilemedi. Yazılım destek birimini arayınız.";
            }
            else
            {
                ViewBag.mailMesaj = "Şifre sıfırlama kodunuz mail adresinize gönderildi. Mailinize gönderilen Şifre sıfırlama kodunu aşağıdaki alana girin ve Tamam düğmesine basınız.";
            }
        }
        public void AktivasyonKoduMailGonder(kullanici kln)
        {
            if (db == null)
                db = new faturaEntities(); //6666666666 bizim sirket, smtp bilgileri aliniyor 
            firma dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == Utilities.vknAdmin); //6666666666 bizim sirket, smtp bilgileri aliniyor 
            if (dSirket == null) // ilk kez bizim firma kaydı oluşturuluyor.
            {
                StringBuilder query = new StringBuilder();
                query.Append("insert into firma values(@p1,'Bizim Şirket', 'ÜMRANİYE', '', 'Adresi',100,'TÜRKİYE',34,'İSTANBUL',0,'','yasar@msn.com', 'www.enucuzefatura.com', '05555555555', '03163333333','TR01010101010101','TR02020202020202','TR030303030303','notlar',dateadd(day, +30, getdate()),'dene9876543210@gmail.com','your password','E-Fatura Sistemi','smtp.gmail.com',587,1,'','',GETDATE(),0);");
                var sp1 = new SqlParameter("p1", kln.vkn_tckn);
                int sil = db.Database.ExecuteSqlCommand(query.ToString(), sp1);
                dSirket = db.firmas.FirstOrDefault(x => x.vkn_tckn == Utilities.vknAdmin);
            }
            SenderInfo sInfo = new SenderInfo
            {
                senderMail = dSirket.emailUserName,
                senderPass = dSirket.emailUserPassword,
                senderDisplayName = dSirket.emailDisplayName,
                senderSmtpServer = dSirket.smtpServerName,
                senderSmtpPort = (int)dSirket.smtpServerPort,
                senderSSL = (bool)dSirket.smtpServerSSLEnable
            };
            Utilities util = new Utilities(HttpContext.Request.Url.Host.Split('.')[0]);
            IslemSonuc iSonuc = util.SendEmail(sInfo, kln.kullanici_mail, "E-Fatura Aktivasyon", "E-Fatura Sistemi Aktivasyon Kodunuz: " + kln.activationcode1);
            if (iSonuc.hataliMi)
            {
                ViewBag.Mesaj = "Tebrikler, sisteme başarıyla kaydoldunuz.";
                ViewBag.mailMesaj = "Ancak aktivasyon kodunuz mail adresinize gönderilemedi. Yazılım destek birimini arayınız.";
            }
            else
            {
                ViewBag.Mesaj = "Tebrikler, sisteme başarıyla kaydoldunuz. Son adımdasınız.";
                ViewBag.mailMesaj = "Mailinize gönderilen aktivasyon kodunu aşağıdaki alana girin ve Tamam düğmesine basınız.";
            }
        }
    }
}