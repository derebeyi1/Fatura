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
    using System;
    using System.Collections.Generic;
    
    public partial class fatura_gelen_satir
    {
        public int id { get; set; }
        public string guid { get; set; }
        public string stok_adi { get; set; }
        public string birimi { get; set; }
        public decimal miktari { get; set; }
        public string doviz_cinsi { get; set; }
        public Nullable<decimal> doviz_kuru { get; set; }
        public Nullable<decimal> kdv_orani { get; set; }
        public Nullable<decimal> kdv_tutar { get; set; }
        public Nullable<decimal> otv_orani { get; set; }
        public Nullable<decimal> otv_tutar { get; set; }
        public Nullable<decimal> oiv_orani { get; set; }
        public Nullable<decimal> oiv_tutar { get; set; }
        public Nullable<decimal> tevkifat_orani { get; set; }
        public Nullable<decimal> tevkifat_tutar { get; set; }
        public Nullable<decimal> iskonto_orani { get; set; }
        public Nullable<decimal> iskonto_tutar { get; set; }
        public decimal fiyat { get; set; }
        public string satirno { get; set; }
        public string sikayet { get; set; }
        public Nullable<decimal> teslim_adet { get; set; }
        public Nullable<decimal> red_adet { get; set; }
        public string red_kodu { get; set; }
        public string red_ack { get; set; }
        public Nullable<decimal> eksik_adet { get; set; }
        public Nullable<decimal> fazla_adet { get; set; }
        public System.DateTime last_update { get; set; }
    }
}