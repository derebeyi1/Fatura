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
    
    public partial class fatura_gelen
    {
        public int id { get; set; }
        public Nullable<long> gonderen_vkn { get; set; }
        public string gonderen_unvan { get; set; }
        public string gonderen_sehir { get; set; }
        public string fatura_turu { get; set; }
        public System.DateTime olusturma_tarihi { get; set; }
        public string invoicetypecodetype { get; set; }
        public System.DateTime faturatarihi { get; set; }
        public string faturano { get; set; }
        public string belgeno { get; set; }
        public int entegrator_tipi { get; set; }
        public int fatura_tipi { get; set; }
        public string doviz { get; set; }
        public decimal kur_carpani { get; set; }
        public decimal meblag { get; set; }
        public decimal aratoplam { get; set; }
        public decimal vergitutar { get; set; }
        public decimal iskontotutar { get; set; }
        public string guid { get; set; }
        public string fatura_notu { get; set; }
        public string aktarimmesaji { get; set; }
        public bool issucceded { get; set; }
        public decimal otvtutar { get; set; }
        public string alicimusteriobj { get; set; }
        public string gonderenmusteriobj { get; set; }
        public string kdvtoplamobj { get; set; }
        public string hareketsatirlariobj { get; set; }
        public Nullable<int> durum { get; set; }
        public string mesaj { get; set; }
        public string not { get; set; }
        public string teslim_alan { get; set; }
        public string teslim_eden { get; set; }
        public System.DateTime teslim_tarihi { get; set; }
        public System.DateTime recorddate { get; set; }
        public System.DateTime last_update { get; set; }
        public List<fatura_gelen_satir> fgs { get; set; }
    }
}
