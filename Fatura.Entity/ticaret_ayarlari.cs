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
    
    public partial class ticaret_ayarlari
    {
        public int id { get; set; }
        public Nullable<long> vkn_tckn { get; set; }
        public Nullable<int> kullanici_id { get; set; }
        public Nullable<bool> testmi { get; set; }
        public Nullable<long> test_gndrn_vkn { get; set; }
        public Nullable<long> test_alici_vkn { get; set; }
        public Nullable<int> site_tipi { get; set; }
        public string api_key { get; set; }
        public string sifre { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public Nullable<System.DateTime> last_update { get; set; }
    }
}
