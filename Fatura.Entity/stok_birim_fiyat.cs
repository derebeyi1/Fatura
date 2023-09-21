using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatura.Entity
{
    public class stok_birim_fiyat
    {
        public int id { get; set; }
        public Nullable<long> vkn_tckn { get; set; }
        public string adi { get; set; }
        public Int64 gtip_no { get; set; }
        public Nullable<int> birim_id_son { get; set; }
        public Nullable<int> doviz_id_son { get; set; }
        public Nullable<decimal> fiyat_son { get; set; }
        public Nullable<decimal> kdv_son { get; set; }
        public Nullable<decimal> otv_son { get; set; }
        public Nullable<decimal> oiv_son { get; set; }
        public Nullable<decimal> gvs_son { get; set; }
        public Nullable<decimal> sgk_son { get; set; }
        public Nullable<decimal> mfv_son { get; set; }
        public Nullable<decimal> btu_son { get; set; }
    }
}
