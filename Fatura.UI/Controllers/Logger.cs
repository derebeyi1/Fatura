using Fatura.BLL;
using Fatura.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fatura.UI.Controllers
{
    public class Logger
    {
        public static void log(Exception ex, string url, string ipaddress, string controller, string method, string tablo_adi, string bilgi)
        {
            faturaEntities db = null;            
            log l = null;
            string url1 = HttpContext.Current.Request.Url.Host;
            try
            {
                if (HttpContext.Current.Session["vkn_tckn"] != null)
                {
                    if (db == null)
                        db = new faturaEntities(url1.Split('.')[0]);
                    long vkn = (long)HttpContext.Current.Session["vkn_tckn"];
                    kullanici user = (kullanici)HttpContext.Current.Session["kullanici"];
                    long vknAdmin = Utilities.vknAdmin;
                    l = new log();
                    l.vkn_tckn = vkn;
                    l.kullanici_id = user.id;
                    l.ip_adresi = ipaddress;
                    l.hata = ex.Message.ToString();
                    DateTime now = DateTime.Now;
                    l.tarih = now;
                    l.controller = controller;
                    l.method = method;
                    l.tablo_adi = tablo_adi;
                    l.url = url;
                    l.bilgi = bilgi;
                    log log = db.logs.Add(l);
                    int v = db.SaveChanges();
                }
            } 
            catch (Exception)
            { 
            }
        }
    }
}