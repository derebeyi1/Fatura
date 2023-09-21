using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace Fatura.UTL
{
    public class Utilities
    {
        //faturaEntities db = new faturaEntities();
        //public static long vknAdmin = 6666666666; //bizim şirket admin yetkisi
        public static string[] Yetkiler = { "Kullanıcı", "Yönetici", "E-Arşiv", "E-Fatura", "E-Müstahsil", "E-İrsaliye", "E-İhracat", "E-SMM" };
        //public static string[] YetkilerMusteri = { "Kullanıcı", "Yönetici" };
        //public static string[] CariTipleri = { "Ticari", "Müstahsil" }; //0=Ticari, 1=Müstahsil    
        //public static string[] FaturaTuru = { "TICARIFATURA", "TEMELFATURA" }; //0=TICARIFATURA, 1=TEMELFATURA    
        public static string[] EntegratorTipleri = { "Seçiniz", "Uyumsoft", "Veriban", "Mikro", "Kolaysoft", "Paraşüt" };
        ////1=E-Arşiv, 2=E-Fatura, 3=E-Müstahsil, 4=E-İrsaliye, 5=E-İhracat
        //public static string[] IslemTipleri = { "", "Toptan", "Perakende" }; //1=Toptan, 2=Perakende

        public static string[] FaturaTipleri = { "Seçiniz", "E-Arşiv Normal", "E-Fatura Normal", "E-Arşiv İade", "E-Fatura İade", "E-Müstahsil", "E-İrsaliye", "E-İhracat", "E-SMM", "Masraf", "Fiyat Farkı" };
        //public static readonly int EArsivNormal = 1;
        //public static readonly int EFaturaNormal = 2;
        //public static readonly int EArsivIade = 3;
        //public static readonly int EFaturaIade = 4;
        //public static readonly int EMustahsil = 5;
        //public static readonly int EIrsaliye = 6;
        //public static readonly int EIhracat = 7;
        //public static readonly int ESMM = 8;
        //public static readonly int EMasraf = 9;
        //public static readonly int EFiyatFarki = 10;

        //public static readonly int Uyumsoft = 1;
        //public static readonly int Veriban = 2;
        //public static readonly int Mikro = 3;
        //public static readonly int Hizli = 4;
        //public static readonly int Kolaysoft = 5;

        //public static int FaturaIptal = 99; //fatura iptal kodu
        //public class PagedData<T> where T : class
        //{
        //    public IEnumerable<T> Data { get; set; }
        //    public int NumberOfPages { get; set; }
        //    public int CurrentPage { get; set; }
        //}
        //public partial class Role_Yetki
        //{
        //    public int Key { get; set; }
        //    public string Display { get; set; }
        //    public string Checked { get; set; }
        //}
        ////public partial class MyListFatura
        ////{
        ////    public int EntegratorTipi { get; set; }
        ////    public int FaturaTipi { get; set; }
        ////    public List<FaturaItem> FaturaItems { get; set; }
        ////}
        ////public partial class MyListMustahsil
        ////{
        ////    public int EntegratorTipi { get; set; }
        ////    public int FaturaTipi { get; set; }
        ////    public List<MustahsilItem> MustahsilItems { get; set; }
        ////}
        ////public partial class MyListIrsaliye
        ////{
        ////    public int EntegratorTipi { get; set; }
        ////    public int FaturaTipi { get; set; }
        ////    public List<FaturaItem> FaturaItems { get; set; }
        ////}
        ////public partial class MyListSMM
        ////{
        ////    public int EntegratorTipi { get; set; }
        ////    public int FaturaTipi { get; set; }
        ////    public List<FaturaItem> FaturaItems { get; set; }
        ////}
        //public partial class MyListTable
        //{
        //    public string Key { get; set; }
        //    public string Display { get; set; }
        //    public bool Selected { get; set; }
        //}
        //public partial class MyListTableBoolean
        //{
        //    public bool Key { get; set; }
        //    public string Display { get; set; }
        //    public bool Selected { get; set; }
        //}
        //public partial class MyListTableInt
        //{
        //    public int Key { get; set; }
        //    public string Display { get; set; }
        //    public bool Selected { get; set; }
        //}
        //public static string DosyaAdiDuzenle(string dosyaAdi)
        //{
        //    string uzantisizDosyaAdi = Path.GetFileNameWithoutExtension(dosyaAdi);
        //    string uzanti = Path.GetExtension(dosyaAdi);
        //    return AdresDuzenle(uzantisizDosyaAdi) + uzanti;
        //}
        //public static string AdresDuzenle(object a)
        //{
        //    string s = a.ToString();

        //    //s = oncul + id + "-" + s;
        //    if (string.IsNullOrEmpty(s)) //string yok ve ya boş ise true döndürür
        //    {
        //        return "";

        //    }

        //    if (s.Length > 80)
        //    {
        //        s = s.Substring(0, 80); //string den belli karakter alır.
        //    }
        //    s = s.Replace("ş", "s"); //karakter değişimi için kullanılır
        //    s = s.Replace("Ş", "S");
        //    s = s.Replace("ğ", "g");
        //    s = s.Replace("Ğ", "G");
        //    s = s.Replace("İ", "I");
        //    s = s.Replace("ı", "i");
        //    s = s.Replace("ç", "c");
        //    s = s.Replace("Ç", "C");
        //    s = s.Replace("ö", "o");
        //    s = s.Replace("Ö", "O");
        //    s = s.Replace("ü", "u");
        //    s = s.Replace("Ü", "U");
        //    s = s.Replace("'", "");
        //    s = s.Replace("\"", "");
        //    Regex r = new Regex("[^a-zA-Z0-9_-]");
        //    //if (r.IsMatch(s))
        //    s = r.Replace(s, "-");
        //    if (!string.IsNullOrEmpty(s))
        //        while (s.IndexOf("--") > -1)
        //            s = s.Replace("--", "-");
        //    if (s.StartsWith("-")) s = s.Substring(1);
        //    if (s.EndsWith("-")) s = s.Substring(0, s.Length - 1);
        //    return s;
        //}
        ////[ExceptionHandler]
        //public IslemSonuc SendEmail(SenderInfo sInfo, string receiver, string subject, string message)
        //{
        //    IslemSonuc iSonuc = new IslemSonuc();
        //    try
        //    {
        //        var senderEmail = new MailAddress(sInfo.senderMail, sInfo.senderDisplayName);
        //        var receiverEmail = new MailAddress(receiver, "Alıcı");
        //        var password = sInfo.senderPass;
        //        var sub = subject;
        //        var body = message;
        //        var smtp = new SmtpClient
        //        {
        //            Host = sInfo.senderSmtpServer,
        //            Port = sInfo.senderSmtpPort,
        //            EnableSsl = sInfo.senderSSL,
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential(senderEmail.Address, password)
        //        };
        //        using (var mess = new MailMessage(senderEmail, receiverEmail)
        //        {
        //            Subject = subject,
        //            Body = body
        //        })
        //        {
        //            mess.IsBodyHtml = true;
        //            smtp.Send(mess);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        iSonuc.hataliMi = true;
        //        iSonuc.mesaj = ex.Message;
        //        return iSonuc;
        //    }
        //    iSonuc.hataliMi = false;
        //    return iSonuc;
        //}
        //public partial class SenderInfo
        //{
        //    public string senderMail { get; set; }
        //    public string senderPass { get; set; }
        //    public string senderDisplayName { get; set; }
        //    public string senderSmtpServer { get; set; }
        //    public int senderSmtpPort { get; set; }
        //    public bool senderSSL { get; set; }
        //}
        //public partial class IslemSonuc
        //{
        //    public bool hataliMi { get; set; }
        //    public string mesaj { get; set; }
        //}
        //public Random generator;
        //public Random Generator
        //{
        //    get
        //    {
        //        if (this.generator == null)
        //        {
        //            this.generator = new Random();
        //        }
        //        return this.generator;
        //    }
        //}
        //public static List<MyListTableBoolean> getSelectListForBoolean(bool ssl)
        //{
        //    var list = new List<MyListTableBoolean>();
        //    if (ssl)
        //    {
        //        list.Add(new MyListTableBoolean
        //        {
        //            Key = true,
        //            Display = "Evet",
        //            Selected = ssl
        //        });
        //        list.Add(new MyListTableBoolean
        //        {
        //            Key = false,
        //            Display = "Hayır",
        //            Selected = !ssl
        //        });
        //    }
        //    else
        //    {
        //        list.Add(new MyListTableBoolean
        //        {
        //            Key = false,
        //            Display = "Hayır",
        //            Selected = !ssl
        //        });
        //        list.Add(new MyListTableBoolean
        //        {
        //            Key = true,
        //            Display = "Evet",
        //            Selected = ssl
        //        });
        //    }
        //    return list;
        //}
        //public static List<MyListTable> getSelectListForCariTipi(int selectedID)
        //{
        //    List<MyListTable> list = new List<MyListTable>();
        //    for (int i = 0; i < CariTipleri.Length; i++)
        //    {
        //        if (i == selectedID)
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = i.ToString(),
        //                Display = CariTipleri[i],
        //                Selected = true
        //            });
        //        }
        //        else
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = i.ToString(),
        //                Display = CariTipleri[i],
        //                Selected = false
        //            });
        //        }
        //    }
        //    return list;
        //}
        //public static List<MyListTable> getSelectListForFaturaTuru(string selectedID)
        //{
        //    List<MyListTable> list = new List<MyListTable>();
        //    for (int i = 0; i < FaturaTuru.Length; i++)
        //    {
        //        if (FaturaTuru[i] == selectedID)
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = FaturaTuru[i],
        //                Display = FaturaTuru[i],
        //                Selected = true
        //            });
        //        }
        //        else
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = FaturaTuru[i],
        //                Display = FaturaTuru[i],
        //                Selected = false
        //            });
        //        }
        //    }
        //    return list;
        //}
        //public static List<MyListTable> getSelectListForFaturaTipi(int selectedID)
        //{
        //    List<MyListTable> list = new List<MyListTable>();
        //    //list.Add(new MyListTable
        //    //{
        //    //    Key = "0",
        //    //    Display = "Seçiniz",
        //    //    Selected = false
        //    //});
        //    for (int i = 1; i < FaturaTipleri.Length; i++)
        //    {
        //        if (i == selectedID)
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = (i).ToString(),
        //                Display = FaturaTipleri[i],
        //                Selected = true
        //            });
        //        }
        //        else
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = (i).ToString(),
        //                Display = FaturaTipleri[i],
        //                Selected = false
        //            });
        //        }
        //    }
        //    return list;
        //}
        //public static List<MyListTable> getSelectListForEntegratorTipi(int selectedID)
        //{
        //    List<MyListTable> list = new List<MyListTable>();
        //    //list.Add(new MyListTable
        //    //{
        //    //    Key = "0",
        //    //    Display = "Seçiniz",
        //    //    Selected = false
        //    //});
        //    for (int i = 1; i < EntegratorTipleri.Length; i++)
        //    {
        //        if (i == selectedID)
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = (i).ToString(),
        //                Display = EntegratorTipleri[i],
        //                Selected = true
        //            });
        //        }
        //        else
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = (i).ToString(),
        //                Display = EntegratorTipleri[i],
        //                Selected = false
        //            });
        //        }
        //    }
        //    return list;
        //}
        //public static List<MyListTable> getSelectListForIslemTipi(int selectedID)
        //{
        //    List<MyListTable> list = new List<MyListTable>();
        //    for (int i = 1; i < IslemTipleri.Length; i++)
        //    {
        //        if (i == selectedID)
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = i.ToString(),
        //                Display = IslemTipleri[i],
        //                Selected = true
        //            });
        //        }
        //        else
        //        {
        //            list.Add(new MyListTable
        //            {
        //                Key = i.ToString(),
        //                Display = IslemTipleri[i],
        //                Selected = false
        //            });
        //        }
        //    }
        //    return list;
        //}
        ////public List<MyListTable> getSelectListForUlkeIlIlce(int kod, int ustkod)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var ulkeler = (from c in db.ulke_il_ilce
        ////                   where c.isdeleted == false && c.ustkod == ustkod
        ////                   select c).ToList();
        ////    foreach (ulke_il_ilce ulke in ulkeler)
        ////    {
        ////        bool selected = false;
        ////        if (kod == ulke.kod)
        ////            selected = true;
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = ulke.kod.ToString(),
        ////            Display = ulke.adi.ToString(),
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForUlkeIlIlce(int kod, int ustkod)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var ulkeler = (from c in db.ulke_il_ilce
        ////                   where c.isdeleted == false && c.ustkod == ustkod
        ////                   select c).ToList();
        ////    list.Add(new MyListTable
        ////    {
        ////        Key = "0",
        ////        Display = "Seçiniz",
        ////        Selected = false
        ////    });
        ////    foreach (ulke_il_ilce ulke in ulkeler)
        ////    {
        ////        bool selected = false;
        ////        if (kod == ulke.kod)
        ////            selected = true;
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = ulke.kod.ToString(),
        ////            Display = ulke.adi.ToString(),
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForFaturaAlias(string alias, long vkn)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var faturaAliasList = db.mukellefs.Where(x => x.VergiKimlikNo == vkn).ToList();
        ////    //var cariAlias = db.caris.Where(x => x.vergi_numarasi == vkn && x.vkn_tckn == vkn).FirstOrDefault();
        ////    //List<cari_adres> cariAdresAliasList = null;
        ////    //if (cariAlias != null)
        ////    //    cariAdresAliasList = db.cari_adres.Where(x => x.cari_id == cariAlias.id).ToList();
        ////    bool selected = false;
        ////    if (faturaAliasList != null)
        ////    {
        ////        foreach (mukellef muk in faturaAliasList)
        ////        {
        ////            selected = false;
        ////            if (alias == muk.PostBoxAlias.Trim())
        ////                selected = true;
        ////            list.Add(new MyListTable
        ////            {
        ////                Key = muk.PostBoxAlias.Trim(),
        ////                Display = muk.PostBoxAlias.Trim(),
        ////                Selected = selected
        ////            });
        ////        }
        ////    }
        //    //if (cariAlias != null && cariAlias.email != null && cariAlias.email != "")
        //    //{
        //    //    if (alias == cariAlias.email.Trim())
        //    //        selected = true;
        //    //    list.Add(new MyListTable
        //    //    {
        //    //        Key = cariAlias.email.Trim(),
        //    //        Display = cariAlias.email.Trim(),
        //    //        Selected = false
        //    //    });
        //    //}
        //    //if (cariAdresAliasList != null)
        //    //{
        //    //    foreach (cari_adres ca in cariAdresAliasList)
        //    //    {
        //    //        if (ca != null && ca.email != null && ca.email != "")
        //    //        {
        //    //            selected = false;
        //    //            if (alias == ca.email.Trim())
        //    //                selected = true;
        //    //            list.Add(new MyListTable
        //    //            {
        //    //                Key = ca.email.Trim(),
        //    //                Display = ca.email.Trim(),
        //    //                Selected = selected
        //    //            });
        //    //        }
        //    //    }
        //    //}
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForSatisIban(string iban, long vkn)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var firma = db.firmas.Find(vkn);
        ////    List<string> satisIbanList = new List<string>();
        ////    if ((firma.iban1 ?? "") != "")
        ////        satisIbanList.Add(firma.iban1);
        ////    if ((firma.iban2 ?? "") != "")
        ////        satisIbanList.Add(firma.iban2);
        ////    if ((firma.iban3 ?? "") != "")
        ////        satisIbanList.Add(firma.iban3);
        ////    //list.Add(new MyListTable
        ////    //{
        ////    //    Key = "0",
        ////    //    Display = "Seçiniz",
        ////    //    Selected = false
        ////    //});
        ////    foreach (string str in satisIbanList)
        ////    {
        ////        bool selected = false;
        ////        if (iban == str)
        ////            selected = true;
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = str.Trim(),
        ////            Display = str.Trim(),
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForVergiler(decimal oran, int ustkod)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var vergilers = (from c in db.vergis
        ////                     where c.isdeleted == false && c.ustkod == ustkod
        ////                     select c).ToList();
        ////    list.Add(new MyListTable
        ////    {
        ////        Key = "0.00",
        ////        Display = "Seçiniz",
        ////        Selected = false
        ////    });
        ////    foreach (vergi vergi in vergilers)
        ////    {
        ////        bool selected = false;
        ////        if ((int)oran == vergi.oran)
        ////            selected = true;
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = vergi.oran.ToString(),
        ////            Display = vergi.tanim,
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForDoviz(int selectedID)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var dovizlers = (from c in db.dovizlers
        ////                     where c.isdeleted == false
        ////                     select c).ToList();
        ////    //list.Add(new MyListTable
        ////    //{
        ////    //    Key = "0",
        ////    //    Display = "Seçiniz",
        ////    //    Selected = false
        ////    //});
        ////    foreach (dovizler doviz in dovizlers)
        ////    {
        ////        bool selected = false;
        ////        if (selectedID == doviz.id)
        ////            selected = true;
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = doviz.id.ToString(),
        ////            Display = doviz.kod.ToString(),
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        ////public List<MyListTable> getSelectListForFiyatTipi(int selectedID)
        ////{
        ////    var list = new List<MyListTable>();
        ////    var fiyat_tipleris = (from c in db.fiyat_tipleri
        ////                          where c.isdeleted == false
        ////                          select c).ToList();
        ////    list.Add(new MyListTable
        ////    {
        ////        Key = "0",
        ////        Display = "Seçiniz",
        ////        Selected = false
        ////    });
        ////    foreach (fiyat_tipleri fiyat in fiyat_tipleris)
        ////    {
        ////        bool selected = false;
        ////        if (selectedID == fiyat.id)
        ////            selected = true;
        ////        var kdvDahilMi = fiyat.kdv_dahilmi == true ? "KDV Dahil" : "KDV Dahil Değil";
        ////        list.Add(new MyListTable
        ////        {
        ////            Key = fiyat.kod.ToString(),
        ////            Display = fiyat.adi.ToString() + " (" + kdvDahilMi + ")",
        ////            Selected = selected
        ////        });
        ////    }
        ////    return list;
        ////}
        //public static List<Role_Yetki> getCheckBoxForYetkiler(string gelen)
        //{
        //    List<Role_Yetki> list = new List<Role_Yetki>();
        //    char[] separator = new char[] { ',' };
        //    for (int i = 0; i < Yetkiler.Length; i++)
        //    {
        //        string secildi = "";
        //        if (gelen.Contains(i.ToString()))
        //            secildi = "checked";
        //        list.Add(new Role_Yetki
        //        {
        //            Key = i,
        //            Display = Yetkiler[i],
        //            Checked = secildi
        //        });
        //    }
        //    return list;
        //}
        //public static List<Role_Yetki> getCheckBoxForYetkilerMusteri(string gelen)
        //{
        //    List<Role_Yetki> list = new List<Role_Yetki>();
        //    char[] separator = new char[] { ',' };
        //    for (int i = 0; i < YetkilerMusteri.Length; i++)
        //    {
        //        string secildi = "";
        //        if (gelen.Contains(i.ToString()))
        //            secildi = "checked";
        //        list.Add(new Role_Yetki
        //        {
        //            Key = i,
        //            Display = YetkilerMusteri[i],
        //            Checked = secildi
        //        });
        //    }
        //    return list;
        //}
        ////public List<MyListTable> getSelectListForBirimler(int selectedID)
        ////{
        ////    var list = new List<MyListTable>();
        ////    List<birimler> birimlers = (from c in db.birimlers
        ////                                where c.isdeleted == false
        ////                                select c).ToList();
        ////    //list.Add(new MyListTable
        ////    //{
        ////    //    Key = "0",
        ////    //    Display = "Seçiniz",
        ////    //    Selected = false
        ////    //});
        ////    if (selectedID == 0)
        ////    {
        ////        selectedID = 1; //Adet
        ////    }

        ////    foreach (birimler birim in birimlers)
        ////    {
        ////        bool selected = false;
        ////        if (selectedID == birim.id)
        ////            selected = true;
        ////        list.Add(new MyListTable { Key = birim.id.ToString(), Display = birim.kod.ToString(), Selected = selected });
        ////    }
        ////    return list;
        ////}

        //public class TreeNode
        //{
        //    public int CategoryId { get; set; }
        //    public string CategoryName { get; set; }

        //    public TreeNode ParentCategory { get; set; }
        //    public List<TreeNode> Children { get; set; }
        //}
        ////public static string ToEnglishString(this string text)  //string extension
        ////{
        ////    Encoding iso = Encoding.GetEncoding("Cyrillic");
        ////    Encoding utf8 = Encoding.UTF8;
        ////    byte[] utfBytes = utf8.GetBytes(text);
        ////    byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
        ////    string name = iso.GetString(isoBytes);
        ////    return name;
        ////}

        ////public static string ToJson(object obj)
        ////{
        ////    JavaScriptSerializer serializer = new JavaScriptSerializer();
        ////    //serializer.MaxJsonLength = 2147483644;
        ////    serializer.MaxJsonLength = int.MaxValue;
        ////    return serializer.Serialize(obj);
        ////}
        //public class dovizKuruNotu
        //{
        //    public decimal kur_carpani { get; set; }
        //    public string doviz { get; set; }
        //    public List<string> stok_adi { get; set; }
        //}
        //public static DataTable ToDataTable<T>(List<T> items)
        //{
        //    var tb = new DataTable(typeof(T).Name);

        //    System.Reflection.PropertyInfo[] props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        //    foreach (var prop in props)
        //    {
        //        tb.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    }

        //    //dt.Columns.Add(pi.Name, Nullable.GetUnderlyingType(
        //    //    pi.PropertyType) ?? pi.PropertyType);


        //    foreach (var item in items)
        //    {
        //        var values = new object[props.Length];
        //        for (var i = 0; i < props.Length; i++)
        //        {
        //            values[i] = props[i].GetValue(item, null);
        //        }

        //        tb.Rows.Add(values);
        //    }

        //    return tb;
        //}
        //public List<MyListTable> TeslimSekliListesi()
        //{
        //    var list = new List<MyListTable>();
        //    list.Add(new MyListTable { Key = "", Display = "Seçiniz", Selected = false });
        //    list.Add(new MyListTable { Key = "CFR", Display = "CFR Maliyet ve Navlun", Selected = false });
        //    list.Add(new MyListTable { Key = "CIF", Display = "CIF Maliyet, Sigorta ve Navlun", Selected = false });
        //    list.Add(new MyListTable { Key = "CIP", Display = "CIP Taşıma ve Sigorta Ücreti", Selected = false });
        //    list.Add(new MyListTable { Key = "CPT", Display = "CPT Ödenen Taşıma Ücreti", Selected = false });
        //    list.Add(new MyListTable { Key = "DAF", Display = "DAF Sınırdan Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "DAP", Display = "DAP Belirlenen Yerde Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "DAT", Display = "DAT Terminalde Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "DDP", Display = "DDP Ödenen Gümrük Ücreti", Selected = false });
        //    list.Add(new MyListTable { Key = "DDU", Display = "DDU Ödenmeyen Gümrük Ücreti", Selected = false });
        //    list.Add(new MyListTable { Key = "DEQ", Display = "DEQ Rıhtımdan Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "DES", Display = "DES Gemkoden Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "EXW", Display = "EXW Fabrikadan Teslim", Selected = false });
        //    list.Add(new MyListTable { Key = "FAS", Display = "FAS Gemkode  Ücretsiz", Selected = false });
        //    list.Add(new MyListTable { Key = "FCA", Display = "FCA Ücretsiz Taşıma", Selected = false });
        //    list.Add(new MyListTable { Key = "FOB", Display = "FOB Güvertede Teslim", Selected = false });
        //    return list;
        //}
        //public List<MyListTable> getSelectListForTeslimSekli(string selectedID)
        //{
        //    var list = TeslimSekliListesi();
        //    foreach (MyListTable item in list)
        //    {
        //        if (item.Key == selectedID)
        //            item.Selected = true;
        //    }
        //    return list;
        //}
        //public string getTeslimSekliAck(string selectedID)
        //{
        //    var list = TeslimSekliListesi();
        //    foreach (MyListTable item in list)
        //    {
        //        if (item.Key == selectedID)
        //            return item.Display;
        //    }
        //    return "";
        //}
        //public List<MyListTableInt> GonderilmeSekliListesi()
        //{
        //    var list = new List<MyListTableInt>();
        //    list.Add(new MyListTableInt { Key = 0, Display = "Seçiniz", Selected = false });
        //    list.Add(new MyListTableInt { Key = 1, Display = "1 Deniz Taşımacılığı", Selected = false });
        //    list.Add(new MyListTableInt { Key = 2, Display = "2 Demiryolu Taşımacılığı", Selected = false });
        //    list.Add(new MyListTableInt { Key = 3, Display = "3 Karayolu Taşımacılığı", Selected = false });
        //    list.Add(new MyListTableInt { Key = 4, Display = "4 Hava Taşımacılığı", Selected = false });
        //    list.Add(new MyListTableInt { Key = 5, Display = "5 Posta", Selected = false });
        //    list.Add(new MyListTableInt { Key = 6, Display = "6 Kombine Taşımacılık", Selected = false });
        //    list.Add(new MyListTableInt { Key = 7, Display = "7 Sabit Nakliyat", Selected = false });
        //    list.Add(new MyListTableInt { Key = 8, Display = "8 Ülke İçi Su Taşımacılığı", Selected = false });
        //    list.Add(new MyListTableInt { Key = 9, Display = "9 Uygun Olmayan Taşıma Şekli", Selected = false });
        //    return list;
        //}
        //public List<MyListTableInt> getSelectListForGonderilmeSekli(int selectedID)
        //{
        //    var list = GonderilmeSekliListesi();
        //    foreach (MyListTableInt item in list)
        //    {
        //        if (item.Key == selectedID)
        //            item.Selected = true;
        //    }
        //    return list;
        //}
        //public string getGonderilmeSekliAck(int selectedID)
        //{
        //    var list = GonderilmeSekliListesi();
        //    foreach (MyListTableInt item in list)
        //    {
        //        if (item.Key == selectedID)
        //            return item.Display;
        //    }
        //    return "";
        //}
        //public List<MyListTable> KapCinsiListesi()
        //{
        //    var list = new List<MyListTable>();
        //    list.Add(new MyListTable { Key = "", Display = "Seçiniz", Selected = false });
        //    list.Add(new MyListTable { Key = "AE", Display = "AE AEROSOL", Selected = false });
        //    list.Add(new MyListTable { Key = "AM", Display = "AM KORUMALI AMPUL", Selected = false });
        //    list.Add(new MyListTable { Key = "AT", Display = "AT PÜSKÜRGEÇ", Selected = false });
        //    list.Add(new MyListTable { Key = "BA", Display = "BA VARİL", Selected = false });
        //    list.Add(new MyListTable { Key = "BB", Display = "BB BOBİN", Selected = false });
        //    list.Add(new MyListTable { Key = "BC", Display = "BC ŞİŞE KASASI", Selected = false });
        //    list.Add(new MyListTable { Key = "BD", Display = "BD TAHTA", Selected = false });
        //    list.Add(new MyListTable { Key = "BE", Display = "BE BOHÇA", Selected = false });
        //    list.Add(new MyListTable { Key = "BF", Display = "BF KORUMASIZ BALON", Selected = false });
        //    list.Add(new MyListTable { Key = "BG", Display = "BG ÇANTA", Selected = false });
        //    list.Add(new MyListTable { Key = "BI", Display = "BI KAP", Selected = false });
        //    list.Add(new MyListTable { Key = "BJ", Display = "BJ KOVA", Selected = false });
        //    list.Add(new MyListTable { Key = "BK", Display = "BK SEPET", Selected = false });
        //    list.Add(new MyListTable { Key = "BL", Display = "BL SIKIŞTIRILMIŞ BALYA", Selected = false });
        //    list.Add(new MyListTable { Key = "BN", Display = "BN SIKIŞTIRILMAMIŞ BALYA", Selected = false });
        //    list.Add(new MyListTable { Key = "BO", Display = "BO KORUMASIZ SİLİNDİRİK ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "BP", Display = "BP KORUMALI BALON", Selected = false });
        //    list.Add(new MyListTable { Key = "BQ", Display = "BQ KORUMALI SİLİNDİRİK ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "BR", Display = "BR ÇUBUK", Selected = false });
        //    list.Add(new MyListTable { Key = "BS", Display = "BS KORUMASIZ SOĞAN ŞEKLİNDE ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "BT", Display = "BT CİVATA", Selected = false });
        //    list.Add(new MyListTable { Key = "BU", Display = "BU BÜYÜK FIÇI", Selected = false });
        //    list.Add(new MyListTable { Key = "BV", Display = "BV KORUMALI SOĞAN ŞEKLİNDE ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "BX", Display = "BX KUTU", Selected = false });
        //    list.Add(new MyListTable { Key = "CA", Display = "CA DİKDÖRTGEN MADENİ KAP", Selected = false });
        //    list.Add(new MyListTable { Key = "CB", Display = "CB BİRA KASASI", Selected = false });
        //    list.Add(new MyListTable { Key = "CC", Display = "CC YAYIK", Selected = false });
        //    list.Add(new MyListTable { Key = "CE", Display = "CE BALIK SEPETİ", Selected = false });
        //    list.Add(new MyListTable { Key = "CF", Display = "CF SANDIK", Selected = false });
        //    list.Add(new MyListTable { Key = "CG", Display = "CG KAFES", Selected = false });
        //    list.Add(new MyListTable { Key = "CH", Display = "CH SANDIK", Selected = false });
        //    list.Add(new MyListTable { Key = "CJ", Display = "CJ TABUT", Selected = false });
        //    list.Add(new MyListTable { Key = "CK", Display = "CK AHŞAP VARİL", Selected = false });
        //    list.Add(new MyListTable { Key = "CL", Display = "CL KANGAL", Selected = false });
        //    list.Add(new MyListTable { Key = "CN", Display = "CN KONTEYNER", Selected = false });
        //    list.Add(new MyListTable { Key = "CO", Display = "CO KORUMASIZ DAMACANA", Selected = false });
        //    list.Add(new MyListTable { Key = "CP", Display = "CP KORUMALI DAMACANA", Selected = false });
        //    list.Add(new MyListTable { Key = "CR", Display = "CR KASA", Selected = false });
        //    list.Add(new MyListTable { Key = "CT", Display = "CT MUKAVVA KUTU", Selected = false });
        //    list.Add(new MyListTable { Key = "CU", Display = "CU FİNCAN", Selected = false });
        //    list.Add(new MyListTable { Key = "CV", Display = "CV KAPALI", Selected = false });
        //    list.Add(new MyListTable { Key = "CX", Display = "CX SİLİNDİRİK TENEKE KUTU ", Selected = false });
        //    list.Add(new MyListTable { Key = "CY", Display = "CY SİLİNDİRİK", Selected = false });
        //    list.Add(new MyListTable { Key = "CZ", Display = "CZ ÇATIR BEZİ", Selected = false });
        //    list.Add(new MyListTable { Key = "DJ", Display = "DJ KORUMALI HASIR BÜYÜK ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "DP", Display = "DP KORUMASIZ HASIRLI BÜYÜK ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "DR", Display = "DR DAVUL", Selected = false });
        //    list.Add(new MyListTable { Key = "EN", Display = "EN ZARF", Selected = false });
        //    list.Add(new MyListTable { Key = "FC", Display = "FC MEYVE KASASI", Selected = false });
        //    list.Add(new MyListTable { Key = "FD", Display = "FD ÇERÇEVELİ KASA", Selected = false });
        //    list.Add(new MyListTable { Key = "FI", Display = "FI UFAK YAĞ FIÇISI", Selected = false });
        //    list.Add(new MyListTable { Key = "FL", Display = "FL DAR BOYUNLU KÜÇÜK ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "FO", Display = "FO KÜÇÜK SANDIK", Selected = false });
        //    list.Add(new MyListTable { Key = "FP", Display = "FP FOTOĞRAF FİLMLERİ PAKETİ", Selected = false });
        //    list.Add(new MyListTable { Key = "FR", Display = "FR ÇERÇEVE", Selected = false });
        //    list.Add(new MyListTable { Key = "GB", Display = "GB GAZ ŞİŞESİ", Selected = false });
        //    list.Add(new MyListTable { Key = "GE", Display = "GE GEMİCİ SANDIĞI", Selected = false });
        //    list.Add(new MyListTable { Key = "GI", Display = "GI KİRİŞ", Selected = false });
        //    list.Add(new MyListTable { Key = "HG", Display = "HG BÜYÜK FIÇI (250 LT LİK)", Selected = false });
        //    list.Add(new MyListTable { Key = "HR", Display = "HR KAPAKLI SEPET", Selected = false });
        //    list.Add(new MyListTable { Key = "JC", Display = "JC DİKDÖRTGEN BİDON (20LT LİK)", Selected = false });
        //    list.Add(new MyListTable { Key = "JG", Display = "JG SÜRAHİ", Selected = false });
        //    list.Add(new MyListTable { Key = "JR", Display = "JR KAVANOZ", Selected = false });
        //    list.Add(new MyListTable { Key = "JT", Display = "JT JÜT (KENEVİR) TORBA", Selected = false });
        //    list.Add(new MyListTable { Key = "JY", Display = "JY SİLİNDİRİK BİDON (20 LT LİK)", Selected = false });
        //    list.Add(new MyListTable { Key = "KG", Display = "KG KÜÇÜK FIÇI", Selected = false });
        //    list.Add(new MyListTable { Key = "LG", Display = "LG KÜTÜK", Selected = false });
        //    list.Add(new MyListTable { Key = "MB", Display = "MB ÇOK GÖZLÜ ÇANTA", Selected = false });
        //    list.Add(new MyListTable { Key = "MT", Display = "MT HASIR", Selected = false });
        //    list.Add(new MyListTable { Key = "MX", Display = "MX KİBRİT KUTUSU", Selected = false });
        //    list.Add(new MyListTable { Key = "NE", Display = "NE PAKETLENMEMİŞ VEYA AMBALAJLANMAMIŞ", Selected = false });
        //    list.Add(new MyListTable { Key = "NS", Display = "NS YUVA", Selected = false });
        //    list.Add(new MyListTable { Key = "NT", Display = "NT AĞ", Selected = false });
        //    list.Add(new MyListTable { Key = "PA", Display = "PA PAKET", Selected = false });
        //    list.Add(new MyListTable { Key = "PC", Display = "PC KOLİ", Selected = false });
        //    list.Add(new MyListTable { Key = "PG", Display = "PG TABLA", Selected = false });
        //    list.Add(new MyListTable { Key = "PH", Display = "PH İBRİK", Selected = false });
        //    list.Add(new MyListTable { Key = "PI", Display = "PI BORU", Selected = false });
        //    list.Add(new MyListTable { Key = "PK", Display = "PK AMBALAJ", Selected = false });
        //    list.Add(new MyListTable { Key = "PL", Display = "PL GERDEL", Selected = false });
        //    list.Add(new MyListTable { Key = "PN", Display = "PN KALAS", Selected = false });
        //    list.Add(new MyListTable { Key = "PO", Display = "PO KESE", Selected = false });
        //    list.Add(new MyListTable { Key = "PT", Display = "PT ÇÖMLEK", Selected = false });
        //    list.Add(new MyListTable { Key = "PU", Display = "PU TABLA PAKETİ - TABLA", Selected = false });
        //    list.Add(new MyListTable { Key = "RD", Display = "RD SOPA", Selected = false });
        //    list.Add(new MyListTable { Key = "RG", Display = "RG HALKA (ÇEMBER)", Selected = false });
        //    list.Add(new MyListTable { Key = "RL", Display = "RL MAKARA", Selected = false });
        //    list.Add(new MyListTable { Key = "RO", Display = "RO RULO", Selected = false });
        //    list.Add(new MyListTable { Key = "SA", Display = "SA ÇUVAL", Selected = false });
        //    list.Add(new MyListTable { Key = "SC", Display = "SC DAR KASA", Selected = false });
        //    list.Add(new MyListTable { Key = "SD", Display = "SD İĞ", Selected = false });
        //    list.Add(new MyListTable { Key = "SE", Display = "SE GEMİCİ SANDIĞI", Selected = false });
        //    list.Add(new MyListTable { Key = "SH", Display = "SH KOKU YASTIĞI", Selected = false });
        //    list.Add(new MyListTable { Key = "SK", Display = "SK İSKELET KASA", Selected = false });
        //    list.Add(new MyListTable { Key = "SM", Display = "SM SAC", Selected = false });
        //    list.Add(new MyListTable { Key = "ST", Display = "ST TABAKA", Selected = false });
        //    list.Add(new MyListTable { Key = "SU", Display = "SU BAVUL", Selected = false });
        //    list.Add(new MyListTable { Key = "SW", Display = "SW ÇEKME - SARMA", Selected = false });
        //    list.Add(new MyListTable { Key = "TB", Display = "TB GERDEL", Selected = false });
        //    list.Add(new MyListTable { Key = "TC", Display = "TC ÇAY SANDIĞI", Selected = false });
        //    list.Add(new MyListTable { Key = "TD", Display = "TD AÇILIR KAPANIR TÜP - PORTATİF TÜP", Selected = false });
        //    list.Add(new MyListTable { Key = "TK", Display = "TK DİKDÖRTGEN TANK", Selected = false });
        //    list.Add(new MyListTable { Key = "TN", Display = "TN TENEKE KUTU", Selected = false });
        //    list.Add(new MyListTable { Key = "TO", Display = "TO BÜYÜK FIÇI (250 GALONLUK)", Selected = false });
        //    list.Add(new MyListTable { Key = "TR", Display = "TR BÜYÜK EŞYA SANDIĞI", Selected = false });
        //    list.Add(new MyListTable { Key = "TS", Display = "TS KİRİŞ", Selected = false });
        //    list.Add(new MyListTable { Key = "TU", Display = "TU KÜP", Selected = false });
        //    list.Add(new MyListTable { Key = "TY", Display = "TY SİLİNDİRİK TANK", Selected = false });
        //    list.Add(new MyListTable { Key = "VA", Display = "VA TEKNE", Selected = false });
        //    list.Add(new MyListTable { Key = "VG", Display = "VG HACİM GAZ (1031 m bar ve 15C)", Selected = false });
        //    list.Add(new MyListTable { Key = "VI", Display = "VI CAM ŞİŞE", Selected = false });
        //    list.Add(new MyListTable { Key = "VP", Display = "VP VAKUMLU PAKET", Selected = false });
        //    list.Add(new MyListTable { Key = "VQ", Display = "VQ HACİM SIVI HALE GETİRİLMİŞ GAZ (ANORMAL ISI-BASINÇ)", Selected = false });
        //    list.Add(new MyListTable { Key = "VR", Display = "VR HACİM KATI GRANÜL PARÇACIKLARI (TANELER)", Selected = false });
        //    list.Add(new MyListTable { Key = "VY", Display = "VY HACİM KATI İNCE PARÇACIKLARI (TOZ)", Selected = false });
        //    return list;
        //}
        //public List<MyListTable> getSelectListForKapCinsi(string selectedID)
        //{
        //    var list = KapCinsiListesi();
        //    foreach (MyListTable item in list)
        //    {
        //        if (item.Key == selectedID)
        //            item.Selected = true;
        //    }
        //    return list;
        //}
        //public string getKapCinsiAck(string selectedID)
        //{
        //    var list = KapCinsiListesi();
        //    foreach (MyListTable item in list)
        //    {
        //        if (item.Key == selectedID)
        //            return item.Display;
        //    }
        //    return "";
        //}
        //public static string RastgeleStringGetir(int length)
        //{
        //    StringBuilder str_build = new StringBuilder();
        //    Random random = new Random();
        //    char letter;
        //    for (int i = 0; i < length; i++)
        //    {
        //        double flt = random.NextDouble();
        //        int shift = Convert.ToInt32(Math.Floor(25 * flt));
        //        letter = Convert.ToChar(shift + 65);
        //        str_build.Append(letter);
        //    }
        //    System.Console.WriteLine(str_build.ToString());
        //    return str_build.ToString();
        //}
        ////public static void log(Exception ex, string url, string ipaddress, string controller, string method, string tablo_adi, string bilgi)
        ////{
        ////    faturaEntities db = new faturaEntities();
        ////    long vkn = (long)HttpContext.Current.Session["vkn_tckn"];
        ////    kullanici user = (kullanici)HttpContext.Current.Session["kullanici"];
        ////    long vknAdmin = Utilities.vknAdmin;
        ////    log l = new log();
        ////    l.vkn_tckn = vkn;
        ////    l.kullanici_id = user.id;
        ////    l.ip_adresi = ipaddress;
        ////    l.hata = ex.Message.ToString();
        ////    DateTime now = DateTime.Now;
        ////    l.tarih = now;
        ////    l.controller = controller;
        ////    l.method = method;
        ////    l.tablo_adi = tablo_adi;
        ////    l.url = url;
        ////    l.bilgi = bilgi;
        ////    log log = db.logs.Add(l);
        ////    int v = db.SaveChanges();
        ////}
        //public static bool MailAdresiFormataUygunMu(string email)
        //{
        //    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        //    Match match = regex.Match(email ?? "");
        //    return match.Success;
        //}
    }
}