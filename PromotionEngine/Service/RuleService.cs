using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionEngine
{
    public class RuleService : IRuleService
    {
        /// <summary>
        /// This is a service class, we can fetch data from DB or external repository
        /// </summary>
        /// <returns>List of Rules configured in source system</returns>
        public List<Rule> GetRules()
        {
            List<Rule> rules = new List<Rule>
            {
                // this kind of Rule is to configure for same SKU for N number of units 
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
                    SKU = "A",
                    Unit = 4,
                    Discount = .60
                },

                // this kind of Rule is to configure for clubbing different SKUs for N number of units 
                new Rule
                {
                    SKUWithUnits = new Dictionary<string, int>()
                    {
                            {"C", 1 },
                            {"D", 1 },
                    },
                    Discount = .80
                },

            };

            return rules;

        }
    }
}
