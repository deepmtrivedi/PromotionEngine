using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionalEngine
{
    /// <summary>
    /// SKU Operand Unit (A * 3) 
    /// SKU Operand U
    /// </summary>
    public class Rule
    {
        public string SKU { get; set; }
        public int Unit { get; set; }
        public double Discount { get; set; }
        public Dictionary<string, int> SKUWithUnits { get; set; }
    }
}
