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
    
    public partial class barkodlar
    {
        public int id { get; set; }
        public string kodu { get; set; }
        public string adi { get; set; }
        public Nullable<int> deger { get; set; }
        public Nullable<bool> isdeleted { get; set; }
        public Nullable<System.DateTime> last_update { get; set; }
    }
}
