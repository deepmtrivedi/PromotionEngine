using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PromotionEngine;
using System.Collections.Generic;

namespace PramotionEngineTest
{
    [TestClass]
    public class PromotionTest
    {
        private Mock<IProductService> _mockProductService;
        private Mock<IRuleService> _mockRuleService;


        [TestInitialize]
        public void Setup()
        {
            List<Product> products = new List<Product>
            {
                new Product {SKU = "A", Price = 50},
                new Product {SKU = "B", Price = 30},
                new Product {SKU = "C", Price = 20},
                new Product {SKU = "D", Price = 15}
            };

            List<Rule> rules = new List<Rule>
            {
                new Rule
                {
                    SKU = "A",
                    Unit = 3,
                    Discount = .70
                },
                new Rule
                {
                    SKU = "B",
                    Unit = 2,
                    Discount = .80
                },
                new Rule
                {
                    SKUWithUnits = new Dictionary<string, int>()
                    {
                            {"C", 2 },
                            {"D", 1 },
                    },
                    Discount = .80
                }
            };

            _mockProductService = new Mock<IProductService>();
            _mockProductService.Setup(p => p.GetProdeucts()).Returns(products);

            _mockRuleService = new Mock<IRuleService>();
            _mockRuleService.Setup(r => r.GetRules()).Returns(rules);
        }
    }
}
