using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionalEngine
{
    public interface IRuleService
    {
        List<Rule> GetRules();
    }
}
