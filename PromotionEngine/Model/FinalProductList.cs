using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    public class FinalProductList
    {
        public string SKU { get; set; }

        public int Price { get; set; }

        public int Unit { get; set; }

        public bool HasCalculated { get; set; }
    }
}
