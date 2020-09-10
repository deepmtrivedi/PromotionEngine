using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromotionalEngine
{
    /// <summary>
    /// Dependency injection of services through Ninject container
    /// </summary>
    public class DIContainer : NinjectModule
    {
        public override void Load()
        {
            Bind<IProductService>().To<ProductService>();
            Bind<IRuleService>().To<RuleService>();
        }

    }
}
