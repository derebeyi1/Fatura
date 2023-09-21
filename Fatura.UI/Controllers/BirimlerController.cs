using Fatura.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Fatura.UI.Controllers
{
    public class BirimlerController : Controller
    {
        faturaEntities db = null;
        // GET: Birimler
        //[ValidateAntiForgeryToken]
        public ActionResult BirimListele(string p, int sayfa = 1)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            var model = from d in db.birimlers select d;

            if (!string.IsNullOrEmpty(p))
            {
                model = model.Where(a => a.adi.Contains(p));
            }

            return View(model.ToList().ToPagedList(sayfa, 10));
        }

        public ActionResult Sil(int id)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            var silinecekBirim = db.birimlers.FirstOrDefault(x => x.id == id);
            if (silinecekBirim == null)
                return HttpNotFound();

            db.birimlers.Remove(silinecekBirim);
            db.SaveChanges();
            return RedirectToAction("BirimListele");
        }
        public ActionResult Yeni()
        {
            return View("BirimForm");
        }
        [HttpPost]
        public ActionResult Kaydet(birimler birim)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            if (birim.id == 0)//ekleme
            {
                db.birimlers.Add(birim);
            }
            else//güncelle
            {
                //var guncellenecekKullanici = db.birimlers.FirstOrDefault(x => x.id == kullanici.id);
                //if (guncellenecekKullanici == null)
                //{
                //    return HttpNotFound();
                //}
                //guncellenecekKullanici.ad = kullanici.ad;
                //guncellenecekKullanici.soyad = kullanici.soyad;
                db.Entry(birim).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("BirimListele");
        }
        public ActionResult Guncelle(int id)
        {
            if (db == null)
                db = new faturaEntities(HttpContext.Request.Url.Host.Split('.')[0]);
            var model = db.birimlers.FirstOrDefault(x => x.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View("BirimForm", model);
        }
    }
}