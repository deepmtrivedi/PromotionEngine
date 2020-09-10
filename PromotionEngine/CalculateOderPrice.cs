using PromotionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    public class CalculateOderPrice : IOrder
    {
        private IProductService _productService;
        private IRuleService _ruleService;
        // Inject dependecies of Product and Rule services
        public CalculateOderPrice(IProductService productService, IRuleService ruleService)
        {
            _productService = productService;

            _ruleService = ruleService;
        }

        // Get the list of product from client
        private List<FinalProductList> AllSelectedProducts(List<SelectedProduct> selectedProduct)
        {
            try
            {

                var products = _productService.GetProdeucts();
                var selectedProducts = (from x in products
                                        join y in selectedProduct
                                           on x.SKU equals y.SKU
                                        group new { y.SKU, y.Units } by new { x.SKU, x.Price } into g

                                        select new FinalProductList
                                        {
                                            SKU = g.Key.SKU,
                                            Price = g.Key.Price,
                                            Unit = g.Sum(y => y.Units),
                                            HasCalculated = false
                                        }).ToList();
                return selectedProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception --> {ex.Message}");
                return null;
            }

        }

        // Run Rule engine to find the cost 
        public Dictionary<string, double> CaclculateOrderPrice(List<SelectedProduct> selectedProduct)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();

            List<FinalProductList> selectedProducts = AllSelectedProducts(selectedProduct);

            // if any product has been selected then only the logic should execute
            if (selectedProducts.Any())
            {
                // Get the rules from source system
                var rules = _ruleService.GetRules();
                double price = 0;
                string skuIdentifier = string.Empty;

                #region "Multiplication"

                // Get the rules having mulitiplication / same products combo
                var multiplicationRule = rules.Where(x => x.SKUWithUnits == null).ToList();

                // Get the list of selected product join with same product combo rules
                var productList = multiplicationRule.Join(selectedProducts, mr => mr.SKU, sp => sp.SKU,
                                                (mr, sp) => new { mr.SKU, ruleUnit = mr.Unit, selProductUnit = sp.Unit, sp.Price, mr.Discount }).ToList();

                foreach (var item in productList)
                {
                    // Get the price based on units and discout associated with rules
                    price = CalculatePriceBasedOnRule(item.ruleUnit, item.selProductUnit, item.Price, item.Discount);

                    skuIdentifier = ($"{item.selProductUnit.ToString()} * {item.SKU}");
                    // If there are multiple rules for same product combo then min price should be cons
                    if (dict.ContainsKey(skuIdentifier))
                    {
                        if (dict[skuIdentifier] > price)
                            dict[skuIdentifier] = price;
                    }
                    else
                        dict.Add(skuIdentifier, price);

                    // Once best price of Product has been calculated based on Rules, selected products has been marked as calculated to exclude them in further process
                    var usedSKU = selectedProducts.Where(c => !c.HasCalculated && c.SKU == item.SKU).ToList();
                    usedSKU.ForEach(c => c.HasCalculated = true);
                }
                skuIdentifier = string.Empty;
                #endregion

                #region "Addition of Products"

                // Get the rules having different products combo
                var addintionRule = rules.Where(x => x.SKUWithUnits != null).ToList();
                // Exclude products which have been already processed / calculated 
                selectedProducts = selectedProducts.Where(x => x.HasCalculated == false).ToList();

                price = 0;

                foreach (var item in addintionRule)
                {
                    var SKUWithUnits = item.SKUWithUnits;
                    int cnt = 1, tot = SKUWithUnits.Count;
                    string combo = string.Empty;
                    // Iterate over combo product in a rule
                    foreach (KeyValuePair<string, int> keyValues in SKUWithUnits)
                    {
                        string arSKU = keyValues.Key;

                        int ruleUnit = keyValues.Value;
                        var selProductUnit = selectedProducts.Where(x => x.SKU == arSKU).Select(x => x.Unit).FirstOrDefault();
                        var spPrice = selectedProducts.Where(x => x.SKU == arSKU).Select(x => x.Price).FirstOrDefault();
                        var spDisount = item.Discount;
                        skuIdentifier = ($"[{ruleUnit.ToString()} * {arSKU}]");

                        combo = skuIdentifier + " + " + combo;

                        if (selProductUnit >= ruleUnit)
                        {
                            // discount will be applied after summing of all the products from combo
                            price = cnt == tot ? (price + (spPrice * ruleUnit)) * spDisount : price + (spPrice * ruleUnit);
                            selProductUnit = selProductUnit - ruleUnit;

                            // reset units of selected product
                            var usedSKU = selectedProducts.Where(c => !c.HasCalculated && c.SKU == arSKU).ToList();
                            usedSKU.ForEach(c => c.Unit = selProductUnit);

                            // mark products with processed/calculated 
                            if (selProductUnit == 0)
                            {
                                usedSKU = selectedProducts.Where(c => !c.HasCalculated && c.SKU == arSKU).ToList();
                                usedSKU.ForEach(c => c.HasCalculated = true);
                            }
                            cnt++;
                        }
                        else if (selProductUnit < ruleUnit)
                        {
                            break;
                        }
                    }
                    if (price != 0.0)
                    {
                        // remove last 3 characters from name
                        combo = combo.Remove(combo.Length - 3);
                        // add to result
                        dict.Add(combo.Trim(), price);
                    }
                }
                skuIdentifier = string.Empty;
                #endregion

                #region "Single Product"
                // Exclude products which have been already processed / calculated 
                selectedProducts = selectedProducts.Where(c => !c.HasCalculated).ToList();

                foreach (var item in selectedProducts)
                {
                    price = 0;
                    price = price + (item.Price * item.Unit);
                    skuIdentifier = ($"{item.Unit.ToString()} * {item.SKU}");

                    if (dict.ContainsKey(item.SKU))
                    {
                        if (dict[skuIdentifier] > price)
                            dict[skuIdentifier] = price;
                    }
                    else
                        dict.Add(skuIdentifier, price);
                }
                #endregion
            }

            return dict;
        }

        private double CalculatePriceBasedOnRule(int ruleUnit, int selProductUnit, int price, double discount)
        {
            double finalPrice = 0.0;
            //int ut = spUnit;

            if (ruleUnit <= selProductUnit)
            {
                while (ruleUnit <= selProductUnit)
                {
                    finalPrice = finalPrice + ((price * ruleUnit) * discount);
                    selProductUnit = selProductUnit - ruleUnit;
                }
                if (selProductUnit > 0)
                {
                    finalPrice = finalPrice + (selProductUnit * price);
                }
            }
            else
            {
                finalPrice = finalPrice + (selProductUnit * price);
            }

            return finalPrice;
        }
    }
}
