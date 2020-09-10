using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    public class ProductService : IProductService
    {
        /// <summary>
        /// This is a service class, we can fetch data from DB or external repository
        /// </summary>
        /// <returns>List of available Products in source system</returns>
        public List<Product> GetProdeucts()
        {
            List<Product> products = new List<Product>
            {
                new Product {SKU = "A", Price = 50},
                new Product {SKU = "B", Price = 30},
                new Product {SKU = "C", Price = 20},
                new Product {SKU = "D", Price = 15}
            };

            return products;
        }
    }
}
