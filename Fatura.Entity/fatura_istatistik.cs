using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatura.Entity
{
    public class fatura_istatistik
    {
        public Nullable<long> vkn_tckn { get; set; }
        public Nullable<int> gun_ay_yil { get; set; }
        public Nullable<decimal> tutar { get; set; }
    }
}
