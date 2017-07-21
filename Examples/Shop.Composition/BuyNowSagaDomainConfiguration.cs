using GridDomain.Configuration;
using GridDomain.Node.Configuration.Composition;
using Serilog;
using Shop.Domain.DomainServices.PriceCalculator;
using Shop.Domain.Sagas;

namespace Shop.Composition {
    public class BuyNowSagaDomainConfiguration : IDomainConfiguration
    {
        private readonly IPriceCalculator _calculator;
        private readonly ILogger _log;
        public BuyNowSagaDomainConfiguration(IPriceCalculator calculator, ILogger log)
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