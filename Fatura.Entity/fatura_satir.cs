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
    using System.Web.Mvc;

    public partial class fatura_satir
    {
        public int id { get; set; }
        public Nullable<int> fatura_id { get; set; }
        public Nullable<int> stok_id { get; set; }
        public string stok_adi { get; set; }
        public Nullable<int> birim_id { get; set; }
        public Nullable<decimal> birim_fiyati { get; set; }
        public Nullable<decimal> miktari { get; set; }
        public Nullable<int> doviz_id1 { get; set; }
        public Nullable<decimal> doviz_kuru1 { get; set; }
        public Nullable<decimal> kdv_orani { get; set; }
        public Nullable<decimal> kdv_matrah { get; set; }
        public Nullable<decimal> kdv_tutar { get; set; }
        public Nullable<decimal> otv_orani { get; set; }
        public Nullable<decimal> otv_matrah { get; set; }
        public Nullable<decimal> otv_tutar { get; set; }
        public Nullable<decimal> oiv_orani { get; set; }
        public Nullable<decimal> oiv_matrah { get; set; }
        public Nullable<decimal> oiv_tutar { get; set; }
        public Nullable<decimal> tevkifat_orani { get; set; }
        public Nullable<decimal> tevkifat { get; set; }
        public Nullable<decimal> iskonto_1 { get; set; }
        public Nullable<decimal> iskonto_2 { get; set; }
        public Nullable<decimal> iskonto_3 { get; set; }
        public Nullable<decimal> iskonto_4 { get; set; }
        public Nullable<decimal> iskonto_5 { get; set; }
        public Nullable<decimal> iskonto_toplam { get; set; }
        public Nullable<decimal> fiyat { get; set; }
        public Nullable<decimal> gvs_orani { get; set; }
        public Nullable<decimal> gvs_matrah { get; set; }
        public Nullable<decimal> gvs_tutar { get; set; }
        public Nullable<decimal> btu_orani { get; set; }
        public Nullable<decimal> btu_matrah { get; set; }
        public Nullable<decimal> btu_tutar { get; set; }
        public Nullable<decimal> mfv_orani { get; set; }
        public Nullable<decimal> mfv_matrah { get; set; }
        public Nullable<decimal> mfv_tutar { get; set; }
        public Nullable<decimal> sgk_orani { get; set; }
        public Nullable<decimal> sgk_matrah { get; set; }
        public Nullable<decimal> sgk_tutar { get; set; }
        public Nullable<decimal> smm_gvs_orani { get; set; }
        public Nullable<decimal> smm_gvs_matrah { get; set; }
        public Nullable<decimal> smm_gvs_tutar { get; set; }
        public string teslim_sarti { get; set; }
        public string kap_cinsi { get; set; }
        public string kap_no { get; set; }
        public Nullable<int> kap_adedi { get; set; }
        public Nullable<int> gonderilme_sekli { get; set; }
        public Nullable<long> gtip { get; set; }
        public string satirno { get; set; }
        public string sikayet { get; set; }
        public Nullable<decimal> teslim_adet { get; set; }
        public Nullable<decimal> red_adet { get; set; }
        public string red_kodu { get; set; }
        public string red_ack { get; set; }
        public Nullable<decimal> eksik_adet { get; set; }
        public Nullable<decimal> fazla_adet { get; set; }
        public Nullable<System.DateTime> last_update { get; set; }
        public SelectList DropDownListForDoviz { get; set; }
        public SelectList DropDownListForBirimler { get; set; }
        public SelectList DropDownListForKdv { get; set; }
        public SelectList DropDownListForOtv { get; set; }
        public SelectList DropDownListForOiv { get; set; }
        public SelectList DropDownListForTev { get; set; }
        public SelectList DropDownListForGvs { get; set; }
        public SelectList DropDownListForSgk { get; set; }
        public SelectList DropDownListForMfv { get; set; }
        public SelectList DropDownListForBtu { get; set; }
        public SelectList DropDownListForTeslimSekli { get; set; }
        public SelectList DropDownListForGonderilmeSekli { get; set; }
        public SelectList DropDownListForKapCinsi { get; set; }
        public string birim_ack { get; set; }
        public string doviz_cinsi_ack { get; set; }
        public Nullable<decimal> doviz_fiyat { get; set; }
        public SelectList DropDownListForGvsSMM { get; set; }
    }
}