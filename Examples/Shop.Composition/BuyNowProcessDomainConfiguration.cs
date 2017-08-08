using GridDomain.Configuration;
using Serilog;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Domain.ProcessManagers;

namespace Shop.Composition {
    public class BuyNowProcessDomainConfiguration : IDomainConfiguration
    {
        private readonly IPriceCalculator _calculator;
        private readonly ILogger _log;
        public BuyNowProcessDomainConfiguration(IPriceCalculator calculator, ILogger log)
        {
            _calculator = calculator;
            _log = log;
        }

        public void Register(IDomainBuilder builder)
        {
            builder.RegisterProcessManager(new DefaultProcessManagerDependencyFactory<BuyNowState>(new BuyNowProcessManagerFactory(_calculator, _log), BuyNow.Descriptor));
        }
    }
}