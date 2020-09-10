using Ninject;
using PromotionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var productService = kernel.Get<IProductService>();
            var ruleService = kernel.Get<IRuleService>();

            List<SelectedProduct> selectedProdList = new List<SelectedProduct>();

            bool flag = true;
            while (flag)
            {
                Console.Write("Enter SKU :\t\t\t\t");
                string sku = Console.ReadLine().ToUpper();
                Console.WriteLine("----------------------------------------------");

                Console.Write("Enter SKU Unit:\t\t\t\t");
                int skuUnit;
                int.TryParse(Console.ReadLine(), out skuUnit);
                Console.WriteLine("----------------------------------------------");

                SelectedProduct sp = new SelectedProduct() { SKU = sku, Units = skuUnit };
                selectedProdList.Add(sp);

                Console.WriteLine("----------------------------------------------");
                Console.Write("Do you want to select other SKU? \t");
                flag = Console.ReadLine().Equals("Y", StringComparison.OrdinalIgnoreCase);
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("----------------------------------------------");
            }

            Console.WriteLine("==========================");

            IOrder order = new CalculateOderPrice(productService, ruleService);
            Dictionary<string, double> totalPrice = order.CaclculateOrderPrice(selectedProdList);

            foreach (var item in totalPrice)
            {
                Console.WriteLine($"{item.Key.Trim() } \t  {item.Value}");
            }
            Console.WriteLine("==========================");
            double totPrice = totalPrice.Sum(price => price.Value);
            Console.WriteLine($"Total: \t   {totPrice}");
            Console.WriteLine("==========================");

            Console.Read();
        }

    }
}
