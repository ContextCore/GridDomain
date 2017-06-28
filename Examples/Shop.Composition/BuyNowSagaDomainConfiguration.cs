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
            builder.RegisterSaga(new DefaultSagaDependencyFactory<BuyNow, BuyNowState>(new BuyNowSagaFactory(_calculator, _log), BuyNow.Descriptor));
        }
    }
}