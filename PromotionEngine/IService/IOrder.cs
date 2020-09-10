using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionalEngine
{
    public interface IOrder
    {
        Dictionary<string, double> CaclculateOrderPrice(List<SelectedProduct> selectedProduct);
    }
}
