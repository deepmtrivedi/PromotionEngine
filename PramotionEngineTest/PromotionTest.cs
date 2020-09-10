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

        [TestMethod]
        public void AllSinglePriducts()
        {
            List<SelectedProduct> sp = new List<SelectedProduct>()
            {
                new SelectedProduct()
                {
                    SKU= "A",
                    Units = 1
                },
                new SelectedProduct()
                {
                    SKU= "B",
                    Units = 1
                },
                new SelectedProduct()
                {
                    SKU= "C",
                    Units = 1
                },
                new SelectedProduct()
                {
                    SKU= "D",
                    Units = 1
                }
            };

            CalculateOderPrice objCalculateOderPrice = new CalculateOderPrice(_mockProductService.Object, _mockRuleService.Object);
            Dictionary<string, double> result = objCalculateOderPrice.CaclculateOrderPrice(sp);
            double totalA = 0.0, totalB = 0.0, totalC = 0.0, totalD = 0.0;
            foreach (KeyValuePair<string, double> item in result)
            {
                switch (item.Key)
                {
                    case "1 * A":
                        totalA = item.Value;
                        break;
                    case "1 * B":
                        totalB = item.Value;
                        break;
                    case "1 * C":
                        totalC = item.Value;
                        break;
                    case "1 * D":
                        totalD = item.Value;
                        break;
                    default:
                        break;
                }
            }
            Assert.AreEqual(50, totalA);
            Assert.AreEqual(30, totalB);
            Assert.AreEqual(20, totalC);
            Assert.AreEqual(15, totalD);
        }

        [TestMethod]
        public void ProvidedSenario1()
        {
            List<SelectedProduct> sp = new List<SelectedProduct>()
            {
                new SelectedProduct()
                {
                    SKU= "A",
                    Units = 5
                },
                new SelectedProduct()
                {
                    SKU= "B",
                    Units = 5
                },
                new SelectedProduct()
                {
                    SKU= "C",
                    Units = 2
                },
                new SelectedProduct()
                {
                    SKU= "D",
                    Units = 1
                }
            };

            CalculateOderPrice objCalculateOderPrice = new CalculateOderPrice(_mockProductService.Object, _mockRuleService.Object);
            Dictionary<string, double> result = objCalculateOderPrice.CaclculateOrderPrice(sp);
            double totalA = 0.0, totalB = 0.0, totalCD = 0.0;
            foreach (KeyValuePair<string, double> item in result)
            {
                switch (item.Key)
                {
                    case "5 * A":
                        totalA = item.Value;
                        break;
                    case "5 * B":
                        totalB = item.Value;
                        break;
                    case "[1 * D] + [2 * C]":
                        totalCD = item.Value;
                        break;

                    default:
                        break;
                }
            }
            Assert.AreEqual(205, totalA);
            Assert.AreEqual(126, totalB);
            Assert.AreEqual(44, totalCD);
        }
    }

}
