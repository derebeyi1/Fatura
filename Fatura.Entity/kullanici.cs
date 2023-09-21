//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fatura.Entity
{
    using Fatura.UTL;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class kullanici
    {
        public int id { get; set; }
        public string kullanici_mail { get; set; }
        public string sifre { get; set; }
        public Nullable<long> vkn_tckn { get; set; }
        public string rol_yetki { get; set; }
        public string ad { get; set; }
        public string soyad { get; set; }
        public string cep_no { get; set; }
        public string telefon_no { get; set; }
        public Nullable<System.DateTime> dogum_tarihi { get; set; }
        public Nullable<System.DateTime> kayit_tarihi { get; set; }
        public Nullable<System.DateTime> last_update { get; set; }
        public string activationcode1 { get; set; }
        public string activationcode2 { get; set; }
        public Nullable<bool> isactivated { get; set; }
        public string resetpasscode { get; set; }
        public Nullable<int> passattempts { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public string rol_yetki_str
        {
            get
            {
                char[] sep = new char[] { ',' };
                string[] yetkiler = rol_yetki.Split(sep);
                var uzn = yetkiler.Length - 1;
                string yetkistr = "";
                for (int i = 0; i < uzn; i++)
                {
                    yetkistr = yetkistr + Utilities.Yetkiler[int.Parse(yetkiler[i])] + ",";
                }
                return yetkistr;
            }
            set { }
        }
        //public string rol_yetki_musteri_str
        //{
        //    get
        //    {
        //        char[] sep = new char[] { ',' };
        //        string[] yetkiler = rol_yetki.Split(sep);
        //        var uzn = yetkiler.Length - 1;
        //        string yetkistr = "";
        //        for (int i = 0; i < uzn; i++)
        //        {
        //            yetkistr = yetkistr + Utilities.YetkilerMusteri[i] + ",";
        //        }
        //        return yetkistr;
        //    }
        //    set { }
        //}
        public SelectList DropDownList { get; set; }
    }
}
